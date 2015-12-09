﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using static Nest.Static;

namespace Tests.QueryDsl.Geo.Shape.Circle
{
	public class GeoShapeCircleUsageTests : ShapeQueryUsageTestsBase
	{
		public GeoShapeCircleUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		private readonly IEnumerable<double> _coordinates = new[] {-45.0, 45.0};

		protected override object ShapeJson => new
		{
			type ="circle",
			radius = "100m",
			coordinates = this._coordinates
		};

		protected override QueryContainer QueryInitializer => new GeoShapeCircleQuery
		{
			Name = "named_query",
			Boost = 1.1,
			Field = Field<Project>(p=>p.Location),
			Shape = new CircleGeoShape(this._coordinates) { Radius = "100m" }
		};

		protected override QueryContainer QueryFluent(QueryContainerDescriptor<Project> q) => q
			.GeoShapeCircle(c => c
				.Name("named_query")
				.Boost(1.1)
				.Field(p=>p.Location)
				.Coordinates(this._coordinates)
				.Radius("100m")
			);
	}
}
