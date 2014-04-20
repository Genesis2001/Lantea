// -----------------------------------------------------------------------------
//  <copyright file="HttpService.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Services
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Atlantis.Net.Irc;
	using Atlantis.Web;
	using Lantea.Common.IO;

	public class HttpService : Service
	{
		private Uri requestUri;

		private async Task<String> GetHttpResponse(HttpWebRequest request)
		{
			request.Method = "HEAD";
			request.AllowAutoRedirect = false;

			String statusString;
			Int32 statusCode;
			try
			{
				var response = await request.GetResponseAsync();

				var status = ((HttpWebResponse)response).StatusCode;

				statusCode = Convert.ToInt32(status);
				statusString = HttpHelper.GetStatusString(status);
			}
			catch (WebException e)
			{
				if (e.Response != null)
				{
					var status = ((HttpWebResponse)e.Response).StatusCode;

					statusCode = Convert.ToInt32(status);
					statusString = HttpHelper.GetStatusString(status);
				}
				else throw;
			}

			return String.Format("Request returned a result of {0} ({1})", statusString.Color(4), statusCode);
		}

		#region Overrides of Service

		public override String Check()
		{
			String result;
			try
			{
				TimeSpan span = new TimeSpan(0, 0, 0, Timeout, 0);

				HttpWebRequest request   = (HttpWebRequest)WebRequest.Create(requestUri);
				request.ContinueTimeout  = (int)(span.TotalMilliseconds / 5);
				request.Timeout          = (int)(span.TotalMilliseconds);
				request.ReadWriteTimeout = (int)(span.TotalMilliseconds * 2);

				var response = GetHttpResponse(request);

				result       = response.Result;
			}
			catch (WebException e)
			{
				result =
					String.Format("Error: {0} (Status: {1})", e.Message, (int)(((HttpWebResponse)e.Response).StatusCode)).Color(4);
			}

			return result;
		}

		public override void Initialize(IDictionary<String, String> data)
		{
			var builder = new UriBuilder();

			Boolean ssl = false;
			String sBool;
			if (data.TryGetValue("ssl", out sBool))
			{
				ssl = sBool.ToBoolean();
			}

			if (HostName.StartsWith("http"))
			{
				String[] parts = HostName.Split(new[] {@"//"}, StringSplitOptions.None);

				builder.Scheme = parts[0].TrimEnd(':');
				builder.Host = parts[1];
			}
			else
			{
				builder.Scheme = ssl ? "https" : "http";
				builder.Host = HostName;
			}
			
			builder.Port   = Port;

			if (data.ContainsKey("username") && data.ContainsKey("password"))
			{
				builder.UserName = data["username"];
				builder.Password = data["password"];
			}

			requestUri = builder.Uri;
		}

		#endregion
	}
}
