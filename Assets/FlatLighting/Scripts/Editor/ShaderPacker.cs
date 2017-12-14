/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace FlatLighting {
	public static class ShaderPacker {

		static readonly string packedSuffix = ".fl";
		static readonly string unPackedSuffix = ".shader";
		static readonly string metaSuffix = ".meta";

		public static void UnpackShader(string relativeDirectory, string shaderName) {
			ConvertFile(relativeDirectory, shaderName, packedSuffix, unPackedSuffix);
		}

		public static void PackShader(string relativeDirectory, string shaderName) {
			ConvertFile(relativeDirectory, shaderName, unPackedSuffix, packedSuffix);
		}

		static void ConvertFile(string relativeDirectory, string shaderName, string fromSuffix, string toSuffix) {
			string[] allFiles = Directory.GetFiles(Application.dataPath + relativeDirectory);

			List<string> filteredFiles = new List<string>();
			foreach(string filename in allFiles)
			{
				if(filename.Contains(shaderName + fromSuffix) && filename.EndsWith(fromSuffix))
				{
					filteredFiles.Add(filename);
				}

				if(filename.Contains(shaderName + fromSuffix) && filename.EndsWith(metaSuffix))
				{
					File.Delete(filename);
				}
			}

			foreach(string oldFilename in filteredFiles)
			{
				string newName = oldFilename.Replace(fromSuffix, toSuffix);
				Debug.Log("Extracting " + oldFilename + " in "+ newName);
				File.Move(oldFilename, newName);
			}

			AssetDatabase.Refresh();
		}
	}
}
