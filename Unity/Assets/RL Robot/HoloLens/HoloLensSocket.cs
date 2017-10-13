#define HOLOLENSSOCKET_DEBUG
#if NETFX_CORE
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Jake.Tcp.HoloLens
{
	public static class HoloLensSocket
	{
		public static StreamSocket Connect(string ip, int port, Action onConnected)
		{
			var client = new StreamSocket();
			var hostName = new HostName(ip);
			client.ConnectAsync(hostName, port.ToString()).Completed = (IAsyncAction asyncInfo, AsyncStatus status) =>
			{
#if HOLOLENSSOCKET_DEBUG
				Debug.Log("Connected.");
#endif
				onConnected();
			};

			return client;
		}

		public static StreamSocketListener Connect(int port, Action<StreamSocket> onConnected)
		{
			var server = new StreamSocketListener();
			server.ConnectionReceived += (StreamSocketListener _, StreamSocketListenerConnectionReceivedEventArgs args) =>
			{
#if HOLOLENSSOCKET_DEBUG
				Debug.Log("Connected.");
#endif
				onConnected(args.Socket);
			};

			var __ = server.BindServiceNameAsync(port.ToString());

			return server;
		}

		public static void Send(StreamSocket client, SendState sendState, Action onSent)
		{
			var sending = client.OutputStream.WriteAsync(sendState.buffer.AsBuffer(0, sendState.bufferSize));
			sending.Completed = async (IAsyncOperationWithProgress<uint, uint> asyncOp, AsyncStatus asyncStatus) =>
			{
				await client.OutputStream.FlushAsync();

				if (onSent != null)
				{
#if HOLOLENSSOCKET_DEBUG
					Debug.Log("Sent " + asyncOp.GetResults().ToString() + " bytes.");
#endif
					onSent();
				}
			};
		}

		public static void Receive(TcpSocket socket, StreamSocket client, ReceiveState receiveState, Action<byte[], int> onReceived)
		{
			client.InputStream.ReadAsync(receiveState.buffer.AsBuffer(0, 4), 4u, InputStreamOptions.Partial).Completed =
			(IAsyncOperationWithProgress<IBuffer, uint> numBytesOp, AsyncStatus numBytesStatus) =>
			{
				var numBytes = BitConverter.ToInt32(numBytesOp.GetResults().ToArray(), 0);
#if HOLOLENSSOCKET_DEBUG
				Debug.Log("Expecting " + numBytes.ToString() + " bytes.");
#endif
				while (receiveState.buffer.Length < numBytes)
				{
					socket.ResizeReceiveBuffer(2);
				}

				client.InputStream.ReadAsync(receiveState.buffer.AsBuffer(0, numBytes), (uint)numBytes, InputStreamOptions.Partial).Completed =
				(IAsyncOperationWithProgress<IBuffer, uint> bytesOp, AsyncStatus bytesStatus) =>
				{
					if (onReceived != null)
					{
#if HOLOLENSSOCKET_DEBUG
						Debug.Log("Received " + bytesOp.GetResults().Length + " bytes.");
#endif
						onReceived(receiveState.buffer, receiveState.buffer.Length);
					}
				};
			};
		}
	}
}
#endif
