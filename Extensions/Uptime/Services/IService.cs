// -----------------------------------------------------------------------------
//  <copyright file="Service.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;

	public interface IService
	{
		String Display { get; set; }

		String HostName { get; set; }

		Int32 Port { get; set; }

		String UserName { get; set; }

		String Password { set; }

		String Check();
	}
}
