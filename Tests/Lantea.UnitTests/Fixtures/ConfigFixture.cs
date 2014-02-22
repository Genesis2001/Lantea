// -----------------------------------------------------------------------------
//  <copyright file="Config.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Fixtures
{
	using System.IO;
	using System.Net;
	using Core.IO;
	using Helpers;
	using NUnit.Framework;

	[TestFixture]
	public class ConfigFixture
	{
		private Config SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new Config();
		}

		[Test]
		public void Load_BasicFileWithBasicTypes()
		{
			Stream stream = ConfigReaderStrings.BasicBlock.AsReadOnlyStream();

			/*ConfigDocument doc = SUT.Load(stream);

			Assert.That(doc, Is.Not.Null);*/
		}

		[Test]
		public void Load_FromStream_DownloadedFromGithub()
		{
			string data = new WebClient().DownloadString("https://raw2.github.com/anope/anope/2.0/data/example.conf");

			Stream stream = data.AsReadOnlyStream();

			SUT.Load(stream);
		}
	}
}
