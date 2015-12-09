﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nest.Domain;
using Newtonsoft.Json;

namespace Nest
{
	public interface IMultiGetResponse : IResponse
	{
		IEnumerable<IMultiGetHit<object>> Documents { get; }
		MultiGetHit<T> Get<T>(string id) where T : class;
		MultiGetHit<T> Get<T>(long id) where T : class;
		T Source<T>(string id) where T : class;
		T Source<T>(long id) where T : class;
		IFieldSelection<T> GetFieldSelection<T>(string id) where T : class;
		IFieldSelection<T> GetFieldSelection<T>(long id) where T : class;
		IEnumerable<T> SourceMany<T>(IEnumerable<string> ids) where T : class;
		IEnumerable<T> SourceMany<T>(IEnumerable<long> ids) where T : class;
		IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<string> ids) where T : class;
		IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<long> ids) where T : class;
	}

	[JsonObject]
	//TODO validate this, ported over from ElasticContractResolver but it seems out of place
	[ContractJsonConverter(typeof(MultiGetHitJsonConverter))]
	public class MultiGetResponse : BaseResponse, IMultiGetResponse
	{
		public MultiGetResponse()
		{
			this._Documents = new List<IMultiGetHit<object>>();
		}

		internal ICollection<IMultiGetHit<object>> _Documents { get; set; }

		public IEnumerable<IMultiGetHit<object>> Documents { get { return this._Documents.ToList(); } }


		public MultiGetHit<T> Get<T>(string id) where T : class
		{
			return this.Documents.OfType<MultiGetHit<T>>().FirstOrDefault(m => m.Id == id);
		}
		public MultiGetHit<T> Get<T>(long id) where T : class
		{
			return this.Get<T>(id.ToString(CultureInfo.InvariantCulture));
		}
		public T Source<T>(string id) where T : class
		{
			var multiHit = this.Get<T>(id);
			if (multiHit == null)
				return null;
			return multiHit.Source;
		}
		public T Source<T>(long id) where T : class
		{
			return this.Source<T>(id.ToString(CultureInfo.InvariantCulture));
		}
		public IEnumerable<T> SourceMany<T>(IEnumerable<string> ids) where T : class
		{
			var docs = this.Documents.OfType<IMultiGetHit<T>>();
			return from d in docs
				join id in ids on d.Id equals id
				where d.Found
				select d.Source;
		}
		public IEnumerable<T> SourceMany<T>(IEnumerable<long> ids) where T : class
		{
			return this.SourceMany<T>(ids.Select(i=>i.ToString(CultureInfo.InvariantCulture)));
		}
		
		public IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<string> ids) where T : class
		{
			var docs = this.Documents.OfType<IMultiGetHit<T>>();
			return from d in docs
				join id in ids on d.Id equals id
				select d;
		}
		public IEnumerable<IMultiGetHit<T>> GetMany<T>(IEnumerable<long> ids) where T : class
		{
			return this.GetMany<T>(ids.Select(i=>i.ToString(CultureInfo.InvariantCulture)));
		}
		public IFieldSelection<T> GetFieldSelection<T>(string id) where T : class
		{
			var multiHit = this.Get<T>(id);
			if (multiHit == null)
				return null;
			return multiHit.FieldSelection;
		}
		public IFieldSelection<T> GetFieldSelection<T>(long id) where T : class
		{
			return this.GetFieldSelection<T>(id.ToString(CultureInfo.InvariantCulture));
		}
	}
}
