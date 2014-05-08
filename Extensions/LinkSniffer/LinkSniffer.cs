// -----------------------------------------------------------------------------
//  <copyright file="LinkSniffer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LinkSniffer
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Text.RegularExpressions;
	using Atlantis.Net.Irc;
	using Atlantis.Web;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;
	using Lantea.Common.Linq;

	[Export(typeof(IModule))]
	[Module(Name = ModuleName, Author = ModuleAuthor, Description = ModuleDesc, ModuleType = ModuleType.EXTRA, Version = ModuleVersion)]
	public class LinkSniffer : ModuleCore
	{
		private const String ModuleName = "Sniffer";
		private const String ModuleDesc = "Link Sniffer - Get the <title> of a link sent to a channel.";
		private const String ModuleVersion = "1.0";
		private const String ModuleAuthor = "Zack Loveless";

		private const String linkRegex = @"\b(?:http://|www\.)\S+\b";
		// (?:\w+:\/\/[\w@][\w.:@]+\/?[\w\.?=%&=\-@/$,]*)

		[ImportingConstructor]
		public LinkSniffer([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
		}

		private readonly HashSet<String> allowedChannels = new HashSet<string>(); 

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return ModuleAuthor; }
		}

		public override string Description
		{
			get { return ModuleDesc; }
		}

		public override string Name
		{
			get { return ModuleName; }
		}

		public override string Version
		{
			get { return ModuleVersion; }
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.EXTRA; }
		}

		#endregion

		#region Overrides of ModuleCore

		public override void Load()
		{
			Block config = Config.GetBlock(this);

			if (config != null)
			{
				Block channels = config.GetBlock("channels");
				if (channels != null)
				{
					for (Int32 i = 0; i < channels.CountBlock("channel"); ++i)
					{
						Block chan = channels.GetBlock("channel", i);
						String name = chan.Get<String>("name");

						if (String.IsNullOrEmpty(name)) continue;
						
						allowedChannels.Add(name);
					}
				}
			}

			Bot.Client.MessageReceivedEvent += OnClientMessageReceived;
		}

		private void OnClientMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			if (!e.Target.StartsWith("#")) return;
			if (e.Source.Equals(Bot.Client.Nick)) return;

			if (allowedChannels.Contains(e.Target))
			{
				var ms = Regex.Matches(e.Message, linkRegex);
				if (ms.Count > 0)
				{
					var m = ms[0];

					String title = HttpHelper.GetHtmlTitle(m.Value);
					if (title != null)
					{
						Bot.Client.Message(e.Target, "[{0}] {1} - {2}", e.Source.Color(4), title, m.Value);
					}
				}
			}
		}

		#endregion
	}
}
