﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<SpanContainingQuery>))]
	public interface ISpanContainingQuery : ISpanSubQuery
	{
		[JsonProperty(PropertyName = "little")]
		ISpanQuery Little { get; set; }

		[JsonProperty(PropertyName = "big")]
		ISpanQuery Big { get; set; }

	}

	public class SpanContainingQuery : QueryBase, ISpanContainingQuery
	{
		bool IQuery.Conditionless => IsConditionless(this);
		public ISpanQuery Little { get; set; }
		public ISpanQuery Big { get; set; }

		protected override void WrapInContainer(IQueryContainer c) => c.SpanContaining = this;
		internal static bool IsConditionless(ISpanContainingQuery q)
		{
			var exclude = q.Little as IQuery;
			var include = q.Big as IQuery;

			return (exclude == null && include == null)
				|| (include == null && exclude.Conditionless)
				|| (exclude == null && include.Conditionless)
				|| (exclude != null && exclude.Conditionless && include != null && include.Conditionless);
		}
	}

	public class SpanContainingQueryDescriptor<T>
		: QueryDescriptorBase<SpanContainingQueryDescriptor<T>, ISpanContainingQuery>
		, ISpanContainingQuery where T : class
	{
		bool IQuery.Conditionless => SpanContainingQuery.IsConditionless(this);
		ISpanQuery ISpanContainingQuery.Little { get; set; }
		ISpanQuery ISpanContainingQuery.Big { get; set; }

		public SpanContainingQueryDescriptor<T> Little(Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>> selector) =>
			Assign(a => a.Little = selector(new SpanQueryDescriptor<T>()));

		public SpanContainingQueryDescriptor<T> Big(Func<SpanQueryDescriptor<T>, SpanQueryDescriptor<T>> selector) =>
			Assign(a => a.Big = selector(new SpanQueryDescriptor<T>()));

	}
}
