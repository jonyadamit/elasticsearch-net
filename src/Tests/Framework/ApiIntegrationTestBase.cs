﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using FluentAssertions;
using Nest;
using Tests.Framework.Integration;
using Elasticsearch.Net;

namespace Tests.Framework
{
	public abstract class ApiIntegrationTestBase<TResponse, TInterface, TDescriptor, TInitializer> 
		: ApiTestBase<TResponse, TInterface, TDescriptor, TInitializer>
		where TResponse : class, IResponse
		where TDescriptor : class, TInterface
		where TInitializer : class, TInterface
		where TInterface : class
	{
		protected abstract int ExpectStatusCode { get; }
		protected abstract bool ExpectIsValid { get; }
		protected virtual void ExpectResponse(TResponse response) { }

		protected ApiIntegrationTestBase(IIntegrationCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override TInitializer Initializer => Activator.CreateInstance<TInitializer>();

		[I] protected async Task HandlesStatusCode() =>
			await this.AssertOnAllResponses(r=>r.ApiCall.HttpStatusCode.Should().Be(this.ExpectStatusCode));

		[I] protected async Task ReturnsExpectedIsValid() =>
			await this.AssertOnAllResponses(r=>r.IsValid.Should().Be(this.ExpectIsValid));

		[I] protected async Task ReturnsExpectedResponse() =>
			await this.AssertOnAllResponses(r => ExpectResponse(r));

		protected override Task AssertOnAllResponses(Action<TResponse> assert)
		{
			if (!this.ExpectIsValid) return base.AssertOnAllResponses(assert);

			return base.AssertOnAllResponses((r) =>
			{
				if (TestClient.RunIntegrationTests && !r.IsValid && r.CallDetails.OriginalException != null)
				{
					ExceptionDispatchInfo.Capture(r.CallDetails.OriginalException).Throw();
					return;
				}

				assert(r);
			});
		}
	}
}
