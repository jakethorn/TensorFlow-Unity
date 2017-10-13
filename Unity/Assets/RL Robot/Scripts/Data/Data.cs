using System;
using System.Reflection;
using UnityEngine;

namespace Jake.Reflection
{
	using ArrayExtensions;

	/// <summary>
	/// Editor assignable generic data.
	/// Can assign data from the inspector, fields, properties or methods.
	/// (For an example, see the Settings or SimpleDataHandler components).
	/// </summary>
	[Serializable]
	public class Data
	{
		/*
		 *	Public
		 */

			/*
			 *	Static
			 */

		/// <summary>
		/// Member flags used when accessing sources.
		/// </summary>
		public static BindingFlags MemberFlags
		{
			get
			{
				return BindingFlags.Instance | BindingFlags.Public;
			}
		}

		/// <summary>
		/// Change DataType to Type.
		/// </summary>
		public static Type ToType(DataType type)
		{
			switch (type)
			{
				case DataType.None:			return null;
				case DataType.Bool:			return typeof(bool);
				case DataType.Camera:		return typeof(Camera);
				case DataType.Float:		return typeof(float);
				case DataType.Int:			return typeof(int);
				case DataType.Quaternion:	return typeof(Quaternion);
				case DataType.String:		return typeof(string);
				case DataType.Texture2D:	return typeof(Texture2D);
				case DataType.Vector3:		return typeof(Vector3);
				default:
					throw new Exception("Unknown data type.");
			}
		}

			/*
			 *	Instance
			 */

		/// <summary>
		/// Data value.
		/// </summary>
		public object Value
		{
			get
			{
				switch (source)
				{
					case Source.Field:		return GetFieldValue();
					case Source.Inspector:	return GetInspectorValue();
					case Source.Method:		return GetMethodValue();
					case Source.Property:	return GetPropertyValue();
					default:
						throw new Exception("Unknown source.");
				}
			}

			set
			{
				switch (source)
				{
					case Source.Field:		SetFieldValue(value); break;
					case Source.Inspector:	SetInspectorValue(value); break;
					case Source.Method:		SetMethodValue(value); break;
					case Source.Property:	SetPropertyValue(value); break;
					default:
						throw new Exception("Unknown source.");
				}
			}
		}

		public DataType DataType
		{
			get
			{
				return type;
			}
		}

		/// <summary>
		/// Type of value.
		/// </summary>
		public Type ValueType
		{
			get
			{
				return ToType(type);
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		/*
		 *	Private
		 */

		/*
		 *	Fields
		 */
#pragma warning disable 649
		[SerializeField] private string		name;

		[SerializeField] private DataType	type;
		[SerializeField] private Source		source;

		[SerializeField] private GameObject gameObject;
		[SerializeField] private Component	component;

		[SerializeField] private string		fieldSignature, 
											methodSignature, 
											propertySignature;
#pragma warning restore 649
		[SerializeField] private bool boo;
		[SerializeField] private Camera cam;
		[SerializeField] private float flt;
		[SerializeField] private int i;
		[SerializeField] private Quaternion qtn;
		[SerializeField] private string str;
		[SerializeField] private Texture2D tex;
		[SerializeField] private Vector3 vec;
#if UNITY_EDITOR
		[SerializeField] private bool folded;
#endif
		/*
		 *	Properties
		 */

		private FieldInfo Field
		{
			get
			{
				return component.GetType().GetFields(MemberFlags).First((f) =>
				{
					var fs = f.ToString();
#if NETFX_CORE
					if (f.FieldType == typeof(float) || f.FieldType == typeof(bool) || f.FieldType == typeof(int))
					{
						fs = "System." + fs;
					}
#endif
					return fs == fieldSignature;
				});
			}
		}

		private MethodInfo		Method		{ get { return component.GetType().GetMethods	(MemberFlags).First((m) => m.ToString() == methodSignature);	} }
		private PropertyInfo	Property	{ get { return component.GetType().GetProperties(MemberFlags).First((p) => p.ToString() == propertySignature);	} }
		
			/*
			 *	Methods
			 */
		 
		private object GetFieldValue()
		{
			return Field.GetValue(component);
		}

		private object GetInspectorValue()
		{
			switch (type)
			{
				case DataType.None:			return null;
				case DataType.Bool:			return boo;
				case DataType.Camera:		return cam;
				case DataType.Float:		return flt;
				case DataType.Int:			return i;
				case DataType.Quaternion:	return qtn;
				case DataType.String:		return str;
				case DataType.Texture2D:	return tex;
				case DataType.Vector3:		return vec;
				default:
					throw new Exception("Unknown data type.");
			}
		}

		private object GetMethodValue()
		{
			return Method.Invoke(component, null);
		}

		private object GetPropertyValue()
		{
			return Property.GetValue(component, null);
		}

		private void SetFieldValue(object value)
		{
			Field.SetValue(component, value);
		}

		private void SetInspectorValue(object value)
		{
			switch (type)
			{
				case DataType.None:										break;
				case DataType.Bool:			boo = (bool)		value;	break;
				case DataType.Camera:		cam = (Camera)		value;	break;
				case DataType.Float:		flt = (float)		value;	break;
				case DataType.Int:			i	= (int)			value;	break;
				case DataType.Quaternion:	qtn = (Quaternion)	value;	break;
				case DataType.String:		str = (string)		value;	break;
				case DataType.Texture2D:	tex = (Texture2D)	value;	break;
				case DataType.Vector3:		vec = (Vector3)		value;	break;
				default:
					throw new Exception("Unknown data type.");
			}
		}

		private void SetMethodValue(object value)
		{
			Method.Invoke(component, type == DataType.None ? null : new object[] { value });
		}

		private void SetPropertyValue(object value)
		{
			Property.SetValue(component, value, null);
		}

		private object DefaultValue()
		{
			switch (type)
			{
				case DataType.None:			return null;
				case DataType.Bool:			return default(bool);
				case DataType.Camera:		return default(Camera);
				case DataType.Float:		return default(float);
				case DataType.Int:			return default(int);
				case DataType.Quaternion:	return default(Quaternion);
				case DataType.String:		return default(string);
				case DataType.Texture2D:	return default(Texture2D);
				case DataType.Vector3:		return default(Vector3);
				default:
					throw new Exception("Unknown data type.");
			}
		}
	}
}
