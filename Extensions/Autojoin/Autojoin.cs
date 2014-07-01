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
	using Atlantis.Net.Irc;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	public class Autojoin : IModule
	{
		private readonly List<String> channels = new List<String>();

//		public void Load()
//		{
//			//Block autojoin = Config.GetBlock("autojoin");
////			Block autojoin = Config.GetBlock(this);
//			if (autojoin != null)
//			{
//				for (Int32 i = 0; i < autojoin.CountBlock("channel"); ++i)
//				{
//					Block chan     = autojoin.GetBlock("channel", i);
//					String channel = chan.Get("name", "");
//
//					if (String.IsNullOrEmpty(channel)) continue;
//
//					channels.Add(channel);
//				}
//			}
//
//			Bot.Client.ConnectionEstablishedEvent += OnClientConnect;
//		}
//
//		private void OnClientConnect(object sender, EventArgs e)
//		{
//			channels.ForEach(x =>
//			                 {
//				                 Bot.Client.Send("JOIN {0}", x);
//				                 Bot.Log.DebugFormat("Autojoin: {0}", x);
//			                 });
//		}

	    #region Implementation of IModule

	    public string Author
	    {
	        get { return "Zack Loveless"; }
	    }

	    public string Description
	    {
	        get { return "Auto-joins the default client to a list of channels specified in the config."; }
	    }

	    public string Name
	    {
	        get { return "Autojoin"; }
	    }

	    public string Version
	    {
	        get { return "1.2"; }
	    }

	    public void Dispose()
	    {
	        throw new NotImplementedException();
	    }

	    public void Initialize(Block config, IrcClient client)
	    {
	        throw new NotImplementedException();
	    }

	    #endregion
	}
}
