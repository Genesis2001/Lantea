// -----------------------------------------------------------------------------
//  <copyright file="IBotCore.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.ComponentModel.Composition;
	using Atlantis.Net.Irc;
	using IO;

	[InheritedExport(typeof (IBotCore))]
	public interface IBotCore : IDisposable
	{
		IrcClient Client { get; }

		Configuration Config { get; }

		IModuleLoader ModuleLoader { get; }

		void Initialize(String configFile);

		void Load(String moduleName);
	}
}
