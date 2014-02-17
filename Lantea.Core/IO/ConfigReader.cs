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

	public class ConfigReader : IDisposable
	{
		private HashSet<Block> blocks;
		private Block currentBlock;
		private string lastWord;
		private Stack<string> openBlocks;
		private Stack<string> openKeys; 
		private ConfigurationState state;

		public ConfigReader()
		{
			blocks = new HashSet<Block>();
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
			if (!String.IsNullOrEmpty(line))
			{
				foreach (char c in line)
				{
					if (c == '{')
					{
						if (state == ConfigurationState.NewDocument)
						{
							currentBlock = new Block(lastWord);
							blocks.Add(currentBlock);

							state = ConfigurationState.OpenBracket;
						}
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
							openKeys.Push(lastWord);
							state = ConfigurationState.Key;
						}
					}
				}
			}
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Nested type: ConfigurationState

		private enum ConfigurationState
		{
			NewDocument,
			OpenBracket,
			CloseBracket,
			Key,
		}

		#endregion

		#region Nested type: Block

		internal class Block
		{
			internal Block(string name)
			{
				Name = name;
				Properties = new Dictionary<string, object>();
				Children = new HashSet<Block>();
			}

			public string Name { get; private set; }

			public HashSet<Block> Children { get; private set; }

			public Dictionary<string, object> Properties { get; set; }
		}

		#endregion
	}
}
