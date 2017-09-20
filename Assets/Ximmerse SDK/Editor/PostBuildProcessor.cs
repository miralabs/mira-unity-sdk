//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

namespace Ximmerse {

	public class PostBuildProcessor{

		internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
		{
			if (Directory.Exists(dstPath))
				Directory.Delete(dstPath);
			if (File.Exists(dstPath))
				File.Delete(dstPath);

			Directory.CreateDirectory(dstPath);

			foreach (var file in Directory.GetFiles(srcPath))
				File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

			foreach (var dir in Directory.GetDirectories(srcPath))
				CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
		}

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
#if UNITY_IOS
			if (buildTarget == BuildTarget.iOS) {
				string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

				PBXProject proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText(projPath));

				string target = proj.TargetGuidByName("Unity-iPhone");

				// Set a custom link flag
				proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-framework CoreBluetooth");
				proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-framework IOKit");

				File.WriteAllText(projPath, proj.WriteToString());
				Debug.Log("added liner flag");
				return;
			}
#endif
#if UNITY_ANDROID
			if (buildTarget == BuildTarget.Android) {
				bool hasManifest=File.Exists(Application.dataPath+"/../Temp/StagingArea/AndroidManifest.xml");
				bool hasBlePermission=false;
				if(hasManifest) {
					string text=File.ReadAllText(Application.dataPath+"/../Temp/StagingArea/AndroidManifest.xml");
					hasBlePermission=
						text.IndexOf("android.permission.BLUETOOTH")!=-1&&
						text.IndexOf("android.permission.BLUETOOTH_ADMIN")!=-1&&
						text.IndexOf("android.permission.ACCESS_FINE_LOCATION")!=-1
					;
				}
				if(!hasBlePermission) {
					Debug.LogError("No bluetooth permission in AndroidManifest.xml!!!\nYou can edit AndroidManifest.xml like this:\n<manifest>\n\t...\n"+
"\t<uses-permission android:name=\"android.permission.BLUETOOTH\"/>\n"+
"\t<uses-permission android:name=\"android.permission.BLUETOOTH_ADMIN\"/>\n"+
"\t<uses-permission android:name=\"android.permission.ACCESS_FINE_LOCATION\"/>\n"+
"</manifest>");
					//File.Delete(...);
				}
			}
#endif
		}
	}
}