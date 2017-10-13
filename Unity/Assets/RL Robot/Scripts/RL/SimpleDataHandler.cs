using Matt.Json;

namespace Jake.RL
{
	using ArrayExtensions;
	using Reflection;
	using Serialization;

	/// <summary>
	/// Provides data to Cycle through inspector.
	/// </summary>
	public class SimpleDataHandler : DataHandler
	{
		public Data[] outgoingData;
		public Data[] incomingData;

		public override byte[] Serialize()
		{
			return Serializer.SerializeAll(
				outgoingData.Convert((d) => d.Value)
			);
		}

		public override int Serialize(byte[] buffer, int offset)
		{
			return Serializer.SerializeAll(
				outgoingData.Convert((d) => d.Value), buffer, offset
			);
		}

		public override void Deserialize(byte[] bytes)
		{
			for (int i = 0; i < incomingData.Length; ++i)
			{
				var data = incomingData[i];
				var type = data.ValueType;

				data.Value = (type != null ? Serializer.Deserialize(type, bytes, i) : null);
			}
		}

		public override JsonObject Jsonize()
		{
			return JsonTemplates.ToJson(outgoingData);
		}

		public override void Dejsonize(JsonObject json)
		{
			JsonTemplates.ToData(incomingData, json);
		}
	}
}
