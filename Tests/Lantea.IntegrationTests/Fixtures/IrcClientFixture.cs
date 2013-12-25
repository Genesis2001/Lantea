// -----------------------------------------------------------------------------
//  <copyright file="IrcClientFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.IntegrationTests.Fixtures
{
	using Core.Net.Irc;
	using Helpers;
	using NUnit.Framework;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	public class IrcClientFixture
	{
		[SetUp]
		public void IrcClient_SetUp()
		{
			SUT = new IrcClient(TODO, TODO);

			IrcClient.GetHostEntry = DnsHelper.GetHostEntry;
		}

		private IrcClient SUT;

		[Test]
		[Category("Unit Test")]
		public void Ctor_ShouldHaveGetHostEntrySet_WhenConstructed()
		{
			Assert.That(IrcClient.GetHostEntry, Is.Not.Null);
		}

		[Test]
		[Category("Unit Test")]
		public void Host_ShouldBeValidHostName()
		{
			SUT.Host = "irc.unifiedtech.org";

			var actual = IrcClient.GetHostEntry(SUT.Host);

			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.AddressList, Is.Not.Empty);
		}

		[Test]
		[Category("Unit Test")]
		public void Nick_ShouldNotBeNullOrEmpty_WhenSet()
		{
			SUT.User.Nick = "foo";

			Assert.That(SUT.User.Nick, Is.Not.Null);
			Assert.That(SUT.User.Nick, Is.Not.Empty);
		}

		[Test]
		[Category("Unit Test")]
		public void IsInitialized_ShouldReturnFalse_WhenNickNotSet()
		{
			SUT.User.Nick = null;

			Assert.That(SUT.IsInitialized, Is.False);
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
