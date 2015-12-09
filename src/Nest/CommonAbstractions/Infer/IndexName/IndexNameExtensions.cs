﻿using Nest.Resolvers;

namespace Nest
{
	public static class IndexNameExtensions
	{
		
		public static string Resolve(this IndexName marker, IConnectionSettingsValues connectionSettings)
		{
			if (marker == null)
				return null;
			connectionSettings.ThrowIfNull("connectionSettings");

			if (marker.Type == null)
				return marker.Name;
			return new IndexNameResolver(connectionSettings).GetIndexForType(marker.Type);
		}
	}
}