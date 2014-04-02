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
	using System.Threading.Tasks;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Export(typeof(IModule)), Module(Author = "Zack Loveless", Name = "Autojoin Channels", Type = ModuleType.VENDOR, Version = "1.0")]
	public class Autojoin : ModuleCore
	{
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

		public override string Name
		{
			get { return "Autojoin Channels"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override ModuleType Type
		{
			get { return ModuleType.VENDOR; }
		}

		public override void Load()
		{
			Block autojoin = Config.GetBlock("autojoin");
			if (autojoin != null)
			{
				for (Int32 i = 0; i < autojoin.CountBlock("channel"); ++i)
				{
					Block cBlock = autojoin.GetBlock("channel", i);

					channels.Add(cBlock.Get("name", String.Empty));
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
