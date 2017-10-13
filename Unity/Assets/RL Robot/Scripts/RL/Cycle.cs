//#define CYCLE_DEBUG
using System;
using UnityEngine;

namespace Jake.RL
{
	using Tcp;
	using Threading;

	/// <summary>
	/// Sends data from a DataHandler as fast as possible between C# and Python using a TcpConnection.
	/// </summary>
	public class Cycle : MonoBehaviour
	{
		public TcpConnection	tcpConnection;
		public DataHandler		dataHandler;
		
		private int waitCount;

		/// <summary>
		/// Pause RL cycle.
		/// </summary>
		public bool Wait
		{
			get
			{
				return waitCount > 0;
			}

			set
			{
				if (value)
				{
					waitCount++;
				}
				else
				{
					waitCount--;
				}
				
				if (waitCount == 0)
				{
					Next();
				}
			}
		}

#if CYCLE_DEBUG
		private int cycleCount;
		private float startTime;
#endif
		void Awake()
		{
			if (FindObjectOfType<Dispatcher>() == null)
			{
				Dispatcher.Instantiate();
			}
		}
#if CYCLE_DEBUG
		void OnGUI()
		{
			if (cycleCount > 0)
			{
				var runtime = Time.time - startTime;
				GUILayout.Label("Cycles: " + cycleCount + "c");
				GUILayout.Label("Time: " + runtime + "s");
				GUILayout.Label("Cycles per second: " + (cycleCount / runtime) + "s");
				GUILayout.Label("Seconds per cycle: " + (runtime / cycleCount) + "c");
			}
		}
#endif
		/// <summary>
		/// Begin RL cycle.
		/// </summary>
		public void Begin()
		{
#if CYCLE_DEBUG
			cycleCount = 0;
			startTime = Time.time;
#endif
			Next();
		}
		
		private void Next()
		{
			if (Wait)
				return;
#if CYCLE_DEBUG
			cycleCount++;
#endif
			var numBytes = Serialize();
			SendAndReceive(numBytes);
		}
		
		private int Serialize()
		{
			// try to serialize into send buffer
			// will throw exception if send buffer is not big enough
			// in which case expand send buffer
			// and try again

			var numBytes = 0;
			var serialized = false;
			while (!serialized)
			{
				try
				{
					numBytes = dataHandler.Serialize(tcpConnection.SendBuffer, 4);
					serialized = true;
				}
				catch (InsufficientMemoryException)
				{
					tcpConnection.ResizeSendBuffer(2);
				}
			}

			return numBytes;
		}

		private void SendAndReceive(int numBytes)
		{
			// send
			tcpConnection.Send(numBytes, () =>
			{
				// on sent, receive
				tcpConnection.Receive(() =>
				{
					// on received, deserialize
					dataHandler.Deserialize(tcpConnection.ReceiveBuffer);

					// recurse
					Next();
				});
			});
		}
	}
}
