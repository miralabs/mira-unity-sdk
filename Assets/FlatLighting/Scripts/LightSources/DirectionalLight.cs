/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using System;
using UnityEngine;

namespace FlatLighting {
	[AddComponentMenu("Flat Lighting/Directional Light", 1)]
	[ExecuteInEditMode]
	public class DirectionalLight : LightSource<DirectionalLight> {

		private static readonly string directionalLightCountProperty = "_DirectionalLight_Length";
		private static readonly string directionalLightColorProperty = "_DirectionalLightColor";
		private static readonly string directionalLightForwardProperty = "_DirectionalLightForward";

		public bool isRealTime;

		public Color LightColor = Color.white;

		private bool isFirstPass = true;

		void OnEnable() {
			DirectionalLight.MAX_LIGHTS = 5;
			isFirstPass = true;
			base.InitLightSource(directionalLightCountProperty);
		}

		void OnDisable() {
			base.ReleaseLightSource(directionalLightCountProperty);
		}

		protected override void UpdatedId(int newId, int oldId)
		{
			SetLighting();
			ResetSettings(oldId);
		}

		void Update () {
			#if UNITY_EDITOR
			if (!Application.isPlaying || isRealTime || !isRealTime && isFirstPass) {
			#else
			if (isRealTime || !isRealTime && isFirstPass) {
			#endif
				SetLighting();

				isFirstPass = false;
			}
		}

		#if UNITY_5_4_OR_NEWER
			private static Vector4[] forward = new Vector4[MAX_LIGHTS];
			private static Vector4[] color = new Vector4[MAX_LIGHTS];

			void LateUpdate() {
				if (Id != 0)
					return;

				Shader.SetGlobalVectorArray(directionalLightForwardProperty, forward);
				Shader.SetGlobalVectorArray(directionalLightColorProperty, color);
			}
		#endif

		void SetLighting() {
			#if UNITY_5_4_OR_NEWER
				forward [Id] = transform.forward;
				color [Id] = LightColor;
			#else
				Shader.SetGlobalVector(directionalLightForwardProperty + Id.ToString(), transform.forward);
				Shader.SetGlobalVector(directionalLightColorProperty + Id.ToString(), LightColor);
			#endif
		}

		static void ResetSettings(int id)
		{
			#if UNITY_5_4_OR_NEWER
				forward[id] = Vector3.zero;
				color[id] = Vector4.zero;
			#else
				Shader.SetGlobalVector(directionalLightForwardProperty + id.ToString(), Vector3.zero);
				Shader.SetGlobalVector(directionalLightColorProperty + id.ToString(), Vector3.zero);
			#endif
		}

		void OnDrawGizmosSelected() {
			DrawSelectedGizmo();

			Gizmos.color = Color.yellow;
			const float lineLength = 5.0f;
			Vector3 lightDirection = -transform.forward * lineLength;
			Gizmos.DrawRay(transform.position, lightDirection);
		}
	}
}
