// -----------------------------------------------------------------------------
//  <copyright file="ModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : Attribute, IModuleAttribute
	{
		#region Implementation of IModuleAttribute

		public string Author { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public ModuleType Type { get; set; }

		#endregion
	}
}
