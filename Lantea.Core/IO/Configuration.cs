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
	using System.Xml;
	using Atlantis.Linq;

	public class Configuration : Block
	{
		private readonly Stack<Block> blockStack;
		private readonly StringBuilder buffer;
		private string fileName;
		private string itemname;
		private ConfigState state;

		public Configuration() : base("")
		{
			buffer     = new StringBuilder();
			blockStack = new Stack<Block>();
		}

		public Block GetModule(string name)
		{
			throw new NotImplementedException();
		}

		public void Load(string path)
		{
			fileName = Path.GetFileName(path);

			Load(new FileStream(path, FileMode.Open, FileAccess.Read));

			// TODO: parse directives.
		}

		internal void Load(Stream stream)
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

			if (buffer.Length > 0 || !String.IsNullOrEmpty(itemname))
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
					i = len - 1;
				}
				else if (c == '/' && (i + 1 < len) && line[i + 1] == '*')
				{
					state = ConfigState.CommentOpen;
					++i;
					continue;
				}
				else if (c == '"')
				{
					if (blockStack.Count == 0 || itemname == null)
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

					itemname = buffer.ToString().Trim();
					
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

					string blockName = buffer.ToString();
					Block b          = blockStack.Count == 0 ? this : blockStack.Peek();

					KeyValuePair<string, Block> block = new KeyValuePair<string, Block>(blockName, new Block(blockName));
					b.blocks.Add(block);

					b = block.Value;
					b.lineNumber = lineNumber;

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
						buffer.Append('\n');
						continue;
					}

					if (!string.IsNullOrEmpty(itemname))
					{
						if (blockStack.Count == 0)
						{
							throw new MalformedConfigException("Stray ';' outside of block", fileName, lineNumber);
						}

						Block b = blockStack.Peek();

						/*for (int j = 0; j < CountBlock("define"); ++j)
						{
							Block define = GetBlock("define", j);

							string dname = define.Get<String>("name");
							if (dname == buffer.ToString() && define != b)
							{
								buffer.Clear();
								buffer.Append(define.Get<String>("value"));
							}
						}*/
						
						if (b != null)
						{
							b.items[itemname] = buffer.ToString();
						}

						buffer.Clear();
					}

					if (c == '}')
					{
						if (blockStack.Count == 0)
						{
							throw new MalformedConfigException("Unexpected '}'", fileName, lineNumber);
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
}
