/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using System;
using UnityEngine;

namespace FlatLighting {
	[AddComponentMenu("Flat Lighting/Point Light", 2)]
	[ExecuteInEditMode]
	public class PointLight : LightSource<PointLight> {

		private static readonly string pointLightCountProperty = "_PointLight_Length";
		private static readonly string pointLight0WorldToModelProperty = "_PointLightMatrixC0";
		private static readonly string pointLightColorProperty = "_PointLightColor";
		private static readonly string pointLightDistancesProperty = "_PointLightDistances";
		private static readonly string pointLightIntensitiesProperty = "_PointLightIntensities";
		private static readonly string pointLightSmoothnessProperty = "_PointLightSmoothness";

		public float Range = 1.0f;
		public Color LightColor = Color.white;

		[Tooltip("Every component is a circle of light, starting with X to Z. Example (0.5, 0.7, 1, 0).")]
		[VectorAsSliders("Lighting Distances", 3, 0.0f, 1.0f)] 
		public Vector4 LightDistances = new Vector4(0.5f, 0.7f, 1.0f, 0.0f);

		[Tooltip("Color intensitie of ecery fallof. Example (1, 0.5, 0.25, 0).")]
		[VectorAsSliders("Color Intensities", 3, -1.0f, 1.0f)] 
		public Vector4 LightIntensities = new Vector4(1.0f, 0.5f, 0.25f, 0.0f);

		public bool Smooth;
		public bool isRealTime;

		private bool isFirstPass = true;

		void OnEnable() {
			isFirstPass = true;
			base.InitLightSource(pointLightCountProperty);
		}

		void OnDisable() {
			base.ReleaseLightSource(pointLightCountProperty);
		}

		private float GetSmoothness() {
			return Smooth ? 1.0f : 0.0f;
		}

		protected override void UpdatedId(int newId, int oldId)
		{
			SetLighting();
			ResetSettings(oldId);
		}

		void Update() {
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
			private static Matrix4x4[] worldToModel = new Matrix4x4[MAX_LIGHTS];
			private static Vector4[] distances = new Vector4[MAX_LIGHTS];
			private static Vector4[] intensities = new Vector4[MAX_LIGHTS];
			private static float[] smoothness = new float[MAX_LIGHTS];
			private static Vector4[] color = new Vector4[MAX_LIGHTS];

			void LateUpdate() {
				if (Id != 0)
					return;

				Shader.SetGlobalMatrixArray(pointLight0WorldToModelProperty, worldToModel);
				Shader.SetGlobalVectorArray(pointLightDistancesProperty, distances);
				Shader.SetGlobalVectorArray(pointLightColorProperty, color);
				Shader.SetGlobalVectorArray(pointLightIntensitiesProperty, intensities);
				Shader.SetGlobalFloatArray(pointLightSmoothnessProperty, smoothness);
			}
		#endif

		void SetLighting() {
			#if UNITY_5_4_OR_NEWER
				worldToModel [Id] = transform.worldToLocalMatrix;
				distances [Id] = LightDistances * Range;
				color [Id] = LightColor;
				intensities [Id] = LightIntensities;
				smoothness [Id] = GetSmoothness ();
			#else
				string idStr = Id.ToString();
				Shader.SetGlobalMatrix(pointLight0WorldToModelProperty + idStr, transform.worldToLocalMatrix);
				Shader.SetGlobalVector(pointLightDistancesProperty + idStr, LightDistances * Range);
				Shader.SetGlobalColor(pointLightColorProperty + idStr, LightColor);
				Shader.SetGlobalVector(pointLightIntensitiesProperty + idStr, LightIntensities);
				Shader.SetGlobalFloat(pointLightSmoothnessProperty + idStr, GetSmoothness());
			#endif
		}

		static void ResetSettings(int id)
		{
			#if UNITY_5_4_OR_NEWER
				worldToModel[id] = Matrix4x4.zero;
				distances[id] = Vector4.zero;
				intensities[id] = Vector4.zero;
				smoothness[id] = 0;
				color[id] = Vector4.zero;
			#else
				string idStr = id.ToString();
				Shader.SetGlobalMatrix(pointLight0WorldToModelProperty + idStr, Matrix4x4.zero);
				Shader.SetGlobalVector(pointLightDistancesProperty + idStr, Vector4.zero);
				Shader.SetGlobalColor(pointLightColorProperty + idStr, Vector4.zero);
				Shader.SetGlobalVector(pointLightIntensitiesProperty + idStr, Vector4.zero);
				Shader.SetGlobalFloat(pointLightSmoothnessProperty + idStr, 0);
			#endif
		}

		void OnDrawGizmosSelected() {
			DrawSelectedGizmo();
			DrawPointLightSphere();
		}

		private void DrawPointLightSphere() {
			Color colorWidget = Color.yellow;
			colorWidget.a = 0.5f;
			Gizmos.color = colorWidget;

			Gizmos.DrawWireSphere(transform.position, Range);
		}
	}
}
