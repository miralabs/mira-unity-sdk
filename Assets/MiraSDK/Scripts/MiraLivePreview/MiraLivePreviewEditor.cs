// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils; 
using System;
using Mira;
#if UNITY_EDITOR
using UnityEditor.Networking.PlayerConnection;
#endif

namespace UnityEngine.XR.iOS {
	/// <summary>
	/// Controls the editor side of the editor-player connection for the Mira Live Preview app
	/// </summary>
	public class MiraLivePreviewEditor : MonoBehaviour {
#if UNITY_EDITOR
		/// <summary>
		/// Static instance of MiraLivePreviewEditor which allows it to be accessed by any other script.
		/// </summary>
		public static MiraLivePreviewEditor Instance = null;
		EditorConnection editorConnection;

		private bool bSessionActive;
		/// <summary>
		/// Is the session active?
		/// </summary>
		/// <returns></returns>
        public bool SessionActive { get { return bSessionActive; } }
		int currentPlayerID = -1;
		string guimessage = "none";

		private VirtualRemote vrtlRemote;

		private Quaternion m_attitude;
		/// <summary>
		/// The attitude of the gyro, being sent over from the player device
		/// </summary>
		/// <returns></returns>
		public Quaternion attitude { get { return m_attitude; } }

		private serializableTransform sWikiCam;
		
		private bool playerIsTracking = false;
		
		
		void Awake() {
            // Check if instance already exists, destroy any imposters, and keep it alive during scene changes
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

		void Start() {
			bSessionActive = false;

			editorConnection = EditorConnection.instance;
			editorConnection.Initialize();
			editorConnection.RegisterConnection(PlayerConnected);
			editorConnection.RegisterDisconnection(PlayerDisconnected);

            editorConnection.Register(MiraConnectionMessageIds.gyroMsgId, ReceiveRemoteGyroAttitude);

			editorConnection.Register(MiraConnectionMessageIds.wikiCamMsgId, ReceiveRemoteWikiCam);
			editorConnection.Register(MiraConnectionMessageIds.trackingFoundMsgId, TrackingFound);
			editorConnection.Register(MiraConnectionMessageIds.trackingLostMsgId, TrackingLost);

			editorConnection.Register(MiraConnectionMessageIds.BTRemoteMsgId, ReceiveBTRemote);
			editorConnection.Register(MiraConnectionMessageIds.BTRemoteButtonsMsgId, ReceiveBTRemoteButtons);
			editorConnection.Register(MiraConnectionMessageIds.BTRemoteTouchPadMsgId, ReceiveBTRemoteTouchPad);
			
		}

		void OnDisable()
		{
			SendDisconnectToPlayer();
		}

		void OnGUI() {
			if (!bSessionActive) {
				if (currentPlayerID != -1) {
					guimessage = "Connected to Mira Remote device: " + currentPlayerID.ToString();

					if (GUI.Button (new Rect ((Screen.width / 2) - 200, (Screen.height / 2) - 200, 400, 100), "Start Remote Mira Session")) {
						SendInitToPlayer();
					}
				} 
				else {
					guimessage = "Please connect to player in the console menu";
				}

				GUI.Box(new Rect((Screen.width / 2) - 200, (Screen.height / 2) + 100, 400, 50), guimessage);
			}
		
		}


        void PlayerConnected(int playerID) {
			currentPlayerID = playerID;
			
			
		}

		void PlayerDisconnected(int playerID) {
			if (currentPlayerID == playerID) {
				currentPlayerID = -1;
			}
		}

		public void OnEditorDisconnect() {
			
			#if UNITY_2017_1_OR_NEWER		
			Debug.Log("Disconnect from editor called from computer");
			bSessionActive = false;
			SendDisconnectToPlayer();
			editorConnection.DisconnectAll();
			#endif
		}

        void ReceiveRemoteGyroAttitude(MessageEventArgs mea) {
            m_attitude = mea.data.Deserialize<serializableGyroscope>().attitude;
        }

		void ReceiveRemoteWikiCam(MessageEventArgs mea) {
			if (playerIsTracking)
			{
				sWikiCam = mea.data.Deserialize<serializableTransform>();
				MiraWikitudeManager.Instance.MiraRemoteTracking(sWikiCam.position, sWikiCam.rotation);
			} 
		}
		void TrackingFound(MessageEventArgs mea)
		{
			
			float trackerHeight = mea.data.Deserialize<serializableFloat>().x;
			if(MiraWikitudeManager.Instance != null)
			{
				playerIsTracking = true;
				MiraWikitudeManager.Instance.RemoteTrackingFound(trackerHeight);
			}
				
		}
		void TrackingLost(MessageEventArgs mea)
		{
			
			if(MiraWikitudeManager.Instance != null)
			{
				MiraWikitudeManager.Instance.RemoteTrackingLost();
				playerIsTracking = false;

			}
				
		}

		void ReceiveBTRemote(MessageEventArgs mea) {
            if (vrtlRemote != null) vrtlRemote.UpdateVirtualMotion(mea.data.Deserialize<serializableBTRemote>());
		

        }
		void ReceiveBTRemoteButtons(MessageEventArgs mea) {
            if (vrtlRemote != null) vrtlRemote.UpdateVirtualButtons(mea.data.Deserialize<serializableBTRemoteButtons>());
        }
		void ReceiveBTRemoteTouchPad(MessageEventArgs mea) {
            if (vrtlRemote != null) vrtlRemote.UpdateVirtualTouchpad(mea.data.Deserialize<serializableBTRemoteTouchPad>());
        }

		void SendInitToPlayer() {
			serializableFromEditorMessage sfem = new serializableFromEditorMessage();
			sfem.subMessageId = MiraSubMessageIds.editorInitMiraRemote;
			// sfem.bytes = BitConverter.GetBytes(Mira.MiraArController.Instance.isRotationalOnly);
			sfem.bytes = new byte[]{Convert.ToByte(MiraArController.Instance.isRotationalOnly)};
			SendToPlayer(MiraConnectionMessageIds.fromEditorMiraSessionMsgId, sfem);
			
            bSessionActive = true;
			SetMiraControllerToVirtual();
			if(GyroController.Instance != null)
				GyroController.Instance.MiraRemoteActive();
		}

		void SendDisconnectToPlayer()
		{
			serializableFromEditorMessage sfem = new serializableFromEditorMessage();
			sfem.subMessageId = MiraSubMessageIds.editorDisconnect;
			sfem.bytes = Time.time.ToString().SerializeToByteArray();
			SendToPlayer(MiraConnectionMessageIds.fromEditorMiraSessionMsgId, sfem);
			
    
		}

		void SetMiraControllerToVirtual()
		{
			MiraController.Instance.controllerType = MiraController.ControllerType.MiraVirtual;
			MiraController.Instance.userInput.InitVirtual();
			vrtlRemote = MiraController.Instance.userInput.virtualRemote;
			// MiraController.Instance.assignController ();
		}

		public void SendToPlayer(System.Guid msgId, byte[] data) {
			editorConnection.Send(msgId, data);
		}

		public void SendToPlayer(System.Guid msgId, object serializableObject) {
			byte[] arrayToSend = serializableObject.SerializeToByteArray();
			SendToPlayer(msgId, arrayToSend);
		}


	#endif
	}
	
}
