using Jake.UnityEditor;
using UnityEngine;
using UnityEditor;

namespace Jake.Json.UnityEditor
{
	using IO;

	[CustomPropertyDrawer(typeof(JsonFile))]
	public class JsonFileDrawer : Drawer
	{
		private readonly Color errorColour = new Color(1, .5f, .5f, 1);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init(position, property);

			BeginLine(2, .9f, .1f);

			// label
			var guiColour = GUI.color;
			GUI.color = GetTarget<JsonFile>().JsonObject.IsNull ? errorColour : GUI.color;
			Label("relativePath");
			GUI.color = guiColour;

			// button)
			if (GUI.Button(this.position, "..."))
			{
				var panelPath = Path.Ascend(Application.dataPath, 2);
				var path = EditorUtility.OpenFilePanelWithFilters("Json file", panelPath, new[] { "Json", "json" });
				if (path != "")
				{
					property.FindPropertyRelative("relativePath").stringValue = 
						Path.GetRelative(Application.dataPath, path);
				}
			}

			EndLine();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
