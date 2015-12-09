﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Elasticsearch.Net.ConnectionPool;
using Elasticsearch.Net.Serialization;
using Elasticsearch.Net.Connection.Security;
using Elasticsearch.Net.Extensions;

namespace Elasticsearch.Net.Connection
{
	/// <summary>
	/// ConnectionConfiguration allows you to control how ElasticsearchClient behaves and where/how it connects 
	/// to elasticsearch
	/// </summary>
	public class ConnectionConfiguration : ConnectionConfiguration<ConnectionConfiguration>
	{

		public static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);
		public static TimeSpan DefaultPingTimeout = TimeSpan.FromSeconds(2);
		public static TimeSpan DefaultPingTimeoutOnSSL = TimeSpan.FromSeconds(5);

		/// <summary>
		/// ConnectionConfiguration allows you to control how ElasticsearchClient behaves and where/how it connects 
		/// to elasticsearch
		/// </summary>
		/// <param name="uri">The root of the elasticsearch node we want to connect to. Defaults to http://localhost:9200</param>
		public ConnectionConfiguration(Uri uri = null)
			: this(new SingleNodeConnectionPool(uri ?? new Uri("http://localhost:9200"))) { }

		/// <summary>
		/// ConnectionConfiguration allows you to control how ElasticsearchClient behaves and where/how it connects 
		/// to elasticsearch
		/// </summary>
		/// <param name="connectionPool">A connection pool implementation that'll tell the client what nodes are available</param>
		public ConnectionConfiguration(IConnectionPool connectionPool)
			// ReSharper disable once IntroduceOptionalParameters.Global 
			: this(connectionPool, null, null) { }

		public ConnectionConfiguration(IConnectionPool connectionPool, IConnection connection)
			// ReSharper disable once IntroduceOptionalParameters.Global
			: this(connectionPool, connection, null) { }

		public ConnectionConfiguration(IConnectionPool connectionPool, IElasticsearchSerializer serializer)
			: this(connectionPool, null, serializer) { }

		// ReSharper disable once MemberCanBePrivate.Global
		// eventhough we use don't use this we very much would like to  expose this constructor
		public ConnectionConfiguration(IConnectionPool connectionPool, IConnection connection, IElasticsearchSerializer serializer)
			: base(connectionPool, connection, serializer) { }
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ConnectionConfiguration<T> : IConnectionConfigurationValues, IHideObjectMembers
		where T : ConnectionConfiguration<T>
	{
		private TimeSpan _timeout;
		TimeSpan IConnectionConfigurationValues.Timeout => _timeout;

		private TimeSpan? _pingTimeout;
		TimeSpan? IConnectionConfigurationValues.PingTimeout => _pingTimeout;

		private TimeSpan? _connectTimeout;
		TimeSpan? IConnectionConfigurationValues.ConnectTimeout => _connectTimeout;

		private TimeSpan? _deadTimeout;
		TimeSpan? IConnectionConfigurationValues.DeadTimeout => _deadTimeout;

		private TimeSpan? _maxDeadTimeout;
		TimeSpan? IConnectionConfigurationValues.MaxDeadTimeout => _maxDeadTimeout;

		private TimeSpan? _maxRetryTimeout;
		TimeSpan? IConnectionConfigurationValues.MaxRetryTimeout => _maxRetryTimeout;

		private TimeSpan? _keepAliveTime;
		TimeSpan? IConnectionConfigurationValues.KeepAliveTime => _keepAliveTime;

		private TimeSpan? _keepAliveInterval;
		TimeSpan? IConnectionConfigurationValues.KeepAliveInterval => _keepAliveInterval;

		private string _proxyUsername;
		string IConnectionConfigurationValues.ProxyUsername => _proxyUsername;

		private  string _proxyPassword;
		string IConnectionConfigurationValues.ProxyPassword => _proxyPassword;

		private bool _disablePings;
		bool IConnectionConfigurationValues.DisablePings => _disablePings;

		private string _proxyAddress;
		string IConnectionConfigurationValues.ProxyAddress => _proxyAddress;

		private bool _prettyJson;
		bool IConnectionConfigurationValues.PrettyJson => _prettyJson;

		private bool _disableDirectStreaming = false;
		bool IConnectionConfigurationValues.DisableDirectStreaming => _disableDirectStreaming;

		private bool _enableMetrics = false;
		bool IConnectionConfigurationValues.MetricsEnabled => _enableMetrics;

		private bool _disableAutomaticProxyDetection = false;
		bool IConnectionConfigurationValues.DisableAutomaticProxyDetection => _disableAutomaticProxyDetection;

		private int? _maxRetries;
		int? IConnectionConfigurationValues.MaxRetries => _maxRetries;

		private bool _sniffOnStartup;
		bool IConnectionConfigurationValues.SniffsOnStartup => _sniffOnStartup;

		private bool _sniffOnConnectionFault;
		bool IConnectionConfigurationValues.SniffsOnConnectionFault => _sniffOnConnectionFault;

		private TimeSpan? _sniffLifeSpan;
		TimeSpan? IConnectionConfigurationValues.SniffInformationLifeSpan => _sniffLifeSpan;

		private bool _enableHttpCompression;
		bool IConnectionConfigurationValues.EnableHttpCompression => _enableHttpCompression;

		private bool _traceEnabled;
		bool IConnectionConfigurationValues.TraceEnabled => _traceEnabled;

		private bool _httpPipeliningEnabled;
		bool IConnectionConfigurationValues.HttpPipeliningEnabled => _httpPipeliningEnabled;

		private bool _throwOnServerExceptions;
		bool IConnectionConfigurationValues.ThrowOnElasticsearchServerExceptions => _throwOnServerExceptions;

		private static void DefaultApiCallHandler(IApiCallDetails status) {}
		Action<IApiCallDetails> _apiCallHandler = DefaultApiCallHandler;
		Action<IApiCallDetails> IConnectionConfigurationValues.ApiCallHandler => _apiCallHandler;

		private NameValueCollection _queryString = new NameValueCollection();
		NameValueCollection IConnectionConfigurationValues.QueryStringParameters => _queryString;

		private NameValueCollection _headers = new NameValueCollection();
		NameValueCollection IConnectionConfigurationValues.Headers => _headers;

		BasicAuthenticationCredentials _basicAuthCredentials;
		BasicAuthenticationCredentials IConnectionConfigurationValues.BasicAuthenticationCredentials => _basicAuthCredentials;

		/* */

		private readonly IElasticsearchSerializer _serializer;
		IElasticsearchSerializer IConnectionConfigurationValues.Serializer => _serializer;

		private readonly IConnectionPool _connectionPool;
		IConnectionPool IConnectionConfigurationValues.ConnectionPool => _connectionPool;

		private readonly IConnection _connection;
		IConnection IConnectionConfigurationValues.Connection => _connection;

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Potential Code Quality Issues", "RECS0021:Warns about calls to virtual member functions occuring in the constructor", 
			Justification = "We want the virtual method to run on most derived")]
		protected ConnectionConfiguration(IConnectionPool connectionPool, IConnection connection, IElasticsearchSerializer serializer)
		{
			this._connectionPool = connectionPool;
			this._connection = connection ?? new HttpConnection();
			// ReSharper disable once VirtualMemberCallInContructor
			this._serializer = serializer ?? this.DefaultSerializer();

			this._timeout = ConnectionConfiguration.DefaultTimeout;
			this._sniffOnConnectionFault = true;
			this._sniffOnStartup = true;
			this._sniffLifeSpan = TimeSpan.FromHours(1);
		}

		T Assign(Action<ConnectionConfiguration<T>> assigner) => Fluent.Assign((T)this, assigner);

		protected virtual IElasticsearchSerializer DefaultSerializer() => new ElasticsearchDefaultSerializer();

		public T EnableTcpKeepAlive(TimeSpan keepAliveTime, TimeSpan keepAliveInterval) =>
			Assign(a => { this._keepAliveTime = keepAliveTime; this._keepAliveInterval = keepAliveInterval; });

		public T MaximumRetries(int maxRetries) => Assign(a => a._maxRetries = maxRetries);

		/// <summary>
		/// On connection pools that support reseeding setting this to true (default) will resniff the cluster when a call fails
		/// </summary>
		public T SniffOnConnectionFault(bool sniffsOnConnectionFault = true) => Assign(a => a._sniffOnConnectionFault = sniffsOnConnectionFault);

		/// <summary>
		/// Enables sniffing on first usage of a connection pool if that pool supports reseeding, defaults to true
		/// </summary>
		public T SniffOnStartup(bool sniffsOnStartup = true) => Assign(a => a._sniffOnStartup = sniffsOnStartup);

		/// <summary>
		/// Set the duration after which a cluster state is considered stale and a sniff should be performed again.
		/// An IConnectionPool has to signal it supports reseeding otherwise sniffing will never happen.
		/// Defaults to 1 hour.
		/// Set to null to disable completely. Sniffing will only ever happen on ConnectionPools that return true for SupportsReseeding
		/// </summary>
		/// <param name="sniffLifeSpan">The duration a clusterstate is considered fresh, set to null to disable periodic sniffing</param>
		public T SniffLifeSpan(TimeSpan? sniffLifeSpan) => Assign(a => a._sniffLifeSpan = sniffLifeSpan);

		/// <summary>
		/// Enable gzip compressed requests and responses, do note that you need to configure elasticsearch to set this
		/// <para>http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/modules-http.html"</para>
		/// </summary>
		public T EnableHttpCompression(bool enabled = true) => Assign(a => a._enableHttpCompression = enabled);

		/// <summary>
		/// Enable Trace signals to the IConnection that it should put debug information on the Trace.
		/// </summary>
		public T EnableTrace(bool enabled = true) => Assign(a => this._traceEnabled = enabled);

		public T DisableAutomaticProxyDetection(bool disable = true) => Assign(a => a._disableAutomaticProxyDetection = disable);

		/// <summary>
		/// By enabling metrics more metadata is returned per API call about requests (ping, sniff, failover) and general stats
		/// </summary>
		public T EnableMetrics(bool enabled = true) => Assign(a => a._enableMetrics = enabled);

		//TODO this summary is most likely going out of date when we refactor exceptions
		/// <summary>
		/// Instead of following a c/go like error checking on response.IsValid always throw an ElasticsearchServerException
		/// on the client when a call resulted in an exception on the elasticsearch server. 
		/// <para>Reasons for such exceptions could be search parser errors, index missing exceptions</para>
		/// </summary>
		public T ThrowOnElasticsearchServerExceptions(bool alwaysThrow = true) => Assign(a => a._throwOnServerExceptions = alwaysThrow);

		/// <summary>
		/// When a node is used for the very first time or when it's used for the first time after it has been marked dead
		/// a ping with a very low timeout is send to the node to make sure that when it's still dead it reports it as fast as possible.
		/// You can disable these pings globally here if you rather have it fail on the possible slower original request
		/// </summary>
		public T DisablePing(bool disable = true) => Assign(a => a._disablePings = disable);

		/// <summary>
		/// This NameValueCollection will be appended to every url NEST calls, great if you need to pass i.e an API key.
		/// </summary>
		public T SetGlobalQueryStringParameters(NameValueCollection queryStringParameters) => Assign(a => a._queryString.Add(queryStringParameters));

		/// <summary>
		/// a NameValueCollection that will be send as headers for each request
		/// </summary>
		public T SetGlobalHeaders(NameValueCollection headers) => Assign(a => a._headers.Add(headers));

		/// <summary>
		/// Sets the default timeout in milliseconds for each request to Elasticsearch.
		/// NOTE: You can set this to a high value here, and specify the timeout on Elasticsearch's side.
		/// </summary>
		/// <param name="timeout">time out in milliseconds</param>
		public T SetTimeout(TimeSpan timeout) => Assign(a => a._timeout = timeout);

		/// <summary>
		/// Sets the default ping timeout in milliseconds for ping requests, which are used
		/// to determine whether a node is alive. Pings should fail as fast as possible.
		/// </summary>
		/// <param name="timeout">The ping timeout in milliseconds defaults to 1000, or 2000 is using SSL.</param>
		public T SetPingTimeout(TimeSpan timeout) => Assign(a => a._pingTimeout = timeout);

		/// <summary>
		/// Sets the default connection timeout in milliseconds.
		/// </summary>
		public T SetConnectTimeout(TimeSpan timeout) => Assign(a => a._connectTimeout = timeout);

		/// <summary>
		/// Sets the default dead timeout factor when a node has been marked dead.
		/// </summary>
		/// <remarks>Some connection pools may use a flat timeout whilst others take this factor and increase it exponentially</remarks>
		/// <param name="timeout"></param>
		public T SetDeadTimeout(TimeSpan timeout) => Assign(a => a._deadTimeout = timeout);

		/// <summary>
		/// Sets the maximum time a node can be marked dead. 
		/// Different implementations of IConnectionPool may choose a different default.
		/// </summary>
		/// <param name="timeout">The timeout in milliseconds</param>
		public T SetMaxDeadTimeout(TimeSpan timeout) => Assign(a => a._maxDeadTimeout = timeout);

		/// <summary>
		/// Limits the total runtime including retries separately from <see cref="SetTimeout"/>
		/// <pre>
		/// When not specified defaults to <see cref="SetTimeout"/> which itself defaults to 60seconds
		/// </pre>
		/// </summary>
		public T SetMaxRetryTimeout(TimeSpan maxRetryTimeout) => Assign(a => a._maxRetryTimeout = maxRetryTimeout);

		/// <summary>
		/// If your connection has to go through proxy use this method to specify the proxy url
		/// </summary>
		public T SetProxy(Uri proxyAdress, string username, string password)
		{
			proxyAdress.ThrowIfNull(nameof(proxyAdress));
			this._proxyAddress = proxyAdress.ToString();
			this._proxyUsername = username;
			this._proxyPassword = password;
			return (T)this;
		}

		/// <summary>
		/// Forces all requests to have ?pretty=true, causing elasticsearch to return formatted json. 
		/// Also forces the client to send out formatted json. Defaults to false
		/// </summary>
		public T PrettyJson(bool b = true) => Assign(a => {
			this._prettyJson = b;
			if (!b && this._queryString["pretty"] != null) this._queryString.Remove("pretty");
			else if (b && this._queryString["pretty"] == null)
				this.SetGlobalQueryStringParameters(new NameValueCollection { { "pretty", b.ToString().ToLowerInvariant() } });
        });

		/// <summary>
		/// Make sure the reponse bytes are always available on the ElasticsearchResponse object
		/// <para>Note: that depending on the registered serializer this may cause the respond to be read in memory first</para>
		/// </summary>
		public T DisableDirectStreaming(bool b = true) => Assign(a => a._disableDirectStreaming = b);


		/// <summary>
		/// Global callback for every response that NEST receives, useful for custom logging.
		/// Calling this multiple times will register multiple listeners
		/// </summary>
		public T SetConnectionStatusHandler(Action<IApiCallDetails> handler) =>
			Assign(a => a._apiCallHandler += handler ?? DefaultApiCallHandler);

		/// <summary>
		/// Basic access authentication credentials to specify with all requests.
		/// </summary>
		public T SetBasicAuthentication(string userName, string password)
		{
			this._basicAuthCredentials = new BasicAuthenticationCredentials
			{
				UserName = userName,
				Password = password
			};
			return (T)this;
		}

		/// <summary>
		/// Allows for requests to be pipelined. http://en.wikipedia.org/wiki/HTTP_pipelining
		/// <para>Note: HTTP pipelining must also be enabled in Elasticsearch for this to work properly.</para>
		/// </summary>
		public T HttpPipeliningEnabled(bool enabled = true) => Assign(a => a._httpPipeliningEnabled = enabled);
	}
}

