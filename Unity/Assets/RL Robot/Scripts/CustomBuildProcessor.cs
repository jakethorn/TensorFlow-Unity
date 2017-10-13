#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

namespace Jake
{
	public class CustomBuildProcessor : IPreprocessBuild
	{
		public static event System.Action<string> OnPrepocessBuild;

		public int callbackOrder { get { return 0; } }

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			if (IsStandalone(target))
			{
				OnPrepocessBuild(path);
			}
		}

		private bool IsStandalone(BuildTarget target)
		{
			return	target == BuildTarget.StandaloneWindows ||
					target == BuildTarget.StandaloneWindows64 ||
					target == BuildTarget.StandaloneLinux ||
					target == BuildTarget.StandaloneLinux64 ||
					target == BuildTarget.StandaloneLinuxUniversal;
		}
	}
}
#endif
