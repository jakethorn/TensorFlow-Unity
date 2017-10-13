using Matt.Json;
using System;
using System.IO;
using UnityEngine;

namespace Jake.Json
{
	/// <summary>
	/// Inspector friendly json file.
	/// </summary>
	[Serializable]
	public class JsonFile
	{
		public JsonFile()
		{
#if UNITY_EDITOR
			// change relative path from the asset folder to the build folder
			CustomBuildProcessor.OnPrepocessBuild += (path) =>
			{
				var buildDir = IO.Path.Ascend(path, 1);
				relativeBuildPath = IO.Path.GetRelative(buildDir, Path);
			};
#endif
		}
#pragma warning disable 649, 414
		[SerializeField]
		private string relativePath;

		[SerializeField]
		private string relativeBuildPath;
#pragma warning restore 649, 414

		public string Path
		{
			get
			{
#if UNITY_EDITOR
				return IO.Path.Combine(Application.dataPath, relativePath);
#else
				return IO.Path.Combine(IO.Path.Ascend(Application.dataPath, 1), relativeBuildPath);
#endif
			}
		}

		/// <summary>
		/// Returns JsonObject of type NULL if invalid json or path
		/// </summary>
		public JsonObject JsonObject
		{
			get
			{
				// LogError if invalid path
				var pathExists = File.Exists(Path);
				if (!pathExists && relativePath != "")
				{
					Debug.LogError("Json file does not exist at path: " + Path);
				}

				// JsonObject will LogError if invalid json
				return new JsonObject(
					pathExists ? File.ReadAllText(Path) : null
				);
			}
		}
	}
}
