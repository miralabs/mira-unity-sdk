/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using System;
using UnityEngine;

namespace FlatLighting {
	[AddComponentMenu("Flat Lighting/Shadow Projector", 4)]
	[ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class ShadowProjector : LightSource<ShadowProjector> {

		private static readonly string shadowProjectorCountProperty = "_ShadowProjectors_Length";
		private static readonly string shadowMapMatrixProperty = "_ShadowMapMat";
		private static readonly string shadowModelToViewProperty = "_ShadowMapMV";
		private static readonly string shadowColorProperty = "_ShadowColor";
		private static readonly string shadowBlurProperty = "_ShadowBlur";
		private static readonly string shadowCameraSettingsProperty = "_ShadowCameraSettings";
		private static readonly string shadowTextureProperty = "_ShadowMapTexture";
		private static readonly string DEBUG_SHADOW_TEXTURE_OBJECT_NAME = "DebugShadowMapping";

		public Color ShadowColor = Color.black;
		private Shader ShadowMapShader;
		[Range(0f, 3f)]
		public float Bias = 0.2f;
		[Range(0f, 10f)]
		public float ShadowBlur = 1.2f;
		[RangeWithStep(1024, 4096, 1024)]
		public int MemoryToUse = 1024;
		public bool isRealTime;
		public bool DebugShadowMappingTexture = false;

		private bool isFirstPass = true;

		private GameObject debugObject;
		private Material debugMaterial;

		private Camera ShadowCamera;
		private bool ShadowMapRequested;
		private Matrix4x4 ShadowMapOffset = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f);
		private RenderTexture ShadowMapTexture;

		void Start() {
			ShadowMapShader = Shader.Find("Hidden/ShadowDepth");
			if (ShadowMapShader == null) {
				Debug.LogError("Could not find a shader \"Hidden/ShadowDepth\" for rendering objects depth");
			}

			InitShadowMappingCamera();

			#if UNITY_EDITOR
				LateUpdate();
			#endif
		}

		public void InitShadowMappingCamera() {
			if (ShadowCamera == null) {
				ShadowCamera = GetComponent<Camera>();
			}
		}

		private void OnEnable() {
			ShadowProjector.MAX_LIGHTS = 5;
			isFirstPass = true;
			base.InitLightSource(shadowProjectorCountProperty);
			RequestShadowMap();
		}

		protected override void UpdatedId(int newId, int oldId)
		{
			ProjectShadows();
			DisplayShadowMappingTextureOnDebug();

			ResetSettings(oldId);
		}

		void OnApplicationFocus(bool focusStatus) {
			if (focusStatus && enabled) {
				RequestShadowMap();
			}
		}

		private void RequestShadowMap() {
			ShadowMapRequested = true;
			GenerateShadowMapTexture();
		}

		private void LateUpdate() {
			#if UNITY_EDITOR
			InitShadowMappingCamera();
			if (!Application.isPlaying || ShadowMapRequested || isRealTime || !isRealTime && isFirstPass) {
			#else
			if (ShadowMapRequested || isRealTime || !isRealTime && isFirstPass) {
			#endif

				ShadowCamera.nearClipPlane = 0f;
				ShadowCamera.renderingPath = RenderingPath.VertexLit;
				ShadowCamera.enabled = false;
				ShadowCamera.orthographic = true;
				ShadowCamera.targetTexture = ShadowMapTexture;
				ShadowCamera.aspect = 1f;
				ShadowCamera.depthTextureMode = DepthTextureMode.None;
				ShadowCamera.RenderWithShader(ShadowMapShader, "RenderType");
				ShadowCamera.targetTexture = null;
				ShadowMapRequested = false;

				ProjectShadows();

				DisplayShadowMappingTextureOnDebug();

				isFirstPass = false;
			}
		}

		private void DisplayShadowMappingTextureOnDebug() {
			if (!DebugShadowMappingTexture) {
				RemoveDebugObjects ();
				return;
			}

			FindOrCreateDebugObject ();
			SetupDebugMaterial ();
		}

		void RemoveDebugObjects() {
			if (debugObject != null) {
				DestroyImmediate (debugObject);
			}

			foreach (Transform childObject in transform) {
				if (childObject.name == DEBUG_SHADOW_TEXTURE_OBJECT_NAME) {
					DestroyImmediate (childObject.gameObject);
				}
			}
		}

		void FindOrCreateDebugObject() {
			foreach (Transform childObject in transform) {
				if (childObject.name == DEBUG_SHADOW_TEXTURE_OBJECT_NAME) {
					debugObject = childObject.gameObject;
					break;
				}
			}

			if (debugObject == null) {
				debugObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
				debugObject.name = DEBUG_SHADOW_TEXTURE_OBJECT_NAME;
				debugObject.transform.parent = transform;
				debugObject.transform.position = transform.position + 10 * Vector3.up;;
				debugObject.transform.rotation = Quaternion.Euler(Vector3.back);
				debugObject.transform.localScale *= 10;

				if (debugMaterial != null) {
					debugObject.GetComponent<Renderer> ().material = debugMaterial;
				}
			}
		}

		void SetupDebugMaterial() {
			if (debugMaterial == null) {
				debugMaterial = new Material (Shader.Find ("Unlit/Texture"));
				debugObject.GetComponent<Renderer> ().material = debugMaterial;
			}

			debugMaterial.mainTexture = ShadowMapTexture;
		}

		#if UNITY_5_4_OR_NEWER
			private static Matrix4x4[] mapMatrix = new Matrix4x4[MAX_LIGHTS];
			private static Matrix4x4[] modelToView = new Matrix4x4[MAX_LIGHTS];
			private static Vector4[] cameraSettings = new Vector4[MAX_LIGHTS];
			private static float[] blur = new float[MAX_LIGHTS];
			private static Vector4[] shadowColor = new Vector4[MAX_LIGHTS];

//			private void GenerateShadowMapTextureArray() {
//				int shadowMapResolution = GetShadowMapResolution();
//				if (shadowTextureArray == null || shadowTextureArray.width != shadowMapResolution) {
//					if (SystemInfo.supports2DArrayTextures && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
//						shadowTextureArray = new Texture2DArray(shadowMapResolution, shadowMapResolution, 24, RenderTextureFormat.Depth, false);
//					} else {
//						Debug.LogWarning("Depth textures or 2d texture array are not supported on this platform and currently there is no support for shadows without them.");
//						return;
//					}
//				}
//			}

			private void ApplyShadows() {
				Shader.SetGlobalTexture(shadowTextureProperty + Id.ToString(), ShadowMapTexture);

				if (Id != 0) {
					return;
				}

				Shader.SetGlobalMatrixArray(shadowMapMatrixProperty, mapMatrix);
				Shader.SetGlobalMatrixArray(shadowModelToViewProperty, modelToView);
				Shader.SetGlobalVectorArray(shadowCameraSettingsProperty, cameraSettings);
				Shader.SetGlobalFloatArray(shadowBlurProperty, blur);
				Shader.SetGlobalVectorArray(shadowColorProperty, shadowColor);
			}
		#endif

		private void ProjectShadows() {
			Matrix4x4 shadowMapMatrix = ShadowMapOffset * ShadowCamera.projectionMatrix * ShadowCamera.worldToCameraMatrix;
			Vector2 singleCameraSettings = new Vector2(this.ShadowCamera.farClipPlane + Bias, ShadowCamera.nearClipPlane);

			#if UNITY_5_4_OR_NEWER

				mapMatrix[Id] = shadowMapMatrix;
				modelToView [Id] = ShadowCamera.worldToCameraMatrix;
				cameraSettings [Id] = singleCameraSettings;
				blur [Id] = ShadowBlur * 0.0001f;
				shadowColor [Id] = ShadowColor;

				ApplyShadows();
			#else
				Shader.SetGlobalMatrix(shadowMapMatrixProperty + Id.ToString(), shadowMapMatrix);
				Shader.SetGlobalMatrix(shadowModelToViewProperty + Id.ToString(), ShadowCamera.worldToCameraMatrix);
				Shader.SetGlobalVector(shadowCameraSettingsProperty + Id.ToString(), singleCameraSettings);
				Shader.SetGlobalFloat(shadowBlurProperty + Id.ToString(), ShadowBlur * 0.0001f);
				Shader.SetGlobalColor(shadowColorProperty + Id.ToString(), ShadowColor);
				Shader.SetGlobalTexture(shadowTextureProperty + Id.ToString(), ShadowMapTexture);
			#endif
		}

		static void ResetSettings(int id)
		{
			#if UNITY_5_4_OR_NEWER
				mapMatrix[id] = Matrix4x4.zero;
				modelToView[id] = Matrix4x4.zero;
				cameraSettings[id] = Vector4.zero;
				blur[id] = 0;
				shadowColor[id] = Vector4.zero;
			#else
				Shader.SetGlobalMatrix(shadowMapMatrixProperty + id.ToString(), Matrix4x4.zero);
				Shader.SetGlobalMatrix(shadowModelToViewProperty + id.ToString(), Matrix4x4.zero);
				Shader.SetGlobalVector(shadowCameraSettingsProperty + id.ToString(), Vector4.zer);
				Shader.SetGlobalFloat(shadowBlurProperty + id.ToString(), 0);
				Shader.SetGlobalColor(shadowColorProperty + id.ToString(), Vector4.zer);
			#endif
		}

		private int GetShadowMapResolution() {
			int size = MemoryToUse;

			if (SystemInfo.systemMemorySize < MemoryToUse) {
				if (SystemInfo.systemMemorySize >= 8192)
				{
					size = 4096;
				} else if (SystemInfo.systemMemorySize >= 2048)
				{
					size = 2048;
				} else
				{
					size = 1024;
				}
			}
				
			if (SystemInfo.maxTextureSize <= size) {
				size = SystemInfo.maxTextureSize;
			}

			return size;
		}

		private void GenerateShadowMapTexture() {
			int shadowMapResolution = GetShadowMapResolution();
			if (ShadowMapTexture == null || ShadowMapTexture.width != shadowMapResolution) {
				if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
					ClearShadowMap();
					ShadowMapTexture = new RenderTexture(shadowMapResolution, shadowMapResolution, 24, RenderTextureFormat.Depth);
				} else {
					Debug.LogWarning("Depth textures are not supported on this platform and currently there is no support for shadows without them.");
					return;
				}
				ShadowMapTexture.autoGenerateMips = false;
				ShadowMapTexture.useMipMap = false;
				ShadowMapTexture.Create();
				ShadowMapTexture.hideFlags = HideFlags.DontSave;
			}
		}

		void OnDisable() {
			base.ReleaseLightSource(shadowProjectorCountProperty);
			ClearShadowMap();
		}

		private void ClearShadowMap() {
			if (ShadowMapTexture != null)
				ShadowMapTexture.Release();
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.25f);

			Gizmos.color = Color.blue;
			float lineLength = 5.0f;
			Vector3 lightDirection = transform.forward * lineLength;
			Gizmos.DrawRay(transform.position, lightDirection);
		}
	}
}
