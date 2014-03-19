// -----------------------------------------------------------------------------
//  <copyright file="ModuleLoaderFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.IntegrationTests
{
	using Core.Modules;
	using NUnit.Framework;

	[TestFixture]
	public class ModuleLoaderFixture
	{
		[SetUp]
		public void Setup()
		{
			SUT = new ModuleLoader();
		}

		private ModuleLoader SUT;


	}
}
