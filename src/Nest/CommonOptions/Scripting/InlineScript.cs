﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IInlineScript : IScript
	{
		[JsonProperty("inline")]
		string Inline { get; set; }
	}

	public class InlineScript : Script, IInlineScript
	{
		public InlineScript(string script)
		{
			this.Inline = script;
		}

		public string Inline { get; set; }
		
		public static implicit operator InlineScript(string script) => new InlineScript(script);
	}

	public class InlineScriptDescriptor
		: ScriptDescriptorBase<InlineScriptDescriptor, IInlineScript>, IInlineScript
	{
		string IInlineScript.Inline { get; set; }

		public InlineScriptDescriptor Inline(string script) => Assign(a => a.Inline = script);
	}
}
