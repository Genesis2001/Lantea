// -----------------------------------------------------------------------------
//  <copyright file="Block.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Atlantis.Collections;

	public class Block
	{
		/* 
		 * template<typename T> class map : public std::map<string, T, ci::less> { };				// Dictionary<String, T>
		 * template<typename T> class multimap : public std::multimap<string, T, ci::less> { };		// Dictionary<String, List<T>>
		 * 
		 * item_map items;
		 * block_map blocks;
		 */

		internal readonly DictionaryList<Block> blocks;
		internal readonly Dictionary<string, string> items;
		internal int lineNumber;

		internal Block(string name)
		{
			Name   = name;
			items  = new Dictionary<string, string>();
			blocks = new DictionaryList<Block>();
		}

		public string Name { get; private set; }

		public int CountBlock(string blockName)
		{
			if (blockName == null)
			{
				throw new ArgumentNullException("blockName");
			}

			return !blocks.ContainsKey(blockName) ? 0 : blocks[blockName].Count;
		}

		public T Get<T>(string property, T def = default(T))
		{
			if (items.ContainsKey(property))
			{
				string value = items[property];

				return (T)Convert.ChangeType(value, typeof (T));
			}

			return def;
		}

		public Block GetBlock(string blockName, int num = 0)
		{
			if (blockName == null) throw new ArgumentNullException("blockName");
			if (!blocks.ContainsKey(blockName)) throw new KeyNotFoundException("The specified block name was not found.");

			return blocks[blockName].Where((t, i) => i == num).FirstOrDefault();
		}

		public void Set<T>(string property, T value) where T : class
		{
			items[property] = Convert.ToString(value);
		}

		#region Overrides of Object

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}
