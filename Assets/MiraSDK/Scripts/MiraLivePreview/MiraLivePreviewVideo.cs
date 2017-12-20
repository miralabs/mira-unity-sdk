// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using UnityEngine;
using System.Collections;
using Mira;
using Utils;


namespace UnityEngine.XR.iOS
{
	/// <summary>
	/// Sends images of the screen from the editor to the Live Preview app player
	/// </summary>
	public class MiraLivePreviewVideo : MonoBehaviour {
#if UNITY_EDITOR
		public MiraLivePreviewEditor remoteConnection;

		[Range(1,100)]
		[Tooltip("Quality/size of the streamed image")]
		/// <summary>
		/// Quality/size of the streamed image
		/// </summary>
		public int m_jpegQuality = 50;
		[Range(1,30)]
		[Tooltip("How frequently an image is sent in frames")]
		/// <summary>
		/// How frequently an image is sent in frames
		/// </summary>
		public int m_captureInterval = 3;
		private bool bCapturingFrame = false;
		private bool bCapturedFrameAvailable = false;

		private Texture2D _capturedTex;
		private bool bTexturesInitialized = false;

		private Rect _captureRect;
		private Camera _dcLeft;
        private Camera _dcRight;
		private bool _dcLeftRendered;
		private bool _dcRightRendered;

		private int screenWidth;
		private int screenHeight;

		void Start() {
			// Save screen dimensions
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			if (DistortionCamera.Instance != null) {
				_dcLeft = DistortionCamera.Instance.dcLeft;
				_dcRight = DistortionCamera.Instance.dcRight;
			}
		}

		void Update() {
			if (!bTexturesInitialized || screenWidth != Screen.width || screenHeight != Screen.height) {
				screenWidth = Screen.width;
				screenHeight = Screen.height;

				if (_capturedTex) Destroy(_capturedTex);

				_captureRect = new Rect(0, 0, screenWidth, screenHeight);

				// Create new texture
				_capturedTex = new Texture2D(
					screenWidth, screenHeight,
					TextureFormat.RGB24,
					false,
					QualitySettings.activeColorSpace == ColorSpace.Linear
				);
				bTexturesInitialized = true;
			}

			if (Time.frameCount % m_captureInterval == 0) bCapturingFrame = true;
			//StartCoroutine("MiraRemotePostRender");
			if (bCapturedFrameAvailable && remoteConnection.SessionActive) {
				serializableFromEditorMessage sfem = new serializableFromEditorMessage();
				sfem.subMessageId = MiraSubMessageIds.screenCaptureJPEGMsgID;
				sfem.bytes = _capturedTex.EncodeToJPG(m_jpegQuality);
				remoteConnection.SendToPlayer(MiraConnectionMessageIds.fromEditorMiraSessionMsgId, sfem);
				// Replace above with this for Unity 2017.3
				//remoteConnection.SendToPlayer(MiraConnectionMessageIds.screenCaptureJPEGMsgID, _capturedTex.EncodeToJPG(m_jpegQuality));

				bCapturedFrameAvailable = false;
			}
		}
		

		public void OnEnable() {
			Camera.onPostRender += MiraRemotePostRender;
		}

		public void OnDisable() {
			Camera.onPostRender -= MiraRemotePostRender;
		}

		void MiraRemotePostRender(Camera currentCamera) {
			if (bCapturingFrame) {
				if (currentCamera == _dcLeft) _dcLeftRendered = true;
				if (currentCamera == _dcRight) _dcRightRendered = true;
				
				if (_dcLeftRendered && _dcRightRendered) {
					_capturedTex.ReadPixels(_captureRect, 0, 0);
					_capturedTex.Apply();

					bCapturingFrame = false;
					bCapturedFrameAvailable = true;
					
					_dcLeftRendered = _dcRightRendered = false;
				}
			}
		}
#endif
	}
}

