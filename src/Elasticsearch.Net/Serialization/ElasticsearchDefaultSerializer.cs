using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.Net.Serialization
{
	public class ElasticsearchDefaultSerializer : IElasticsearchSerializer
	{
		private static readonly ElasticsearchNetJsonStrategy Strategy = new ElasticsearchNetJsonStrategy();

		public T Deserialize<T>(Stream stream)
		{
			if (stream == null) return default(T);

			using (var ms = new MemoryStream())
			using(stream)
			{
				stream.CopyTo(ms);
				byte[] buffer = ms.ToArray();
				if (buffer.Length <= 1)
					return default(T);
				return SimpleJson.DeserializeObject<T>(buffer.Utf8String(), ElasticsearchDefaultSerializer.Strategy);
			}
		}

		public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (stream == null)
				return default(T);
			
			using (var ms = new MemoryStream())
			using (stream)
			{
				await stream.CopyToAsync(ms, 8096, cancellationToken);
				var buffer = ms.ToArray();
				if (buffer.Length <= 1)
					return default(T);
				var r = SimpleJson.DeserializeObject<T>(buffer.Utf8String(), ElasticsearchDefaultSerializer.Strategy);
				return r;
			}
		}

		public void Serialize(object data, Stream writableStream, SerializationFormatting formatting = SerializationFormatting.Indented)
		{
			var serialized = SimpleJson.SerializeObject(data);
			if (formatting == SerializationFormatting.None)
				serialized = RemoveNewLinesAndTabs(serialized);
			using (var ms = new MemoryStream(serialized.Utf8Bytes()))
			{
				ms.CopyTo(writableStream);
			}
		}

		public static string RemoveNewLinesAndTabs(string input)
		{
			return new string(input
				.Where(c => c != '\r' && c != '\n')
				.ToArray());
		}
	}
}