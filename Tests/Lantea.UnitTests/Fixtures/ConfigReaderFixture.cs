// -----------------------------------------------------------------------------
//  <copyright file="ConfigReaderFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Fixtures
{
	using System.IO;
	using Core.IO;
	using Helpers;
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

		[Test]
		public void Load_BasicFileWithBasicTypes()
		{
			Stream stream = ConfigReaderStrings.BasicBlock.AsReadOnlyStream();

			ConfigDocument doc = SUT.Load(stream);

			Assert.That(doc, Is.Not.Null);
		}
	}
}
