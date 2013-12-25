// -----------------------------------------------------------------------------
//  <copyright file="Log.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using System.IO;
	using System.Text;
	using Common.IO;

	public class Log : ILog, IDisposable
	{
		private readonly string fileName;
		private readonly FileStream fileStream;
		private readonly StreamWriter stream;

		public Log(string fileName)
		{
			this.fileName = fileName;

			fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
			stream = new StreamWriter(fileStream, Encoding.UTF8) {AutoFlush = true};
		}

		private void Write(LogType logType, string format, params object[] args)
		{
			var sb = new StringBuilder();
			sb.Append(logType.ToString().ToUpper());

			if (PrefixLog)
			{
				sb.Append(' ');
				sb.Append(string.IsNullOrEmpty(Prefix) ? DateTime.Now.ToString("yy-mm-dd HH:mm:ss") : Prefix);
				sb.Append(' ');
			}

			sb.AppendFormat(format, args);

			var line = string.Format(format, args);
			stream.WriteLineAsync(line);
		}

		#region Implementation of ILog

		public bool PrefixLog { get; set; }

		public string Prefix { get; set; }

		public void Debug(string line)
		{
			Write(LogType.Debug, line);
		}

		public void DebugFormat(string format, params object[] args)
		{
			Write(LogType.Debug, format, args);
		}

		public void Error(string line)
		{
			Write(LogType.Error, line);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Write(LogType.Error, format, args);
		}

		public void Fatal(string line)
		{
			Write(LogType.Error, line);
		}

		public void FatalFormat(string format, params object[] args)
		{
			Write(LogType.Error, format, args);
		}

		public void Info(string line)
		{
			Write(LogType.Info, line);
		}

		public void InfoFormat(string format, params object[] args)
		{
			Write(LogType.Info, format, args);
		}

		public void Warn(string line)
		{
			Write(LogType.Warning, line);
		}

		public void WarnFormat(string format, params object[] args)
		{
			Write(LogType.Warning, format, args);
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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			stream.Dispose();
		}

		#endregion
	}
}
