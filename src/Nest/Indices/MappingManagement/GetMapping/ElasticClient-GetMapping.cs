﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace Nest
{
	using GetMappingConverter = Func<IApiCallDetails, Stream, GetMappingResponse>;

	public partial interface IElasticClient
	{
		/// <summary>
		/// The get mapping API allows to retrieve mapping definitions for an index or index/type.
		/// <para> </para>http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices-get-mapping.html
		/// </summary>
		/// <param name="selector">A descriptor that describes the parameters for the get mapping operation</param>
		IGetMappingResponse GetMapping<T>(Func<GetMappingDescriptor<T>, IGetMappingRequest> selector = null) where T : class;

		/// <inheritdoc/>
		IGetMappingResponse GetMapping(IGetMappingRequest getMappingRequest);

		/// <inheritdoc/>
		Task<IGetMappingResponse> GetMappingAsync<T>(Func<GetMappingDescriptor<T>, IGetMappingRequest> selector = null)
			where T : class;

		/// <inheritdoc/>
		Task<IGetMappingResponse> GetMappingAsync(IGetMappingRequest getMappingRequest);
	}

	public partial class ElasticClient
	{
		/// <inheritdoc/>
		public IGetMappingResponse GetMapping<T>(Func<GetMappingDescriptor<T>, IGetMappingRequest> selector = null)
			where T : class => 
			this.GetMapping(selector.InvokeOrDefault(new GetMappingDescriptor<T>()));

		/// <inheritdoc/>
		public IGetMappingResponse GetMapping(IGetMappingRequest getMappingRequest) => 
			this.Dispatcher.Dispatch<IGetMappingRequest, GetMappingRequestParameters, GetMappingResponse>(
				getMappingRequest,
				new GetMappingConverter((r, s) => DeserializeGetMappingResponse(r, getMappingRequest, s)),
				(p, d) => this.LowLevelDispatch.IndicesGetMappingDispatch<GetMappingResponse>(p)
			);

		/// <inheritdoc/>
		public Task<IGetMappingResponse> GetMappingAsync<T>(Func<GetMappingDescriptor<T>, IGetMappingRequest> selector = null)
			where T : class =>
			this.GetMappingAsync(selector.InvokeOrDefault(new GetMappingDescriptor<T>()));

		/// <inheritdoc/>
		public Task<IGetMappingResponse> GetMappingAsync(IGetMappingRequest getMappingRequest) => 
			this.Dispatcher.DispatchAsync<IGetMappingRequest, GetMappingRequestParameters, GetMappingResponse, IGetMappingResponse>(
				getMappingRequest,
				new GetMappingConverter((r, s) => DeserializeGetMappingResponse(r, getMappingRequest, s)),
				(p, d) => this.LowLevelDispatch.IndicesGetMappingDispatchAsync<GetMappingResponse>(p)
			);
		
		//TODO this is too geared towards getting a single mapping
		private GetMappingResponse DeserializeGetMappingResponse(IApiCallDetails response, IGetMappingRequest d, Stream stream)
		{
			var dict = response.Success
				? Serializer.Deserialize<GetRootObjectMappingWrapping>(stream)
				: null;
			return new GetMappingResponse(response, dict);
		}

	}
}