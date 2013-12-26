// -----------------------------------------------------------------------------
//  <copyright file="LogType.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;

	[Flags]
	public enum LogType
	{
		None = 0x00,
		Info = 0x01,
		Warning = 0x02,
		Error = 0x04,
		Debug = 0x08,
	}
}
