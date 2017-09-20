//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;
using Ximmerse.InputSystem;

/// <summary>
/// For controlling in-game "head" with inside-out tracking or outside-in tracking.
/// </summary>
public class TrackedHead:TrackedObject {

	#region Fields

	[Header("Head")]

	/// <summary>
	/// TODO : VR
	/// </summary>
	public Transform eyeContainer;
	
	/// <summary>
	/// The transform of the mark object.
	/// </summary>
	public Transform markTransform;

	[SerializeField]protected GameObject[] m_Gizmos=new GameObject[0];

	#endregion Fields

	#region Unity Messages

    protected override void Awake() {
//#if !UNITY_EDITOR
		SetGizmosActive(false);
//#endif
		//
		source=ControllerType.Hmd;
		trackRotation=false;
		base.Awake();
	}

	protected virtual void OnVRContextInited(VRContext context) {
		//
		if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0)&0xF000)!=0x4000) {
			Destroy(this);
			Ximmerse.Log.w("TrackedHead","TrackedHead only works in Outside-in!!!");
			return;
		}else {
			VRDevice vrDevice=context.vrDevice;
			if(vrDevice.outsideInMarkPose.position!=Vector3.zero) {
				markTransform.localPosition=vrDevice.outsideInMarkPose.position;
			}
		}
		//
		if(eyeContainer==null) {
			eyeContainer=new GameObject("Rotate-Pivot").transform;
			eyeContainer.SetParent(transform);
			//
			eyeContainer.localPosition=Vector3.zero;
			eyeContainer.localRotation=Quaternion.identity;
			eyeContainer.localScale=Vector3.one;
			//
			if(context.vrDevice!=null&&context.vrDevice.family!="Dummy") {
				markTransform.localPosition=markTransform.localPosition+context.vrDevice.neckToEye;
				eyeContainer.localPosition=context.vrDevice.neckToEye;
#if UNITY_EDITOR
				// Editor features.
				if(m_Gizmos.Length>0&&m_Gizmos[0]!=null){
					m_Gizmos[0].transform.localPosition=m_Gizmos[0].transform.localPosition+context.vrDevice.neckToEye;
				}
#endif

			}
		}
		//
		for(int i=0;i<3;++i) {
			Transform eye=context.GetAnchor(VRNode.LeftEye+i,null);
			if(eye!=null) {
				eye.SetParent(eyeContainer,false);
			}
		}
		//
		switch(PlayerPrefsEx.GetInt("XimmerseDevice.type",0)) {
			// No head tracking.
			case 0x1010:
				source=ControllerType.None;
				m_ControllerInput=null;
			break;
		}
	}

		
#if UNITY_EDITOR

	protected virtual void UpdateGizmos() {
		GameObject[] gos=UnityEditor.Selection.gameObjects;
		if(gos.Length>=1) {
			GameObject go=gos[0];
			for(int i=0,imax=m_Gizmos.Length;i<imax;++i) {
				if(m_Gizmos[i]==go) {
					SetGizmosActive(true);
					return;
				}
			}
			SetGizmosActive(go==gameObject);
		}else {
			SetGizmosActive(false);
		}
	}

	protected override void Update() {
		if(Application.isPlaying) {
			if(Camera.current!=null
#if UNITY_5&&!UNITY_5_0&&!UNITY_5_1
				&&Camera.current.cameraType==CameraType.SceneView
#endif
			) {
				UpdateGizmos();
			}else {
				//SetGizmosActive(false);
			}
		}
		base.Update();
	}

	protected virtual void OnDrawGizmos() {
		if(!Application.isPlaying) {
			UpdateGizmos();
		}
	}

	protected virtual void OnDrawGizmosSelected() {
		if(!Application.isPlaying) {
			SetGizmosActive(true);
		}
	}

#endif

	#endregion Unity Messages

	#region Methods

	public virtual void SetGizmosActive(bool value) {
		for(int i=0,imax=m_Gizmos.Length;i<imax;++i) {
			if(m_Gizmos[i]!=null&&m_Gizmos[i].activeSelf!=value) {
				m_Gizmos[i].SetActive(value);
			}
		}
	}

	public override void UpdateTransform(float factor) {
		if(m_ControllerInput==null) {
			return;
		}
		//
		if((m_ControllerInput.trackingResult&TrackingResult.PositionTracked)==0) {
			return;
		}
		//
		Vector3 position=m_ControllerInput.GetPosition();
		if(factor==1.0f) {
			target.localPosition=position;
		}else {
			target.localPosition=Vector3.Lerp(target.localPosition,position,factor);
		}
    }
	
	#endregion Methods

}
