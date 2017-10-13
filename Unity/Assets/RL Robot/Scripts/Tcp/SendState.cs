using System;

namespace Jake.Tcp
{
	/// <summary>
	/// Send state of TcpSocket.
	/// </summary>
	public class SendState
	{
		public byte[]	buffer;
		public int		bufferSize;
		public int		numBytesSent;
		public Action	callback;

		public SendState()
		{
			buffer = new byte[1024];
		}
	}
}
