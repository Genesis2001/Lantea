// -----------------------------------------------------------------------------
//  <copyright file="ConfigReader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Linq;
	using Atlantis.Collections;
	using Atlantis.Linq;

	public class Configuration : Block
	{
		private readonly Stack<Block> blockStack;
		private readonly StringBuilder buffer;
		private string fileName;
		private readonly Stack<string> keyStack;
		private ConfigState state;

		public Configuration() : base("")
		{
			buffer     = new StringBuilder();
			blockStack = new Stack<Block>();
			keyStack   = new Stack<string>();
		}

		public Block GetModule(string name)
		{
			throw new NotImplementedException();
		}

		public void Load(string path)
		{
			fileName = Path.GetFileName(path);

			Load(new FileStream(path, FileMode.Open, FileAccess.Read));
		}

		public void Load(Stream stream)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = "(memory)";
			}

			using (stream)
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					while (!reader.EndOfStream)
					{
						lineNumber++;

						string line = reader.ReadLine().TrimIfNotNull();
						ProcessLine(line);
					}
				}
			}

			if (state == ConfigState.CommentOpen)
			{
				throw new MalformedConfigException(string.Format("Unterminated multiline comment at end of file: {0}", fileName));
			}

			if (state == ConfigState.StringOpen)
			{
				throw new MalformedConfigException(string.Format("Untermniated string value at end of file: {0}", fileName));
			}

			if (buffer.Length > 0 || keyStack.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unexpected junk at the end of file: {0}", fileName));
			}

			if (blocks.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unterminated block '{0}' at end of file. {1}:{2}",
					blockStack.Peek().Name,
					fileName,
					blockStack.Peek().lineNumber));
			}
		}

		private void ProcessLine(string line)
		{
			int len = line.Length;

			for (int i = 0; i < len; ++i)
			{
				char c = line[i];
				if (state == ConfigState.StringOpen)
				{
					if (i == 0)
					{
						// Strip leading white spaces from multiline quotes
						while (i < len && char.IsWhiteSpace(c))
						{
							++i;
						}

						c = line[i];
					}

					// Allow \" in quotes.
					if (c == '\\' && i + 1 < len && line[i + 1] == '"')
					{
						buffer.Append(line[++i]);
					}
					else if (c == '"')
					{
						state = ConfigState.StringClosed;
					}
					else
					{
						buffer.Append(c);
					}
				}
				else if (state == ConfigState.CommentOpen)
				{
					if (c == '*' && (i + 1 < len) && line[i + 1] == '/')
					{
						state = ConfigState.CommentClosed;
						++i;
					}

					continue;
				}
				else if (c == '#' || (c == '/' && i + 1 < len && line[i + 1] == '/'))
				{
					i = len;
				}
				else if (c == '/' && (i + 1 < len) && line[i + 1] == '*')
				{
					state = ConfigState.CommentOpen;
					++i;
					continue;
				}
				else if (c == '"')
				{
					if (blockStack.Count == 0 || keyStack.Peek() == null)
					{
						throw new MalformedConfigException("Unexpected quote string", fileName, lineNumber);
					}

					state = ConfigState.StringOpen;
				}
				else if (c == '=')
				{
					if (blockStack.Count == 0/* && state != ConfigState.BlockOpen*/)
					{
						throw new MalformedConfigException(string.Format("Unexpected config item outside of section: {0}:{1}", fileName, lineNumber));
					}

					string item = buffer.ToString().Trim();
					keyStack.Push(item);
					
					buffer.Clear();
				}
				else if (c == '{')
				{
					if (buffer.Length == 0)
					{
						// commented or unnamed section.

						blockStack.Push(null);
						continue;
					}

					if (blockStack.Count > 0 && blockStack.Peek() != null)
					{
						buffer.Clear();
						blockStack.Push(null);
						continue;
					}
					string item = buffer.ToString();

					Block b = blockStack.Count == 0 ? this : blockStack.Peek();
					b.lineNumber = lineNumber;
					b.blocks.Add(item, new Block(item));

					blockStack.Push(b);
					buffer.Clear();
					continue;
				}
				else if (c == ';' || c == '}')
				{
					// terminate word.
				}
				else
				{
					buffer.Append(c);
				}


				if (c == ';' || c == '}' || i + 1 >= len)
				{
					bool eol = i + 1 >= len;

					if (!eol && state == ConfigState.StringOpen)
					{
						continue;
					}

					if (state == ConfigState.StringOpen)
					{
						buffer.Append(c);
					}

					if (keyStack.Count > 0 && keyStack.Peek() != null)
					{
						if (blockStack.Count == 0)
						{
							throw new MalformedConfigException(string.Format("Stray ';' outside of block: {0}:{1}", fileName, lineNumber));
						}

						Block b = blockStack.Peek();

						if (b != null)
						{
							string key = keyStack.Pop();
							string value = buffer.ToString();

							b.items[key] = value;
						}

						buffer.Clear();
					}

					if (c == '}')
					{
						if (blockStack.Count == 0)
						{
							throw new MalformedConfigException(string.Format("Unexpected '}}': {0}:{1}", fileName, lineNumber));
						}

						blockStack.Pop();
						state = ConfigState.BlockClosed;
					}
				}
			}
		}

		private enum ConfigState
		{
			None = 0,
			BlockOpen,
			BlockClosed,
			StringOpen,
			StringClosed,
			CommentOpen,
			CommentClosed,
		}
	}
	
	public class Block
	{
		/* 
		 * template<typename T> class map : public std::map<string, T, ci::less> { };				// Dictionary<String, T>
		 * template<typename T> class multimap : public std::multimap<string, T, ci::less> { };		// Dictionary<String, List<T>>
		 * 
		 * 
		 * item_map items;
		 * block_map blocks;
		 */

		internal int lineNumber;
		internal readonly Dictionary<string, string> items;
		internal readonly DictionaryList<string, Block> blocks;

		internal Block(string name)
		{
			Name   = name;
			items  = new Dictionary<string, string>();
			blocks = new DictionaryList<string, Block>();
		}

		public string Name { get; private set; }

		public int CountBlock(string blockName)
		{
			return blocks.Count(x => x.Key.Equals(blockName));
		}

		public T Get<T>(string property, T def) where T : class
		{
			if (items.ContainsKey(property))
			{
				string value = items[property];

				return (T)Convert.ChangeType(value, typeof (T));
			}

			return def;
		}

		public Block GetBlock(string blockName, int num)
		{
			List<String> list = blocks.Where(x => x.Key.Equals(blockName)).Select(x => x.Key).ToList();

			for (int i = 0; i < list.Count; ++i)
			{
				if (i == num)
				{
					
				}
			}

			throw new NotImplementedException();
		}

		public void Set<T>(string property, T value) where T : class
		{
			string data = Convert.ToString(value);

			if (items.ContainsKey(property))
			{
				items[property] = data;
			}
			else
			{
				items.Add(property, data);
			}
		}

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}
