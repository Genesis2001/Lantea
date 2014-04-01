// -----------------------------------------------------------------------------
//  <copyright file="IBotCore.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.ComponentModel.Composition;
	using Atlantis.IO;
	using Atlantis.Net.Irc;
	using IO;

	[InheritedExport(typeof (IBotCore))]
	public interface IBotCore : IDisposable
	{
		IrcClient Client { get; }

		/// <summary>
		/// Gets a <see cref="T:Lantea.Common.IO.Configuration" /> instance of the configuration.
		/// </summary>
		Configuration Config { get; }

		ILog Log { get; }

		/// <summary>
		/// Gets an <see cref="T:IModuleLoader" /> instance for the current bot core.
		/// </summary>
		IModuleLoader ModuleLoader { get; }

		/// <summary>
		/// Initializes the core bot with the specified configuration file.
		/// </summary>
		/// <param name="configFile">The path to the configuration file.</param>
		void Initialize(String configFile);

		/// <summary>
		/// Loads the specified module's configuration into memory.
		/// </summary>
		/// <param name="moduleName">The name of the module to look for a standardized configuration.</param>
		void Load(String moduleName);
	}
}
