// -----------------------------------------------------------------------------
//  <copyright file="MessageReceivedEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;

	public class MessageReceivedEventArgs : EventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
		/// </summary>
		public MessageReceivedEventArgs(string nick, string message)
		{
			Nick = nick;
			Message = message;
		}

		public MessageReceivedEventArgs(string nick, string target, string message) : this(nick, message)
		{
			Target = target;
		}

		public string Nick { get; private set; }
		public string Target { get; private set; }
		public string Message { get; private set; }
	}
}
