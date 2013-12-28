// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Timers;
	using Common.Linq;

	public partial class IrcClient
	{
		private bool registered;
		private DateTime lastMessage;
		private Timer timeoutTimer;

		internal const string IrcRawRegex = @"^(:(?<prefix>\S+) )?(?<command>\S+)( (?!:)(?<params>.+?))?( :(?<trail>.+))?$";

		private void StartTimeoutTimer()
		{
			timeoutTimer          = new Timer(Timeout.TotalMilliseconds);
			timeoutTimer.Elapsed += TimeoutTimerElapsed;
			timeoutTimer.Start();
		}

		#region Handlers

		private void CancellationNoticeHandler()
		{
			if (!tokenSource.IsCancellationRequested) return;

			Send("QUIT :Exiting.");
			client.Close();
		}

		private void TimeoutTimerElapsed(object sender, ElapsedEventArgs args)
		{
			if ((args.SignalTime - lastMessage) < Timeout)
			{
				TimeoutEvent.Raise(this, EventArgs.Empty);

				tokenSource.Cancel();
			}
		}

		protected virtual void JoinPartHandler(object sender, RawMessageEventArgs args)
		{
			var message = args.Message;
			Match m;

			// reg. expression credit
			// http://cjh.im/ - Chris J. Hogben

			// :Lantea!lantea@unified-nac.jhi.145.98.IP JOIN :#UnifiedTech
			if (message.TryMatch(@":?([^!]+)\!([^@]+)@(\S+)\W(JOIN|PART)\W:?(\#?[^\W]+)\W?:?(.+)?", out m))
			{
				var user    = m.Groups[1].Value;
				var channel = m.Groups[5].Value;

				if (m.Groups[4].Value.EqualsIgnoreCase("join"))
				{
					ChannelJoinEvent.Raise(this, new JoinPartEventArgs(user, channel));
				}
				else if (m.Groups[4].Value.EqualsIgnoreCase("part"))
				{
					ChannelPartEvent.Raise(this, new JoinPartEventArgs(user, channel));
				}
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

			Send("NICK {0}", My.Nick);
			Send("USER {0} 0 * :{1}", My.Ident, My.RealName);

			RawMessageEvent -= RegistrationHandler;
			registered       = true;
		}

		protected virtual void PingHandler(object sender, RawMessageEventArgs args)
		{
			if (args.Message.StartsWithIgnoreCase("ping"))
			{
				// Bypass the queue for sending pong responses.
				Send(string.Format("PONG {0}", args.Message.Substring(5)));
			}
		}

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
		
		/*protected void ThreadWorkerCallback()
		{
			SetDefaults();
			
			// queueRunner = Task.Run(new Action(QueueHandler), tokenSource.Token);
			
			while (client != null && client.Connected)
			{
				if (!client.DataAvailable) continue;
				
				while (!client.EndOfStream)
				{
					var line = client.ReadLine().Trim();
					
					if (!string.IsNullOrEmpty(line))
					{
						OnDataReceived(line);
					}
				}
			}
		}*/

		protected async void QueueHandler()
		{
			try
			{
				while (client != null && client.Connected)
				{
					if (messageQueue.Count > 0)
					{
						Send(messageQueue.Pop());
					}

					await Task.Delay(QueueInteval, token);
				}
			}
			catch (TaskCanceledException)
			{
				// nom nom.
			}
		}

		#endregion
	}
}
