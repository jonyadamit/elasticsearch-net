﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Tests.Framework;
using Tests.Framework.MockData;
using static Tests.Framework.UrlTester;

namespace Tests.Modules.SnapshotAndRestore.Repositories.CreateRepository
{
	public class CreateRepositoryUrlTests
	{
		[U] public async Task Urls()
		{
			var repository = "repos";

			await PUT($"/_snapshot/{repository}")
				.Fluent(c => c.CreateRepository(repository, s=>s))
				.Request(c => c.CreateRepository(new CreateRepositoryRequest(repository)))
				.FluentAsync(c => c.CreateRepositoryAsync(repository, s=>s))
				.RequestAsync(c => c.CreateRepositoryAsync(new CreateRepositoryRequest(repository)))
				;
		}
	}
}
