// -----------------------------------------------------------------------------
//  <copyright file="StubObject.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using System;

	internal class StubObject
	{
		public Guid Id { get; set; }

		#region Overrides of Object

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Id.ToString();
		}

		#endregion
	}
}
