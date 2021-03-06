﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Tests.Framework;
using Tests.Framework.MockData;
using static Tests.Framework.UrlTester;

namespace Tests.Indices.Warmers.PutWarmer
{
	public class PutWarmerUrlTests
	{
		[U] public async Task Urls()
		{
			var index = "indexx";
			var type = "commits";
			var name = "id";

			await PUT($"/_warmer/{name}")
				.Fluent(c => c.PutWarmer(name, s=>s))
				.Request(c => c.PutWarmer(new PutWarmerRequest(name)))
				.FluentAsync(c => c.PutWarmerAsync(name, s=>s))
				.RequestAsync(c => c.PutWarmerAsync(new PutWarmerRequest(name)))
				;

			await PUT($"/{index}/_warmer/{name}")
				.Fluent(c => c.PutWarmer(name, w=>w.Index(index)))
				.Request(c => c.PutWarmer(new PutWarmerRequest(index, name)))
				.FluentAsync(c => c.PutWarmerAsync(name, w=>w.Index(index)))
				.RequestAsync(c => c.PutWarmerAsync(new PutWarmerRequest(index, name)))
				;

			await PUT($"/{index}/{type}/_warmer/{name}")
				.Fluent(c => c.PutWarmer(name, w=>w.Index(index).Type<CommitActivity>()))
				.Request(c => c.PutWarmer(new PutWarmerRequest(index, typeof(CommitActivity), name)))
				.FluentAsync(c => c.PutWarmerAsync(name, w=>w.Index(index).Type("commits")))
				.RequestAsync(c => c.PutWarmerAsync(new PutWarmerRequest(index, "commits", name)))
				;

		}
	}
}
