// -----------------------------------------------------------------------------
//  <copyright file="Hello.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Hello
{
    using System;
    using System.ComponentModel.Composition;
    using Atlantis.Net.Irc;
    using Lantea.Common;
    using Lantea.Common.Extensibility;
    using Lantea.Common.IO;

    [Module(ConfigBlock = "hello")]
	public class HelloModule : IModule
    {
        private readonly IIoCContainer iocc;

        [ImportingConstructor]
        public HelloModule([Import] IIoCContainer iocc)
        {
            this.iocc = iocc;
        }

        #region Implementation of IModule

        public string Author
        {
            get { return "Zack Loveless"; }
        }

        public string Description
        {
            get { return "An example module for Lantea."; }
        }

        public string Name
        {
            get { return "Hello"; }
        }

        public string Version
        {
            get { return "1.3"; }
        }

        public ModuleType ModType
        {
            get { return ModuleType.VENDOR; }
        }

        public void Dispose()
        {
        }

        public void Initialize(Block config, IrcClient client)
        {
            Console.WriteLine("Hello World");
        }

        public void Rehash(Block config)
        {
        }

        #endregion
	}
}
