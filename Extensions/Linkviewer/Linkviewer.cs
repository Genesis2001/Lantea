// -----------------------------------------------------------------------------
//  <copyright file="Linkviewer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Linkviewer
{
    using System;
    using System.Collections.Generic;
    using Atlantis.Net.Irc;
    using Lantea.Common.Extensibility;
    using Lantea.Common.IO;

    [Module(ConfigBlock = "linkviewer")]
    public class Linkviewer : IModule
    {

        // http://gdata.youtube.com/feeds/api/videos/  (search) ?v=2&alt=json
//        private Dictionary<string, string> linkmap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
//                                                     {
//                                                         {"youtube", @""},
//                                                         {"imdb", @"(http:\/\/?)(www\.?)imdb\.com\/title\/(tt(\d+))"},
//                                                     };

        private HashSet<ILinkSniffer> linkmap = new HashSet<ILinkSniffer>
                                                {
                                                    new YoutubeSniffer(),
                                                };

        #region Implementation of IModule

        public string Author
        {
            get { return "Zack Loveless"; }
        }

        public string Description
        {
            get { return "Displays meta information about a URI sent to a channel."; }
        }

        public string Name
        {
            get { return "Linkviewer"; }
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
            client.MessageReceivedEvent += OnClientMessageReceived;
        }

        private void OnClientMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IrcClient client = sender as IrcClient;
        }

        #endregion
    }
}
