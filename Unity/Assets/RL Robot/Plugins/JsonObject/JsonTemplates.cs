using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSONTemplates class
 * for use with Unity
 * Copyright Matt Schoen 2010
 */

namespace Matt.Json
{
	public static partial class JsonTemplates
	{
		static readonly HashSet<object> touched = new HashSet<object>();

		public static bool ToBool(JsonObject json)
		{
			return json.IsBool ? json.b : bool.Parse(json.str);
		}
		
		public static float ToFloat(JsonObject json)
		{
			return json.IsNumber ? json.n : float.Parse(json.str);
		}
		
		public static int ToInt(JsonObject json)
		{
			return json.IsNumber ? Mathf.RoundToInt(json.n) : int.Parse(json.str);
		}
		
		public static JsonObject ToJson(object obj)
		{       
			//For a generic guess
			if (touched.Add(obj))
			{
				JsonObject result = JsonObject.obj;
				//Fields
				FieldInfo[] fieldinfo = obj.GetType().GetFields();
				foreach (FieldInfo fi in fieldinfo)
				{
					JsonObject val = JsonObject.nullJO;
					if (!fi.GetValue(obj).Equals(null))
					{
						MethodInfo info = typeof(JsonTemplates).GetMethod("From" + fi.FieldType.Name);
						if (info != null)
						{
							object[] parms = new object[1];
							parms[0] = fi.GetValue(obj);
							val = (JsonObject)info.Invoke(null, parms);
						}
						else if (fi.FieldType == typeof(string))
							val = JsonObject.CreateStringObject(fi.GetValue(obj).ToString());
						else
							val = JsonObject.Create(fi.GetValue(obj).ToString());
					}
					if (val)
					{
						if (val.type != JsonObject.Type.NULL)
							result.AddField(fi.Name, val);
						else Debug.LogWarning("Null for this non-null object, property " + fi.Name + " of class " + obj.GetType().Name + ". Object type is " + fi.FieldType.Name);
					}
				}
				//Properties
				PropertyInfo[] propertyInfo = obj.GetType().GetProperties();
				foreach (PropertyInfo pi in propertyInfo)
				{
					//This section should mirror part of AssetFactory.AddScripts()
					JsonObject val = JsonObject.nullJO;
					if (!pi.GetValue(obj, null).Equals(null))
					{
						MethodInfo info = typeof(JsonTemplates).GetMethod("From" + pi.PropertyType.Name);
						if (info != null)
						{
							object[] parms = new object[1];
							parms[0] = pi.GetValue(obj, null);
							val = (JsonObject)info.Invoke(null, parms);
						}
						else if (pi.PropertyType == typeof(string))
							val = JsonObject.CreateStringObject(pi.GetValue(obj, null).ToString());
						else
							val = JsonObject.Create(pi.GetValue(obj, null).ToString());
					}
					if (val)
					{
						if (val.type != JsonObject.Type.NULL)
							result.AddField(pi.Name, val);
						else Debug.LogWarning("Null for this non-null object, property " + pi.Name + " of class " + obj.GetType().Name + ". Object type is " + pi.PropertyType.Name);
					}
				}
				return result;
			}
			Debug.LogWarning("trying to save the same data twice");
			return JsonObject.nullJO;
		}
	}
}

/*
 * Some helpful code templates for the JSON class
 * 
 * LOOP THROUGH OBJECT
for(int i = 0; i < obj.Count; i++){
	if(obj.keys[i] != null){
		switch((string)obj.keys[i]){
			case "key1":
				do stuff with (JSONObject)obj.list[i];
				break;
			case "key2":
				do stuff with (JSONObject)obj.list[i];
				break;		
		}
	}
}
 *
 * LOOP THROUGH ARRAY
foreach(JSONObject ob in obj.list)
	do stuff with ob;
 */
