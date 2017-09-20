using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public class EmulatedHand:IInputTracking {

		public static readonly Vector3 NECK_TO_EYE=new Vector3(0.0f,0.075f,0.0805f);
		public static readonly Vector3 DEFAULT_SHOULDER_RIGHT = new Vector3(0.19f, -0.19f, -0.03f);

		public string name;
		public ControllerType handedness;
		public TrackedControllerInput Controller;
		[System.NonSerialized]public Vector3 neckToShoulder;
		[System.NonSerialized]public Transform centerEye;
		[System.NonSerialized]public Transform trackingSpace;
		[System.NonSerialized]public Transform unityHelper;
		[System.NonSerialized]public Transform pointer;

		public EmulatedHand(string name,ControllerType handedness) {
			this.name=name;
			this.handedness=handedness;
			//
			neckToShoulder=DEFAULT_SHOULDER_RIGHT;

			switch(handedness) {
				case ControllerType.LeftController:
					neckToShoulder.x=-Mathf.Abs(neckToShoulder.x);
				break;
				case ControllerType.RightController:
					neckToShoulder.x=+Mathf.Abs(neckToShoulder.x);
				break;
			}
		}

		public virtual void Start() {
		}

		public virtual void OnControllerUpdate() {
			if(centerEye==null) {
				centerEye=VRContext.GetAnchor(VRNode.CenterEye);
				trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
			}
			if(unityHelper==null) {
				unityHelper=new GameObject("UnityHelper-"+handedness).transform;
				pointer=new GameObject("Pointer-"+handedness).transform;
				//
				unityHelper.SetParent(trackingSpace);
				unityHelper.localPosition=Vector3.zero;
				unityHelper.localRotation=Quaternion.identity;
				unityHelper.localScale=Vector3.one;
				pointer.SetParent(unityHelper);
				pointer.localPosition=Vector3.zero;
				pointer.localRotation=Quaternion.identity;
				pointer.localScale=Vector3.one;
			}
			if(centerEye!=null) {
				//
				unityHelper.position=centerEye.position//@Eye
					-centerEye.rotation*NECK_TO_EYE//@Neck
					+Quaternion.AngleAxis(centerEye.rotation.eulerAngles.y,Vector3.up)*neckToShoulder;//@Shoulder
				//
				Quaternion rotation=Quaternion.LookRotation(Controller.GetRotation()*Vector3.forward,Vector3.up);
				if(trackingSpace!=null) {
					unityHelper.rotation=trackingSpace.rotation*rotation;
				}else {
					unityHelper.rotation=rotation;
				}
				//
				if((XDevicePlugin.GetInt(Controller.handle,XDevicePlugin.kField_TrackingResultInt,0)&(int)TrackingResult.PositionTracked)==0) {
				}else {
					if(trackingSpace==null) {
						pointer.position=Controller.GetPosition();
					}else {
						pointer.position=trackingSpace.TransformPoint(Controller.GetPosition());
					}
				}
			}
		}

		public virtual bool Emulates(int node) {
			return true;
		}

		public virtual bool Exists(int node) {
			if(Controller!=null&&pointer!=null) {
				return XDevicePlugin.GetInt(Controller.handle,XDevicePlugin.kField_ConnectionStateInt,0)==(int)DeviceConnectionState.Connected&&
					pointer.localPosition.sqrMagnitude>0.0f;
			}
			return false;
		}

		public virtual Vector3 GetLocalPosition(int node) {
			if(pointer==null) {
				return Vector3.zero;
			}
			//
			if(trackingSpace==null) {
				return pointer.position;
			}else {
				return trackingSpace.InverseTransformPoint(pointer.position);
			}
		}

		public virtual Quaternion GetLocalRotation(int node) {
			return Controller.GetRotation();
		}

		public ArmModel.GazeBehavior followGaze = ArmModel.GazeBehavior.DuringMotion;

		public virtual bool showGizmos {
			get;
			set;
		}
	}

}