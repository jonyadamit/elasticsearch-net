﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IFileScript : IScript
	{
		[JsonProperty("file")]
		string File { get; set; }
	}

	public class FileScript : Script, IFileScript
	{
		public FileScript(string file)
		{
			this.File = file;
		}

		public string File { get; set; }
	}

	public class FileScriptDescriptor
		: ScriptDescriptorBase<FileScriptDescriptor, IFileScript>, IFileScript
	{
		string IFileScript.File { get; set; }

		public FileScriptDescriptor File(string file) => Assign(a => a.File = file);
	}
}
