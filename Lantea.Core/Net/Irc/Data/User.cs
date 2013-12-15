// -----------------------------------------------------------------------------
//  <copyright file="User.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc.Data
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public class User : IIrcUser
	{
		private readonly IrcClient client;
		private string nick;
		private string realName;

		public User(IrcClient client, string nick, string realName)
		{
			this.client = client;
			this.nick = nick;
			this.realName = realName;
		}

		#region Implementation of IIrcUser

		public string Nick
		{
			get { return nick; }
			set
			{
				nick = value;
				OnPropertyChanged();
				SendNickCommand(value);
			}
		}

		public string RealName
		{
			get { return realName; }
			set
			{
				realName = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private void SendNickCommand(string value)
		{
			// if (client != null && client.Connected) client.Send("NICK {0}", value);
		}
	}
}
