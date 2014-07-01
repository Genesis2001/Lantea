// -----------------------------------------------------------------------------
//  <copyright file="ModuleManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Atlantis.Net.Irc;
    using Common;
    using Common.Extensibility;
    using Common.IO;
    using Common.Linq;

    public class ModuleManager
    {
        private readonly IIoCContainer iocc;
        private CompositionContainer container;
        private AggregateCatalog catalog;

        private bool temp;

        public ModuleManager(IIoCContainer iocc)
        {
            this.iocc = iocc;
        }

        private void ProcessModule(Lazy<IModule, IModuleAttribute> lazy)
        {
            IModule module        = lazy.Value;
            IModuleAttribute attr = lazy.Metadata;
            
            Configuration config  = iocc.RetrieveContract<Configuration>();
            Block mBlock          = config.GetModule(module);

            if (mBlock == null)
            {
                // Module disabled.
                // Do not continue and call it's initialize method at the end of this method.
                return;
            }

            // TODO: Replace these console writes with the logging system.
            Console.WriteLine("Loading module: {0} v{1} by {2}", module.Name, module.Version, module.Author);

            Block block = config.GetBlock(module.Name);
            if (!String.IsNullOrEmpty(attr.ConfigBlock))
            {
                block = config.GetBlock(attr.ConfigBlock);
            }

            IrcClient client = null;
            if (attr.CreateNewClient)
            {
                if (block != null)
                {
                    Block uplink = block.GetBlock("uplink");
                    if (uplink != null)
                    {
                        var ircinfo = uplink.AsIrcConfig(); // possible null ref, but acceptable.
                        client = new IrcClient(ircinfo);
                    }
                }

                if (client == null)
                {
                    client = new IrcClient(); // failed to load conventioned config, let the module handle initializing.
                }
            }
            else
            {
                client = iocc.RetrieveContract<IrcClient>();
            }

            module.Initialize(block, client);

            Console.WriteLine("Loaded: {0}", module.Name);
        }

        #region Implementation of IModuleManager

        [ImportMany(typeof(IModule), AllowRecomposition = true)]
        public List<Lazy<IModule, IModuleAttribute>> Modules { get; set; }

        public void Dispose()
        {
            Modules.ForEach(x => x.Value.Dispose());
        }

        public void LoadDirectory(string directory)
        {
            if (temp)
            {
                throw new NotImplementedException();
            }

            if (container != null)
            {
                var item =
                    catalog.OfType<DirectoryCatalog>()
                        .FirstOrDefault(x => x.Path.Equals(directory, StringComparison.OrdinalIgnoreCase));

                if (item != null)
                {
                    item.Refresh();
                }
                else
                {
                    item = new DirectoryCatalog(directory, "*.dll");
                    catalog.Catalogs.Add(item);
                    
                    // todo...this might be bad.
//                    container.ComposeParts(this);
//                    modules.ForEach(ProcessModule);
                }
            }
            else
            {
                var item = new DirectoryCatalog(directory, "*.dll");
                if (catalog == null)
                {
                    catalog = new AggregateCatalog(item);
                }

                container = new CompositionContainer(catalog);
                Modules   = container.GetExports<IModule, IModuleAttribute>().ToList();

                Modules.ForEach(ProcessModule);
                temp = true;
            }
        }

        public void Unload(IModule module)
        {
            throw new NotSupportedException("Not supported at this time.");
        }

        public void Unload(string module)
        {
            throw new NotSupportedException("Not supported at this time.");
        }

        #endregion
    }

    public static class ConfigurationBlockExtensions
    {
        public static IrcConfiguration AsIrcConfig(this Block source)
        {
            if (source == null) throw new ArgumentNullException("source");

            // TODO: Load "SslEnabled"... (ircclient doesnt even support ssl atm)
            return new IrcConfiguration
                   {
                       Host     = source.GetString("host", "irc.cncfps.com"),
                       Port     = source.GetInt32("port", 6667),
                       Nick     = source.GetString("nick", "Lantea"),
                       Ident    = source.GetString("username", "lantea"),
                       RealName = source.GetString("name", "Lantea IRC Bot"),
                       Password = source.GetString("password", ""),
                   };
        }
    }
}
