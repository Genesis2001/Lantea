// -----------------------------------------------------------------------------
//  <copyright file="ModuleA.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.IntegrationTests.Mocks
{
	using Common.Modules;

	public class ModuleA : IModule
	{
		#region Implementation of IModuleMeta

		public string Author
		{
			get { return "AuthorA"; }
		}

		public Version Version
		{
			get { return "1.0"; }
		}

		public bool IsEnabled { get; set; }

		#endregion

		#region Implementation of IModule

		public void Load()
		{
			throw new System.NotImplementedException();
		}

		public void Unload()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}

	public class ModuleB : IModule
	{
		#region Implementation of IModuleMeta

		public string Author
		{
			get { return "AuthorB"; }
		}

		public Version Version
		{
			get { return "1.2.5.3"; }
		}

		public bool IsEnabled { get; set; }

		#endregion

		#region Implementation of IModule

		public void Load()
		{
		}

		public void Unload()
		{
		}

		#endregion
	}
}
