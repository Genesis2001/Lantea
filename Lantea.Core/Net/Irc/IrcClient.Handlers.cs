// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Timers;
	using Common.Linq;

	public partial class IrcClient
	{
		#region Fields

		internal string accessPrefixes;
		internal string channelModes;

		private bool registered;
		private DateTime lastMessage;
		private Timer timeoutTimer;

		#endregion

		// internal const string IrcRawRegex = @"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$";

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

		protected virtual void ProtocolHandler(object sender, RfcNumericEventArgs args)
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
			}
		}

		protected virtual void ChannelAccessHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			var message = args.Message;

			if (header == IrcHeaders.RPL_NAMREPLY)
			{
				var toks   = message.Split(' ');
				var c      = GetChannel(toks[2]);
				var names  = message.Substring(message.IndexOf(':') + 1);

				MatchCollection collection;
				var accessRegex = string.Format(@"(?<prefix>[{0}]?)(?<user>\S+)", accessPrefixes);
				if (names.TryMatches(accessRegex, out collection))
				{
					foreach (Match item in collection)
					{
						var prefix = item.Groups["prefix"].Value.Length > 0 ? item.Groups["prefix"].Value[0] : (char)0;
						var user   = item.Groups["user"].Value;

						if (c.Users.ContainsKey(user))
						{
							c.Users[user].AddPrefix(prefix);
						}
						else
						{
							var p = new PrefixList(this);
							p.AddPrefix(prefix);
							c.Users.Add(user, p);
						}
					}
				}
			}
		}

		protected virtual void ListModeHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			var message = args.Message;

			if (header == IrcHeaders.RPL_BANLIST || header == IrcHeaders.RPL_EXCEPTLIST || header == IrcHeaders.RPL_INVITELIST)
			{
				// 
			}
		}

		protected virtual void NickInUseHandler(object sender, RfcNumericEventArgs args)
		{
			var header  = (IrcHeaders)args.Numeric;
			var message = args.Message;

			if (header == IrcHeaders.ERR_NICKNAMEINUSE)
			{
				var toks    = message.Split(' ');
				var newNick = string.Concat(toks[1], "_");

				ChangeNick(newNick);

				if (RetryNick)
				{
					Task.Factory.StartNew(async () =>
					                            {
						                            await Task.Delay(Convert.ToInt32(RetryInterval), token);

						                            ChangeNick(Nick);
					                            }, token);
				}
			}
		}

		#endregion

		#region IRC Raw Message Handlers

		protected virtual void JoinPartHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			// reg. expression credit
			// http://cjh.im/ - Chris J. Hogben

			// :Lantea!lantea@unified-nac.jhi.145.98.IP JOIN :#UnifiedTech
			if (message.TryMatch(@"^:?(?<nick>[^!]+)\!((?<ident>[^@]+)@(?<host>\S+)) (?<command>PRIVMSG|NOTICE|JOIN|PART|QUIT|MODE|NICK) :?(?<target>\#?[^\W]+)\W?:?(?<params>.+)?$", out m))
			{
				if (m.Groups["command"].Value.Matches(@"join|part"))
				{
					var nick   = m.Groups[1].Value;
					var target = m.Groups[5].Value;

					if (m.Groups["command"].Value.EqualsIgnoreCase("join"))
					{
						ChannelJoinEvent.Raise(this, new JoinPartEventArgs(nick, target));
					}
					else if (m.Groups["command"].Value.EqualsIgnoreCase("part"))
					{
						ChannelPartEvent.Raise(this, new JoinPartEventArgs(nick, target));
					}

					if (StrictNames)
					{
						Send("NAMES {0}", target);
					}
				}
			}
		}

		protected virtual void MessageNoticeHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			if (message.TryMatch(@":?([^!]+)\!(([^@]+)@(\S+)) (PRIVMSG|NOTICE) :?(\#?[^\W]+)\W?:?(.+)?", out m))
			{
				var nick   = m.Groups[1].Value;
				var target = m.Groups[6].Value;
				var msg    = m.Groups[7].Value;

				if (m.Groups[5].Value.EqualsIgnoreCase("privmsg"))
				{
					MessageReceivedEvent.Raise(this, new MessageReceivedEventArgs(nick, target, msg));
				}
				else if (m.Groups[5].Value.EqualsIgnoreCase("notice"))
				{
					NoticeReceivedEvent.Raise(this, new MessageReceivedEventArgs(nick, target, msg));
				}
			}
		}

		private enum ChannelModeType
		{
			
		}

		protected virtual void ModeHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			if (message.TryMatch(@":?([^!]+)\!(([^@]+)@(\S+)) MODE :?(\#?[^\W]+)\W?:?(.+)?", out m))
			{

			}
		}

		protected virtual void NickHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			// :Genesis2001!zack@unifiedtech.org NICK Genesis2002
			if (message.TryMatch(@":?([^!]+)\!(([^@]+)@(\S+)) NICK :?(\#?[^\W]+)\W?:?(.+)?", out m))
			{
				var nick = m.Groups[1].Value;
				var target = m.Groups[5].Value;

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
