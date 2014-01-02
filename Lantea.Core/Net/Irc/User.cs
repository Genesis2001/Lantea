// -----------------------------------------------------------------------------
//  <copyright file="Nick.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Text.RegularExpressions;
	using Common;
	using Common.Linq;

	public class User : IObservableClass
	{
		private IrcClient client;
		private string host;
		private string ident;
		private string nick;
		private string realName;
		
		public User(string nick)
		{
			this.nick     = nick;
		}

		#region Properties
		
		public string Host
		{
			get { return host; }
			set
			{
				NotifyPropertyChanging();
				host = value;
				NotifyPropertyChanged();
			}
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

		#endregion

		#region Methods

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

		public void SetClient(IrcClient pClient)
		{
			if (client == null)
			{
				client = pClient;
			}
		}

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Nick;
		}

		#endregion

		public static User FromRfcString(string value)
		{
			Match m;
			if (!value.TryMatch(@":?([^!]+)!(([^@]+)@(\S+))", out m))
				throw new FormatException("Unable to parse RFC1459 Nick String.");

			return new User(m.Groups[1].Value) {Ident = m.Groups[3].Value, Host = m.Groups[4].Value};
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
