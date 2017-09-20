//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;
using Ximmerse.InputSystem;

/// <summary>
/// This class is a virtual represnetation of the controller in real world. 
/// </summary>
public class TrackedObject:MonoBehaviour {
	
	#region Fields

	/// <summary>
	/// The transform controlled by tracking data.
	/// </summary>
	public Transform target;
	
	public ControllerType source;
	public string sourceName;

	public bool trackPosition=true;
	public bool trackRotation=true;

	public bool checkParent=false;
	public bool canRecenter=true;

	[System.NonSerialized]protected ControllerInput m_ControllerInput;
	public ControllerInput controllerInput{get{return m_ControllerInput;}}
	[System.NonSerialized]protected TrackingResult m_PrevTrackingResult;

	#endregion Fields

	#region Unity Messages

	protected virtual void Awake() {
		if(target==null) {target=transform;}
	}

	protected virtual void Start() {
		//
		if(checkParent) {
			Transform p=VRContext.GetAnchor(VRNode.TrackingSpace);
			if(p!=null) {
				target.SetParent(p,true);
			}
		}
		//
		if(source==ControllerType.None&&string.IsNullOrEmpty(sourceName)) {
			if(name.ToLower().IndexOf("left")!=-1) {
				source=ControllerType.LeftController;
			}else if(name.ToLower().IndexOf("right")!=-1) {
				source=ControllerType.RightController;
			}
		}
		if(source!=ControllerType.None) {
			m_ControllerInput=ControllerInputManager.instance.GetControllerInput(source);
		}else if(!string.IsNullOrEmpty(sourceName)) {
			m_ControllerInput=ControllerInputManager.instance.GetControllerInput(sourceName);
		}
		//
		VRNode node=VRNode.None;
		switch(source) {
			case ControllerType.LeftController:
				node=VRNode.LeftHand;
			break;
			case ControllerType.RightController:
				node=VRNode.RightHand;
			break;
		}
		if(node!=VRNode.None) {
			VRContext.SetAnchor(node,target);
		}
		//
		if(m_ControllerInput!=null&&canRecenter) {
			// Like SteamVR,hmd and controllers don't need to reset yaw angle.
			//canRecenter=!m_ControllerInput.isAbsRotation;
		}
		//
		if(canRecenter){
			// Invoke Recenter() on VRContext recenter event.
			VRContext ctx=VRContext.main;
			if(ctx!=null){
				ctx.onRecenter+=Recenter;
			}
		}
		//
		return;
	}

	protected virtual void Update() {
		UpdateTransform();
	}

	#endregion Unity Messages

	#region Methods

	/// <summary>
	/// Get transform informations.
	/// </summary>
	public virtual void GetTransform(ref Vector3 position, ref Quaternion rotation) {
        //
		if(m_ControllerInput!=null) {
			position=m_ControllerInput.GetPosition();
			rotation=m_ControllerInput.GetRotation();
		}
	}
	
	/// <summary>
	/// Update target's transform. This should be called in every frame. 
	/// </summary>
	public virtual void UpdateTransform(float factor=1.0f){
		if(m_ControllerInput==null) {
			return;
		}
		//
		Vector3 position=Vector3.zero;
		Quaternion rotation=Quaternion.identity;
		GetTransform(ref position,ref rotation);
		//
		if(trackPosition) {
			if((m_ControllerInput.trackingResult&TrackingResult.PositionTracked)==0) {
			} else if((target.localPosition-position).sqrMagnitude==0.0f) {
				//Ximmerse.Log.i("TrackedObject","target.localPosition==position");
			}else{
				if(factor==1.0f//||
					// Abort lerp if lost position tracking at previous frame.
					//(m_PrevTrackingResult&TrackingResult.PositionTracked)==0
				) {
					target.localPosition=position;
				}else {
					target.localPosition=Vector3.Lerp(target.localPosition,position,factor);
				}
            }
        }
		if(trackRotation) {
			if(factor==1.0f) {
				target.localRotation=rotation;
			}else {
				target.localRotation=Quaternion.Slerp(target.localRotation,rotation,factor);
			}
		}
		// Cache trackingResult at this frame.
		m_PrevTrackingResult=m_ControllerInput.trackingResult;
	}

	/// <summary>
	/// Resets the yaw of this object.
	/// </summary>
	public virtual void Recenter() {
		if(m_ControllerInput!=null) {
			m_ControllerInput.Recenter();
		}
	}

	#endregion Methods

}