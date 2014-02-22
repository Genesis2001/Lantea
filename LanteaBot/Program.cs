// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LanteaBot
{
	using System;
	using Lantea.Core;
	using Lantea.Core.IO;

	public class Program
	{
		public static void Main(string[] args)
		{
			Console.Title = "Lantea Bot";
			Console.SetWindowSize(125, 30);

			Config config = new Config();

			config.Load("example.conf.txt");

			/*using (var bot = new Bot())
			{
				bot.LoadSettings("Settings.xml");
				bot.Start();

				Console.WriteLine("Press <ENTER> to terminate the bot.");
				Console.ReadLine();
			}*/

#if DEBUG
			Console.Write("Bot running in debug mode. Press <ENTER> to exit completely.");
			Console.ReadLine();
#endif
		}
	}
}
