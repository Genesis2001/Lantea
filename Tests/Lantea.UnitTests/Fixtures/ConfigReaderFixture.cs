// -----------------------------------------------------------------------------
//  <copyright file="ConfigReaderFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Fixtures
{
	using Core.IO;
	using NUnit.Framework;

	[TestFixture]
	public class ConfigReaderFixture
	{
		private ConfigReader SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new ConfigReader();
		}


	}
}
