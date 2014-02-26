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
	using Atlantis.Collections;
	using Atlantis.Linq;

	public class Configuration : Block
	{
		/*
		private static readonly string[] ValueKeywords = { Boolean.TrueString, Boolean.FalseString, "true", "false" };
		private static readonly string[] DirectiveKeywords = { "include" };
		private static readonly char[] SyntaxChars = { '{', '}', '=', '"' };
		 */

		private readonly Stack<Block> blocks;
		private readonly StringBuilder buffer;
		private string fileName;
		private readonly Stack<string> keys;
		private ConfigState state;

		public Configuration() : base("")
		{
			buffer = new StringBuilder();
			blocks = new Stack<Block>();
			keys   = new Stack<string>();
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
				fileName = "(null)";
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

			if (buffer.Length > 0 || keys.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unexpected junk at the end of file: {0}", fileName));
			}

			if (blocks.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unterminated block '{0}' at end of file. {1}:{2}",
					blocks.Peek().Name,
					fileName,
					blocks.Peek().lineNumber));
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

						string key = keys.Pop();
						string value = buffer.ToString();

						buffer.Clear();
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
				// 
				else if (c == '"')
				{
					if (blocks.Count == 0 || keys.Peek() == null)
					{
						throw new MalformedConfigException(string.Format("Unexpected quote string: {0}:{1}", fileName, lineNumber));
					}

					state = ConfigState.StringOpen;
				}
				else if (c == '=')
				{
					if (blocks.Count == 0)
					{
						throw new MalformedConfigException(string.Format("Unexpected config item outside of section: {0}:{1}", fileName, lineNumber));
					}

					string item = buffer.ToString().Trim();
					keys.Push(item);
					
					buffer.Clear();
				}
				else if (c == '{')
				{
					if (buffer.Length == 0)
					{
						// commented or unnamed section.
						blocks.Push(null);
						continue;
					}

					if (blocks.Count > 0 && blocks.Peek() != null)
					{
						buffer.Clear();
						blocks.Push(null);
						continue;
					}

					Block b = new Block(buffer.ToString());
					b.lineNumber = lineNumber;

					blocks.Push(b);
					buffer.Clear();
					continue;
				}
				else if (c == ';' || c == '}')
				{
					// 
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

					// if itemname.empty()

					if (c == '}')
					{
						if (blocks.Count == 0)
						{
							throw new MalformedConfigException(string.Format("Unexpected '}}': {0}:{1}", fileName, lineNumber));
						}
						
						blocks.Pop();
						state = ConfigState.BlockClosed;
					}
				}
			}
		}
	}

	internal enum ConfigState
	{
		None = 0,
		BlockOpen,
		BlockClosed,
		StringOpen,
		StringClosed,
		CommentOpen,
		CommentClosed,
		CommentSingle,
	}

	public class Block
	{
		protected int lineNumber;
		protected readonly DictionaryList<string, object> data;

		protected Block(string name)
		{
			Name = name;
			data = new DictionaryList<string, object>();
		}

		public string Name { get; private set; }

		public T Get<T>(string property, T def = default(T)) where T : class
		{
			throw new NotImplementedException();
		}

		public void Set<T>(string property, T value) where T : class 
		{
			// 
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
