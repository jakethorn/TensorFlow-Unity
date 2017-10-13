using Jake.ArrayExtensions;
using Jake.CameraExtensions;
using Jake.Reflection;
using System;
using UnityEngine;

namespace Matt.Json
{
	public static partial class JsonTemplates
	{
		/// <summary>
		/// Fill Data[] with compatible JsonObject
		/// </summary>
		public static void ToData(Data[] data, JsonObject json)
		{
			if (!Compatible(data, json))
				return;

			foreach (var d in data)
			{
				var field = json[d.Name];
				if (field.IsNull)
					continue;

				switch (d.DataType)
				{
					case DataType.Bool:
						d.Value = ToBool(field);
						break;
					case DataType.Camera:
						throw new Exception("Camera not supported.");
					case DataType.Float:
						d.Value = ToFloat(field);
						break;
					case DataType.Int:
						d.Value = ToInt(field);
						break;
					case DataType.None:
						d.Value = null;
						break;
					case DataType.Quaternion:
						d.Value = ToQuaternion(field);
						break;
					case DataType.String:
						d.Value = field.str;
						break;
					case DataType.Texture2D:
						var tex = new Texture2D(2, 2);
						tex.LoadImage(Convert.FromBase64String(json.str));
						d.Value = tex;
						break;
					case DataType.Vector3:
						d.Value = ToVector3(field);
						break;
					default:
						throw new Exception("Unknown data type.");
				}
			}
		}

		/// <summary>
		/// Convert Data[] to JsonObject
		/// </summary>
		public static JsonObject ToJson(Data[] data)
		{
			var json = new JsonObject();
			
			foreach (var d in data)
			{
				//var value = "";
				switch (d.DataType)
				{
					case DataType.Bool:
						json.AddField(d.Name, (bool)d.Value);
						break;
					case DataType.Camera:
						json.AddField(d.Name, 
							Convert.ToBase64String(
								(d.Value as Camera).ScreenshotAsBytes(Format.JPG)
							)
						);
						break;
					case DataType.Float:
						json.AddField(d.Name, (float)d.Value);
						break;
					case DataType.Int:
						json.AddField(d.Name, (int)d.Value);
						break;
					case DataType.None:
						json.AddField(d.Name, new JsonObject());
						break;
					case DataType.Quaternion:
						json.AddField(d.Name, ToJson((Quaternion)d.Value));
						break;
					case DataType.String:
						json.AddField(d.Name, (string)d.Value);
						break;
					case DataType.Texture2D:
						json.AddField(d.Name, 
							Convert.ToBase64String(
								(d.Value as Texture2D).EncodeToJPG()
							)
						);
						break;
					case DataType.Vector3:
						json.AddField(d.Name, ToJson((Vector3)d.Value));
						break;
					default:
						throw new Exception("Unknown data type.");
				}
			}

			return json;
		}

		private static bool Compatible(Data[] data, JsonObject json)
		{
			var passed = json.HasFields(
				data.Convert((d) => d.Name)
			);

			Debug.Assert(passed, "Data[] did not match JsonObject");

			return passed;
		}
	}
}
