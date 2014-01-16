// -----------------------------------------------------------------------------
//  <copyright file="ConnectOptions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------
using System;

namespace Lantea.Core.Net.Irc
{
	[Flags]
	public enum ConnectOptions
	{
		Default = 0,
		Secure = 1,
	}
}