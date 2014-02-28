// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LanteaBot
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Reflection;
	using System.Threading;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;
	using NDesk.Options;

	public class Program : IDisposable
	{
		public static void Main(string[] args)
		{
			Console.Title = "Lantea Bot";
			Console.SetWindowSize(125, 30);

			Configuration configuration = new Configuration();

			configuration.Load("example.conf.txt");


			using (Program p = new Program())
			{
				p.Compose();
				p.Run(args);
			}

#if DEBUG
			Console.Write("Bot running in debug mode. Press <ENTER> to exit completely.");
			Console.ReadLine();
#endif
		}

		public Program()
		{
			parser = new OptionSet
			         {
				         {"config=", "Sets the configuration file the bot will use for it's configuration.", x => config = x},
			         };
		}

		private string config;
		private readonly OptionSet parser;
		[Import] private IBotCore bot;

		public void Compose()
		{
			var asm       = Assembly.GetAssembly(typeof(Program));
			var catalog   = new AssemblyCatalog(asm);
			var container = new CompositionContainer(catalog);

			bot = container.GetExportedValue<IBotCore>();
		}

		public void Run(params string[] args)
		{
			parser.Parse(args);

			if (String.IsNullOrEmpty(config))
			{
				Console.WriteLine("No configuration file discovered in command line. Terminating.");
				Environment.Exit(2);
			}

			bot.Load(config);
			bot.Initialize();

			bool exit = false;
			do
			{
				Thread.Sleep(100);

				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
				{
					exit = true;
				}
			} while (!exit);
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			bot.Dispose();
		}

		#endregion
	}
}
