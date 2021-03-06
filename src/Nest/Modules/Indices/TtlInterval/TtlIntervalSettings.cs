using System;
using System.Reflection;

namespace Nest
{
	public interface ITtlIntervalSettings
	{
		/// <summary>How often the deletion process runs. Defaults to 60s.</summary>
		TimeUnitExpression Interval { get; set; }

		/// <summary>The deletions are processed with a bulk request. The number of deletions processed can be configured with this settings. Defaults to 10000.</summary>
		int? BulkSize { get; set; }
	}

	public class TtlIntervalSettings : ITtlIntervalSettings
	{
		/// <inheritdoc/>
		public TimeUnitExpression Interval { get; set; }

		/// <summary> defaults to 2</summary>
		public int? BulkSize { get; set; }
	}

	public class TtlIntervalSettingsDescriptor 
		: DescriptorBase<TtlIntervalSettingsDescriptor, ITtlIntervalSettings>, ITtlIntervalSettings
	{
		TimeUnitExpression ITtlIntervalSettings.Interval { get; set; }
		
		int? ITtlIntervalSettings.BulkSize { get; set; }

		/// <inheritdoc/>
		public TtlIntervalSettingsDescriptor Interval(TimeUnitExpression interval) => Assign(a => a.Interval = interval);

		/// <inheritdoc/>
		public TtlIntervalSettingsDescriptor BulkSize(int? bulkSize) => Assign(a => a.BulkSize = bulkSize);

	}
}