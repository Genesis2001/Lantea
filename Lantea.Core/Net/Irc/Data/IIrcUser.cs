// -----------------------------------------------------------------------------
//  <copyright file="IIrcUser.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc.Data
{
	using System.ComponentModel;

	public interface IIrcUser : INotifyPropertyChanged
	{
		string Nick { get; set; }
		string RealName { get; set; }
	}
}
