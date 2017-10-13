using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Jake.Serialization
{
	using ArrayExtensions;
	using CameraExtensions;

	/// <summary>
	/// (De)serialize objects.
	/// </summary>
	public static class Serializer
	{
		private static Dictionary<Type, Func<object, byte[]>>			serializers		= new Dictionary<Type, Func<object, byte[]>>();
		private static Dictionary<Type, Func<byte[], int, int, object>> deserializers	= new Dictionary<Type, Func<byte[], int, int, object>>();

		static Serializer()
		{
			// serializers
			AddSerializer<bool>((v) => BitConverter.GetBytes((bool)v));
			AddSerializer<Camera>((v) =>
			{
				return (v as Camera).ScreenshotAsBytes(Format.JPG);
			});
			AddSerializer<float>((v) => BitConverter.GetBytes((float)v));
			AddSerializer<int>((v) => BitConverter.GetBytes((int)v));
			AddSerializer<Quaternion>((v) =>
			{
				var bytes = new byte[16];

				var q = (Quaternion)v;
				Buffer.BlockCopy(BitConverter.GetBytes(q.x), 0, bytes, 0, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(q.y), 0, bytes, 4, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(q.z), 0, bytes, 8, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(q.w), 0, bytes, 12, 4);

				return bytes;
			});
			AddSerializer<string>((v) => Encoding.UTF8.GetBytes((string)v));
			AddSerializer<Texture2D>((v) => (v as Texture2D).EncodeToJPG());
			AddSerializer<Vector3>((v) =>
			{
				var bytes = new byte[12];

				var w = (Vector3)v;
				Buffer.BlockCopy(BitConverter.GetBytes(w.x), 0, bytes, 0, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(w.y), 0, bytes, 4, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(w.z), 0, bytes, 8, 4);

				return bytes;
			});

			// deserializers
			AddDeserializer<bool>((bs, o, s) => BitConverter.ToBoolean(bs, o));
			AddDeserializer<float>((bs, o, s) => BitConverter.ToSingle(bs, o));
			AddDeserializer<int>((bs, o, s) => BitConverter.ToInt32(bs, o));
			AddDeserializer<Quaternion>((bs, o, s) =>
			{
				return new Quaternion(
					BitConverter.ToSingle(bs, o + 0),
					BitConverter.ToSingle(bs, o + 4),
					BitConverter.ToSingle(bs, o + 8),
					BitConverter.ToSingle(bs, o + 12)
				);
			});
			AddDeserializer<string>(Encoding.UTF8.GetString);
			AddDeserializer<Texture2D>((bs, o, s) =>
			{
				var tex = new Texture2D(2, 2);
				var data = new byte[s];
				Buffer.BlockCopy(bs, o, data, 0, s);
				tex.LoadImage(data);
				return tex;
			});
			AddDeserializer<Vector3>((bs, o, s) =>
			{
				return new Vector3(
					BitConverter.ToSingle(bs, o + 0),
					BitConverter.ToSingle(bs, o + 4),
					BitConverter.ToSingle(bs, o + 8)
				);
			});
		}
		
		/*
		 *	Serialization
		 */

		/// <summary>
		/// Serialize value.
		/// </summary>
		public static byte[] Serialize(object value)
		{
			return Serialize(value, serializers[value.GetType()]);
		}

		/// <summary>
		/// Serialize value into buffer.
		/// </summary>
		public static int Serialize(object value, byte[] buffer, int offset)
		{
			return Serialize(value, buffer, offset, serializers[value.GetType()]);
		}

		/// <summary>
		/// Serialize value using specified serializer.
		/// </summary>
		public static byte[] Serialize(object value, Func<object, byte[]> serializer)
		{
			var body = serializer(value);
			var size = BitConverter.GetBytes(body.Length);
			var bytes = new byte[size.Length + body.Length];
			Buffer.BlockCopy(size, 0, bytes, 0, size.Length);
			Buffer.BlockCopy(body, 0, bytes, 4, body.Length);

			return bytes;
		}

		/// <summary>
		/// Serialize value into buffer using specified serializer.
		/// </summary>
		public static int Serialize(object value, byte[] buffer, int offset, Func<object, byte[]> serializer)
		{
			var body = serializer(value);
			var size = BitConverter.GetBytes(body.Length);

			if (body.Length + size.Length > buffer.Length - offset)
			{
				throw new InsufficientMemoryException();
			}

			Buffer.BlockCopy(size, 0, buffer, offset,				size.Length);
			Buffer.BlockCopy(body, 0, buffer, offset + size.Length,	body.Length);

			return body.Length + size.Length;
		}

		/// <summary>
		/// Serialize all values.
		/// </summary>
		public static byte[] SerializeAll(object[] values)
		{
			var bytes = new List<byte>();
			foreach (var value in values)
			{
				bytes.AddRange(Serialize(value));
			}

			return bytes.ToArray();
		}

		/// <summary>
		/// Serialize all values into buffer.
		/// </summary>
		public static int SerializeAll(object[] values, byte[] buffer, int offset)
		{
			var size = 0;
			foreach (var value in values)
			{
				size += Serialize(value, buffer, offset + size);
			}

			return size;
		}

		/// <summary>
		/// Serialize all public fields in object using reflection.
		/// </summary>
		public static byte[] SerializeObject(object obj)
		{
			return SerializeAll(
				obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Convert((f) => f.GetValue(obj))
			);
		}

		/// <summary>
		/// Serialize all public fields in object using reflection into buffer.
		/// </summary>
		public static int SerializeObject(object obj, byte[] buffer, int offset)
		{
			return SerializeAll(
				obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Convert((f) => f.GetValue(obj)), 
				buffer, 
				offset
			);
		}

		/// <summary>
		/// Add serializer.
		/// </summary>
		public static void AddSerializer<T>(Func<object, byte[]> serializer)
		{
			serializers[typeof(T)] = serializer;
		}

		/*
		 *	Deserialization
		 */

		/// <summary>
		/// Deserialize value.
		/// </summary>
		public static T Deserialize<T>(byte[] bytes, int valueIndex)
		{
			return (T)Deserialize(typeof(T), bytes, valueIndex);
		}

		/// <summary>
		/// Deserialize value using specified deserializer.
		/// </summary>
		public static T Deserialize<T>(byte[] bytes, int valueIndex, Func<byte[], int, int, object> deserializer)
		{
			return (T)Deserialize(bytes, valueIndex, deserializer);
		}

		/// <summary>
		/// Deserialize value.
		/// </summary>
		public static object Deserialize(Type type, byte[] bytes, int valueIndex)
		{
			return Deserialize(bytes, valueIndex, deserializers[type]);
		}

		/// <summary>
		/// Deserialize value using specified deserializer.
		/// </summary>
		public static object Deserialize(byte[] bytes, int valueIndex, Func<byte[], int, int, object> deserializer)
		{
			var size = 0;
			var offset = 0;
			for (int i = 0; i < valueIndex; ++i)
			{
				size = BitConverter.ToInt32(bytes, offset);
				offset += sizeof(int) + size;
			}

			size = BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);

			return deserializer(bytes, offset, size);
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T>(byte[] bytes)
		{
			var values = new object[1];
			values[0] = Deserialize<T>(bytes, 0);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1>(byte[] bytes)
		{
			var values = new object[2];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1, T2>(byte[] bytes)
		{
			var values = new object[3];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			values[2] = Deserialize<T2>(bytes, 2);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1, T2, T3>(byte[] bytes)
		{
			var values = new object[4];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			values[2] = Deserialize<T2>(bytes, 2);
			values[3] = Deserialize<T3>(bytes, 3);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1, T2, T3, T4>(byte[] bytes)
		{
			var values = new object[5];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			values[2] = Deserialize<T2>(bytes, 2);
			values[3] = Deserialize<T3>(bytes, 3);
			values[4] = Deserialize<T4>(bytes, 4);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1, T2, T3, T4, T5>(byte[] bytes)
		{
			var values = new object[6];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			values[2] = Deserialize<T2>(bytes, 2);
			values[3] = Deserialize<T3>(bytes, 3);
			values[4] = Deserialize<T4>(bytes, 4);
			values[5] = Deserialize<T5>(bytes, 5);
			return values;
		}

		/// <summary>
		/// Deserialize all values.
		/// </summary>
		public static object[] DeserializeAll<T0, T1, T2, T3, T4, T5, T6>(byte[] bytes)
		{
			var values = new object[7];
			values[0] = Deserialize<T0>(bytes, 0);
			values[1] = Deserialize<T1>(bytes, 1);
			values[2] = Deserialize<T2>(bytes, 2);
			values[3] = Deserialize<T3>(bytes, 3);
			values[4] = Deserialize<T4>(bytes, 4);
			values[5] = Deserialize<T5>(bytes, 5);
			values[6] = Deserialize<T6>(bytes, 6);
			return values;
		}

		/// <summary>
		/// Add deserializer.
		/// </summary>
		public static void AddDeserializer<T>(Func<byte[], int, int, object> deserializer)
		{
			deserializers[typeof(T)] = deserializer;
		}
	}
}
