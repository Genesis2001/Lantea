// -----------------------------------------------------------------------------
//  <copyright file="NullService.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using System.Collections.Generic;

	public class NullService : Service
	{
		#region Overrides of Service

		public override string Check()
		{
			throw new NotSupportedException();
		}

		public override void Initialize(IDictionary<string, string> data)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
