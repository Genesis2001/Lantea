// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Timers;
	using Common.Linq;

	public partial class IrcClient
	{
		private bool registered;
		private DateTime lastMessage;
		private Timer timeoutTimer;

		#region Handlers

		private void CancellationNoticeHandler()
		{
			if (!tokenSource.IsCancellationRequested) return;

			Send("QUIT :Exiting.");
			client.Close();
		}

		private void OnDataReceived(string input)
		{
			RawMessageEvent.Raise(this, new RawMessageEventArgs(input));

			lastMessage = DateTime.Now;
		}

		private void TimeoutTimerElapsed(object sender, ElapsedEventArgs args)
		{
			if ((args.SignalTime - lastMessage) < TimeOut)
			{
				TimeoutEvent.Raise(this, EventArgs.Empty);

				tokenSource.Cancel();
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

			Send("NICK {0}", User.Nick);
			Send("USER {0} 0 * :{1}", User.Ident, User.RealName);

			RawMessageEvent -= RegistrationHandler;
			registered       = true;
		}

		protected virtual void PingHandler(object sender, RawMessageEventArgs args)
		{
			if (args.Message.StartsWithIgnoreCase("ping"))
			{
				Send(string.Format("PONG {0}", args.Message.Substring(5)));
			}
		}

		private void OnAsyncRead(Task<String> task)
		{
			if (task.Exception == null && task.Result != null && !task.IsCanceled)
			{
				OnDataReceived(task.Result);

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
			while (client != null && client.Connected)
			{
				if (messageQueue.Count > 0)
				{
					Send(messageQueue.Pop());
				}

				await Task.Delay(QueueInteval, token);
			}
		}

		#endregion
	}
}
