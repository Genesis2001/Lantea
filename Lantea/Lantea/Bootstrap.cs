// -----------------------------------------------------------------------------
//  <copyright file="Bootstrap.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
	using System;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using Common.Extensibility;
	using NDesk.Options;

	public class Bootstrap : IDisposable
	{
		public static void Main(string[] args)
		{
			Console.Title = "Lantea IRC Bot";
			Console.SetWindowSize(125, 30);

			using (Bootstrap b = new Bootstrap())
			{
				b.Initialize();
				
				Boolean exit = false;
				do
				{
					System.Threading.Thread.Sleep(100);

					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
					{
						exit = true;
					}
				} while (!exit);

				b.Bot.Dispose();

#if DEBUG
				Console.Write("Bot running in debug mode. Press <ENTER> to exit completely.");
				Console.ReadLine();
#endif
			}
		}

		public IBotCore Bot { get; private set; }
		
		public void Initialize()
		{
			var container = new CompositionContainer(new DirectoryCatalog("."));
			
			try
			{
				Bot = container.GetExportedValue<IBotCore>();
			}
			catch (ImportCardinalityMismatchException e)
			{
				throw new InvalidOperationException("Unable to load a bot core. Either no bot core was found, or multiple were detected.", e);
			}
			finally
			{
				if (Bot != null)
				{
					String configFile = null;
					OptionSet options = new OptionSet
				                    {
					                    {"config=", "Sets the configuration file the bot will use for it's configuration.", x => configFile = x},
				                    };

					options.Parse(Environment.GetCommandLineArgs());

					if (String.IsNullOrEmpty(configFile))
					{
						Console.Write("No config file detected in command line.");
						Console.ReadKey(true);

						Environment.Exit(1);
					}

					Bot.Initialize(configFile);
				}
			}
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
