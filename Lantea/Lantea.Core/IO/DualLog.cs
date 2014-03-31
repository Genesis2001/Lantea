// -----------------------------------------------------------------------------
//  <copyright file="DualLog.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using Atlantis.IO;

	public class DualLog : ILog
	{
		private readonly ILog log1;
		private readonly ILog log2;

		public DualLog(ILog log1, ILog log2)
		{
			this.log1 = log1;
			this.log2 = log2;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			log1.Dispose();
			log2.Dispose();
		}

		#endregion

		#region Implementation of ILog

		public LogThreshold Threshold
		{
			get { throw new NotImplementedException(); }
			set
			{
				log1.Threshold = value;
				log2.Threshold = value;
			}
		}

		public bool PrefixLog
		{
			get { return log1.PrefixLog && log2.PrefixLog; }
			set
			{
				log1.PrefixLog = value;
				log2.PrefixLog = value;
			}
		}

		public string Prefix
		{
			get { throw new NotImplementedException(); }
			set
			{
				log1.Prefix = value;
				log2.Prefix = value;
			}
		}

		public void Debug(string message)
		{
			log1.Debug(message);
			log2.Debug(message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			log1.DebugFormat(format, args);
			log2.DebugFormat(format, args);
		}

		public void Error(string message)
		{
			log1.Error(message);
			log2.Error(message);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			log1.ErrorFormat(format, args);
			log2.ErrorFormat(format, args);
		}

		public void Fatal(string message)
		{
			log1.Fatal(message);
			log2.Fatal(message);
		}

		public void FatalFormat(string format, params object[] args)
		{
			log1.FatalFormat(format, args);
			log2.FatalFormat(format, args);
		}

		public void Info(string message)
		{
			log1.Info(message);
			log2.Info(message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			log1.InfoFormat(format, args);
			log2.InfoFormat(format, args);
		}

		public void Warn(string message)
		{
			log1.Warn(message);
			log2.Warn(message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			log1.WarnFormat(format, args);
			log2.WarnFormat(format, args);
		}

		#endregion
	}
}
