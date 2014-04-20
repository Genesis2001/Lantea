// -----------------------------------------------------------------------------
//  <copyright file="ModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.ComponentModel.Composition;

	[MetadataAttribute, AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
	public class ModuleAttribute : Attribute, IModuleAttribute
	{
		public ModuleAttribute()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Attribute"/> class.
		/// </summary>
		public ModuleAttribute(string author, string description, string name, string version, ModuleType moduleType)
		{
			Author      = author;
			Description = description;
			Name        = name;
			Version     = version;
			ModuleType        = moduleType;
		}

		#region Implementation of IModuleAttribute

		public string Author { get; set; }

		public string Description { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public ModuleType ModuleType { get; set; }

		#endregion
	}
}
