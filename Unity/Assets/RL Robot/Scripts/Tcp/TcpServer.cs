#define TCPSERVER_DEBUG

using System;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Net.Sockets;
#elif NETFX_CORE
using Windows.Networking.Sockets;
#endif
#if TCPSERVER_DEBUG
using UnityEngine;
#endif
namespace Jake.Tcp
{
#if NETFX_CORE
	using HoloLens;
#endif
	/// <summary>
	/// Tcp server.
	/// </summary>
	public class TcpServer : TcpSocket
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		private Socket server;
#endif

		/// <summary>
		/// Connect to client through specified ip and port.
		/// Port can also be "any" and "loopback".
		/// </summary>
		public override void Connect(string ip, int port, Action onConnected)
		{
#if TCPSERVER_DEBUG
			Debug.Log("Waiting for client on: " + ip + ":" + port.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
			var ipAddress = default(IPAddress);
			if		(ip == "any")		ipAddress = IPAddress.Any;
			else if (ip == "loopback")	ipAddress = IPAddress.Loopback;
			else						ipAddress = IPAddress.Parse(ip);

			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Bind(new IPEndPoint(ipAddress, port));
			server.Listen(1);

			connectState.callback = onConnected;
			server.BeginAccept(OnConnected, null);
#elif NETFX_CORE
			HoloLensSocket.Connect(port, (StreamSocket client) =>
			{
				this.client = client;
				if (onConnected != null)
					onConnected();
			});
#endif
		}
#if UNITY_EDITOR || UNITY_STANDALONE
		void OnConnected(IAsyncResult result)
		{
			client = server.EndAccept(result);
#if TCPSERVER_DEBUG
			Debug.Log("Connected.");
#endif
			if (connectState.callback != null)
			{
				connectState.callback();
			}
		}
#endif
	}
}
