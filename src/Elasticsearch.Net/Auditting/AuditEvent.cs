namespace Elasticsearch.Net.Connection
{
	//TODO SNIFFONFAIL AND SKIPNODE ARE NEVER USED
	public enum AuditEvent
	{
		SniffOnStartup,
		SniffOnFail,
		SniffOnStaleCluster,

		SniffSuccess,
		SniffFailure,
		PingSuccess,
		PingFailure,

		SkipNode,
		BadResponse,
		HealthyResponse
	}
}