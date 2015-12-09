﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Nest
{
	[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter<NamedFiltersContainer, string, IQueryContainer>))]
	public interface INamedFiltersContainer : IIsADictionary<string, IQueryContainer>
	{
	}

	public class NamedFiltersContainer: IsADictionary<string, IQueryContainer>, INamedFiltersContainer
	{
		public NamedFiltersContainer() : base() { }
		public NamedFiltersContainer(IDictionary<string, IQueryContainer> container) : base(container) { }
		public NamedFiltersContainer(Dictionary<string, QueryContainer> container)
			: base(container.Select(kv => kv).ToDictionary(kv => kv.Key, kv => (IQueryContainer)kv.Value))
		{ }

		public void Add(string name, IQueryContainer filter) => BackingDictionary.Add(name, filter);
		public void Add(string name, QueryContainer filter) => BackingDictionary.Add(name, filter);
	}

	public class NamedFiltersContainerDescriptor<T> : IsADictionaryDescriptor<NamedFiltersContainerDescriptor<T>, INamedFiltersContainer, string, IQueryContainer>
		where T : class
	{
		public NamedFiltersContainerDescriptor() : base(new NamedFiltersContainer()) { }

		public NamedFiltersContainerDescriptor<T> Filter(string name, IQueryContainer filter) => Assign(name, filter);

		public NamedFiltersContainerDescriptor<T> Filter(string name, Func<QueryContainerDescriptor<T>, QueryContainer> selector) =>
			Assign(name, selector?.InvokeQuery(new QueryContainerDescriptor<T>()));

		public NamedFiltersContainerDescriptor<T> Filter<TOther>(string name, Func<QueryContainerDescriptor<TOther>, QueryContainer> selector)
			where TOther : class =>
			Assign(name, selector?.InvokeQuery(new QueryContainerDescriptor<TOther>()));

	}

}
