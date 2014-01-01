// -----------------------------------------------------------------------------
//  <copyright file="Target.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc.Data
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using Common;

	public class Channel : IObservableClass
	{
		// TODO: Add channel user list.
		// TODO: Add channel permissions for users.

		private IrcClient client;

		public Channel(string channelName)
		{
			Name = channelName;
		}

		#region Properties

		public string Name { get; private set; }

		#endregion

		#region Methods

		public void SetClient(IrcClient pClient)
		{
			if (client == null)
			{
				client = pClient;
			}
		}

		public void Message(string format, params object[] args)
		{
			if (!client.Connected) return;

			var message = string.Format(format, args);
			client.Send("PRIVMSG {0} :{1}", Name, message);
		}

		public void Notice(string format, params object[] args)
		{
			if (!client.Connected) return;

			var message = string.Format(format, args);
			client.Send("NOTICE {0} :{1}", Name, message);
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
			return Name;
		}

		#endregion

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
