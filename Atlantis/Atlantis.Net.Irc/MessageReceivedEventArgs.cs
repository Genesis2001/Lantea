// -----------------------------------------------------------------------------
//  <copyright file="MessageReceivedEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Net.Irc
{
	using System;

	public class MessageReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
		/// </summary>
		public MessageReceivedEventArgs(string nick, string target, string message)
		{
			Nick    = nick;
			Message = message;
			Target  = target;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Nick { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Target { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Message { get; private set; }
	}
}
