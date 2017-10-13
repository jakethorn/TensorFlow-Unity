#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;

namespace Jake
{
	public class CustomBuildProcessor : IPreprocessBuild
	{
		public static event Action<string> OnPrepocessBuild;

		public int callbackOrder { get { return 0; } }

		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			if (IsStandalone(target) && OnPrepocessBuild != null)
			{
				OnPrepocessBuild(path);
			}
		}

		private bool IsStandalone(BuildTarget target)
		{
			return	target == BuildTarget.StandaloneWindows			||
					target == BuildTarget.StandaloneWindows64		||
					target == BuildTarget.StandaloneLinux			||
					target == BuildTarget.StandaloneLinux64			||
					target == BuildTarget.StandaloneLinuxUniversal	||
					target == BuildTarget.StandaloneOSXIntel		||
					target == BuildTarget.StandaloneOSXIntel64		||
					target == BuildTarget.StandaloneOSXUniversal;
		}
	}
}
#endif
