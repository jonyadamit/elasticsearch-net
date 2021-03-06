﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{

	public class FieldMappingProperties : Dictionary<string, FieldMapping>
	{

	}

	public class TypeFieldMappings
	{
		[JsonProperty("mappings")]
		public Dictionary<string, FieldMappingProperties> Mappings { get; set; }

	}
	public class FieldMapping
	{
		[JsonProperty("full_name")]
		public string FullName { get; set; }

		[JsonProperty("mapping")]
		[JsonConverter(typeof(FieldMappingJsonConverter))]
		public Dictionary<string, IFieldMapping> Mapping { get; set; }

	}

	public class IndexFieldMappings : Dictionary<string, TypeFieldMappings>
	{

	}

	public interface IGetFieldMappingResponse : IResponse
	{
		IndexFieldMappings Indices { get; set; }

		IFieldMapping MappingFor(string indexName, string typeName, string fieldName);

		IFieldMapping MappingFor<T>(string fieldName)
			where T : class;

		IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class;

		FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class;

		FieldMappingProperties MappingsFor(string indexName, string typeName);
	}

	public class GetFieldMappingResponse : BaseResponse, IGetFieldMappingResponse
	{
		public GetFieldMappingResponse()
		{
			this.Indices = new IndexFieldMappings();
		}

		internal GetFieldMappingResponse(IApiCallDetails status, IndexFieldMappings dict)
		{
			this.Indices = dict ?? new IndexFieldMappings();
			//TODO can dict truely ever be null, whats the response look like when field mapping is not found.
			//does status.Success not already reflect this?
			//this.IsValid = status.Success && dict != null && dict.Count > 0;
		}

		public IndexFieldMappings Indices { get; set; }

		public FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class
		{

			//TODO figure out a new way to get MappingsFor<T>.
			throw new NotImplementedException("responses no longer have settings");
			//indexName = indexName ?? Settings.Inferrer.IndexName<T>();
			//typeName = typeName ?? Settings.Inferrer.TypeName<T>();

			//return this.MappingsFor(indexName, typeName);
		}

		public FieldMappingProperties MappingsFor(string indexName, string typeName)
		{
			TypeFieldMappings index;
			FieldMappingProperties type;

			if (!this.Indices.TryGetValue(indexName, out index) || index.Mappings == null) return null;
			if (!index.Mappings.TryGetValue(typeName, out type)) return null;

			return type;
		}

		public IFieldMapping MappingFor(string indexName, string typeName, string fieldName)
		{
			if (fieldName.IsNullOrEmpty()) return null;

			var type = this.MappingsFor(indexName, typeName);
			if (type == null) return null;

			FieldMapping field;
			if (!type.TryGetValue(fieldName, out field) || field.Mapping == null) return null;

			var name = fieldName.Split('.').Last();
			return field.Mapping[name];
		}

		public IFieldMapping MappingFor<T>(string fieldName)
			where T : class
		{
			//TODO figure out a new way to get MappingsFor<T>.
			throw new NotImplementedException("responses no longer have settings");
			//var indexName = Settings.Inferrer.IndexName<T>();
			//var typeName = Settings.Inferrer.TypeName<T>();
			//return this.MappingFor(indexName, typeName, fieldName);
		}
		public IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class
		{
			//TODO figure out a new way to get MappingsFor<T>.
			throw new NotImplementedException("responses no longer have settings");
			//var path = Settings.Inferrer.Field(fieldName);
			//return this.MappingFor<T>(path);

		}
	}
}