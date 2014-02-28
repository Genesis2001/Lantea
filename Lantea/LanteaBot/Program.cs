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
	using System.Threading.Tasks;
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

		private readonly OptionSet parser = new OptionSet();
		[Import] private IBotCore bot;

		public void Compose()
		{
			var asm       = Assembly.GetAssembly(typeof(Program));
			var catalog   = new AssemblyCatalog(asm);
			var container = new CompositionContainer(catalog);

			 bot = container.GetExportedValue<IBotCore>();
		}

		public async void Run(params string[] args)
		{
			string config = null;
			parser.Add("config=", "", x => config = x);
			parser.Parse(args);

			if (String.IsNullOrEmpty(config))
			{
				Console.WriteLine("No configuration file discovered in command line. Terminating.");
				Environment.Exit(2);
			}

			bot.Load(config);
			bot.Initialize();

			ConsoleKeyInfo key;
			do
			{
				await Task.Delay(100);

				key = Console.ReadKey(true);
			} while (!(key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C));
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
