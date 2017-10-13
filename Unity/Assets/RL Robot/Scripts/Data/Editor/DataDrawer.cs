using Jake.UnityEditor;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Jake.Reflection.UnityEditor
{
	using ArrayExtensions;

	[CustomPropertyDrawer(typeof(Data))]
	public class DataDrawer : Drawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init(position, property);
			
			if (!Foldout(GetTitle(), "folded"))
				return;

			PropertyField<string>("name");
			var dataType = PropertyField<DataType>("type");
			var type = Data.ToType(dataType);
			var source = PropertyField<Source>("source");
			switch (source)
			{
				case Source.Field:
					DrawRect(3);
					MemberField<FieldInfo>(type, "gameObject", "component", "fieldSignature");
					break;
				case Source.Inspector:
					DrawRect(1);
					InspectorField(dataType);
					break;
				case Source.Method:
					DrawRect(3);
					MemberField<MethodInfo>(type, "gameObject", "component", "methodSignature");
					break;
				case Source.Property:
					DrawRect(3);
					MemberField<PropertyInfo>(type, "gameObject", "component", "propertySignature");
					break;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var source = (Source)property.FindPropertyRelative("source").enumValueIndex;

			var lines = 4;
			switch (source)
			{
				case Source.Field:		lines += 3; break;
				case Source.Inspector:	lines += 1; break;
				case Source.Method:		lines += 3; break;
				case Source.Property:	lines += 3; break;
			}

			if (!property.FindPropertyRelative("folded").boolValue)
				lines = 1;

			return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines;
		}
		
		/// <summary>
		/// Warning: Extremely messy code.
		/// </summary>
		private string GetTitle()
		{
			var name = property.FindPropertyRelative("name").stringValue;
			if (name != null && name != "")
				return name;

			try
			{
				var source = (Source)property.FindPropertyRelative("source").enumValueIndex;
				var gameObject = property.FindPropertyRelative("gameObject").objectReferenceValue as GameObject;
				var component = property.FindPropertyRelative("component").objectReferenceValue as Component;

				var title = "";
				switch (source)
				{
					case Source.Field:
						title += gameObject.name;
						title += "." + component.GetType().Name;
						var field = component.GetType().GetFields(Data.MemberFlags).First(
							(f) => f.ToString() == property.FindPropertyRelative("fieldSignature").stringValue
						);
						title += "." + field.Name;
						break;
					case Source.Inspector:
						title += InspectorValue((DataType)property.FindPropertyRelative("type").enumValueIndex).ToString();
						break;
					case Source.Method:
						title += gameObject.name;
						title += "." + component.GetType().Name;
						var method = component.GetType().GetMethods(Data.MemberFlags).First(
							(m) => m.ToString() == property.FindPropertyRelative("methodSignature").stringValue
						);
						title += "." + method.Name;
						break;
					case Source.Property:
						title += gameObject.name;
						title += "." + component.GetType().Name;
						var member = component.GetType().GetProperties(Data.MemberFlags).First(
							(p) => p.ToString() == property.FindPropertyRelative("propertySignature").stringValue
						);
						title += "." + member.Name;
						break;
				}

				return title;
			}
			catch
			{
				return "None";
			}
		}

		private void InspectorField(DataType type)
		{
			switch (type)
			{
				case DataType.Bool:			PropertyField<bool>			("Value", "boo");	break;
				case DataType.Camera:		PropertyField<Camera>		("Value", "cam");	break;
				case DataType.Float:		PropertyField<float>		("Value", "flt");	break;
				case DataType.Int:			PropertyField<int>			("Value", "i");		break;
				case DataType.Quaternion:	PropertyField<Quaternion>	("Value", "qtn");	break;
				case DataType.String:		PropertyField<string>		("Value", "str");	break;
				case DataType.Texture2D:	PropertyField<Texture2D>	("Value", "tex");	break;
				case DataType.Vector3:		PropertyField<Vector3>		("Value", "vec");	break;
				default:
					throw new Exception("Unknown data type.");
			}
		}

		private object InspectorValue(DataType type)
		{
			switch (type)
			{
				case DataType.Bool:			return property.FindPropertyRelative("boo").boolValue;
				case DataType.Camera:		return property.FindPropertyRelative("cam").objectReferenceValue;
				case DataType.Float:		return property.FindPropertyRelative("flt").floatValue;
				case DataType.Int:			return property.FindPropertyRelative("i").intValue;
				case DataType.Quaternion:	return property.FindPropertyRelative("qtn").quaternionValue;
				case DataType.String:		return property.FindPropertyRelative("str").stringValue;
				case DataType.Texture2D:	return property.FindPropertyRelative("tex").objectReferenceValue;
				case DataType.Vector3:		return property.FindPropertyRelative("vec").vector3Value;
				default:
					throw new Exception("Unknown data type.");
			}
		}
	}
}
