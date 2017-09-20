/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

namespace FlatLighting {
	[AddComponentMenu("Flat Lighting/Spot Light", 3)]
	[ExecuteInEditMode]
	public class SpotLight : LightSource<SpotLight> {

		private static readonly string spotLightCountProperty = "_SpotLight_Length";
		private static readonly string spotLightWorldToModelProperty = "_SpotLightMatrixC0";
		private static readonly string spotLightForwardProperty = "_SpotLightObjectSpaceForward";
		private static readonly string spotLightColorProperty = "_SpotLightColor";
		private static readonly string spotLightBaseRadiusProperty = "_SpotLightBaseRadius";
		private static readonly string spotLightHeightProperty = "_SpotLightHeight";
		private static readonly string spotLightDistancesProperty = "_SpotLightDistances";
		private static readonly string spotLightIntensitiesProperty = "_SpotLightIntensities";
		private static readonly string spotLightSmoothnessProperty = "_SpotLightSmoothness";

		public float BaseRadius = 2.0f;
		public float Height = 4.0f;
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
			base.InitLightSource(spotLightCountProperty);
		}

		void OnDisable() {
			base.ReleaseLightSource(spotLightCountProperty);
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
		private static Vector4[] forward = new Vector4[MAX_LIGHTS];
		private static float[] baseRadius = new float[MAX_LIGHTS];
		private static float[] height = new float[MAX_LIGHTS];
		private static Vector4[] distances = new Vector4[MAX_LIGHTS];
		private static Vector4[] intensities = new Vector4[MAX_LIGHTS];
		private static float[] smoothness = new float[MAX_LIGHTS];
		private static Vector4[] color = new Vector4[MAX_LIGHTS];
		
		void LateUpdate() {				
			if (Id != 0)
				return;

			Shader.SetGlobalMatrixArray(spotLightWorldToModelProperty, worldToModel);
			Shader.SetGlobalVectorArray(spotLightForwardProperty, forward);
			Shader.SetGlobalFloatArray(spotLightBaseRadiusProperty, baseRadius);
			Shader.SetGlobalFloatArray(spotLightHeightProperty, height);
			Shader.SetGlobalVectorArray(spotLightDistancesProperty, distances);
			Shader.SetGlobalVectorArray(spotLightColorProperty, color);
			Shader.SetGlobalVectorArray(spotLightIntensitiesProperty, intensities);
			Shader.SetGlobalFloatArray(spotLightSmoothnessProperty, smoothness);
		}
	#endif

		void SetLighting() {
			#if UNITY_5_4_OR_NEWER
				worldToModel [Id] = transform.worldToLocalMatrix;
				forward [Id] = Vector3.forward;
				baseRadius [Id] = BaseRadius;
				height [Id] = Height;
				distances [Id] = LightDistances * BaseRadius;
				color [Id] = LightColor;
				intensities [Id] = LightIntensities;
				smoothness [Id] = GetSmoothness ();
			#else
				string idStr = Id.ToString();
				Shader.SetGlobalMatrix(spotLightWorldToModelProperty + idStr, transform.worldToLocalMatrix);
				Shader.SetGlobalVector(spotLightForwardProperty + idStr, Vector3.forward);
				Shader.SetGlobalFloat(spotLightBaseRadiusProperty + idStr, BaseRadius);
				Shader.SetGlobalFloat(spotLightHeightProperty + idStr, Height);
				Shader.SetGlobalVector(spotLightDistancesProperty + idStr, LightDistances * BaseRadius);
				Shader.SetGlobalColor(spotLightColorProperty + idStr, LightColor);
				Shader.SetGlobalVector(spotLightIntensitiesProperty + idStr, LightIntensities);
				Shader.SetGlobalFloat(spotLightSmoothnessProperty + idStr, GetSmoothness());
			#endif
		}

		static void ResetSettings(int id)
		{
			#if UNITY_5_4_OR_NEWER
				worldToModel[id] = Matrix4x4.zero;
				forward[id] = Vector3.zero;
				baseRadius[id] = 0;
				height[id] = 0;
				distances[id] = Vector4.zero;
				intensities[id] = Vector4.zero;
				smoothness[id] = 0;
				color[id] = Vector4.zero;
			#else
				string idStr = id.ToString();
				Shader.SetGlobalMatrix(spotLightWorldToModelProperty + idStr, transform.worldToLocalMatrix);
				Shader.SetGlobalVector(spotLightForwardProperty + idStr, Vector3.forward);
				Shader.SetGlobalFloat(spotLightBaseRadiusProperty + idStr, BaseRadius);
				Shader.SetGlobalFloat(spotLightHeightProperty + idStr, Height);
				Shader.SetGlobalVector(spotLightDistancesProperty + idStr, LightDistances * BaseRadius);
				Shader.SetGlobalColor(spotLightColorProperty + idStr, LightColor);
				Shader.SetGlobalVector(spotLightIntensitiesProperty + idStr, LightIntensities);
				Shader.SetGlobalFloat(spotLightSmoothnessProperty + idStr, GetSmoothness());
			#endif
		}

		private void DrawSpotLightConePairLines(Vector3 side) {
//			Vector3 lightDirection = transform.TransformDirection( (Vector3.forward * Height) + side );
//			Gizmos.DrawRay(transform.position, lightDirection);
			Vector3 lightDirection = (transform.forward * Height) + transform.rotation * side;
			Gizmos.DrawRay(transform.position, lightDirection);

			lightDirection = (Quaternion.AngleAxis(45.0f, transform.forward)) * lightDirection;
			Gizmos.DrawRay(transform.position, lightDirection);
		}

		private void DrawSpotLightCone() {
			Color colorWidget = Color.yellow;
			colorWidget.a = 0.5f;
			Gizmos.color = colorWidget;

			Vector3 spotDirectionX = new Vector3(BaseRadius, 0, 0);
			Vector3 spotDirectionY = new Vector3(0, BaseRadius, 0);

			DrawSpotLightConePairLines(spotDirectionX);
			DrawSpotLightConePairLines(-spotDirectionX);
			DrawSpotLightConePairLines(spotDirectionY);
			DrawSpotLightConePairLines(-spotDirectionY);
		}

		void OnDrawGizmosSelected() {
			DrawSelectedGizmo();
			DrawSpotLightCone();
		}
	}
}
