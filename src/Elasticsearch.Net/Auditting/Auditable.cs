using Elasticsearch.Net.Providers;
using System;
using System.Collections.Generic;

namespace Elasticsearch.Net.Connection
{
	internal class Auditable : IDisposable
	{
		readonly IDateTimeProvider _dateTimeProvider;
		private readonly Audit _audit;

		public Node Node { set { this._audit.Node = value; } }
		public AuditEvent Event { set { this._audit.Event = value; } }
		public string Path { set { this._audit.Path = value; } }

		public Auditable(AuditEvent type, List<Audit> auditTrail, IDateTimeProvider dateTimeProvider)
		{
			this._dateTimeProvider = dateTimeProvider;
			var started = _dateTimeProvider.Now();

			this._audit = new Audit(type, started);
			auditTrail.Add(this._audit);
		}

		public void Dispose()
		{
			this._audit.Ended = this._dateTimeProvider.Now();
		}
	}
}