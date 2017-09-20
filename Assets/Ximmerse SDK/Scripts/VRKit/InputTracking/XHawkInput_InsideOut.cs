//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkInput_InsideOut:XHawkInput{

		#region Fields
		
		[Header("Inside-Out")]
		public bool makeSmooth=false;
		[System.NonSerialized]protected Transform m_CenterEye;
		[System.NonSerialized]protected Vector3 m_AnchorPosition;
		[System.NonSerialized]protected Quaternion m_AnchorRotation;

		#endregion Fields

		#region Methods

		public override bool InitInternal(){
			if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0)&0xF000)!=0x3000) {
				return false;
			}
			// TODO : Read offsets from the plugin.
#if !true
			m_AnchorPosition=Vector3.zero;
			m_AnchorRotation=Quaternion.identity;
#else
			VRDevice vrDevice=VRContext.currentDevice;
			if(vrDevice!=null) {
				m_AnchorPosition=vrDevice.inOutsideMarkPose.position;
				m_AnchorRotation=vrDevice.inOutsideMarkPose.rotation;
			}else {
				m_AnchorPosition=Vector3.zero;
				m_AnchorRotation=Quaternion.identity;
			}
#endif
			//
			return base.InitInternal();
		}

		public override bool CreateAnchor() {
			if(anchor!=null) {
				return false;
			}
			//
			anchor=new GameObject().transform;
			anchor.name="TrackerAnchor(Inside-Out)";
			anchor.SetParent(trackingSpace);
			//
			return true;
		}

		public override void UpdateAnchor() {
			if(m_CenterEye==null) {
				m_CenterEye=VRContext.GetAnchor(VRNode.CenterEye);
				if(m_CenterEye==null) {
					return;
				}
			}
			//
			anchor.localPosition=m_CenterEye.localPosition+m_CenterEye.localRotation*m_AnchorPosition;
			anchor.localRotation=m_CenterEye.localRotation*m_AnchorRotation;
		}

		public override void Recenter() {
			//
			RecenterAllControllers(VRContext.GetHmdRotation().eulerAngles.y);
		}

		#endregion Methods
	}
}
