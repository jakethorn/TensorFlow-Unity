using UnityEngine;
using UnityEditor;

namespace Jake.Tcp
{
	[CustomEditor(typeof(TcpConnection))]
	public class TcpConnectionEditor : Editor
	{
		TcpConnection tcpConnection;
		SerializedProperty autoConnect;
		SerializedProperty socketType;
		SerializedProperty ip;
		SerializedProperty port;
		SerializedProperty useJson;
		SerializedProperty jsonFile;

		bool jsonLoaded;

		void OnEnable()
		{
			tcpConnection = target as TcpConnection;
			autoConnect	= serializedObject.FindProperty("autoConnect");
			socketType	= serializedObject.FindProperty("socketType");
			ip			= serializedObject.FindProperty("ip");
			port		= serializedObject.FindProperty("port");
			useJson		= serializedObject.FindProperty("useJson");
			jsonFile	= serializedObject.FindProperty("jsonFile");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as TcpConnection), typeof(TcpConnection), false);
			GUI.enabled = true;

			EditorGUILayout.PropertyField(autoConnect);
			EditorGUILayout.PropertyField(socketType);

			if (tcpConnection.useJson)
			{
				if (!jsonLoaded)
				{
					try
					{
						tcpConnection.LoadJson();
						jsonLoaded = true;
					}
					catch
					{

					}
				}

				GUI.enabled = false;
			}
			else
			{
				jsonLoaded = false;
			}

			EditorGUILayout.PropertyField(ip);
			EditorGUILayout.PropertyField(port);

			if (tcpConnection.useJson)
			{
				GUI.enabled = true;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(useJson);

			if (tcpConnection.useJson)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.PropertyField(jsonFile, GUIContent.none);
			}

			EditorGUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
