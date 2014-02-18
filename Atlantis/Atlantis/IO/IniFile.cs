// -----------------------------------------------------------------------------
//  <copyright file="IniFile.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.IO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Linq;

	public class IniFile
	{
		private readonly StringBuilder lastWord;
		private readonly string path;
		private HashSet<IniSection> sections;
		private IniReaderState state;
		private int lineNumber;
		private IniSection lastSection;

		private IniFile()
		{
			lastWord = new StringBuilder();
		}

		public IniFile(string path) : this()
		{
			this.path = path;
		}

		public string this[string section, string key]
		{
			get
			{
				if (sections.Any(x => x.Name.Equals(section)))
				{
					IniSection s = sections.Single(x => x.Name.Equals(section));

					if (s != null)
					{
						if (!s.Any(x => x.Key.Equals(key)))
						{
							throw new KeyNotFoundException("The specified key was not found in the section.");
						}


					}
				}
			}
		}

		public bool Load()
		{
			try
			{
				using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					while (!reader.EndOfStream)
					{
						lineNumber++;
						ProcessLine(reader.ReadLine().TrimIfNotNull());
					}
				}

				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

		private void ProcessLine(string line)
		{
			if (String.IsNullOrEmpty(line)) return;

			for (int i = 0; i < line.Length; ++i)
			{
				char c = line[i];

				if (c == '[')
				{
					state = IniReaderState.SectionOpen;
				}
				else if (c == ']')
				{
					state = IniReaderState.SectionClose;

					lastSection = new IniSection(lastWord.ToString());
					sections.Add(lastSection);
				}
				else if (c == ';')
				{
					state = IniReaderState.Comment;
				}
				else if (state == IniReaderState.Comment || state == IniReaderState.SectionOpen)
				{
					lastWord.Append(c);

					if (i == line.Length - 1)
					{
						if (state == IniReaderState.Comment && lastWord.Length > 0)
						{
						}
						else if (state == IniReaderState.SectionOpen)
						{
							throw new InvalidOperationException(string.Format("Open INI section detected at end of line. Line {0}, character {1}.", lineNumber, lineNumber));
						}
					}
				}
			}
		}

		public void Save(string filepath)
		{
		}

		#region Nested type: IniReaderState

		internal enum IniReaderState
		{
			SectionOpen,
			SectionClose,
			IniKey,
			IniValue,
			Comment,
		}

		#endregion

		#region Nested type: IniSection

		public class IniSection : IDictionary<string, string>
		{
			private readonly Dictionary<string, string> dict;

			private IniSection()
			{
				dict = new Dictionary<string, string>();
			}

			public IniSection(string name) : this()
			{
				Name = name;
			}

			public string Name { get; private set; }
			
			#region Implementation of IEnumerable

			/// <summary>
			///     Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>
			///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
			/// </returns>
			public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
			{
				return dict.GetEnumerator();
			}

			/// <summary>
			///     Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
			/// </returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region Implementation of ICollection<KeyValuePair<string,string>>

			/// <summary>
			///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <exception cref="T:System.NotSupportedException">
			///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
			///     read-only.
			/// </exception>
			void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
			{
				dict.Add(item.Key, item.Value);
			}

			/// <summary>
			///     Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <exception cref="T:System.NotSupportedException">
			///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
			///     read-only.
			/// </exception>
			public void Clear()
			{
				dict.Clear();
			}

			/// <summary>
			///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
			/// </summary>
			/// <returns>
			///     true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />;
			///     otherwise, false.
			/// </returns>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
			{
				return dict.Any(x => x.Key.Equals(item.Key) && x.Value.Equals(item.Value));
			}

			/// <summary>
			///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
			///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
			/// </summary>
			/// <param name="array">
			///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
			///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
			///     zero-based indexing.
			/// </param>
			/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
			/// <exception cref="T:System.ArgumentException">
			///     The number of elements in the source
			///     <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
			///     <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
			/// </exception>
			void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Removes the first occurrence of a specific object from the
			///     <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <returns>
			///     true if <paramref name="item" /> was successfully removed from the
			///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
			///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </returns>
			/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <exception cref="T:System.NotSupportedException">
			///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
			///     read-only.
			/// </exception>
			bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <returns>
			///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </returns>
			public int Count
			{
				get { return dict.Count; }
			}

			/// <summary>
			///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
			/// </summary>
			/// <returns>
			///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
			/// </returns>
			public bool IsReadOnly
			{
				get { return false; }
			}

			#endregion

			#region Implementation of IDictionary<string,string>

			/// <summary>
			///     Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the
			///     specified key.
			/// </summary>
			/// <returns>
			///     true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise,
			///     false.
			/// </returns>
			/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
			public bool ContainsKey(string key)
			{
				return dict.ContainsKey(key);
			}

			/// <summary>
			///     Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <param name="key">The object to use as the key of the element to add.</param>
			/// <param name="value">The object to use as the value of the element to add.</param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
			/// <exception cref="T:System.ArgumentException">
			///     An element with the same key already exists in the
			///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </exception>
			/// <exception cref="T:System.NotSupportedException">
			///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is
			///     read-only.
			/// </exception>
			public void Add(string key, string value)
			{
				dict.Add(key, value);
			}

			/// <summary>
			///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <returns>
			///     true if the element is successfully removed; otherwise, false.  This method also returns false if
			///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </returns>
			/// <param name="key">The key of the element to remove.</param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
			/// <exception cref="T:System.NotSupportedException">
			///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is
			///     read-only.
			/// </exception>
			public bool Remove(string key)
			{
				return dict.Remove(key);
			}

			/// <summary>
			///     Gets the value associated with the specified key.
			/// </summary>
			/// <returns>
			///     true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element
			///     with the specified key; otherwise, false.
			/// </returns>
			/// <param name="key">The key whose value to get.</param>
			/// <param name="value">
			///     When this method returns, the value associated with the specified key, if the key is found;
			///     otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
			///     uninitialized.
			/// </param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
			public bool TryGetValue(string key, out string value)
			{
				return dict.TryGetValue(key, out value);
			}

			/// <summary>
			///     Gets or sets the element with the specified key.
			/// </summary>
			/// <returns>
			///     The element with the specified key.
			/// </returns>
			/// <param name="key">The key of the element to get or set.</param>
			/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
			/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
			///     The property is retrieved and
			///     <paramref name="key" /> is not found.
			/// </exception>
			/// <exception cref="T:System.NotSupportedException">
			///     The property is set and the
			///     <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
			/// </exception>
			public string this[string key]
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			/// <summary>
			///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
			///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <returns>
			///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements
			///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </returns>
			public ICollection<string> Keys
			{
				get { return dict.Keys; }
			}

			/// <summary>
			///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
			///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </summary>
			/// <returns>
			///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements
			///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
			/// </returns>
			public ICollection<string> Values
			{
				get { return dict.Values; }
			}

			#endregion
		}

		#endregion
	}
}
