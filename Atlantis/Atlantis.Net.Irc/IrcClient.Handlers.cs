// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Net.Irc
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Timers;
	using Linq;

	public partial class IrcClient
	{
		#region Fields

		internal string accessPrefixes;
		internal string accessRegex;
		internal string channelModes;

		private bool registered;
		private DateTime lastMessage;
		private Timer timeoutTimer;
		private string rfcStringCase;

		#endregion

		// TODO: Handle disconnection events.
		// TODO: DEBUG RECV: ERROR :Closing link: (lantea@1.2.3.4) [Killed (Genesis2001 (foo))]

		private void TickTimeout()
		{
			timeoutTimer          = new Timer(Timeout.TotalMilliseconds);
			timeoutTimer.Elapsed += OnTimeoutTimerElapsed;
			timeoutTimer.Start();
		}

		#region Handlers

		#region IRC Numeric Handlers

		protected virtual void ConnectionHandler(object sender, RfcNumericEventArgs args)
		{
			var header = (IrcHeaders)args.Numeric;

			if (header == IrcHeaders.RPL_WELCOME)
			{
				ConnectionEstablishedEvent.Raise(this, EventArgs.Empty);
			}
		}
		
		protected virtual void RfcProtocolHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			var message = args.Message;

			if (header == IrcHeaders.RPL_PROTOCTL)
			{
				Match m;
				if (message.TryMatch(@"PREFIX=\((\S+)\)(\S+)", out m))
				{
					accessPrefixes = m.Groups[2].Value;
				}
				else if (message.TryMatch(@"CHANMODES=(\S+)", out m))
				{
					channelModes = m.Groups[1].Value;
				}
				else if (message.TryMatch(@"CASEMAPPING=([a-z\-])", out m))
				{
					rfcStringCase = m.Groups[1].Value;
				}
			}
		}

		protected virtual void RfcNamesHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			var message = args.Message;

			if (header == IrcHeaders.RPL_NAMREPLY)
			{
				var toks   = message.Split(' ');
				var c      = GetChannel(toks[2]);
				var names  = message.Substring(message.IndexOf(':') + 1);

				MatchCollection collection;
				if (string.IsNullOrEmpty(accessRegex))
				{
					accessRegex = string.Format(@"(?<prefix>[{0}]?)(?<nick>\S+)", accessPrefixes);
				}

				if (names.TryMatches(accessRegex, out collection))
				{
					foreach (Match item in collection)
					{
						var prefix = item.Groups["prefix"].Value.Length > 0 ? item.Groups["prefix"].Value[0] : (char)0;
						var nick   = item.Groups["nick"].Value;

						if (c.Users.ContainsKey(nick))
						{
							c.Users[nick].AddPrefix(prefix);
						}
						else
						{
							var p = new PrefixList(this);
							p.AddPrefix(prefix);
							c.Users.Add(nick, p);
						}
					}
				}
			}
		}

		protected virtual void ListModeHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			
			if (header == IrcHeaders.RPL_BANLIST || header == IrcHeaders.RPL_EXCEPTLIST || header == IrcHeaders.RPL_INVITELIST)
			{
				var message = args.Message;

				var c    = GetChannel("");
				var type = '\0';

				switch (header)
				{
					case IrcHeaders.RPL_BANLIST:
						type = 'b';
						break;
					
					case IrcHeaders.RPL_EXCEPTLIST:
						type = 'e';
						break;

					case IrcHeaders.RPL_INVITELIST:
						type = 'I';
						break;
				}

				if (c.ListModes.Find(x => x.Mask.Equals("")) != null)
				{
					return;
				}

				var l = new ListMode(type, DateTime.Now, "", "");
				c.ListModes.Add(l);
			}
		}

		protected virtual void NickInUseHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			
			if (header == IrcHeaders.ERR_NICKNAMEINUSE)
			{
				var message = args.Message;
				var toks    = message.Split(' ');
				var newNick = string.Concat(toks[1], "_");

				ChangeNick(newNick);

				if (RetryNick)
				{
					Task.Factory.StartNew(async () =>
					                        {
						                        if (!registered)
						                        {
							                        await Task.Delay(Convert.ToInt32(RetryInterval), token);
						                        }

						                        ChangeNick(Nick);
					                        }, token);
				}
			}
		}

		#endregion

		#region IRC Raw Message Handlers

		protected virtual void ProtocalMessageHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			// Regular Expression Credits
			// created by Chris J. Hogben (http://cjh.im/)
			// modified by Zack Loveless (http://zloveless.com)

			if (message.TryMatch(@"^:?(?<source>[^!]+)\!((?<ident>[^@]+)@(?<host>\S+)) (?<command>PRIVMSG|NOTICE|JOIN|PART|QUIT|MODE|NICK) :?(?<target>\#?[^\W]+)\W?:?(?<params>.+)?$", out m))
			{
				ProtocolMessageReceivedEvent.Raise(this, new ProtocolMessageEventArgs(m, message));
			}
		}

		protected virtual void QuitHandler(object sender, ProtocolMessageEventArgs args)
		{
			var m = args.Match;

			if (m.Groups["command"].Value.Equals("QUIT"))
			{
				var source  = m.Groups["source"].Value;
				var message = m.Groups["params"].Value;

				foreach (var item in Channels)
				{
					item.Users.Remove(source);
				}

				QuitEvent.Raise(this, new QuitEventArgs(source, message));
			}
		}

		protected virtual void JoinPartHandler(object sender, ProtocolMessageEventArgs args)
		{
			var match   = args.Match;

			// :Lantea!lantea@unified-nac.jhi.145.98.IP JOIN :#UnifiedTech
			if (match.Groups["command"].Value.Matches(@"JOIN|PART"))
			{
				var nick   = match.Groups["source"].Value;
				var target = match.Groups["target"].Value;

				if (match.Groups["command"].Value.EqualsIgnoreCase("join"))
				{
					ChannelJoinEvent.Raise(this, new JoinPartEventArgs(nick, target));

					if (FillListsOnJoin && nick.EqualsIgnoreCase(Nick))
					{
						if (FillListsDelay > 0)
						{
							Task.Factory.StartNew(async () =>
													{
														// ReSharper disable once MethodSupportsCancellation
														await Task.Delay((int)FillListsDelay);

														FillChannelList(target);
													}, token);
						}
						else
						{
							FillChannelList(target);
						}
					}
				}
				else if (match.Groups["command"].Value.EqualsIgnoreCase("part"))
				{
					ChannelPartEvent.Raise(this, new JoinPartEventArgs(nick, target));
				}

				if (StrictNames)
				{
					Send("NAMES {0}", target);
				}
			}
		}

		private void FillChannelList(string channelName)
		{
			if (string.IsNullOrEmpty(channelName)) throw new ArgumentNullException("channelName");

			var listModes = channelModes.Split(',')[0];
			foreach (var mode in listModes)
			{
				Send("MODE {0} +{1}", channelName, mode);
			}
		}

		protected virtual void MessageNoticeHandler(object sender, ProtocolMessageEventArgs args)
		{
			var m = args.Match;

			if (m.Groups["command"].Value.Matches(@"PRIVMSG|NOTICE"))
			{
				var nick   = m.Groups["source"].Value;
				var target = m.Groups["target"].Value;
				var msg    = m.Groups["params"].Value;

				if (m.Groups["command"].Value.EqualsIgnoreCase("privmsg"))
				{
					MessageReceivedEvent.Raise(this, new MessageReceivedEventArgs(nick, target, msg));
				}
				else if (m.Groups["command"].Value.EqualsIgnoreCase("notice"))
				{
					NoticeReceivedEvent.Raise(this, new MessageReceivedEventArgs(nick, target, msg));
				}

				// TODO: Add support for (S)NOTICE messages.
				// TODO: Add support for NOTICE AUTH messages.
			}
		}

		private enum ChannelModeType
		{
			LIST,
			SETUNSET,
			SET,
			NOPARAM,
			ACCESS,
		}

		protected virtual void ModeHandler(object sender, ProtocolMessageEventArgs args)
		{
			var m = args.Match;

			/*
if ((m = Patterns.rUserHost.Match(toks[0])).Success && (n = Patterns.rChannelRegex.Match(toks[2])).Success)
{
    //Console.WriteLine("debug: {0}", input.Substring(input.IndexOf(toks[3])));
    if (toks.Length > 4)
    {
        // chan-user mode
        string s1 = input.Substring(input.IndexOf(toks[3])).Substring(toks[3].Length + 1);
        OnRawChannelMode(n.Groups[1].Value, m.Groups[1].Value, toks[3], s1.Split(' '));
    }
    else
    {
        // generic channel mode
        OnRawChannelMode(n.Groups[1].Value, m.Groups[1].Value, toks[3]);
    }
}
			 */

			if (m.Groups["command"].Value.Equals("MODE"))
			{

			}
		}

		protected virtual void NickHandler(object sender, ProtocolMessageEventArgs args)
		{
			var m = args.Match;

			// :Genesis2001!zack@unifiedtech.org NICK Genesis2002
			if (m.Groups["command"].Value.Equals("NICK"))
			{
				var nick   = m.Groups["source"].Value;
				var target = m.Groups["target"].Value;

				NickChangedEvent.Raise(this, new NickChangeEventArgs(nick, target));
			}
		}

		protected virtual void RfcNumericHandler(object sender, RawMessageEventArgs args)
		{
			var toks = args.Message.Split(' ');

			int num;
			if (Int32.TryParse(toks[1], out num))
			{
				var message = string.Join(" ", toks.Skip(2));
				RfcNumericEvent.Raise(this, new RfcNumericEventArgs(num, message));
			}
		}

		protected virtual void RegistrationHandler(object sender, RawMessageEventArgs args)
		{
			if (registered) return;
			if (!string.IsNullOrEmpty(Password)) Send("PASS :{0}", Password);

			// TODO: Check for NOTICE AUTH ...
			// Accident waiting to happen I think.

			Send("NICK {0}", Nick);
			Send("USER {0} 0 * :{1}", Ident, RealName);

			RawMessageEvent -= RegistrationHandler;
			registered = true;
		}

		protected virtual void PingHandler(object sender, RawMessageEventArgs args)
		{
			if (args.Message.StartsWithIgnoreCase("ping"))
			{
				// Bypass the queue for sending pong responses.
				Send(string.Format("PONG {0}", args.Message.Substring(5)));
				PingReceiptEvent.Raise(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Callbacks

		private void OnAsyncRead(Task<String> task)
		{
			if (task.Exception == null && task.Result != null && !task.IsCanceled)
			{
				lastMessage = DateTime.Now;
				RawMessageEvent.Raise(this, new RawMessageEventArgs(task.Result));
				client.ReadLineAsync().ContinueWith(OnAsyncRead, token);
			}
			else if (task.Result == null)
			{
				client.Close();
			}
		}
		
		protected async void QueueProcessor()
		{
			try
			{
				while (client != null && client.Connected)
				{
					if (messageQueue.Count > 0)
					{
						Send(messageQueue.Pop());
					}

					await Task.Delay(QueueInteval, queueToken);
				}
			}
			catch (TaskCanceledException)
			{
				// nom nom.
			}
		}

		private void CancellationNoticeHandler()
		{
			if (!tokenSource.IsCancellationRequested) return;

			Send("QUIT :Exiting.");
			client.Close();
		}

		private void OnTimeoutTimerElapsed(object sender, ElapsedEventArgs args)
		{
			if ((args.SignalTime - lastMessage) < Timeout)
			{
				TimeoutEvent.Raise(this, EventArgs.Empty);

				tokenSource.Cancel();
				queueTokenSource.Cancel();
			}
		}

		#endregion

		#endregion
	}
}
