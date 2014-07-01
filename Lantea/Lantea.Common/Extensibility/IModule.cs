// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
    using System.ComponentModel.Composition;
    using Atlantis.Net.Irc;
    using IO;

    public interface IModule
    {
        string Author { get; }
        
        string Description { get; }
        
        string Name { get; }
        
        string Version { get; }

        void Dispose();

        void Initialize(Block config, IrcClient client);
    }
}
