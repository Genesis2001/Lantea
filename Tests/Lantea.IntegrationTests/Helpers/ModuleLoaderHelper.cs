// -----------------------------------------------------------------------------
//  <copyright file="ModuleLoaderHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.IntegrationTests.Helpers
{
	using System.Collections.Generic;
	using Common.Modules;
	using Mocks;

	public static class ModuleLoaderHelper
	{
		public static IEnumerable<IModule> GetMockModules()
		{
			return new IModule[]
			       {
				       new ModuleA(),
					   new ModuleB()
			       };
		}


	}
}
