using FluentAssertions;
using Nest;
using Tests.Framework.MockData;
using static Nest.Static;

namespace Tests.Framework.Integration
{
	public class Seeder
	{
		private IElasticClient Client { get; }

		public Seeder(int port)
		{
			var client = TestClient.GetClient(seederSettings, port);
			this.Client = client;
		}

		private ConnectionSettings seederSettings(ConnectionSettings settings) => settings;

		public void SeedNode()
		{
			var rawFieldsTemplateExists = this.Client.IndexTemplateExists("raw_fields").Exists;
			//if raw_fields exists assume this cluster is already seeded
			//sometimes we run against an manually started elasticsearch when writing tests
			//to cut down on cluster startup times
			if (rawFieldsTemplateExists) return;
			this.CreateIndicesAndMappings();

		}

		public void CreateIndicesAndMappings()
		{
			CreateRawFieldsIndexTemplate();

			CreateProjectIndex();
			CreateDeveloperIndex();

			this.Client.IndexMany(Project.Projects);
			this.Client.IndexMany(Developer.Developers);

			this.Client.Refresh(Nest.Indices.Index<Project>().And<Developer>());
		}

		private void CreateRawFieldsIndexTemplate()
		{
			var putTemplateResult = this.Client.PutIndexTemplate("raw_fields", p => p
				.Template("*") //match on all created indices
				.Settings(s => s
					.NumberOfReplicas(0)
					.NumberOfShards(2)
				)
				.Mappings(pm => pm
					.Map("_default_", m => m
						.DynamicTemplates(dt => dt
							.DynamicTemplate("raw_field", dtt => dtt
								.Match("*") //matches all fields
								.MatchMappingType("string") //that are a string
								.Mapping(tm => tm
									.String(sm => sm //map as string
										.Fields(f => f //with a multifield 'raw' that is not analyzed
											.String(ssm => ssm.Name("raw").Index(FieldIndexOption.NotAnalyzed))
										)
									)
								)
							)
						)
					)
				)
				);
			putTemplateResult.IsValid.Should().BeTrue();
		}

		private void CreateDeveloperIndex()
		{
			var createDeveloperIndex = this.Client.CreateIndex(Index<Developer>(), c => c
				.Mappings(map => map
					.Map<Developer>(m => m
						.AutoMap()
						.Properties(DeveloperProperties)
					)
				)
			);
			createDeveloperIndex.IsValid.Should().BeTrue();
		}

		private void CreateProjectIndex()
		{
			var createProjectIndex = this.Client.CreateIndex(typeof(Project), c => c
				.Aliases(a => a
					.Alias("projects-alias")
				)
				.Mappings(map => map
					.Map<Project>(m => m
						.Properties(props => props
							.String(s => s.Name(p => p.Name).NotAnalyzed())
							.Date(d => d.Name(p => p.StartedOn))
							.String(d => d.Name(p => p.State).NotAnalyzed())
							.Nested<Tag>(mo => mo
								.Name(p => p.Tags)
								.Properties(TagProperties)
							)
							.Object<Developer>(o => o
								.Name(p => p.LeadDeveloper)
								.Properties(DeveloperProperties)
							)
							.GeoPoint(g => g.Name(p => p.Location))
						)
					)
					.Map<CommitActivity>(m => m
						.SetParent<Project>()
						.Properties(props => props
							.Object<Developer>(o => o
								.Name(p => p.Committer)
								.Properties(DeveloperProperties)
							)
							.String(prop => prop.Name(p => p.ProjectName).NotAnalyzed())
						)
					)
				)
				);
			createProjectIndex.IsValid.Should().BeTrue();
		}

		private static PropertiesDescriptor<Tag> TagProperties(PropertiesDescriptor<Tag> props) => props
			.String(s => s
				.Name(p => p.Name).NotAnalyzed()
				.Fields(f => f
					.String(st => st.Name("vectors").TermVector(TermVectorOption.WithPositionsOffsetsPayloads))
				)
			);

		private static PropertiesDescriptor<Developer> DeveloperProperties(PropertiesDescriptor<Developer> props) => props
			.String(s => s.Name(p => p.OnlineHandle).NotAnalyzed())
			.String(s => s.Name(p => p.Gender).NotAnalyzed())
			.String(s => s.Name(p => p.FirstName).TermVector(TermVectorOption.WithPositionsOffsetsPayloads))
			.Ip(s => s.Name(p => p.IPAddress))
			//.GeoPoint(g=>g.Name(p=>p.Location))
			;
	}
}
