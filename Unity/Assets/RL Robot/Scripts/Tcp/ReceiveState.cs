using System;

namespace Jake.Tcp
{
	/// <summary>
	/// Receive state of TcpSocket.
	/// </summary>
	public class ReceiveState
	{
		public byte[]	buffer;
		public int		bufferSize;
		public int		numBytesReceived;
		public Action	callback;

		public ReceiveState()
		{
			buffer = new byte[1024];
		}
	}
}
