// -----------------------------------------------------------------------------
//  <copyright file="TS3Client.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Net.Query
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using Linq;

    // ReSharper disable once InconsistentNaming
    public class TS3Client
    {
        private readonly TcpClient client;
        private DateTime lastMessage;
        private StreamReader reader;
        private StreamWriter writer;

        private readonly Queue<string> commandQueue = new Queue<string>();

        /// <summary>
        /// Represents a mapping of commands to their mapping.
        /// </summary>
        private readonly ReadOnlyDictionary<string, string> map =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
                                                   {
                                                       {"hostinfo", "instance_uptime"},
                                                       {"instanceinfo", "serverinstance"},
                                                       {"version", "version"},
                                                       {"bindinglist", "ip"},
                                                       {"serverlist", "virtualserver_id"},
                                                       {"servercreate", "sid"},
                                                   });

        public TS3Client()
        {
            client = new TcpClient();

            RawMessageReceivedEvent += RegistrationHandler;
            RawMessageReceivedEvent += ResponseHandler;
        }

        #region Properties

        public string Host { get; set; }

        public int QueryPort { get; set; }

        public int ServerPort { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        #endregion

        #region Events

        private event EventHandler<RawMessageReceivedEventArgs> RawMessageReceivedEvent;

        #endregion
        
        #region Methods

        public async void Connect()
        {
            if (!CheckIfInitialized())
            {
                throw new ArgumentException(String.Format("The TS3Client has not been initialized properly. Please check the Host ({0}), QueryPort ({1}), and ServerPort ({2})", Host, QueryPort, ServerPort));
            }

            try
            {
                var entry = await Dns.GetHostEntryAsync(Host);
                if (entry.AddressList.Length > 0)
                {
                    var ip = entry.AddressList[0];
                    client.Connect(new IPEndPoint(ip, QueryPort));

                    var stream = client.GetStream();
                    reader     = new StreamReader(stream, new UTF8Encoding(false));
                    writer     = new StreamWriter(stream, new UTF8Encoding(false));

                    await
                        reader.ReadLineAsync().
                            ContinueWith(OnAsyncRead, new object(), TaskContinuationOptions.LongRunning);
                }
            }
            catch (SocketException e)
            {
            }
        }

        private bool CheckIfInitialized()
        {
            return !string.IsNullOrEmpty(Host) && (QueryPort != 0 && ServerPort != 0);
        }

        public void Disconnect()
        {
        }

        private async void OnAsyncRead(Task<string> task, object state)
        {
            if (task.Exception == null && task.Result != null && !task.IsCanceled)
            {
                lastMessage = DateTime.Now;
                RawMessageReceivedEvent.Raise(this, new RawMessageReceivedEventArgs(task.Result));

                await reader.ReadLineAsync().ContinueWith(OnAsyncRead, state, TaskContinuationOptions.LongRunning);
            }
            else if (task.Exception != null)
            {
                client.Close();
            }
            else if (task.Result == null || task.IsCanceled)
            {
                client.Close();
            }
        }

        private IDictionary<string, string> ReadProperties(string input)
        {
            string[] kvps = input.Split(' ');

            return kvps.Select(item => item.Split(new[] {'='}, 2)).ToDictionary(kvp => kvp[0], kvp => kvp[1]);
        }

        public async void Send(string format, params object[] args)
        {
            string message = String.Format(format, args);
            string command = message.Split(' ')[0];

            commandQueue.Enqueue(command);

            await writer.WriteAsync(message);
            await writer.FlushAsync();
        }

        #endregion
        
        #region Handlers

        private void ResponseHandler(object sender, RawMessageReceivedEventArgs e)
        {
            // 
        }

        private void RegistrationHandler(object sender, RawMessageReceivedEventArgs e)
        {
            if (e.Message.StartsWith("TS3", StringComparison.OrdinalIgnoreCase))
            {
                Send("login client_login_name={0} client_login_password={1}", Login, Password);
                Send("use port={0}", ServerPort);

                RawMessageReceivedEvent -= RegistrationHandler;
            }
        }

        #endregion

        #region Nested type: TS3Response

        // ReSharper disable InconsistentNaming
        public struct TS3Response
        {
            public TS3Response(int id, string message, IDictionary<string, string> data) : this()
            {
                Id         = id;
                Message    = message;
                Properties = new ReadOnlyDictionary<string, string>(data);
            }

            public int Id { get; private set; }

            public string Message { get; private set; }

            public ReadOnlyDictionary<string, string> Properties { get; private set; }
        }
        // ReSharper restore InconsistentNaming

        #endregion
        
        #region Nested type: RawMessageReceivedEventArgs

        internal class RawMessageReceivedEventArgs : EventArgs
        {
            public string Message { get; private set; }

            public string[] Tokens { get; private set; }

            public RawMessageReceivedEventArgs(string message)
            {
                Message = message;
                Tokens = message.Split(' ');
            }
        }

        #endregion
    }
}
