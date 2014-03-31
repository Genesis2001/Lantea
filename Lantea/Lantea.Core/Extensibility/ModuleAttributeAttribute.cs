// -----------------------------------------------------------------------------
//  <copyright file="ModuleAttributeAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttributeAttribute : Attribute, IModuleAttribute
	{
		#region Implementation of IModuleAttribute

		public string Author { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public ModuleType Type { get; set; }

		#endregion
	}
}
