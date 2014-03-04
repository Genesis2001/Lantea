// -----------------------------------------------------------------------------
//  <copyright file="DictionaryListFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using Atlantis.Collections;
	using NUnit.Framework;

	[TestFixture]
	public class DictionaryListFixture
	{
		private DictionaryList<StubObject> SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new DictionaryList<StubObject>();
		}
	}
}
