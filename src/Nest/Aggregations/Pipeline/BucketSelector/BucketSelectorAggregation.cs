﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<BucketSelectorAggregation>))]
	public interface IBucketSelectorAggregation : IPipelineAggregation
	{
		[JsonProperty("script")]
		IScript Script { get; set; }
	}

	public class BucketSelectorAggregation
		: PipelineAggregationBase, IBucketSelectorAggregation
	{
		public IScript Script { get; set; }

		internal BucketSelectorAggregation () { }

		public BucketSelectorAggregation(string name, MultiBucketsPath bucketsPath)
			: base(name, bucketsPath) { }

		internal override void WrapInContainer(AggregationContainer c) => c.BucketSelector = this;
	}

	public class BucketSelectorAggregationDescriptor
		: PipelineAggregationDescriptorBase<BucketSelectorAggregationDescriptor, IBucketSelectorAggregation, MultiBucketsPath>
		, IBucketSelectorAggregation
	{
		IScript IBucketSelectorAggregation.Script { get; set; }

		public BucketSelectorAggregationDescriptor Script(string script) => Assign(a => a.Script = (InlineScript)script);

		public BucketSelectorAggregationDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) =>
			Assign(a => a.Script = scriptSelector?.Invoke(new ScriptDescriptor()));

		public BucketSelectorAggregationDescriptor BucketsPath(Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>> selector) =>
			Assign(a => a.BucketsPath = selector?.Invoke(new MultiBucketsPathDescriptor())?.Value);
	}
}
