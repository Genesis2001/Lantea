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
		#region Implementation of IModuleAttribute

		public string Name { get; set; }

		public string Author { get; set; }

		public ModuleType Type { get; set; }

		public string Version { get; set; }

		#endregion
	}
}
