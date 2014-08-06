// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
    using Atlantis.Net.Irc;
    using IO;

    public interface IModule
    {
        string Author { get; }
        
        string Description { get; }
        
        string Name { get; }
        
        string Version { get; }

        ModuleType ModType { get; }

        void Dispose();

        void Initialize(Block config, IrcClient client);

        void Rehash(Block config);
    }
}
