// -----------------------------------------------------------------------------
//  <copyright file="LogBaseClass.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.IO
{
	using System;
	using System.IO;
	using System.Text;

	public abstract class LogBaseClass : ILog
	{
		protected Stream stream;
		protected StringBuilder messageBuilder;

		#region Methods

		protected virtual void BuildLogMessage(LogThreshold threshold, String format, params object[] args)
		{
			messageBuilder = new StringBuilder();

			if (PrefixLog)
			{
				messageBuilder.Append(threshold.ToString().ToUpper());

				if (!String.IsNullOrEmpty(Prefix))
				{
					messageBuilder.Append(" ");
					messageBuilder.Append(Prefix);
				}
				else
				{
					messageBuilder.Append(" ");
					messageBuilder.Append(DateTime.Now.ToString("g"));
				}

				messageBuilder.Append(" ");
			}

			messageBuilder.AppendFormat(format, args);
			messageBuilder.Append('\n');
		}

		protected virtual void Write(LogThreshold threshold, String format, params object[] args)
		{
			if (Threshold.HasFlag(threshold))
			{
				BuildLogMessage(threshold, format, args);

				var buf = Encoding.Default.GetBytes(messageBuilder.ToString());
				stream.Write(buf, 0, buf.Length);
				stream.Flush();
			}
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (!disposing) return;

			if (stream != null) stream.Dispose();
		}

		#endregion
		
		#region Implementation of ILog

		public LogThreshold Threshold { get; set; }

		public bool PrefixLog { get; set; }

		public string Prefix { get; set; }
		
		public virtual void Debug(String message)
		{
			Write(LogThreshold.Debug, message);
		}

		public virtual void DebugFormat(String format, params Object[] args)
		{
			Write(LogThreshold.Debug, format, args);
		}

		public virtual void Error(String message)
		{
			Write(LogThreshold.Error, message);
		}

		public virtual void ErrorFormat(String format, params Object[] args)
		{
			Write(LogThreshold.Error, format, args);
		}

		public virtual void Fatal(String message)
		{
			Write(LogThreshold.Fatal, message);
		}

		public void FatalFormat(String format, params Object[] args)
		{
			Write(LogThreshold.Fatal, format, args);
		}

		public virtual void Info(String message)
		{
			Write(LogThreshold.Info, message);
		}

		public virtual void InfoFormat(String format, params Object[] args)
		{
			Write(LogThreshold.Info, format, args);
		}

		public virtual void Warn(String message)
		{
			Write(LogThreshold.Warning, message);
		}

		public virtual void WarnFormat(String format, params Object[] args)
		{
			Write(LogThreshold.Warning, format, args);
		}

		#endregion
	}
}
