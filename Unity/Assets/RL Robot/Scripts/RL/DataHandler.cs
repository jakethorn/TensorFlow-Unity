using Matt.Json;
using System;
using UnityEngine;

namespace Jake.RL
{
	/// <summary>
	/// Implement your own DataHandlers and pass them to Cycle in the inspector.
	/// </summary>
	public abstract class DataHandler : MonoBehaviour
	{
		public virtual byte[] Serialize()
		{
			throw new NotImplementedException();
		}

		public virtual int Serialize(byte[] bytes, int offset)
		{
			throw new NotImplementedException();
		}

		public virtual void Deserialize(byte[] bytes)
		{
			throw new NotImplementedException();
		}

		public virtual JsonObject Jsonize()
		{
			throw new NotImplementedException();
		}

		public virtual void Dejsonize(JsonObject json)
		{
			throw new NotImplementedException();
		}
	}
}
