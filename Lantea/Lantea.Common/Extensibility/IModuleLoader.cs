// -----------------------------------------------------------------------------
//  <copyright file="IModuleLoader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.Collections.Generic;

	public interface IModuleLoader : IIoCContainer
	{
		/// <summary>
		/// Represents an event triggered when modules are done loading.
		/// </summary>
		event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value path relative to the main entry point.
		/// </summary>
		String Location { get; set; }

		/// <summary>
		/// Gets a collection of modules loaded by the module loader.
		/// </summary>
		IEnumerable<IModule> Modules { get; }

		/// <summary>
		/// Returns a collection of T from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> GetExportedValues<T>();

		/// <summary>
		/// Returns a collection of T from the composition container that have the specified metadata
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMetadata"></typeparam>
		/// <returns></returns>
		IEnumerable<Lazy<T, TMetadata>> GetExportedValues<T, TMetadata>();

		/// <summary>
		/// Returns a single item from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T GetExportedValue<T>();

		/// <summary>
		/// Initializes the composition container and loads the bot.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Loads discovered modules.
		/// </summary>
		void Load();
		
		/// <summary>
		/// Unloads modules.
		/// </summary>
		void Unload();
	}
}
