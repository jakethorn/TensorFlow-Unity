using System.Collections.Generic;
using System.Linq;

namespace Jake.IO
{
	using StringExtensions;

	/// <summary>
	/// File system helper methods.
	/// </summary>
	public static class Path
	{
		/// <summary>
		/// Combine a directory and a path.
		/// </summary>
		public static string Combine(string dir, string path)
		{
			if (path == null || path == "")
				return dir;

			// stack directories and paths
			var pathSplit = path.Split('/');
			var dirSplit = new Stack<string>(dir.Split('/'));
			for (int i = 0; i < pathSplit.Length; ++i)
			{
				if (pathSplit[i] == "..")
				{
					dirSplit.Pop();
				}
				else if (pathSplit[i] != "")
				{
					dirSplit.Push(pathSplit[i]);
				}
			}

			// combine
			var combinedPath = "";
			foreach (var s in dirSplit.Reverse())
			{
				combinedPath += s + "/";
			}
			
			return combinedPath.RemoveFromEnd("/");
		}

		/// <summary>
		/// Get relative path from dir to path.
		/// </summary>
		public static string GetRelative(string dir, string path)
		{
			var dirSplit = dir.Split('/');
			var pathSplit = path.Split('/');
			var relativePath = "";
			for (int i = 0; i < dirSplit.Length; ++i)
			{
				if (dirSplit[i] != pathSplit[i])
				{
					for (int j = i; j < dirSplit.Length; ++j)
					{
						relativePath += "/..";
					}

					for (int j = i; j < pathSplit.Length; ++j)
					{
						relativePath += "/" + pathSplit[j];
					}

					break;
				}
			}

			if (relativePath == "")
			{
				for (int i = dirSplit.Length; i < pathSplit.Length; ++i)
				{
					relativePath += "/" + pathSplit[i];
				}
			}

			return relativePath;
		}

		/// <summary>
		/// Go up directories.
		/// </summary>
		public static string Ascend(string path, int levels)
		{
			var pathSplit = path.Split('/');
			var pathEnd = "";
			for (int i = -levels; i < 0; ++i)
			{
				pathEnd += "/" + pathSplit[pathSplit.Length + i];
			}

			return path.RemoveFromEnd(pathEnd);
		}

		/// <summary>
		/// Get file name from path.
		/// </summary>
		public static string GetFile(string path)
		{
			return path.Substring(path.LastIndexOf("/") + 1);
		}

		/// <summary>
		/// Get directory from path.
		/// </summary>
		public static string GetDirectory(string path)
		{
			return path.Remove(path.LastIndexOf("/") + 1);
		}
	}
}
