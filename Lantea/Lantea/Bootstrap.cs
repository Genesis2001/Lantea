// -----------------------------------------------------------------------------
//  <copyright file="Bootstrap.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
    using System;
    using System.IO;
    using Atlantis.Net.Irc;
    using Common;
    using Common.IO;
    using Common.Linq;
    using NDesk.Options;

    public class Bootstrap
    {
        private static readonly TimeSpan interval       = new TimeSpan(0, 0, 0, 0, 100);
        private static readonly IIoCContainer container = new IoCContainer();
        private static readonly OptionSet options = new OptionSet
                                                    {
                                                         {"config=","", x => configFile = x}
                                                    };

        private static IrcClient client;
        private static string configFile;

        public static void Main(string[] args)
        {
            options.Parse(Environment.GetCommandLineArgs());

            if (String.IsNullOrEmpty(configFile))
            {
                // defaulting to a standard config.
                configFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Lantea.conf");
            }

            if (!File.Exists(configFile))
            {
                Console.Write("Unable to find the specified configuration file: {0} (Located: {1})",
                    Path.GetFileName(configFile),
                    Path.GetPathRoot(configFile));

                Console.ReadLine();
                Environment.Exit(3);
            }

            Configuration config = new Configuration();
//            config.ConfigurationLoadEvent += OnConfigLoaded;
            config.Load(configFile);

            Block uplink = config.GetBlock("uplink");
            if (uplink == null)
            {
                Console.Write("Unable to load IRC uplink information. Be sure to include an <uplink> block inside your configuration file and re-launch the application.");
                Console.ReadLine();
                Environment.Exit(2);
            }

            var ircinfo = uplink.AsIrcConfig();
            client = new IrcClient(ircinfo);

            ModuleManager manager = new ModuleManager(container);

            container.RegisterContract(config);
            container.RegisterContract(client);
            container.RegisterContract(manager);

            Block modules = config.GetBlock("modules");
            string modulesDirectory = @".\Extensions";

            if (modules != null)
            {
                modulesDirectory = modules.GetString("directory", modulesDirectory);
            }

            modulesDirectory = modulesDirectory.GetAbsolutePath();
            
            manager.LoadDirectory(modulesDirectory);

            client.Start();

            Console.Write("Press <CTRL+C> to terminate.");
            bool exit = false;
            do
            {
                System.Threading.Thread.Sleep(interval);

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
                {
                    exit = true;
                    client.Disconnect("Received console termination request.");
                }
            } while (!exit);

#if DEBUG
            Console.Write("Debug build detected. Press <ENTER> to continue control-breaking the app.");
            Console.ReadLine();
#endif
        }
        
        private static void OnConfigLoaded(object sender, ConfigurationLoadEventArgs args)
        {
            if (args.Success && args.Exception == null)
            {
                // I forget...
            }
        }
    }
}
