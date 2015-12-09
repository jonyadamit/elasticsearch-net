﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nest
{
	[JsonConverter(typeof(ReadAsTypeJsonConverter<MappingTransform>))]
	public interface IMappingTransform
	{
		[JsonProperty("script")]
		string Script { get; set; }

		[JsonProperty("script_file")]
		string ScriptFile { get; set; }

		[JsonProperty("params")]
		IDictionary<string, string> Parameters { get; set; }

		[JsonProperty("lang")]
		string Language { get; set; }
	}

	public class MappingTransform: IMappingTransform
	{
		public string Script { get; set; }

		public string ScriptFile { get; set; }

		public IDictionary<string, string> Parameters { get; set; }

		public string Language { get; set; }
	}

	public class MappingTransformDescriptor : DescriptorBase<MappingTransformDescriptor, IMappingTransform>, IMappingTransform
	{
		string IMappingTransform.Script { get; set; }

		string IMappingTransform.ScriptFile { get; set; }

		IDictionary<string, string> IMappingTransform.Parameters { get; set; }

		string IMappingTransform.Language { get; set; }

		public MappingTransformDescriptor Script(string script) => Assign(a => a.Script = script);

		public MappingTransformDescriptor ScriptFile(string scriptFile) => Assign(a => a.ScriptFile = scriptFile);

		public MappingTransformDescriptor Params(IDictionary<string, string> parameters) => Assign(a => a.Parameters = parameters);

		public MappingTransformDescriptor Language(string language) => Assign(a => a.Language = language);

		public MappingTransformDescriptor Language(ScriptLang language) => Assign(a => a.Language = language.GetStringValue());

	}

	public class MappingTransformsDescriptor: DescriptorPromiseBase<MappingTransformsDescriptor, IList<IMappingTransform>>
	{
		public MappingTransformsDescriptor() : base(new List<IMappingTransform>()) { }

		public MappingTransformsDescriptor Add(Func<MappingTransformDescriptor, IMappingTransform> selector) =>
			Assign(a => a.AddIfNotNull(selector?.Invoke(new MappingTransformDescriptor())));

	}
}
