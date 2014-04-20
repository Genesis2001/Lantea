// -----------------------------------------------------------------------------
//  <copyright file="Autojoin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Autojoin
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Export(typeof(IModule))]
	[Module(Author = "Zack Loveless", Description = ModuleDesc, Name = ModuleName, ModuleType = ModuleType.VENDOR, Version = ModuleVersion)]
	public class Autojoin : ModuleCore
	{
		private const String ModuleName    = "Autojoin";
		private const String ModuleVersion = "1.0";
		private const String ModuleDesc    = "";

		[ImportingConstructor]
		public Autojoin([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
		}

		private readonly List<String> channels = new List<String>();

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return "Zack Loveless"; }
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
			get { return ModuleType.VENDOR; }
		}

		public override void Load()
		{
			//Block autojoin = Config.GetBlock("autojoin");
			Block autojoin = Config.GetBlock(this);
			if (autojoin != null)
			{
				for (Int32 i = 0; i < autojoin.CountBlock("channel"); ++i)
				{
					Block chan     = autojoin.GetBlock("channel", i);
					String channel = chan.Get("name", "");

					if (String.IsNullOrEmpty(channel)) continue;

					channels.Add(channel);
				}
			}

			Bot.Client.ConnectionEstablishedEvent += OnClientConnect;
		}

		private void OnClientConnect(object sender, EventArgs e)
		{
			channels.ForEach(x =>
			                 {
				                 Bot.Client.Send("JOIN {0}", x);
				                 Bot.Log.DebugFormat("Autojoin: {0}", x);
			                 });
		}

		#endregion
	}
}
