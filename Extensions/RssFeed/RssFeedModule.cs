namespace RssFeed
{
    using System;
    using Atlantis.Net.Irc;
    using Lantea.Common.Extensibility;
    using Lantea.Common.IO;
    using Lantea.Common.Linq;

    [Module(ConfigBlock = "feeds")]
    public class RssFeedModule : IModule
    {
        private IrcClient client;

        #region Implementation of IModule

        public string Author
        {
            get { return "Zack Loveless"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public string Name
        {
            get { return "Rssfeed"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public void Dispose()
        {
        }

        public void Rehash(Block config)
        {
            string database = config.GetString("database");

            
        }

        // ReSharper disable once ParameterHidesMember
        public void Initialize(Block config, IrcClient client)
        {
            Rehash(config);

            this.client                  = client;
            client.MessageReceivedEvent += OnClientMessageReceved;
        }

        #endregion

        private void OnClientMessageReceved(object sender, MessageReceivedEventArgs e)
        {
            if (e.Source.Equals(client.Nick) || !e.Target.StartsWith("#"))
            {
                return;
            }

            // stuff. parse commands. etc.
        }
    }
}
