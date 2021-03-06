﻿using System;
using System.Collections.Specialized;
using System.Net;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Elasticsearch.Net.ConnectionPool;
using Nest;
using System.Text;
using Elasticsearch.Net.Providers;
using FluentAssertions;
using Tests.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Framework.MockData;

namespace Tests.ClientConcepts.ConnectionPooling.BuildingBlocks
{
	public class Transports
	{
		/** == Transports
		*
		* The ITransport interface can be seen as the motor block of the client. It's interface is deceitfully simple. 
		* Its ultimately responsible from translating a client call to a response. If for some reasons you do not agree with the way we wrote
		* the internals of the client by implementing a custom ITransport you can circumvent all of it and introduce your own.
		*/

		public async Task InterfaceExplained()
		{
			/** 
			* Transport is generically typed to a type that implements IConnectionConfigurationValues 
			* This is the minimum ITransport needs to report back for the client to function.
			*
			* e.g in the low level client transport is instantiated like this:
			*/
			var lowLevelTransport = new Transport<ConnectionConfiguration>(new ConnectionConfiguration());

			/** In the high level client like this. */
			var highlevelTransport = new Transport<ConnectionSettings>(new ConnectionSettings());

			var connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
			var inMemoryTransport = new Transport<ConnectionSettings>(new ConnectionSettings(connectionPool, new InMemoryConnection()));

			/**
			* The only two methods on ITransport are Request and DoRequestAsync, the default ITransport implementation is responsible for introducing
			* many of the building blocks in the client, if these do not work for you can swap them out for your own custom ITransport implementation. 
			* If you feel this need report back to us we would love to learn why you'd go down this route!
			*/

			var response = inMemoryTransport.Request<SearchResponse<Project>>(HttpMethod.GET, "/_search", new { query = new { match_all = new { } } });
			response = await inMemoryTransport.RequestAsync<SearchResponse<Project>>(HttpMethod.GET, "/_search", new { query = new { match_all = new { } } });
		}
	}
}
