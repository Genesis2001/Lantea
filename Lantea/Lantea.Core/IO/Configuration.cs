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
		private readonly Stack<Block> block_stack;
		private readonly StringBuilder buffer;
		private string currentFileName;
		private string itemname;
		private int currentLine;

		private bool in_word;
		private bool in_quote;
		private bool in_comment;

		public Configuration() : base("")
		{
			buffer     = new StringBuilder();
			block_stack = new Stack<Block>();
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
				string root = Path.GetDirectoryName(path);

				Load(new FileStream(path, FileMode.Open, FileAccess.Read));

				for (int i = 0; i < CountBlock("include"); ++i)
				{
					Block include = GetBlock("include", i);
					
					string file = include.Get<String>("name");
					Load(new FileStream(Path.Combine(root, file), FileMode.Open, FileAccess.Read));
				}

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

		public void Load(Stream stream)
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

			if (in_comment)
			{
				throw new MalformedConfigException(string.Format("Unterminated multiline comment at end of file: {0}", currentFileName));
			}

			if (in_quote)
			{
				throw new MalformedConfigException(string.Format("Untermniated string value at end of file: {0}", currentFileName));
			}

			if (buffer.Length > 0 || !String.IsNullOrEmpty(itemname))
			{
				throw new MalformedConfigException(string.Format("Unexpected junk at the end of file: {0}", currentFileName));
			}

			if (block_stack.Count > 0)
			{
				throw new MalformedConfigException(string.Format("Unterminated block '{0}' at end of file. {1}:{2}",
					block_stack.Peek().Name,
					currentFileName,
					block_stack.Peek().lineNumber));
			}
		}

		private void ProcessLine(string line)
		{
			int len = line.Length;
			for (int i = 0; i < len; ++i)
			{
				char c = line[i];
				if (in_quote)
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
						in_quote = in_word = false;
					}
					else
					{
						buffer.Append(c);
					}
				}
				else if (in_comment)
				{
					if (c == '*' && (i + 1 < len) && line[i + 1] == '/')
					{
						in_comment = false;
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
					in_comment = true;
					++i;
					continue;
				}
				else if (c == '"')
				{
					if (block_stack.Count == 0 || string.IsNullOrWhiteSpace(itemname))
					{
						throw new MalformedConfigException("Unexpected quote string", currentFileName, currentLine);
					}

					if (in_word || buffer.Length > 0)
					{
						throw new MalformedConfigException("Unexpected quoted string (prior unhandled words)", currentFileName, lineNumber);
					}

					in_quote = in_word = true;
				}
				else if (c == '=')
				{
					if (block_stack.Count == 0)
					{
						throw new MalformedConfigException(string.Format("Unexpected config item outside of section: {0}:{1}", currentFileName, currentLine));
					}

					if (!String.IsNullOrEmpty(itemname) || buffer.Length == 0)
					{
						throw new MalformedConfigException("Stray '=' or item without value", currentFileName, currentLine);
					}

					in_word  = false;
					itemname = buffer.ToString().Trim();
					buffer.Clear();
				}
				else if (c == '{')
				{
					if (buffer.Length == 0)
					{
						// commented or unnamed section.
						block_stack.Push(new Block(null));
						continue;
					}

					if (block_stack.Count > 0 && block_stack.Peek().Name == null)
					{
						// named block inside of comment block.
						in_word = false;
						buffer.Clear();
						block_stack.Push(new Block(null));
						continue;
					}

					String blockName = buffer.ToString();
					Block b          = block_stack.Count == 0 ? this : block_stack.Peek();

					Tuple<String, Block> pair = new Tuple<string, Block>(blockName, new Block(blockName));
					b.blocks.Add(pair.Item1, pair.Item2);

					b            = pair.Item2;
					b.lineNumber = currentLine;
					block_stack.Push(b);

					in_word = false;
					buffer.Clear();
					continue;
				}
				else if (c == ' ' || c == '\r' || c == '\t')
				{
					in_word = false;
				}
				else if (c == ';' || c == '}')
					;
				else
				{
					if (!in_word && buffer.Length > 0)
					{
						throw new MalformedConfigException("Unexpected word", currentFileName, lineNumber);
					}

					// if !in_word and buffer.Length > 0 // unexpected word?
					buffer.Append(c);
					in_word = true;
				}

				if (c == ';' || c == '}' || i + 1 >= len)
				{
					bool eol = i + 1 >= len;

					if (!eol && in_quote)
					{
						continue;
					}

					if (in_quote)
					{
						buffer.Append('\n');
						continue;
					}

					in_word = false;
					if (!string.IsNullOrEmpty(itemname))
					{
						if (block_stack.Count == 0)
						{
							throw new MalformedConfigException("Stray ';' outside of block", currentFileName, currentLine);
						}

						Block b = block_stack.Peek();
						
						if (b != null)
						{
							b.items[itemname] = buffer.ToString().Trim();
							itemname = null;
						}

						buffer.Clear();
					}

					if (c == '}')
					{
						if (block_stack.Count == 0)
						{
							throw new MalformedConfigException("Unexpected '}'", currentFileName, currentLine);
						}

						block_stack.Pop();
					}
				}
			}
		}
	}
}
