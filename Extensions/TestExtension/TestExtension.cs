// -----------------------------------------------------------------------------
//  <copyright file="TestExtension.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestExtension
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;

	[Module(ModuleType.EXTRA, "1.0", Name = "Example Extension", Author = "Zack Loveless")]
	public class TestExtension : ModuleBase
	{
		[ImportingConstructor]
		public TestExtension([Import] IBotCore bot) : base(bot)
		{
		}

		#region Overrides of ModuleBase

		public override void Initialize()
		{
			Console.WriteLine("Test Extension loaded.");

			Bot.Client.MessageReceivedEvent += OnMessageReceived;
		}
		
		#endregion

		protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			string[] toks  = e.Message.Split(' ');
			string command = toks[0];
			string[] args  = toks.Skip(1).ToArray();

			Channel channel = Bot.Client.GetChannel(e.Target);
			if (channel != null)
			{
				PrefixList l;
				if (channel.Users.TryGetValue(e.Source, out l))
				{
					foreach (ICommand c in Commands)
					{
						foreach (string trigger in c.Triggers)
						{
							if (trigger.EqualsIgnoreCase(command))
							{
								if (c.CanExecute(l))
								{
									c.Execute(new Parameter {Channel = channel.Name, Source = e.Source, Args = args});
								}
							}
						}
					}
				}
			}
		}
	}

	internal struct Parameter
	{
		public string Channel;
		public string Source;
		public string[] Args;
	}
}
