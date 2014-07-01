// -----------------------------------------------------------------------------
//  <copyright file="Autojoin.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Autojoin
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Atlantis.Net.Irc;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;
	using Lantea.Common.Linq;

    [Module(ConfigBlock = "autojoin")]
	public class Autojoin : IModule
	{
		private readonly List<String> channels = new List<String>();
        private readonly TimeSpan wait = new TimeSpan(0, 0, 0, 2);

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
	    }

	    public void Initialize(Block config, IrcClient client)
	    {
	        client.ConnectionEstablishedEvent += OnClientConnect;
	        for (int i = 0; i < config.CountBlock("channel"); ++i)
	        {
	            Block c = config.GetBlock("channel", i);
                string channel = c.GetString("name");

	            if (string.IsNullOrEmpty(channel))
	                continue;

	            channels.Add(channel);
	        }
	    }

        private async void OnClientConnect(object sender, EventArgs e)
        {
            await Task.Delay(wait);

            IrcClient client = sender as IrcClient;
            foreach (string channel in channels)
            {
                // ReSharper disable once PossibleNullReferenceException
                client.Send("JOIN {0}", channel);
            }
        }

        #endregion
	}
}
