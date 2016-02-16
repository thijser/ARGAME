using System;
using System.IO;
using UnityEngine;

namespace Meta
{
	public class MetaPlugin
	{
		static MetaPlugin()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
			string text;
			if (Application.get_isEditor())
			{
				text = string.Concat(new object[]
				{
					Environment.CurrentDirectory,
					Path.DirectorySeparatorChar,
					"Assets",
					Path.DirectorySeparatorChar,
					"Plugins",
					Path.DirectorySeparatorChar,
					"x86"
				});
			}
			else
			{
				text = Application.get_dataPath() + Path.DirectorySeparatorChar + "Plugins";
			}
			if (!environmentVariable.Contains(text))
			{
				Environment.SetEnvironmentVariable("PATH", text + Path.PathSeparator + environmentVariable, EnvironmentVariableTarget.Process);
			}
		}

		public static void Load()
		{
		}
	}
}
