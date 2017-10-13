#define TCPCLIENT_DEBUG

using System;
#if TCPCLIENT_DEBUG
using UnityEngine;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Net.Sockets;
#elif NETFX_CORE
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
#endif
namespace Jake.Tcp
{
#if NETFX_CORE
	using HoloLens;
#endif
	/// <summary>
	/// Tcp client.
	/// </summary>
	public class TcpClient : TcpSocket
	{
		/// <summary>
		/// Connect to client through specified ip and port.
		/// Port can also be "loopback".
		/// </summary>
		public override void Connect(string ip, int port, Action onConnected)
		{
#if TCPCLIENT_DEBUG
			Debug.Log("Connecting to server on: " + ip + ":" + port.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
			client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var endPoint = new IPEndPoint(
				ip == "loopback" ? IPAddress.Loopback : IPAddress.Parse(ip), 
				port
			);

			connectState.callback = onConnected;
			client.BeginConnect(endPoint, OnConnected, null);
#elif NETFX_CORE
			client = HoloLensSocket.Connect(ip, port, () => { } + onConnected);
#endif
		}
#if UNITY_EDITOR || UNITY_STANDALONE
		void OnConnected(IAsyncResult result)
		{
			client.EndConnect(result);
#if TCPCLIENT_DEBUG
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
