//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using Ximmerse.InputSystem;

public class DeviceStatus:MonoBehaviour{
	
	#region Nested Types

	[System.Serializable]
	public class DeviceUI{
		public string deviceName;
		[Header("Connection State")]
		public GameObject[] connectUI=new GameObject[5];
		[Header("Battery")]
		public Image battImage;
		public Text battText;
		public Sprite[] battSprites=new Sprite[4];

		[System.NonSerialized]public int handle;
		[System.NonSerialized]public DeviceConnectionState connectionState=(DeviceConnectionState)(-1);

		public void Awake() {
			handle=XDevicePlugin.GetInputDeviceHandle(deviceName);
		}

		public void Update() {
			if(handle<0){return;}
			//
			XDevicePlugin.UpdateInputState(handle);
			//
			DeviceConnectionState s=(DeviceConnectionState)XDevicePlugin.
				GetInt(handle,XDevicePlugin.kField_ConnectionStateInt,0);
			if(connectionState!=s) {
				connectionState=s;
				//
				for(int i=0,imax=connectUI.Length;i<imax;++i) {
					if(connectUI[i]!=null) {
						connectUI[i].SetActive(i==(int)s);
					}
				}
			}
			//
			int b=XDevicePlugin.
				GetInt(handle,XDevicePlugin.kField_BatteryLevelInt,0);
			if(battImage!=null) {
				//battImage.sprite=battSprites[(int)s];
			}
			if(battText!=null) {
				battText.text=(b==-1)?"?":(b+"%");
			}
		}
	}

	#endregion Nested Types
	
	#region Fields

	public bool alwaysShowUI=true;
	public GameObject uiRoot;
	public DeviceUI[] devices=new DeviceUI[1]{new DeviceUI()};
	[System.NonSerialized]protected Ximmerse.UI.UIFade m_FadeUiRoot;

	#endregion Fields

	#region Unity Messages

	protected virtual void Awake() {
		m_FadeUiRoot=uiRoot.GetComponent<Ximmerse.UI.UIFade>();
		//
		XDevicePlugin.Init();
		for(int i=0,imax=devices.Length;i<imax;++i) {
			devices[i].Awake();
		}
	}

	protected virtual void Update() {
		bool showUI=false;
		for(int i=0,imax=devices.Length;i<imax;++i) {
			devices[i].Update();
			//
			if(devices[i].connectionState!=DeviceConnectionState.Connected) {
				showUI=true;
			}
		}
		if(showUI||alwaysShowUI) {
			ShowUI(true);
		}else {
			ShowUI(false);
		}
	}

	//protected virtual void OnDestroy() {
	//	XDevicePlugin.Exit();
	//}

	#endregion Unity Messages

	#region For Editor

#if UNITY_EDITOR

	[ContextMenu("Merge All Children")]
	protected virtual void MergeAllChildren() {
		DeviceStatus[] children=GetComponentsInChildren<DeviceStatus>();
		devices=new DeviceUI[0];
		for(int i=0,imax=children.Length;i<imax;++i) {
			if(children[i]!=this) {
				children[i].enabled=false;
				UnityEditor.ArrayUtility.AddRange<DeviceStatus.DeviceUI>(ref devices,children[i].devices);
				DestroyImmediate(children[i]);
			}
		}
	}

#endif

	#endregion For Editor

	#region Methods

	public virtual void ShowUI(bool value) {
		if(uiRoot!=null) {
			if(m_FadeUiRoot!=null) {
				m_FadeUiRoot.Play(value);
			}else {
				uiRoot.SetActive(value);
			}
		}
	}

	#endregion Methods

}