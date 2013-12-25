// -----------------------------------------------------------------------------
//  <copyright file="ILog.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	public enum LogType
	{
		None,
		Info,
		Warning,
		Error,
		Debug,
	}

	public interface ILog
	{
		bool PrefixLog { get; set; }

		string Prefix { get; set; }

		void Debug(string line);

		void DebugFormat(string format, params object[] args);

		void Error(string line);

		void ErrorFormat(string format, params object[] args);

		void Fatal(string line);

		void FatalFormat(string format, params object[] args);

		void Info(string line);

		void InfoFormat(string format, params object[] args);

		void Warn(string line);

		void WarnFormat(string format, params object[] args);
	}
}
