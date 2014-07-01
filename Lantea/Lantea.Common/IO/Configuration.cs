// -----------------------------------------------------------------------------
//  <copyright file="ConfigReader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

#pragma warning disable 642

namespace Lantea.Common.IO
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Atlantis.Linq;
	using Extensibility;
	using Linq;

    // ReSharper disable InconsistentNaming

	public class Configuration : Block
	{
		private readonly Stack<Block> block_stack = new Stack<Block>();
		private readonly StringBuilder buffer     = new StringBuilder();
		private String currentFileName;
		private String itemname;
		private Int32 currentLine;
		
		private bool in_word;
		private bool in_quote;
		private bool in_comment;

		private readonly Dictionary<String, Block> modules = new Dictionary<String, Block>(StringComparer.OrdinalIgnoreCase);
		private String root;

		public Configuration() : base("")
		{
			buffer      = new StringBuilder();
			block_stack = new Stack<Block>();
		}

		#region Events

		public event EventHandler<ConfigurationLoadEventArgs> ConfigurationLoadEvent;

		#endregion
		
		#region Methods

		public Block GetModule(IModule module)
		{
			return module == null ? null : GetModule(module.Name);
		}

		public Block GetModule(String name)
		{
			Block block;

			return modules.TryGetValue(name, out block) ? block : null;
		}

		/// <summary>
		/// Loads the specified config file into memory according to the Anope configuration standard.
		/// </summary>
		/// <param name="path">The path of the file to be loaded.</param>
		/// <exception cref="T:System.IO.FileNotFoundException" />
		/// <exception cref="T:Lantea.Common.IO.MalformedConfigException" />
		public void Load(String path)
		{
		    if (path == null) throw new ArgumentNullException("path");

		    try
			{
				currentFileName = Path.GetFileName(path);
                root            = Path.GetDirectoryName(path.GetAbsolutePath());

				Load(new FileStream(path, FileMode.Open, FileAccess.Read));

				for (Int32 i = 0; i < CountBlock("include"); ++i)
				{
					Block include = GetBlock("include", i);
					String file = include.Get<String>("name");

                    ValidateNotEmpty("include", "name", file);

                    file = Path.GetFullPath(Path.Combine(root, file));

                    ValidateFilePath("include", "name", file);

					Load(new FileStream(file, FileMode.Open, FileAccess.Read));
				}
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
			finally
			{
				for (Int32 i = 0; i < CountBlock("module"); ++i)
				{
					Block module = GetBlock("module", i);
					if (module != null)
					{
						String name = module.Get<String>("name");

						ValidateNotEmpty("module", "name", name);
						ValidateNoSpaces("module", "name", name);

						modules.Add(name, module);
					}
				}
			}
		}

	    /// <summary>
		/// Loads the specified stream into memory according to the Anope configuration standard.
		/// </summary>
		/// <param name="stream"></param>
		/// <exception cref="T:Lantea.Common.IO.MalformedConfigException" />
		public void Load(Stream stream)
		{
			if (String.IsNullOrEmpty(currentFileName))
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
				throw new MalformedConfigException(string.Format("Unterminated multiline comment at end of file: {0}",
					currentFileName));
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

			ConfigurationLoadEvent.Raise(this, new ConfigurationLoadEventArgs(true));
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
						throw new MalformedConfigException(string.Format("Unexpected config item outside of section: {0}:{1}",
							currentFileName,
							currentLine));
					}

					if (!String.IsNullOrEmpty(itemname) || buffer.Length == 0)
					{
						throw new MalformedConfigException("Stray '=' or item without value", currentFileName, currentLine);
					}

					in_word = false;
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
					Block b = block_stack.Count == 0 ? this : block_stack.Peek();

					Tuple<String, Block> pair = new Tuple<string, Block>(blockName, new Block(blockName));
					b.blocks.Add(pair.Item1, pair.Item2);

					b = pair.Item2;
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
        
	    #region TODO

        // TODO: Refactor these into extension methods possibly to fit more of a C# style of programming.

	    public static void ValidateNotEmpty(String block, String name, String value)
	    {
	        if (String.IsNullOrEmpty(value))
	        {
	            throw new MalformedConfigException(String.Format("The value for <{0}:{1}> cannot be empty.", block, name));
	        }
	    }

	    public static void ValidateNotZero(String block, String name, Int32 value)
	    {
	        if (value == 0)
	        {
	            throw new MalformedConfigException(String.Format("The value for <{0}:{1}> cannot be zero.", block, name));
	        }
	    }

	    public static void ValidateNoSpaces(String block, String name, String value)
	    {
	        if (value.Contains(" "))
	        {
	            throw new MalformedConfigException(String.Format("The value for <{0}:{1}> cannot contain spaces.",
	                block,
	                name));
	        }
	    }

	    public static void ValidateDirectory(String block, String name, String value)
	    {
	        if (!Directory.Exists(value))
	        {
	            throw new MalformedConfigException(String.Format("The value for <{0}:{1}> is not a valid directory.",
	                block,
	                name));
	        }
	    }

	    public static void ValidateFilePath(String block, String name, String value)
	    {
	        if (!File.Exists(value))
	        {
	            throw new MalformedConfigException(String.Format("The value for <{0}:{1}> is not a valid file path.",
	                block,
	                name));
	        }
	    }

	    #endregion
        
		#endregion
	}

	// ReSharper restore InconsistentNaming
}
