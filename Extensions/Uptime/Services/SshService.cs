// -----------------------------------------------------------------------------
//  <copyright file="SshService.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using System.Collections.Generic;
	using Lantea.Common.IO;
	using Renci.SshNet;
	using Renci.SshNet.Common;

	public class SshService : Service
	{
		private AuthType authType;
		private String password;
		private String privateKeyPath;
		private String privateKeyPhrase;
		private String username;
		private int timeoutInt32;
		private TimeSpan timeout;

		private ConnectionInfo CreateConnectionInfo()
		{
			if (authType.HasFlag(AuthType.PublicKey))
			{
				return new ConnectionInfo(HostName,
					Port,
					username,
					new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(privateKeyPath, privateKeyPhrase)));				
			}

			if (authType.HasFlag(AuthType.Username))
			{
				return new ConnectionInfo(HostName,
					Port,
					username,
					new PasswordAuthenticationMethod(username, password));
			}

			return null;
		}

		#region Overrides of Service

		public override int Timeout
		{
			get { return timeoutInt32; }
			set
			{
				timeoutInt32 = value;
				timeout = new TimeSpan(0, 0, 0, value, 0);
			}
		}

		public override String Check()
		{
			String result;
			try
			{
				ConnectionInfo info = CreateConnectionInfo();
				if (info != null)
				{
					info.Timeout = timeout;
					info.RetryAttempts = RetryAttempts;

					using (SshClient client = new SshClient(info))
					{
						client.Connect();

						var cmd = client.CreateCommand("uptime");
						result  = cmd.Execute();

						client.Disconnect();
					}
				}
				else result = "Unable to connect: invalid connection info.";
			}
			catch (SshOperationTimeoutException e)
			{
				result = String.Format("Operation timed out: {0}", e.Message);
			}
			catch (SshConnectionException e)
			{
				result = String.Format("Unable to connect: {0}", e.Message);
			}

			return result.Trim();
		}

		public override void Initialize(IDictionary<String, String> data)
		{
			String auth;
			if (!data.TryGetValue("authtype", out auth))
			{
				throw new MissingRequiredPropertyException("No authtype provided for service.", "authtype");
			}

			switch (auth)
			{
				case "pubkey":
				{
					authType = AuthType.PublicKey;

					if (!data.TryGetValue("username", out username))
					{
						throw new MissingRequiredPropertyException("AuthType PublicKey requires a username be specified.", "username");
					}

					if (!data.TryGetValue("private_key_path", out privateKeyPath))
					{
						throw new MissingRequiredPropertyException("AuthType PublicKey requires a private key path be set.", "private_key_path");
					}

					privateKeyPath = PathHelper.ExpandPath(privateKeyPath);
					data.TryGetValue("key_passphrase", out privateKeyPhrase);
				}
					break;

				case "username":
				{
					authType = AuthType.Username;

					if (!data.ContainsKey("username") || !data.ContainsKey("password"))
					{
						throw new MissingRequiredPropertyException("AuthType Username requires both a username and password be specified.");
					}

					username = data["username"];
					password = data["password"];
				}
					break;

				default:
					authType = AuthType.None;
					break;
			}
		}

		#endregion

		#region Nested type: AuthType

		private enum AuthType
		{
			None      = 0,
			PublicKey = 1,
			Username  = 2,
		}

		#endregion
	}
}
