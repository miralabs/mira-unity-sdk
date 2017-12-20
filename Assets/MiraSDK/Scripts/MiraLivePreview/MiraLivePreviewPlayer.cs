// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils;
using Wikitude;
using UnityEngine.UI;
using System;

namespace UnityEngine.XR.iOS {
	/// <summary>
	/// Controls the player side of the editor-player connection for the Mira Live Preview app.
	/// This gets run on the device itself, and sends data to the editor
	/// </summary>
	public class MiraLivePreviewPlayer : MonoBehaviour {
		PlayerConnection playerConnection;
		bool bSessionActive;
		int editorID;

        Texture2D liveViewScreenTex;
		bool bTexturesInitialized;
		private bool isTracking;
		// Is the user input necessary at all?

		private static MiraBTRemoteInput m_userInput;

		// Subscribe to remote controller connected// disconnected events
		void OnEnable()
		{
			RemoteManager.Instance.OnRemoteConnected += RemoteConnected;
			RemoteManager.Instance.OnRemoteDisconnected += RemoteDisconnected;
		}
		void OnDisable()
		{
			RemoteManager.Instance.OnRemoteConnected -= RemoteConnected;
			RemoteManager.Instance.OnRemoteDisconnected -= RemoteDisconnected;
		}
		void Start() {
			m_userInput = new MiraBTRemoteInput();

            bSessionActive = false;
            bTexturesInitialized = false;
			InitializeTrackers();

			Debug.Log("STARTING ConnectToEditor");
			editorID = -1;
			playerConnection = PlayerConnection.instance;
			playerConnection.RegisterConnection(EditorConnected);
			playerConnection.RegisterDisconnection(EditorDisconnected);
			playerConnection.Register(MiraConnectionMessageIds.fromEditorMiraSessionMsgId, HandleEditorMessage);
			//playerConnection.Register(MiraConnectionMessageIds.screenCaptureJPEGMsgID, ReceiveJPEGFrame);
		}

		void OnGUI() {
			if (!bSessionActive) {	
				GUI.Box(new Rect((Screen.width / 2) - 200, (Screen.height / 2), 400, 50), "Waiting for editor connection...");
			}
		}

		void HandleEditorMessage(MessageEventArgs mea) {
			var message = mea.data.Deserialize<serializableFromEditorMessage>();
			
			if (message.subMessageId == MiraSubMessageIds.editorInitMiraRemote) {
				// Debug.Log("Editor connected at: " + Encoding.UTF8.GetString(message.bytes));
				bool isRotational = Convert.ToBoolean(message.bytes[0]);
				Debug.Log("Is rotational only: " + isRotational);
				// Deactivate or Activate WikiCam depending on Rotational Only Mode
				GetComponent<MiraLivePreviewWikiConfig>().RotationalOnlyMode(isRotational);
            	InitializeLivePreview();
			}
			else if (message.subMessageId == MiraSubMessageIds.editorDisconnect)
			{
				EditorDisconnected(editorID);
			}
		
			else { // Assuming it's a JPEG frame
				// Debug.Log("JPG Received");
				ReceiveJPEGFrame(message);
			}
		}

		void InitializeLivePreview() {
			#if !UNITY_EDITOR

			Application.targetFrameRate = 60;
            bSessionActive = true;
            InitializeTextures(transform.GetComponent<Camera>());

			#endif
		}

		void RemoteConnected(Remote remote, EventArgs args)
		{
			// Makes sure controller is initialized properly (connects the found remote to m_userInput)
			m_userInput.init();
		}
		void RemoteDisconnected(Remote remote, EventArgs args)
		{
		}


        void InitializeTextures(Camera camera) {
			int yWidth = camera.pixelWidth;
			int yHeight = camera.pixelHeight;
			
			if (liveViewScreenTex == null) {
				if (liveViewScreenTex) Destroy(liveViewScreenTex);

				liveViewScreenTex = new Texture2D(yWidth, yHeight, TextureFormat.RGB24, false, true);
                Debug.Log("GenTex: " + yWidth + " " + yHeight);
				camera.GetComponent<MiraARVideo>().m_clearTexture = liveViewScreenTex;
			}

			bTexturesInitialized = true;
		}

		void InitializeTrackers()
		{
			ImageTrackable[] trackers = GameObject.FindObjectsOfType<ImageTrackable>();
			// If it's rotational only mode, don't initialize anything
			if(trackers.Length > 0){
				foreach (ImageTrackable thisTracker in trackers)
				{
					thisTracker.OnImageRecognized.AddListener(OnTrackingFound);
					thisTracker.OnImageLost.AddListener(OnTrackingLost);
				}
			}
		}

        void ReceiveJPEGFrame(MessageEventArgs mea) {
            // Only load the image if the textures are initialized
			if (!bTexturesInitialized) return;

			liveViewScreenTex.LoadImage(mea.data);
		}

		void ReceiveJPEGFrame(serializableFromEditorMessage message) {
            // Only load the image if the textures are initialized
			if (!bTexturesInitialized) return;

			liveViewScreenTex.LoadImage(message.bytes);
		}

        void Update() {
            if (bSessionActive) 
			{
                UpdateGyro(Input.gyro);

				UpdateBTRemote();
				if(isTracking)
					UpdateWikiCam();
            }
        }

		// void LateUpdate() 
		// {
		// 	if (bSessionActive)
		// 	{
		// 		// Update the remote so that all events are called
		// 		// m_userInput.UpdateLastFrameButtonData();

		// 	}
			
		// }

        void UpdateGyro(Gyroscope gyro) {
            serializableGyroscope sGyro = gyro;
            SendToEditor(MiraConnectionMessageIds.gyroMsgId, sGyro);
        }

		void UpdateWikiCam() 
		{
			serializableTransform sWikiCam = Camera.main.transform;
			SendToEditor(MiraConnectionMessageIds.wikiCamMsgId, sWikiCam);
		}

		public void OnTrackingFound(ImageTarget imgTarget)
		{
			serializableFloat targetHeight = imgTarget.PhysicalTargetHeight;
			isTracking = true;
			SendToEditor(MiraConnectionMessageIds.trackingFoundMsgId, targetHeight);
			// Debug.Log("Tracking Found with Img Height: " + imgTarget.PhysicalTargetHeight);

		}

		public void OnTrackingLost(ImageTarget imgTarget)
		{
			isTracking = false;
			SendToEditor(MiraConnectionMessageIds.trackingLostMsgId, true);
		}

		int btFrameCounter = 0;
		// How often should se send controller button and touchpad data?
		int btSendRate = 3;

		void UpdateBTRemote() {
			if(RemoteManager.Instance.connectedRemote != null)
			{
				serializableBTRemote sBTRemote = RemoteManager.Instance.connectedRemote;
				SendToEditor(MiraConnectionMessageIds.BTRemoteMsgId, sBTRemote);

				if(btFrameCounter < btSendRate)
					btFrameCounter += 1;
				else
				{
					serializableBTRemoteButtons sBTRemoteButtons = RemoteManager.Instance.connectedRemote;
					serializableBTRemoteTouchPad sBTRemoteTouchPad = RemoteManager.Instance.connectedRemote;

					SendToEditor(MiraConnectionMessageIds.BTRemoteButtonsMsgId, sBTRemoteButtons);
					SendToEditor(MiraConnectionMessageIds.BTRemoteTouchPadMsgId, sBTRemoteTouchPad);
					
					btFrameCounter = 0;

				}
				
			
			}
			
			
		}

		void EditorConnected(int playerID)
		{
			Debug.Log("Connected to Editor");
			editorID = playerID;

		}

		void EditorDisconnected(int playerID) {
			// if (editorID == playerID) {
			// 	editorID = -1;
			// }
            Debug.Log("Editor has been disconnected");

			DisconnectFromEditor();
			#if !UNITY_EDITOR
			if (bSessionActive) {
				bSessionActive = false;
			}
			#endif
		}


		public void SendToEditor(System.Guid msgId, object serializableObject) {
			byte[] arrayToSend = serializableObject.SerializeToByteArray();
			SendToEditor(msgId, arrayToSend);
		}

		public void SendToEditor(System.Guid msgId, byte[] data) {
			if (playerConnection.isConnected) {
				playerConnection.Send(msgId, data);
			}
		}

		public void DisconnectFromEditor() {
			#if UNITY_2017_1_OR_NEWER
			Debug.Log("Disconnect from editor succeeded");		
			// playerConnection.DisconnectAll();
			#endif
		}
	}
}
