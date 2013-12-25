// -----------------------------------------------------------------------------
//  <copyright file="TcpClientAsyncAdapter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using Common.Net;

	public class TcpClientAsyncAdapter : ITcpClient
	{
		private readonly TcpClient client;
		private Encoding encoding;
		private StreamReader reader;
		private NetworkStream stream;

		public TcpClientAsyncAdapter(TcpClient client, Encoding encoding)
		{
			this.client   = client;
			this.encoding = encoding;

			InitializeAdapter();
		}

		private void InitializeAdapter()
		{
			if (client == null) return;

			stream   = client.GetStream();
			encoding = encoding ?? new UTF8Encoding(false);
			reader   = new StreamReader(client.GetStream(), encoding);
		}

		#region Implementation of ITcpClient

		public bool Connected
		{
			get { return client != null && client.Connected; }
		}

		public bool DataAvailable
		{
			get { return stream != null && stream.DataAvailable; }
		}

		public bool EndOfStream
		{
			get { return reader.EndOfStream; }
		}

		public void Connect(string host, int port)
		{
			var entry = Dns.GetHostEntry(host);
			if (entry == null)
			{
				throw new ArgumentNullException("host", "Unable to resolve host. Check network configuration.");
			}

			var connection = new IPEndPoint(entry.AddressList[0], port);
			client.Connect(connection);
		}

		public string ReadLine()
		{
			var result = reader.ReadLineAsync();

			return result.Result;
		}

		public string ReadAll()
		{
			var result = reader.ReadToEndAsync();

			return result.Result;
		}

		public void Write(string format, params object[] args)
		{
			var s = string.Format(format, args);
			var buf = encoding.GetBytes(s);

			stream.WriteAsync(buf, 0, buf.Length);
		}

		public void WriteLine(string format, params object[] args)
		{
			var s = new StringBuilder();
			s.AppendFormat(format, args);
			s.AppendLine();

			var buf = encoding.GetBytes(s.ToString());

			stream.WriteAsync(buf, 0, buf.Length);
		}

		#endregion
	}
}
