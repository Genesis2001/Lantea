// -----------------------------------------------------------------------------
//  <copyright file="ModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ModuleAttribute : Attribute, IModuleAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Attribute"/> class.
		/// </summary>
		public ModuleAttribute(ModuleType type, string version)
		{
			Type    = type;
			Version = version;
		}

		#region Implementation of IModuleAttribute

		public string Name { get; private set; }
		
		public string Author { get; set; }
		
		public ModuleType Type { get; private set; }

		public string Version { get; private set; }

		#endregion
	}
}
