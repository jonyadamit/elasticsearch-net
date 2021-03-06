﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IIndexedScript : IScript
	{
		[JsonProperty("id")]
		string Id { get; set; }
	}

	public class IndexedScript : Script, IIndexedScript
	{
		public IndexedScript(string id)
		{
			this.Id = id;
		}

		public string Id { get; set; }
	}

	public class IndexedScriptDescriptor
		: ScriptDescriptorBase<IndexedScriptDescriptor, IIndexedScript>, IIndexedScript
	{
		string IIndexedScript.Id { get; set; }

		public IndexedScriptDescriptor Id(string id) => Assign(a => a.Id = id);
	}
}
