﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof (FieldNameQueryJsonConverter<PrefixQuery>))]
	public interface IPrefixQuery : ITermQuery
	{
		[JsonProperty(PropertyName = "rewrite")]
		[JsonConverter(typeof (StringEnumConverter))]
		RewriteMultiTerm? Rewrite { get; set; }
	}

	public class PrefixQuery : FieldNameQueryBase, IPrefixQuery
	{
		bool IQuery.Conditionless => TermQuery.IsConditionless(this);
		public object Value { get; set; }
		public RewriteMultiTerm? Rewrite { get; set; }

		protected override void WrapInContainer(IQueryContainer c) => c.Prefix = this;
	}

	public class PrefixQueryDescriptor<T> : TermQueryDescriptorBase<PrefixQueryDescriptor<T>, T>, 
		IPrefixQuery where T : class
	{
		RewriteMultiTerm? IPrefixQuery.Rewrite { get; set; }

		public PrefixQueryDescriptor<T> Rewrite(RewriteMultiTerm rewrite)
		{
			((IPrefixQuery)this).Rewrite = rewrite;
			return this;
		}
	}
}
