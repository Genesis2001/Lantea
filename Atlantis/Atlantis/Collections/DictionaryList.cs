// -----------------------------------------------------------------------------
//  <copyright file="DictionaryList.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Collections
{
	using System;
	using System.Collections.Generic;

	public class DictionaryList<T>
	{
		private readonly Dictionary<string, List<T>> dict = new Dictionary<string, List<T>>();

		public void Add(string key, T value)
		{
			List<T> values;
			if (!dict.TryGetValue(key, out values))
			{
				values = new List<T> {value};
				dict[key] = values;
			}
			else
			{
				values.Add(value);
			}
		}

		public void Add(KeyValuePair<string, T> pair)
		{
			List<T> values;
			if (!dict.TryGetValue(pair.Key, out values))
			{
				values = new List<T>();
				dict[pair.Key] = values;
			}

			if (!values.Contains(pair.Value))
			{
				values.Add(pair.Value);
			}
		}

		public bool Remove(string key, T value)
		{
			List<T> values;

			return dict.TryGetValue(key, out values) && values.Remove(value);
		}

		public IEnumerable<String> Keys
		{
			get { return dict.Keys; }
		}

		public List<T> this[string key]
		{
			get
			{
				List<T> values;
				if (!dict.TryGetValue(key, out values))
				{
					values = new List<T>();
					dict[key] = values;
				}

				return values;
			}
		}
	}
}