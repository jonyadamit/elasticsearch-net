﻿using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Framework.Integration;
using Tests.Framework.MockData;

namespace Tests.Aggregations.Pipeline.MovingAverage
{
	public class MovingAverageEwmaAggregationUsageTests : AggregationUsageTestBase
	{
		public MovingAverageEwmaAggregationUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override object ExpectJson => new
		{
			size = 0,
			aggs = new
			{
				projects_started_per_month = new
				{
					date_histogram = new
					{
						field = "startedOn",
						interval = "month",
					},
					aggs = new
					{
						commits = new
						{
							sum = new
							{
								field = "numberOfCommits"
							}
						},
						commits_moving_avg = new
						{
							moving_avg = new
							{
								buckets_path = "commits",
								model = "ewma",
								settings = new
								{
									alpha = 0.3,
								}
							}
						}
					}
				}
			}
		};

		protected override Func<SearchDescriptor<Project>, ISearchRequest> Fluent => s => s
			.Size(0)
			.Aggregations(a => a
				.DateHistogram("projects_started_per_month", dh => dh
					.Field(p => p.StartedOn)
					.Interval(DateInterval.Month)
					.Aggregations(aa => aa
						.Sum("commits", sm => sm
							.Field(p => p.NumberOfCommits)
						)
						.MovingAverage("commits_moving_avg", mv => mv
							.BucketsPath("commits")
							.Model(m => m
								.Ewma(e => e
									.Alpha(0.3f)
								)
							)
						)
					)
				)
			);

		protected override SearchRequest<Project> Initializer => new SearchRequest<Project>()
		{
			Size = 0,
			Aggregations = new DateHistogramAggregation("projects_started_per_month")
			{
				Field = "startedOn",
				Interval = DateInterval.Month,
				Aggregations = 
					new SumAggregation("commits", "numberOfCommits") &&
					new MovingAverageAggregation("commits_moving_avg", "commits")
					{
						Model = new EwmaModel
						{
							Alpha = 0.3f,
						}
					}
			}
		};

		protected override void ExpectResponse(ISearchResponse<Project> response)
		{
			response.IsValid.Should().BeTrue();

			var projectsPerMonth = response.Aggs.DateHistogram("projects_started_per_month");
			projectsPerMonth.Should().NotBeNull();
			projectsPerMonth.Items.Should().NotBeNull();
			projectsPerMonth.Items.Count.Should().BeGreaterThan(0);
	
			// average not calculated for the first bucket
			foreach(var item in projectsPerMonth.Items.Skip(1))
			{
				var movingAvg = item.MovingAverage("commits_moving_avg");
				movingAvg.Should().NotBeNull();
				movingAvg.Value.Should().BeGreaterThan(0);
			}
		}
	}
}
