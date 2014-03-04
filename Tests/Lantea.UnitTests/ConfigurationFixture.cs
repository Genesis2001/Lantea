// -----------------------------------------------------------------------------
//  <copyright file="ConfigurationFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.UnitTests
{
	using System.IO;
	using Core.IO;
	using NUnit.Framework;
	using TestHelpers;

	[TestFixture]
	public class ConfigurationFixture
	{
		private Configuration SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new Configuration();
		}

		[Test]
		public void CountBlock_FromStream_ShouldReturnOne()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			const int expected = 1;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}
	}
}
