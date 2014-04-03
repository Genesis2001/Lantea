// -----------------------------------------------------------------------------
//  <copyright file="ISshService.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using Renci.SshNet;

	public interface ISshService : IService
	{
		SshAuthType AuthType { get; set; }

		String PrivateKey { get; set; }

		String Passphrase { set; }
	}

	public class SshService : ISshService
	{
		private ConnectionInfo CreateConnectionInfo()
		{
			return new ConnectionInfo(HostName,
				Port,
				UserName,
				new PasswordAuthenticationMethod(UserName, Password),
				new PrivateKeyAuthenticationMethod(UserName, new PrivateKeyFile(PrivateKey, Passphrase)));
		}

		#region Implementation of IService

		public string Display { get; set; }
		
		public string HostName { get; set; }

		public int Port { get; set; }

		public string UserName { get; set; }

		public string Password { set; private get; }

		public string Check()
		{
			ConnectionInfo info = CreateConnectionInfo();
			String result;
			using (SshClient client = new SshClient(info))
			{
				client.Connect();

				var cmd = client.CreateCommand("uptime");
				result  = cmd.Execute();

				client.Disconnect();
			}

			return result;
		}

		#endregion

		#region Implementation of ISshService

		public SshAuthType AuthType { get; set; }
		
		public string PrivateKey { get; set; }

		public string Passphrase { private get; set; }

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
			return Display;
		}

		#endregion
	}
}
