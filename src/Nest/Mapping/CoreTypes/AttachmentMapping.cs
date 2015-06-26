using System.Collections.Generic;
using Nest.Resolvers.Converters;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;

namespace Nest
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AttachmentMapping : IElasticType
	{
		public PropertyNameMarker Name { get; set; }

		[JsonProperty("type")]
		public virtual TypeNameMarker Type { get { return new TypeNameMarker { Name = "attachment" }; } }

		[JsonProperty("similarity")]
		public string Similarity { get; set; }

		[JsonProperty("fields"), JsonConverter(typeof(ElasticCoreTypeConverter))]
		public IDictionary<PropertyNameMarker, IElasticCoreType> Fields { get; set; }

		public AttachmentMapping()
		{
			this.Fields = new Dictionary<PropertyNameMarker, IElasticCoreType>();
		}

	}
	public class AttachmentMappingDescriptor<T> where T : class
	{
		internal AttachmentMapping _Mapping = new AttachmentMapping();

		public AttachmentMappingDescriptor<T> Name(string name)
		{
			this._Mapping.Name = name;
			return this;
		}
		public AttachmentMappingDescriptor<T> Name(Expression<Func<T, object>> objectPath)
		{
			this._Mapping.Name = objectPath;
			return this;
		}

		public AttachmentMappingDescriptor<T> FileField(Func<StringMappingDescriptor<T>, StringMappingDescriptor<T>> stringMapper)
		{
			stringMapper.ThrowIfNull("stringMapper");
			var d = stringMapper(new StringMappingDescriptor<T>());
			d.ThrowIfNull("stringMapper return value");
			
			this._Mapping.Fields[this._Mapping.Name] = d._Mapping;
			return this;
		}

		public AttachmentMappingDescriptor<T> AuthorField(Func<StringMappingDescriptor<T>, StringMappingDescriptor<T>> stringMapper)
		{
			stringMapper.ThrowIfNull("stringMapper");
			var d = stringMapper(new StringMappingDescriptor<T>());
			d.ThrowIfNull("attachment author field mapping");

			this._Mapping.Fields["author"] = d._Mapping;
			return this;
		}
		public AttachmentMappingDescriptor<T> TitleField(Func<StringMappingDescriptor<T>, StringMappingDescriptor<T>> stringMapper)
		{
			stringMapper.ThrowIfNull("stringMapper");
			var d = stringMapper(new StringMappingDescriptor<T>());
			d.ThrowIfNull("attachment title field");

			this._Mapping.Fields["title"] = d._Mapping;
			return this;
		}
		public AttachmentMappingDescriptor<T> MetadataField(string metadataFieldName, Func<StringMappingDescriptor<T>, StringMappingDescriptor<T>> stringMapper)
		{
			metadataFieldName.ThrowIfNullOrEmpty("metadataFieldName");
			stringMapper.ThrowIfNull("stringMapper");
			var d = stringMapper(new StringMappingDescriptor<T>());
			d.ThrowIfNull("attachment metadata field");

			this._Mapping.Fields[metadataFieldName] = d._Mapping;
			return this;
		}
		public AttachmentMappingDescriptor<T> DateField(Func<DateMappingDescriptor<T>, DateMappingDescriptor<T>> dateMapper)
		{
			dateMapper.ThrowIfNull("stringMapper");
			var d = dateMapper(new DateMappingDescriptor<T>());
			d.ThrowIfNull("stringMapper return value");

			this._Mapping.Fields["date"] = d._Mapping;
			return this;
		}
		
	}
}