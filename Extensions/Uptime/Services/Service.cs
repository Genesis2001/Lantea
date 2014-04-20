// -----------------------------------------------------------------------------
//  <copyright file="Service.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using System.Collections.Generic;

	public abstract class Service
	{
		#region Properties

		public virtual String DisplayName { get; set; }

		public virtual String HostName { get; set; }

		public virtual Int32 Port { get; set; }

		public virtual Int32 Timeout { get; set; }

		public virtual Int32 RetryAttempts { get; set; }

		#endregion

		#region Methods

		public abstract String Check();

		public abstract void Initialize(IDictionary<String, String> data);

		#endregion
	}
}
