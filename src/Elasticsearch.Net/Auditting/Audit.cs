using System;

namespace Elasticsearch.Net.Connection
{
	public class Audit
	{
		public AuditEvent Event { get; internal set; }
		public DateTime Started { get; }
		public DateTime Ended { get; internal set; }
		public Node Node { get; internal set; }
		public string Path { get; internal set; }

		public Audit(AuditEvent type, DateTime occured)
		{
			this.Event = type;
			this.Started = occured;
		}

		public override string ToString()
		{
			var took = Started - Ended;
			return $"Node: {Node?.Uri}, Event: {Event.GetStringValue()} NodeAlive: {Node?.IsAlive}, Took: {took.ToString()}";
		}
	}
}