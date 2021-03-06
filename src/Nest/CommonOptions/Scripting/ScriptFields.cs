﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<ScriptField>))]
	public interface IScriptField
	{
		[JsonProperty("script")]
		IScript Script { get; set; }
	}

	public class ScriptField : IScriptField
	{
		public IScript Script { get; set; }
	}

	public class ScriptFieldDescriptor
		: DescriptorBase<ScriptFieldDescriptor, IScriptField>, IScriptField
	{
		IScript IScriptField.Script { get; set; }

		public ScriptFieldDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) =>
			Assign(a => a.Script = scriptSelector?.Invoke(new ScriptDescriptor()));
	}

	[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter<ScriptFields, string, IScriptField>))]
	public interface IScriptFields : IIsADictionary<string, IScriptField> { }

	public class ScriptFields : IsADictionary<string, IScriptField>, IScriptFields
	{
		public ScriptFields() : base() { }
		public ScriptFields(IDictionary<string, IScriptField> container) : base(container) { }
		public ScriptFields(Dictionary<string, IScriptField> container)
			: base(container.Select(kv => kv).ToDictionary(kv => kv.Key, kv => kv.Value))
		{ }

		public void Add(string name, IScriptField script) => this.BackingDictionary.Add(name, script);
	}

	public class ScriptFieldsDescriptor
		: IsADictionaryDescriptor<ScriptFieldsDescriptor, IScriptFields, string, IScriptField>
	{
		public ScriptFieldsDescriptor() : base(new ScriptFields()) { }

		public ScriptFieldsDescriptor ScriptField(string name, Func<ScriptFieldDescriptor, IScriptField> selector) =>
			Assign(name, selector?.Invoke(new ScriptFieldDescriptor()));
	}
}
