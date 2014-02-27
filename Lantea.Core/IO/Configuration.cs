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
	using Atlantis.Linq;

	public class Configuration : Block
	{
		private readonly Stack<Block> blockStack;
		private readonly StringBuilder buffer;
		private string currentFileName;
		private string itemname;
		private ConfigState state;
		private int currentLine;

		public Configuration() : base("")
		{
			buffer     = new StringBuilder();
			blockStack = new Stack<Block>();
		}

		public event EventHandler<ConfigurationLoadEventArgs> ConfigurationLoadEvent;

		public Block GetModule(string name)
		{
			throw new NotImplementedException();
		}

		public void Load(string path)
		{
			try
			{
				currentFileName = Path.GetFileName(path);
				string rootPath = Path.GetDirectoryName(path);

				Load(new FileStream(path, FileMode.Open, FileAccess.Read));

				/*for (int i = 0; i < CountBlock("include"); ++i)
				{
					Block include = GetBlock("include", i);
					
					string file = include.Get<String>("name");
					Load(new FileStream(Path.Combine(rootPath, file), FileMode.Open, FileAccess.Read));
				}*/

				ConfigurationLoadEvent.Raise(this, new ConfigurationLoadEventArgs(true));
			}
			catch (FileNotFoundException e)
			{
				ConfigurationLoadEvent.Raise(this, new ConfigurationLoadEventArgs(false, e));
			}
#if !DEBUG
			catch (MalformedConfigException e)
			{
				ConfigurationLoadEvent.Raise(this, new ConfigurationLoadEventArgs(false, e));
			}
#endif
		}

		internal void Load(Stream stream)
		{
			if (string.IsNullOrEmpty(currentFileName))
			{
				currentFileName = "(memory)";
			}

			using (stream)
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					while (!reader.EndOfStream)
					{
						currentLine++;

						string line = reader.ReadLine().TrimIfNotNull();
						ProcessLine(line);
					}
				}
			}

			if (state == ConfigState.CommentOpen)
			{
				throw new MalformedConfigException(string.Format("Unterminated multiline comment at end of file: {0}", currentFileName));
			}

			if (state == ConfigState.StringOpen)
			{
				throw new MalformedConfigException(string.Format("Untermniated string value at end of file: {0}", currentFileName));
			}

			if (buffer.Length > 0 || !String.IsNullOrEmpty(itemname))
			{
				throw new MalformedConfigException(string.Format("Unexpected junk at the end of file: {0}", currentFileName));
			}

			if (blockStack.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unterminated block '{0}' at end of file. {1}:{2}",
					blockStack.Peek().Name,
					currentFileName,
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
					else
					{
						continue;
					}
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
						throw new MalformedConfigException("Unexpected quote string", currentFileName, currentLine);
					}

					state = ConfigState.StringOpen;
				}
				else if (c == '=')
				{
					if (blockStack.Count == 0)
					{
						throw new MalformedConfigException(string.Format("Unexpected config item outside of section: {0}:{1}", currentFileName, currentLine));
					}

					if (!String.IsNullOrEmpty(itemname) || buffer.Length == 0)
					{
						throw new MalformedConfigException("Stray '=' or item without value", currentFileName, currentLine);
					}

					itemname = buffer.ToString().Trim();
					buffer.Clear();
				}
				else if (c == '{')
				{
					if (buffer.Length == 0)
					{
						// commented or unnamed section.
						blockStack.Push(new Block(null));
						continue;
					}

					if (blockStack.Count > 0 && blockStack.Peek() != null)
					{
						// named block inside of comment block.
						buffer.Clear();
						blockStack.Push(new Block(null));
						continue;
					}

					string blockName = buffer.ToString();
					Block b          = blockStack.Count == 0 ? this : blockStack.Peek();

					KeyValuePair<string, Block> block = new KeyValuePair<string, Block>(blockName, new Block(blockName));
					b.blocks.Add(block);

					b = block.Value;
					b.lineNumber = currentLine;

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
					// if !in_word and buffer.Length > 0 // unexpected word?

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
							throw new MalformedConfigException("Stray ';' outside of block", currentFileName, currentLine);
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
							itemname = null;
						}

						buffer.Clear();
					}

					if (c == '}')
					{
						if (blockStack.Count == 0)
						{
							throw new MalformedConfigException("Unexpected '}'", currentFileName, currentLine);
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
