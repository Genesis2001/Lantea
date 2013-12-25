// -----------------------------------------------------------------------------
//  <copyright file="User.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc.Data
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using Common;

	public class User : IObservableClass
	{
		private readonly IrcClient client;
		private string ident;
		private string nick;
		private string realName;

		public User(IrcClient client)
		{
			this.client = client;
		}

		public User(IrcClient client, string nick, string realName)
			: this(client)
		{
			this.nick = nick;
			this.realName = realName;
		}

		public string Ident
		{
			get { return ident; }
			set
			{
				NotifyPropertyChanging();
				ident = value;
				NotifyPropertyChanged();
			}
		}

		public string Nick
		{
			get { return nick; }
			set
			{
				NotifyPropertyChanging();
				nick = value;
				NotifyPropertyChanged();
			}
		}

		public string RealName
		{
			get { return realName; }
			set
			{
				NotifyPropertyChanging();
				realName = value;
				NotifyPropertyChanged();
			}
		}

		public void Message(string format, params object[] args)
		{
			if (!client.Connected) return;

			var message = string.Format(format, args);
			client.Send("PRIVMSG {0} :{1}", Nick, message);
		}

		public void Notice(string format, params object[] args)
		{
			if (!client.Connected) return;

			var message = string.Format(format, args);
			client.Send("NOTICE {0} :{1}", Nick, message);
		}

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Implementation of INotifyPropertyChanging

		public event PropertyChangingEventHandler PropertyChanging;

		#endregion

		#region Implementation of IObservableClass

		public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public void NotifyPropertyChanging([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanging;
			if (handler != null) handler(this, new PropertyChangingEventArgs(propertyName));
		}

		#endregion
	}
}
