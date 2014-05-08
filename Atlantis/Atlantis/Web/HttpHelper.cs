// -----------------------------------------------------------------------------
//  <copyright file="HttpHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Web
{
	using System;
	using System.Linq;
	using System.Net;
	using System.Text.RegularExpressions;

	public static class HttpHelper
	{
		public static String GetStatusString(HttpStatusCode statusCode)
		{
			switch ((int)statusCode)
			{
				case 101: return "Switching protocols";
				case 203: return "Non-authoritative information";
				case 204: return "No content";
				case 205: return "Reset content";
				case 206: return "Partial content";
				case 300: return "Multiple choices";
				case 301: return "Moved permanently";
				case 303: return "Redirect method";
				case 304: return "Not modified";
				case 305: return "Use proxy";
				case 307: return "Temporary Redirect";
				case 400: return "Bad Request";
				case 402: return "Payment Required";
				case 404: return "Not Found";
				case 405: return "Method not allowed";
				case 406: return "Not acceptable";
				case 407: return "Proxy authentication required";
				case 408: return "Request Timeout";
				case 411: return "Length required";
				case 412: return "Precondition failed";
				case 413: return "Request entity too large";
				case 414: return "Request URI too long";
				case 415: return "Unsupported media type";
				case 416: return "Requested range not satisfiable";
				case 417: return "Expectation failed";
				case 500: return "Internal Server Error";
				case 501: return "Not Implemented";
				case 502: return "Bad Gateway";
				case 503: return "Service Unavailable";
				case 504: return "Gateway Timeout";
				case 505: return "HTTP version not supported";
				default: return statusCode.ToString();
			}
		}

		public static String GetHtmlTitle(String uri)
		{
			const String regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)";

			HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
			if (request == null) return null;

			HttpWebResponse response;
			try
			{
				response = request.GetResponse() as HttpWebResponse;
			}
			catch (WebException)
			{
				return null;
			}

			if (response != null)
			{
				using (response)
				{
					if (response.Headers.AllKeys.Any(x => x.Equals("content-type", StringComparison.OrdinalIgnoreCase)))
					{
						if (response.Headers["Content-Type"].StartsWith("text/html"))
						{
							WebClient client = new WebClient();
							String data = client.DownloadString(uri);

							Match m;
							if ((m = Regex.Match(data, regex)).Success)
							{
								return m.Value;
							}
						}
					}
				}
			}

			return null;
		}
	}
}
