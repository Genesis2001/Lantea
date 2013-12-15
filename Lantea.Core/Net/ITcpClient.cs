// -----------------------------------------------------------------------------
//  <copyright file="ITcpClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net
{
	public interface ITcpClient
	{
		string ReadLine();

		string ReadAll();

		void Write(string format, params object[] args);

		void WriteLine(string format, params object[] args);
	}
}
