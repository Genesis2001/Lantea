// -----------------------------------------------------------------------------
//  <copyright file="MySqlService.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading.Tasks;
	using Atlantis.Linq;
	using Lantea.Common.IO;
	using MySql.Data.MySqlClient;

	public class MySqlService : Service
	{
		private MySqlConnectionStringBuilder connectionStringBuilder;

		private async Task<String> GetServiceUptime()
		{
			MySqlConnection connection = null;
			String result;
			try
			{
				connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
				await connection.OpenAsync();

				using (MySqlCommand cmd = connection.CreateCommand())
				{
					cmd.CommandText = "SHOW GLOBAL STATUS LIKE 'Uptime'";
					var reader = await cmd.ExecuteReaderAsync();

					if (reader.HasRows && reader.Read())
					{
						var uptime = reader.GetInt32("value");
						var span = new TimeSpan(0, 0, 0, uptime, 0);

						result = span.Prettify();
					}
					else
					{
						result =
							"(this message should never, ever be viewed. if you are viewing it, something has gone horribly, horribly wrong)";
					}
				}
			}
			catch (MySqlException e)
			{
				result = String.Format("Unable to retrieve uptime. (MySQL Error: {0})", e.Number);
				//result = e.Message;
			}
			finally
			{
				if (connection != null && connection.State == ConnectionState.Open)
				{
					connection.Close();
					connection.Dispose();
				}
			}

			return result;
		}

		#region Overrides of Service

		public override String Check()
		{
			return GetServiceUptime().Result;
		}

		public override void Initialize(IDictionary<String, String> data)
		{
			if (!data.ContainsKey("username") || !data.ContainsKey("password"))
			{
				throw new MissingRequiredPropertyException("A MySql service requires a username *and* a password.");
			}

			connectionStringBuilder = new MySqlConnectionStringBuilder
			                          {
				                          Server            = HostName,
				                          Port              = Convert.ToUInt32(Port),
				                          ConnectionTimeout = Convert.ToUInt32(Timeout),
										  UserID            = data["username"],
										  Password          = data["password"],
			                          };
		}

		#endregion
	}
}
