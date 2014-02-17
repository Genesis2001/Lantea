// -----------------------------------------------------------------------------
//  <copyright file="ConfigReader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Atlantis.Linq;

	public class ConfigReader
	{
		private static readonly string[] keywords = {"include", "True", "False", "true", "false"};

		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private HashSet<Block> blocks;
		private Block currentBlock;
		private StringBuilder lastWord;
		private Stack<string> openBlocks;
		private Stack<string> openKeys;
		private ConfigurationState state;
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		public ConfigReader()
		{
			blocks = new HashSet<Block>();
			lastWord = new StringBuilder();
			openBlocks = new Stack<string>();
			openKeys   = new Stack<string>();
		}

		public ConfigDocument Load(string configFile)
		{
			try
			{
				using (FileStream stream = new FileStream(configFile, FileMode.Open, FileAccess.Read))
				{
					state = ConfigurationState.NewDocument;

					using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
					{
						while (!reader.EndOfStream)
						{
							ProcessLine(reader.ReadLine().TrimIfNotNull());
						}
					}

					if (openBlocks.Count > 0)
					{
						string lastBlock = openBlocks.Pop();

						throw new MalformedConfigException(
							string.Format("The specified file is properly structured.\n\nBlock \"{0}\" has been left open.", lastBlock));
					}

					return new ConfigDocument(blocks);
				}
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}

		private void ProcessLine(string line)
		{
			if (String.IsNullOrEmpty(line)) return;

			if (line.StartsWith("#")) return; // ignore commented lines.

			foreach (char c in line)
			{
				if (c == '"')
				{
					if (state == ConfigurationState.StringOpen)
					{
						state = ConfigurationState.StringClosed;

						if (openKeys.Count > 0)
						{
							// TODO: Determine whether the string has spanned multiple lines which isn't allowed.
							string key = openKeys.Pop();
							currentBlock.Add(key, lastWord.ToString());
							lastWord.Clear();
						}
					}
					else
					{
						state = ConfigurationState.StringOpen;
					}
				}
				else if (state == ConfigurationState.StringOpen || state == ConfigurationState.OpenBracket)
				{
					lastWord.Append(c);
				}
				else if (c == '{')
				{
					if (state == ConfigurationState.NewDocument)
					{
						if (lastWord.Length == 0)
						{
							throw new MalformedConfigException("New document does not have a root block name.");
						}

						currentBlock = new Block(lastWord.ToString());
						blocks.Add(currentBlock);
						openBlocks.Push(lastWord.ToString());
					}

					state = ConfigurationState.OpenBracket;
				}
				else if (c == '}')
				{
					openBlocks.Pop();
					state = ConfigurationState.CloseBracket;
				}
				else if (c == '=')
				{
					if (state == ConfigurationState.OpenBracket)
					{
						openKeys.Push(lastWord.ToString());
						state = ConfigurationState.Key;
					}
				}
			}
		}

		#region Nested type: ConfigurationState

		private enum ConfigurationState
		{
			NewDocument,
			OpenBracket,
			CloseBracket,
			Key,
			StringOpen,
			StringClosed,
			Value,
		}

		#endregion

		#region Nested type: Block

		internal class Block : IDictionary<string, object>
		{
			private readonly Dictionary<string, object> dict;

			internal Block(string name)
			{
				dict     = new Dictionary<string, object>();
				Name     = name;
				Children = new HashSet<Block>();
			}

			public string Name { get; private set; }

			public HashSet<Block> Children { get; private set; }

			#region Implementation of IEnumerable

			/// <summary>
			/// Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
			/// </returns>
			public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
			{
				return dict.GetEnumerator();
			}

			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
			/// </returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region Implementation of ICollection<KeyValuePair<string,object>>

			/// <summary>
			/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </summary>
			/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
			void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </summary>
			/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
			public void Clear()
			{
				dict.Clear();
			}

			/// <summary>
			/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
			/// </summary>
			/// <returns>
			/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
			/// </returns>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
			bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
			/// </summary>
			/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
			void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </summary>
			/// <returns>
			/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </returns>
			/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
			bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </summary>
			/// <returns>
			/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
			/// </returns>
			public int Count
			{
				get { return dict.Count; }
			}

			/// <summary>
			/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
			/// </summary>
			/// <returns>
			/// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
			/// </returns>
			public bool IsReadOnly
			{
				get { return false; }
			}

			#endregion

			#region Implementation of IDictionary<string,object>

			/// <summary>
			/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
			/// </summary>
			/// <returns>
			/// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
			/// </returns>
			/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
			bool IDictionary<string, object>.ContainsKey(string key)
			{
				return dict.ContainsKey(key);
			}

			/// <summary>
			/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </summary>
			/// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
			public void Add(string key, object value)
			{
				dict.Add(key, value);
			}

			/// <summary>
			/// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </summary>
			/// <returns>
			/// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </returns>
			/// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
			public bool Remove(string key)
			{
				return dict.Remove(key);
			}

			/// <summary>
			/// Gets the value associated with the specified key.
			/// </summary>
			/// <returns>
			/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
			/// </returns>
			/// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
			public bool TryGetValue(string key, out object value)
			{
				return dict.TryGetValue(key, out value);
			}

			/// <summary>
			/// Gets or sets the element with the specified key.
			/// </summary>
			/// <returns>
			/// The element with the specified key.
			/// </returns>
			/// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
			public object this[string key]
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			/// <summary>
			/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </returns>
			public ICollection<string> Keys
			{
				get { return dict.Keys; }
			}

			/// <summary>
			/// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
			/// </returns>
			ICollection<object> IDictionary<string, object>.Values
			{
				get { return dict.Values; }
			}

			#endregion
		}

		#endregion
	}
}
