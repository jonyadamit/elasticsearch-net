using Elasticsearch.Net.Connection.Security;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Elasticsearch.Net.Connection.Configuration
{
	public class RequestConfigurationDescriptor : IRequestConfiguration
	{
		private IRequestConfiguration Self => this;

		TimeSpan? IRequestConfiguration.RequestTimeout { get; set; }

		TimeSpan? IRequestConfiguration.ConnectTimeout { get; set; }
	
		string IRequestConfiguration.ContentType { get; set; }
		
		int? IRequestConfiguration.MaxRetries { get; set; }
		
		Uri IRequestConfiguration.ForceNode { get; set; }
		
		bool? IRequestConfiguration.DisableSniff { get; set; }
		
		bool? IRequestConfiguration.DisablePing { get; set; }
		
		IEnumerable<int> IRequestConfiguration.AllowedStatusCodes { get; set; }

		BasicAuthenticationCredentials IRequestConfiguration.BasicAuthenticationCredentials { get; set; }

		bool IRequestConfiguration.EnableHttpPipelining { get; set; }

		CancellationToken IRequestConfiguration.CancellationToken { get; set; }

		//TODO none of these request overrides are called from tests meaning these ALL need to have tests written against

		public RequestConfigurationDescriptor RequestTimeout(TimeSpan requestTimeout)
		{
			Self.RequestTimeout = requestTimeout;
			return this;
		}

		public RequestConfigurationDescriptor ConnectTimeout(TimeSpan connectTimeout)
		{
			Self.ConnectTimeout = connectTimeout;
			return this;
		}

		public RequestConfigurationDescriptor AcceptContentType(string acceptContentTypeHeader)
		{
			Self.ContentType = acceptContentTypeHeader;
			return this;
		}

		public RequestConfigurationDescriptor AllowedStatusCodes(IEnumerable<int> codes)
		{
			Self.AllowedStatusCodes = codes;
			return this;
		}

		public RequestConfigurationDescriptor AllowedStatusCodes(params int[] codes)
		{
			Self.AllowedStatusCodes = codes;
			return this;
		}

		public RequestConfigurationDescriptor DisableSniffing(bool? disable = true)
		{
			Self.DisableSniff = disable;
			return this;
		}

		public RequestConfigurationDescriptor DisablePing(bool? disable = true)
		{
			Self.DisablePing = disable;
			return this;
		}

		public RequestConfigurationDescriptor ForceNode(Uri uri)
		{
			Self.ForceNode = uri;
			return this;
		}
		public RequestConfigurationDescriptor MaxRetries(int retry)
		{
			Self.MaxRetries = retry;
			return this;
		}
		public RequestConfigurationDescriptor CancellationToken(CancellationToken token)
		{
			Self.CancellationToken = token;
			return this;
		}

		public RequestConfigurationDescriptor BasicAuthentication(string userName, string password)
		{
			if (Self.BasicAuthenticationCredentials == null)
				Self.BasicAuthenticationCredentials = new BasicAuthenticationCredentials();
			Self.BasicAuthenticationCredentials.UserName = userName;
			Self.BasicAuthenticationCredentials.Password = password;
			return this;
		}

		public RequestConfigurationDescriptor EnableHttpPipelining(bool enable = true)
		{
			Self.EnableHttpPipelining = enable;
			return this;
		}
	}
}