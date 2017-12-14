/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730


#if !(UNITY_4_5 || UNITY_4_6 || UNITY_5_0)
#define UNITY_5_1_PLUS
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FlatLighting {
	public class FlatLightingAboutWindow : EditorWindow {

		private static string pathImages;

		private Texture2D headerPicture = null;
		private GUIStyle richLabelStyle;
		private GUIStyle richButtonStyle;
		private GUIStyle iconButtonStyle;

		private const string ImageNameAbout = "Logo512.png";

		[MenuItem("Help/Flat Lighting", false, 0)]
		public static void AboutInit() {
			FlatLightingAboutWindow.InitAndShow();
		}

		public static void InitAndShow() {
				FindRelativePath();				

				FlatLightingAboutWindow window;
				window = EditorWindow.GetWindow<FlatLightingAboutWindow>(true, "About Flat Lighting", true);
				Vector2 size = new Vector2(512, 620);
				window.minSize = size;
				window.maxSize = size;
				window.ShowUtility();
		}

		public static void FindRelativePath() {
			string[] results = AssetDatabase.FindAssets("FlatLightingAboutWindow t:Script", null);
			if (results.Length > 0) {
				string p = AssetDatabase.GUIDToAssetPath(results[0]);
				p = System.IO.Path.GetDirectoryName(p);
				p = p.Substring(0, p.LastIndexOf('/'));
				p = p.Substring(0, p.LastIndexOf('/'));
				pathImages = p + "/Scripts/Editor/Images/";
			}
		}

		public static T LoadAssetAt<T>(string path) where T : UnityEngine.Object {
			#if UNITY_5_1_PLUS
			return AssetDatabase.LoadAssetAtPath<T>(path);
			#else
			return Resources.LoadAssetAtPath<T>(path);
			#endif
		}

		void OnEnable() {
			FindRelativePath();
			headerPicture = LoadAssetAt<Texture2D>(pathImages + ImageNameAbout);
		}

		void OnGUI() {
			if (richLabelStyle == null)	{
				richLabelStyle = new GUIStyle(GUI.skin.label);
				richLabelStyle.richText = true;
				richLabelStyle.wordWrap = true;
				richButtonStyle = new GUIStyle(GUI.skin.button);
				richButtonStyle.richText = true;
				iconButtonStyle = new GUIStyle(GUI.skin.button);
				iconButtonStyle.normal.background = null;
				iconButtonStyle.imagePosition = ImagePosition.ImageOnly;
				iconButtonStyle.fixedWidth = 96;
				iconButtonStyle.fixedHeight = 96;
			}

			Rect headerRect = new Rect(0, 0, 512, 512);
			GUI.DrawTexture(headerRect, headerPicture, ScaleMode.ScaleAndCrop, false);

			GUILayout.Space(525);

			using (new UITools.GUIVertical()) {
				using (new UITools.GUIHorizontal()) {
					if (GUILayout.Button("<b>Documentation</b>\n<size=9>Complete manual</size>", richButtonStyle, GUILayout.MaxWidth(260), GUILayout.Height(36)))
						Application.OpenURL("https://dl.dropboxusercontent.com/u/49026722/FlatLightingDocumentation.pdf");

					if (GUILayout.Button("<b>Rate it</b>\n<size=9>Leave a review on the Asset Store</size>", richButtonStyle, GUILayout.Height(36)))
						Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/67730");
				}

				using (new UITools.GUIHorizontal()) {
					if (GUILayout.Button("<b>E-mail</b>\n<size=9>bogdan.dgochev@gmail.com</size>", richButtonStyle, GUILayout.MaxWidth(172), GUILayout.Height(36)))
						Application.OpenURL("mailto:bogdan.dgochev@gmail.com");

					if (GUILayout.Button("<b>Twitter</b>\n<size=9>@BogdanGochev</size>", richButtonStyle, GUILayout.Height(36)))
						Application.OpenURL("http://twitter.com/BogdanGochev");

					if (GUILayout.Button("<b>Support Forum</b>\n<size=9>Unity Community</size>", richButtonStyle, GUILayout.MaxWidth(172), GUILayout.Height(36)))
						Application.OpenURL("http://forum.unity3d.com/threads/flat-lighting-fl.418937/"); 
				}	
			}
		}
	}
}
