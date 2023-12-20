using LCHack;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;


namespace badcheats
{
	public class Cheat
	{
		private static GameObject Load;
		
		public static void Init()
		{
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			string[] resourceNames = currentAssembly.GetManifestResourceNames();

			foreach (string resourceName in resourceNames)
			{
				Console.WriteLine(resourceName);
			}

			Settings.UIDesign = H4cks_Class.UrlFileToByteArray("h4cksui.asset");

            Cheat.Load = new UnityEngine.GameObject();
            Cheat.Load.AddComponent<Hacks>();
			UnityEngine.Object.DontDestroyOnLoad(Cheat.Load);

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

		}
		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return LoadAssem();
		}

		public static Assembly LoadAssem()
		{
			byte[] ba = null;
			string resource = "LCHack.0Harmony.dll";
			Assembly curAsm = Assembly.GetExecutingAssembly();
			using (Stream stm = curAsm.GetManifestResourceStream(resource))
			{
				ba = new byte[(int)stm.Length];
				stm.Read(ba, 0, (int)stm.Length);
				return Assembly.Load(ba);
			}
		}

		public static void Unload()
		{
			_Unload();
		}

		private static void _Unload()
		{
			GameObject.Destroy(Load);
			
		}

	}
}