// -----------------------------------------------------------------------------
//  <copyright file="IBotCore.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using Atlantis.Net.Irc;
	using IO;

	[InheritedExport(typeof (IBotCore))]
	public interface IBotCore : IDisposable
	{
		IrcClient Client { get; }

		Configuration Config { get; }

		IEnumerable<IModule> Modules { get; }

		void Initialize();

		void Load(String path, String module = null);
	}
}
