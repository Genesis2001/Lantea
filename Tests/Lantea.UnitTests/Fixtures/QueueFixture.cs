// -----------------------------------------------------------------------------
//  <copyright file="QueueFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Fixtures
{
	using Common.Collections;
	using NUnit.Framework;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	public class QueueFixture
	{
		[SetUp]
		public void IQueue_SetUp()
		{
			SUT = new QueueAdapter<string>();
		}

		private IQueue<string> SUT;

		private const string firstString  = "First String";
		private const string secondString = "Second String";
		private const string thirdString  = "Third String";

		[Test]
		public void Push_ShouldAddItemToBackOfQueue()
		{
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
