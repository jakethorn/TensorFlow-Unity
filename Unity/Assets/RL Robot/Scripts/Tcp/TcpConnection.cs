using System;
using UnityEngine;

namespace Jake.Tcp
{
	using Json;

	/// <summary>
	/// MonoBehaviour access to a TcpSocket.
	/// </summary>
	public class TcpConnection : MonoBehaviour
	{
		public bool autoConnect;
		public TcpSocketType socketType;
		public string ip;
		public int port;
		public bool useJson;
		public JsonFile jsonFile;
		
		private TcpSocket socket;
		private bool connected;
		
		public bool Connected { get { return connected; } }

		void Start()
		{
			if (autoConnect)
			{
				Connect();
			}
		}
		
		void OnApplicationQuit()
		{
			if (connected)
			{
				socket.Close();
			}
		}

		/// <summary>
		/// Buffer sent by next Send call.
		/// </summary>
		public byte[] SendBuffer
		{
			get
			{
				return socket.SendBuffer;
			}
		}

		/// <summary>
		/// Buffer received by next Receive call.
		/// </summary>
		public byte[] ReceiveBuffer
		{
			get
			{
				return socket.ReceiveBuffer;
			}
		}

		/// <summary>
		/// Resive send buffer.
		/// </summary>
		public void ResizeSendBuffer(float size)
		{
			socket.ResizeSendBuffer(size);
		}

		/// <summary>
		/// Connect to client.
		/// </summary>
		public void Connect()
		{
			Connect(null);
		}

		/// <summary>
		/// Connect to client.
		/// </summary>
		public void Connect(Action onStarted)
		{
			if (useJson)
				LoadJson();
			
			switch (socketType)
			{
				case TcpSocketType.Client:	socket = new TcpClient();	break;
				case TcpSocketType.Server:	socket = new TcpServer();	break;
			}

			socket.Connect(ip, port, (() => { connected = true; }) + onStarted);
		}

		/// <summary>
		/// Send buffer to connected client (Non-blocking).
		/// </summary>
		public void Send(int size, Action onSent)
		{
			socket.Send(size, onSent);
		}

		/// <summary>
		/// Receive buffer from connected client (Non-blocking).
		/// </summary>
		public void Receive(Action onReceived)
		{
			socket.Receive(onReceived);
		}

		public void LoadJson()
		{
			var json = jsonFile.JsonObject;

			Debug.Assert(
				json.HasFields(new[] { "ip", "port" }), 
				jsonFile.Path + " does not contain fields: ip and port"
			);

			ip		= json["ip"].str;
			port	= Mathf.RoundToInt(json["port"].n);
		}
	}
}
