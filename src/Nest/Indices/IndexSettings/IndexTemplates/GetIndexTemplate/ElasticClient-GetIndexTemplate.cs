﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace Nest
{
	using GetIndexTemplateConverter = Func<IApiCallDetails, Stream, GetIndexTemplateResponse>;

	public partial interface IElasticClient
	{
		/// <summary>
		/// Gets an index template
		/// <para> </para>http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices-templates.html#getting
		/// </summary>
		/// <param name="name">The name of the template to get</param>
		/// <param name="selector">An optional selector specifying additional parameters for the get template operation</param>
		IGetIndexTemplateResponse GetIndexTemplate(Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null);

		/// <inheritdoc/>
		IGetIndexTemplateResponse GetIndexTemplate(IGetIndexTemplateRequest request);

		/// <inheritdoc/>
		Task<IGetIndexTemplateResponse> GetIndexTemplateAsync(Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null);

		/// <inheritdoc/>
		Task<IGetIndexTemplateResponse> GetIndexTemplateAsync(IGetIndexTemplateRequest request);

	}

	//TODO discuss with @gmarz changing this and other methods that can actually return multiple to plural form e.g GetIndexTemplates/GetIndexTemplates
	
	public partial class ElasticClient
	{
		/// <inheritdoc/>
		public IGetIndexTemplateResponse GetIndexTemplate(Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null) =>
			this.GetIndexTemplate(selector.InvokeOrDefault(new GetIndexTemplateDescriptor()));
		
		/// <inheritdoc/>
		public IGetIndexTemplateResponse GetIndexTemplate(IGetIndexTemplateRequest request)
		{
			return this.Dispatcher.Dispatch<IGetIndexTemplateRequest, GetIndexTemplateRequestParameters, GetIndexTemplateResponse>(
				request,
				new GetIndexTemplateConverter(DeserializeGetIndexTemplateResponse),
				(p, d) => this.LowLevelDispatch.IndicesGetTemplateDispatch<GetIndexTemplateResponse>(p)
			);
		}

		/// <inheritdoc/>
		public Task<IGetIndexTemplateResponse> GetIndexTemplateAsync(Func<GetIndexTemplateDescriptor, IGetIndexTemplateRequest> selector = null) =>
			this.GetIndexTemplateAsync(selector.InvokeOrDefault(new GetIndexTemplateDescriptor()));

		/// <inheritdoc/>
		public Task<IGetIndexTemplateResponse> GetIndexTemplateAsync(IGetIndexTemplateRequest request) => 
			this.Dispatcher.DispatchAsync<IGetIndexTemplateRequest, GetIndexTemplateRequestParameters, GetIndexTemplateResponse, IGetIndexTemplateResponse>(
				request,
				new GetIndexTemplateConverter(DeserializeGetIndexTemplateResponse),
				(p, d) => this.LowLevelDispatch.IndicesGetTemplateDispatchAsync<GetIndexTemplateResponse>(p)
			);

		//TODO DictionaryResponse!
		private GetIndexTemplateResponse DeserializeGetIndexTemplateResponse(IApiCallDetails response, Stream stream)
		{
			if (!response.Success) return new GetIndexTemplateResponse();

			var dict = this.Serializer.Deserialize<Dictionary<string, TemplateMapping>>(stream);
			if (dict.Count == 0)
				throw new DslException("Could not deserialize TemplateMapping");

			return new GetIndexTemplateResponse
			{
				Name = dict.First().Key,
				TemplateMapping = dict.First().Value
			};
		}
	}
}