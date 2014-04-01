// -----------------------------------------------------------------------------
//  <copyright file="ConfigurationLoadEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;

	public class ConfigurationLoadEventArgs : EventArgs
	{
		public ConfigurationLoadEventArgs(bool success)
		{
			Success = success;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
		/// </summary>
		public ConfigurationLoadEventArgs(bool success, Exception exception)
		{
			Success = success;
			Exception = exception;
		}

		public Exception Exception { get; private set; }
		public bool Success { get; private set; }
	}
}
