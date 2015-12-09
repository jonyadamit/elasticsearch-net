﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nest.Resolvers;

namespace Nest
{
	internal class TermsQueryJsonConverter : JsonConverter
	{
		public override bool CanRead => true;

		public override bool CanWrite => true;

		public override bool CanConvert(Type objectType) => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var t = value as ITermsQuery;
			if (t == null) return;

			string field = null;

			var contract = serializer.ContractResolver as SettingsContractResolver;
			if (contract != null && contract.ConnectionSettings != null)
				field = contract.Infer.Field(t.Field);

			writer.WriteStartObject();
			{
				if (t.Terms.HasAny())
				{
					writer.WritePropertyName(field);
					serializer.Serialize(writer, t.Terms);
				}
				else if (t.ExternalField != null)
				{
					writer.WritePropertyName(field);
					serializer.Serialize(writer, t.ExternalField);
				}
				if (t.DisableCoord.HasValue)
				{
					writer.WritePropertyName("disable_coord");
					writer.WriteValue(t.DisableCoord.Value);
				}
				if (!t.MinimumShouldMatch.IsNullOrEmpty())
				{
					writer.WritePropertyName("minimum_should_match");
					writer.WriteValue(t.MinimumShouldMatch);
				}
				if (t.Boost.HasValue)
				{
					writer.WritePropertyName("boost");
					writer.WriteValue(t.Boost.Value);
				}
				if (!t.Name.IsNullOrEmpty())
				{
					writer.WritePropertyName("_name");
					writer.WriteValue(t.Name);
				}
				
			}
			writer.WriteEndObject();
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var filter = new TermsQueryDescriptor<object, object>();
			ITermsQuery f = filter;
			if (reader.TokenType != JsonToken.StartObject)
				return null;

			var depth = reader.Depth;
			while (reader.Read() && reader.Depth >= depth && reader.Value != null)
			{
				var property = reader.Value as string;
				switch (property)
				{
					case "disable_coord":
						reader.Read();
						f.DisableCoord = reader.Value as bool?;
						break;
					case "minimum_should_match":
						f.MinimumShouldMatch = reader.ReadAsString();	
						break;
					case "boost":
						reader.Read();
						f.Boost = reader.Value as double?;
						break;
					case "_name":
						f.Name = reader.ReadAsString();
						break;
					default:
						f.Field = property;
						//reader.Read();
						ReadTerms(f, reader);
						//reader.Read();
						break;
				}
			}
			return filter;

		}

		private void ReadTerms(ITermsQuery termsQuery, JsonReader reader)
		{
			reader.Read();
			if (reader.TokenType == JsonToken.StartObject)
			{
				var ef = new ExternalFieldDeclaration();
				var depth = reader.Depth;
				while (reader.Read() && reader.Depth >= depth && reader.Value != null)
				{
					var property = reader.Value as string;
					switch (property)
					{
						case "id":
							reader.Read();
							ef.Id = reader.Value as string;
							break;
						case "index":
							reader.Read();
							ef.Index = reader.Value as string;
							break;
						case "type":
							reader.Read();
							ef.Type = reader.Value as string;
							break;
						case "path":
							reader.Read();
							ef.Path = reader.Value as string;
							break;
					}
				}
				termsQuery.ExternalField = ef;
			}
			else if (reader.TokenType == JsonToken.StartArray)
			{
				var values = JArray.Load(reader).Values<string>();
				termsQuery.Terms = values;
			}
		}
	}

}
