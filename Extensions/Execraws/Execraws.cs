// -----------------------------------------------------------------------------
//  <copyright file="Execraws.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Execraws
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Atlantis.Net.Irc;
    using Lantea.Common.Extensibility;
    using Lantea.Common.IO;
    using Lantea.Common.Linq;

    [Module(ConfigBlock = "raws", CreateNewClient = false)]
    public class Execraws : IModule
    {
        private readonly Queue<string> commands = new Queue<string>();
        private readonly TimeSpan interval      = new TimeSpan(0, 0, 0, 2);

        #region Implementation of IModule

        public string Author
        {
            get { return "Zack Loveless"; }
        }

        public string Description
        {
            get { return "Executes a list of raw commands upon connect."; }
        }

        public string Name
        {
            get { return "Execraws"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public void Dispose()
        {
        }

        public void Initialize(Block config, IrcClient client)
        {
            client.ConnectionEstablishedEvent += OnClientConnect;

            for (int i = 0; i < config.CountBlock("command"); ++i)
            {
                Block cmd   = config.GetBlock("command", i);
                string exec = cmd.GetString("exec");

                if (!String.IsNullOrEmpty(exec))
                {
                    commands.Enqueue(exec);
                }
            }
        }

        public void Rehash(Block config)
        {
        }

        private async void OnClientConnect(object sender, EventArgs e)
        {
            await Task.Delay(interval);

            IrcClient client = sender as IrcClient;
            System.Diagnostics.Debug.Assert(client != null, "client != null");

            while (commands.Count > 0)
            {
                client.Send(commands.Dequeue());
            }
        }

        #endregion
    }
}
