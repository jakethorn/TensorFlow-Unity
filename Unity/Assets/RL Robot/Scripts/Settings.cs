using Matt.Json;
using System.IO;
using UnityEngine;

namespace Jake
{
	using IO;
	using Json;
	using Reflection;
	using Threading;

	/// <summary>
	/// Edit data at runtime from a json file (Not compatible with HoloLens).
	/// </summary>
	public class Settings : MonoBehaviour
	{
		public JsonFile jsonFile;
		public Data[] data;
		
		void Start()
		{
			LoadSettings();
			WatchSettings();
		}
		
		private void LoadSettings()
		{
			JsonTemplates.ToData(data, jsonFile.JsonObject);
		}

		private void WatchSettings()
		{
			var dir = Path.GetDirectory(jsonFile.Path);
			var file = Path.GetFile(jsonFile.Path);
#if !NETFX_CORE
			var watcher = new FileSystemWatcher(dir)
			{
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = file,
				EnableRaisingEvents = true
			};

			watcher.Changed += (object source, FileSystemEventArgs e) => 
			{
				Dispatcher.AddJob(LoadSettings, false);
			};
#endif
		}
	}
}
