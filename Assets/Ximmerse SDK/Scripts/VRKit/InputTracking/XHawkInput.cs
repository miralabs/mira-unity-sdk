//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;
//using MyEmulatedHand=Ximmerse.InputSystem.ArmModel;
using MyEmulatedHand=Ximmerse.InputSystem.EmulatedHand;

namespace Ximmerse.InputSystem {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkInput:TrackerInput {

		#region Nested Types

		[System.Serializable]public class StringIntPair:UKeyValuePair<string,int>{}
#if UNITY_EDITOR
		[UnityEditor.CustomPropertyDrawer(typeof(StringIntPair))]public class StringIntPairDrawer:UKeyValuePairDrawer<string,int>{}
#endif

		#endregion Nested Types

		#region Fields

		[SerializeField]protected bool m_EnableEmulatedHand=true;
		[SerializeField]protected MyEmulatedHand[] m_EmulatedHands=new MyEmulatedHand[2] {
			new MyEmulatedHand("LeftController",ControllerType.LeftController),
			new MyEmulatedHand("RightController",ControllerType.RightController),
		};

		[SerializeField]protected StringIntPair[] m_Controllers=new StringIntPair[3]{
			new StringIntPair{key="XCobra-0",value=0},
			new StringIntPair{key="XCobra-1",value=1},
			new StringIntPair{key="VRDevice",value=2}
		};

		[System.NonSerialized]public TrackedControllerInput[] controllers;
		[System.NonSerialized]protected bool m_IsRequestVR;
		[System.NonSerialized]protected ControllerInput m_HmdInput;

		#endregion Fields

		#region Messages

		public override int InitInput() {
			// Change device name forcely.
			deviceName="XHawk-0";
			//
			int ret=base.InitInput();
			if(ret==0) {
				return InitInternal()?ret:-1;
			}
			return ret;
		}

		#endregion Messages
		
		#region Methods

		/// <summary>
		/// Init X-Hawk for different user environments.
		/// </summary>
		public virtual bool InitInternal() {
			//
			XDevicePlugin.SetInt(m_Handle,XDevicePlugin.kField_TrackingOriginInt,(int)VRContext.trackingOrigin);
			XDevicePlugin.SendMessage(m_Handle,XDevicePlugin.kMessage_RecenterSensor,0,0);
			//
			if(true){
				int i=0,imax=m_Controllers.Length;
				controllers=new TrackedControllerInput[imax];
				//
				ControllerInputManager mgr=ControllerInputManager.instance;
				ControllerInput ci;
				if(mgr!=null) {
					for(;i<imax;++i) {
						ci=mgr.GetControllerInput(m_Controllers[i].key);
						if(ci is TrackedControllerInput) {
							controllers[i]=ci as TrackedControllerInput;
							controllers[i].inputTracking=this;
							controllers[i].node=m_Controllers[i].value;
						}else {
							controllers[i]=CreateControllerInput(m_Controllers[i].key,this,m_Controllers[i].value);
							if(controllers[i].handle!=-1) {
							mgr.AddControllerInput(controllers[i].name,controllers[i]);
							}else {
								controllers[i]=null;
							}
						}
						//
						if(i<2) {
							m_EmulatedHands[i].Controller=controllers[i];
							m_EmulatedHands[i].followGaze=ArmModel.GazeBehavior.Always;
							m_EmulatedHands[i].Start();
						}
					}
				}
			}
			// VR Context
			m_HmdInput=ControllerInputManager.GetInput(ControllerType.Hmd);
			  // VRContext must have a CenterEyeAnchor at least.
			m_IsRequestVR=(VRContext.GetAnchor(VRNode.CenterEye)!=null);
			EnsureAnchor();
			//
			Log.i("XHawkInput","Initialize successfully.");
			//
			return true;
		}

		public override void UpdateState() {
			if(m_Handle>=0) {
			if(Time.frameCount!=m_PrevFrameCount){
			int t=m_PrevFrameCount;
			m_PrevFrameCount=Time.frameCount;
			if(EnsureAnchor()) {
				//
				UpdateAnchor();
				m_PrevFrameCount=t;
				base.UpdateState();
				//
				if(m_EnableEmulatedHand) {
					for(int i=0,imax=m_EmulatedHands.Length;i<imax;++i) {
						if(m_EmulatedHands[i].handedness!=ControllerType.None) {
							m_EmulatedHands[i].OnControllerUpdate();
							if(m_IsRequestVR) {
								m_EmulatedHands[i].showGizmos=(XDevicePlugin.GetNodePosition(m_Handle,0,i,null)&TrackingResult.PositionTracked)==0;
							}
						}
					}
				}
			}}}
		}

		public virtual TrackedControllerInput CreateControllerInput(string name,TrackerInput trackerInput,int defaultNode) {
			return new TrackedControllerInput(name,trackerInput,defaultNode);
		}

		/// <summary>
		/// We will lose the VR context,when reloading level.
		/// Calling this function per frame can ensure that the anchor is alive.
		/// </summary>
		public virtual bool EnsureAnchor() {
			// <!-- TODO: VR Legacy Mode. -->
			// If the X-Hawk isn't connected,the game will run as legacy VR mode(Gets input events with GearVR touchpad).
			if(XDevicePlugin.GetInt(m_Handle,XDevicePlugin.kField_ConnectionStateInt,0)!=(int)DeviceConnectionState.Connected) {
				if(m_HmdInput!=null) XDevicePlugin.SetInt(m_HmdInput.handle,XDevicePlugin.kField_ConnectionStateInt,(int)DeviceConnectionState.Disconnected);
				return false;
			}
			//
			if(trackingSpace==null) {
				trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
			}
			//
			if(anchor==null) {
				Transform centerEyeAnchor=VRContext.GetAnchor(VRNode.CenterEye);
				if(m_IsRequestVR&&centerEyeAnchor==null) {
					return false;
				}else {
					CreateAnchor();
					//
					if(m_IsRequestVR) {
						VRContext.main.onRecenter-=Recenter;
						VRContext.main.onRecenter+=Recenter;
					}
				}
			}
			return true;
		}

		public virtual bool CreateAnchor() {
			return true;
		}

		public virtual void UpdateAnchor() {
			return;
		}

		public override void Recenter() {
			return;
		}

		public virtual void RecenterAllControllers(float finalYaw=0.0f) {
			for(int i=0,imax=2;i<imax;++i) {
				if(controllers[i].name!="VRDevice") {
					bool b=controllers[i].isAbsRotation;
					controllers[i].isAbsRotation=false;
						controllers[i].Recenter(finalYaw);
					controllers[i].isAbsRotation=b;
				}
			}
		}

		public virtual System.Collections.IEnumerator RecenterAllControllersDelayed(float delay,float finalYaw=0.0f) {
			yield return new WaitForSeconds(delay);
			 RecenterAllControllers(finalYaw);
		}

		public virtual ControllerInput GetControllerInput(int node) {
			if(controllers==null) {
				return null;
			}
			//
			for(int i=0,imax=controllers.Length;i<imax;++i) {
				if(controllers[i]!=null){
				if(controllers[i].node==node) {
					return controllers[i];
				}}
			}
			//
			return null;
		}

		public virtual MyEmulatedHand GetEmulatedHand(int node) {
			MyEmulatedHand eh=null;
			for(int i=0,imax=m_EmulatedHands.Length;i<imax;++i) {
				if(m_EmulatedHands[i].Controller.node==node) {
					eh=m_EmulatedHands[i];
					break;
				}
			}
			return eh;
		}

		public override bool Emulates(int node) {
			if(m_EnableEmulatedHand) {
				MyEmulatedHand eh=GetEmulatedHand(node);
				//
				if(eh!=null) {
					if(!base.Exists(node)) {
						return eh.Exists(node);
					}
				}
			}
			//
			return false;
		}

		public override bool Exists(int node) {
			bool result=base.Exists(node);
			if(!result&&m_EnableEmulatedHand) {
				MyEmulatedHand eh=GetEmulatedHand(node);
				//
				if(eh!=null) {
					return eh.Exists(0);
				}
			}
			//
			return result;
		}

		public override Quaternion GetLocalRotation(int node) {
			UpdateState();
			//
			ControllerInput input=GetControllerInput(node);
			if(input==null) {
				return Quaternion.identity;
			}else {
				return input.GetRotation();
			}
		}

		public virtual Vector3 GetEmulatedHandPosition(int node) {
			MyEmulatedHand eh=GetEmulatedHand(node);
			//
			if(eh!=null) {
				return eh.GetLocalPosition((int)ArmModelNode.Pointer);
			}else {
				return Vector3.zero;
			}
		}

		#endregion Methods

	}
}
