// -----------------------------------------------------------------------------
//  <copyright file="TcpClientAdapter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net
{
	using System.IO;
	using System.Net.Sockets;
	using System.Text;

	public class TcpClientAdapter : ITcpClient
	{
		private readonly TcpClient client;

		private StreamReader clientReader;
		private StreamWriter clientWriter;

		public TcpClientAdapter(TcpClient client)
		{
			this.client = client;
			InitializeStreams();
		}

		public TcpClientAdapter(TcpClient client, Encoding encoding) : this(client)
		{
			InitializeStreams(encoding);
		}

		private void InitializeStreams(Encoding encoding = null)
		{
			if (client == null) return;

			var stream = client.GetStream();

			clientReader = new StreamReader(stream, encoding ?? Encoding.Default);
			clientWriter = new StreamWriter(stream, encoding ?? Encoding.Default);
		}

		#region Implementation of ITcpClient

		public string ReadLine()
		{
			return clientReader.ReadLine();
		}

		public string ReadAll()
		{
			return clientReader.ReadToEnd();
		}

		public void Write(string format, params object[] args)
		{
			clientWriter.Write(format, args);
		}

		public void WriteLine(string format, params object[] args)
		{
			clientWriter.WriteLine(format, args);
		}

		#endregion
	}
}
