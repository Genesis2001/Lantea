// -----------------------------------------------------------------------------
//  <copyright file="MockModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.UnitTests.Mocks
{
	using System;
	using Common.Modules;

	public class MockModule : IModule
	{
		private bool isEnabled;
		private bool isLoaded;

		#region Implementation of IModuleMeta

		public string Author
		{
			get { return "Zack Loveless"; }
		}

		public string Version
		{
			get { return "1.0.0"; }
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

		#endregion

		#region Implementation of IModule

		public bool IsLoaded
		{
			get { return isLoaded; }
			private set { isLoaded = value; }
		}

		public void Load()
		{
			if (IsLoaded)
			{
				throw new InvalidOperationException("The module has already been loaded.");
			}

			IsLoaded = true;
		}

		public void Unload()
		{
			if (!IsLoaded)
			{
				throw new InvalidOperationException("The module is not loaded. Cannot unload.");
			}

			IsLoaded = false;
		}

		#endregion
	}
}
