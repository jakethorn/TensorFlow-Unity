#define TCPSOCKET_DEBUG

using System;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Net.Sockets;
#elif NETFX_CORE
using Windows.Networking.Sockets;
#endif
#if TCPSOCKET_DEBUG
using UnityEngine;
#endif
namespace Jake.Tcp
{
#if NETFX_CORE
	using HoloLens;
#endif
	using Threading;

	/// <summary>
	/// Abstract socket class. 
	/// Extended by TcpServer and TcpClient.
	/// </summary>
	public abstract class TcpSocket
	{
		protected ConnectState connectState = new ConnectState();

		/*
		 *	Abstract interface
		 */

		public abstract void Connect(string ip, int port, Action onConnected);

		/*
		 *	Concrete interface
		 */
#if UNITY_EDITOR || UNITY_STANDALONE
		protected Socket client;
#elif NETFX_CORE
		protected StreamSocket client;
#endif
		private SendState		sendState		= new SendState		();
		private ReceiveState	receiveState	= new ReceiveState	();
		
		/// <summary>
		/// Buffer sent by next Send call.
		/// </summary>
		public byte[] SendBuffer
		{
			get
			{
				return sendState.buffer;
			}
		}

		/// <summary>
		/// Buffer received by next Receive call.
		/// </summary>
		public byte[] ReceiveBuffer
		{
			get
			{
				return receiveState.buffer;
			}
		}

		/// <summary>
		/// Resive send buffer.
		/// </summary>
		public void ResizeSendBuffer(float size)
		{
			sendState.buffer = new byte[(int)(sendState.buffer.Length * size)];
#if TCPSOCKET_DEBUG
			Debug.Log("Send buffer resized to " + sendState.buffer.Length.ToString() + " bytes.");
#endif
		}

		/// <summary>
		/// Resize receive buffer.
		/// </summary>
		public void ResizeReceiveBuffer(float size)
		{
			receiveState.buffer = new byte[(int)(receiveState.buffer.Length * size)];
#if TCPSOCKET_DEBUG
			Debug.Log("Receive buffer resized to " + receiveState.buffer.Length.ToString() + " bytes.");
#endif
		}

		/// <summary>
		/// Send buffer to connected client (Non-blocking).
		/// </summary>
		public void Send(int size, Action onSent)
		{
			// prepend size
			var head = BitConverter.GetBytes(size);
			Buffer.BlockCopy(head, 0, sendState.buffer, 0, 4);

			// resete send state
			sendState.bufferSize	= 4 + size;
			sendState.numBytesSent	= 0;
			sendState.callback		= onSent;
#if TCPSOCKET_DEBUG
			Debug.Log("Sending " + sendState.bufferSize.ToString() + " bytes...");
#endif
			// send
#if UNITY_EDITOR || UNITY_STANDALONE
			client.BeginSend(sendState.buffer, 0, sendState.bufferSize, SocketFlags.None, OnSent, null);
#elif NETFX_CORE
			HoloLensSocket.Send(client, sendState, onSent);
#endif
		}

		/// <summary>
		/// Receive buffer from connected client (Non-blocking).
		/// </summary>
		public void Receive(Action onReceived)
		{
			receiveState.bufferSize			= 0;
			receiveState.numBytesReceived	= 0;
			receiveState.callback			= onReceived;
#if TCPSOCKET_DEBUG
			Debug.Log("Receiving...");
#endif
			// receive
#if UNITY_EDITOR || UNITY_STANDALONE
			client.BeginReceive(receiveState.buffer, 0, 4, SocketFlags.None, OnReceived, null);
#elif NETFX_CORE
			HoloLensSocket.Receive(this, client, receiveState, onReceived);
#endif
		}

		/// <summary>
		/// Close connection.
		/// </summary>
		public void Close()
		{
#if TCPSOCKET_DEBUG
			Debug.Log("Closing...");
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
			client.Close();
#endif
#if TCPSOCKET_DEBUG
			Debug.Log("Closed.");
#endif
		}
#if UNITY_EDITOR || UNITY_STANDALONE
		private void OnSent(IAsyncResult result)
		{
			var fragmentSize = client.EndSend(result);

			// calculate number of bytes sent
			sendState.numBytesSent += fragmentSize;
#if TCPSOCKET_DEBUG
			Debug.Log(sendState.numBytesSent.ToString() + " out of " + sendState.bufferSize.ToString() + " bytes sent.");
#endif
			// if more bytes need to be sent
			if (sendState.numBytesSent < sendState.bufferSize)
			{
				// continue sending
				client.BeginSend(sendState.buffer, sendState.numBytesSent, sendState.bufferSize - sendState.numBytesSent, SocketFlags.None, OnSent, null);
			}
			else
			{
				// stop sending
				// invoke callback on main unity thread
				Dispatcher.AddJob(sendState.callback, false);
			}
		}

		private void OnReceived(IAsyncResult result)
		{
			var fragmentSize = client.EndReceive(result);

			// first fragment
			if (receiveState.bufferSize == 0)
			{
				// resize buffer
				receiveState.bufferSize = BitConverter.ToInt32(receiveState.buffer, 0);
				while (receiveState.buffer.Length < receiveState.bufferSize)
				{
					ResizeReceiveBuffer(2);
				}
#if TCPSOCKET_DEBUG
				Debug.Log("Receiving " + receiveState.bufferSize.ToString() + " bytes...");
#endif
			}
			else
			{
				// acknowledge bytes received
				receiveState.numBytesReceived += fragmentSize;
#if TCPSOCKET_DEBUG
				Debug.Log(receiveState.numBytesReceived.ToString() + " out of " + receiveState.bufferSize.ToString() + " bytes received.");
#endif
			}

			// if more bytes need to be received
			if (receiveState.numBytesReceived < receiveState.bufferSize)
			{
				// continue receiving
				client.BeginReceive(receiveState.buffer, receiveState.numBytesReceived, receiveState.bufferSize - receiveState.numBytesReceived, SocketFlags.None, OnReceived, null);
			}
			else
			{
				// stop receiving
				// invoke callback on main unity thread
				Dispatcher.AddJob(receiveState.callback, false);
			}
		}
#endif
	}
}
