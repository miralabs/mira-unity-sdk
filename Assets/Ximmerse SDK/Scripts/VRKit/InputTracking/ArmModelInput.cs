//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.VR;

namespace Ximmerse.InputSystem {

	public enum ArmModelNode {
		Shoulder,
		Elbow,
		Wrist,
		Pointer,
		Count,
	}

	[System.Serializable]
	public partial class ArmModel:IInputTracking{

		public string name;
		public TrackedControllerInput Controller;
		public ArmModelNode defaultNode=ArmModelNode.Wrist;
		public ControllerType handedness;

		public ArmModel(string name,ControllerType handedness) {
			this.name=name;
			this.handedness=handedness;
			OnArmModelUpdate+=InternalOnArmModelUpdate;
		}

		public virtual Vector3 GetHeadOrientation() {
			Transform centerEye=VRContext.GetAnchor(VRNode.CenterEye);
			Transform trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
			if(trackingSpace==null) {
				return centerEye.forward;
			}else {
				return trackingSpace.InverseTransformDirection(centerEye.forward);
			}
		}

		public virtual bool IsControllerRecentered() {
			// TODO : 
			return false;
		}

		public virtual bool Emulates(int node) {
			return true;
		}

		public virtual bool Exists(int node) {
			if(Controller!=null) {
				return XDevicePlugin.GetInt(Controller.handle,XDevicePlugin.kField_ConnectionStateInt,0)==(int)DeviceConnectionState.Connected;
				//return Controller.connectionState==DeviceConnectionState.Connected;
			}
			return false;
		}

		public virtual Vector3 GetLocalPosition(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			Transform head=VRContext.GetAnchor(VRNode.Head);
			Vector3 headPosition=(head==null)?Vector3.zero:head.localPosition;
			//
			switch((ArmModelNode)node) {
				case ArmModelNode.Pointer:return headPosition+pointerPosition;
				case ArmModelNode.Wrist:return headPosition+wristPosition;
				case ArmModelNode.Elbow:return headPosition+elbowPosition;
				case ArmModelNode.Shoulder:return headPosition+shoulderPosition;
			}
			//
			return Vector3.zero;
		}

		public virtual Quaternion GetLocalRotation(int node) {
			if(node==-1) {
				node=(int)defaultNode;
			}
			//
			switch((ArmModelNode)node) {
				case ArmModelNode.Pointer:return pointerRotation;
				case ArmModelNode.Wrist:return wristRotation;
				case ArmModelNode.Elbow:return elbowRotation;
				case ArmModelNode.Shoulder:return shoulderRotation;
			}
			//
			return Quaternion.identity;
		}
		
		public virtual bool showGizmos {
			get {
				return m_ShowGizmos;
			}
			set {
				m_ShowGizmos=value;
				//
				if(m_Gizmos!=null) {
					m_Gizmos.SetActive(m_ShowGizmos);
				}
			}
		}

		[SerializeField]protected Material[] m_Materials=new Material[2];
		[System.NonSerialized]protected bool m_ShowGizmos=false;
		[System.NonSerialized]protected GameObject m_Gizmos;
		[System.NonSerialized]protected Transform[] m_Nodes=new Transform[4];
		[System.NonSerialized]protected Transform[] m_Links=new Transform[3];

		public virtual void InternalOnArmModelUpdate() {
			if(!m_ShowGizmos) {
				return;
			}
			//
			if(m_Gizmos==null) {
				m_Gizmos=new GameObject("ArmModel-"+handedness);
				Transform parent=m_Gizmos.transform;
				parent.SetParent(VRContext.GetAnchor(VRNode.TrackingSpace));
				parent.localPosition=Vector3.zero;
				parent.localRotation=Quaternion.identity;
				parent.localScale=Vector3.one;
				//
				if(m_Materials[0]==null) {
					m_Materials[0]=new Material(Shader.Find("Unlit/Color"));
					m_Materials[0].color=Color.green;
				}
				if(m_Materials[1]==null) {
					m_Materials[1]=new Material(Shader.Find("Unlit/Color"));
					m_Materials[1].color=Color.yellow;
				}
				//
				for(int i=0,imax=(int)ArmModelNode.Count;i<imax;++i) {
					m_Nodes[i]=GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
					m_Nodes[i].SetParent(parent);
					m_Nodes[i].localScale=Vector3.one*0.05f;
					//
					var collider=m_Nodes[i].GetComponent<Collider>();
					if(collider!=null) {
						Object.Destroy(collider);
					}
					//
					Renderer renderer=m_Nodes[i].GetComponent<Renderer>();
					if(renderer!=null) {
						renderer.sharedMaterial=m_Materials[0];
					}
				}
				for(int i=0,imax=(int)ArmModelNode.Count-1;i<imax;++i) {
					m_Links[i]=GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
					m_Links[i].SetParent(parent);
					m_Links[i].localScale=new Vector3(0.025f,0.025f,0.0f);
					//
					var collider=m_Links[i].GetComponent<Collider>();
					if(collider!=null) {
						Object.Destroy(collider);
					}
					//
					Renderer renderer=m_Links[i].GetComponent<Renderer>();
					if(renderer!=null) {
						renderer.sharedMaterial=m_Materials[1];
					}
				}
			}
			//
			for(int i=0,imax=(int)ArmModelNode.Count;i<imax;++i) {
				m_Nodes[i].localPosition=GetLocalPosition(i);
			}
			for(int i=0,imax=(int)ArmModelNode.Count-1;i<imax;++i) {
				Vector3 offset=m_Nodes[i+1].localPosition-m_Nodes[i].localPosition;
				Quaternion rotation=Quaternion.LookRotation(offset,Vector3.up);
				Vector3 scale=m_Links[i].localScale;
				if(scale.z==0.0f) {
					scale.z=offset.magnitude;
				}

				m_Links[i].localPosition=m_Nodes[i].localPosition+rotation*((scale.z*.5f)*Vector3.forward);
				m_Links[i].localRotation=rotation;
				m_Links[i].localScale=scale;
			}
		}

	}

	public class ArmModelInput:MonoBehaviour,IInputModule{

		#region Fields

		[SerializeField]protected ArmModel[] m_Controllers=new ArmModel[2]{
			// new ArmModel("LeftController",ControllerType.LeftController),
			// new ArmModel("RightController",ControllerType.RightController),
			new ArmModel("LeftController",ControllerType.None),
			new ArmModel("RightController",ControllerType.None),
		};

		#endregion Fields

		#region Messages

		public virtual int InitInput() {
			if((XDevicePlugin.GetInt(XDevicePlugin.ID_CONTEXT,XDevicePlugin.kField_CtxDeviceVersionInt,0)&0xF000)!=0x1000) {
				return -1;
			}
			// Override ControllerInput in ControllerInputManager.
			if(true){
				//
				ControllerInputManager mgr=ControllerInputManager.instance;
				ControllerInput ci;
				if(mgr!=null) {
					for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
						if(m_Controllers[i].handedness!=ControllerType.None) {
							ci=mgr.GetControllerInput(m_Controllers[i].name);
							if(ci is TrackedControllerInput) {
								m_Controllers[i].Controller=ci as TrackedControllerInput;
								m_Controllers[i].Controller.inputTracking=m_Controllers[i];
								m_Controllers[i].Controller.node=-1;
							}else {
								m_Controllers[i].Controller=new TrackedControllerInput(m_Controllers[i].name,m_Controllers[i],-1);
								if(m_Controllers[i].Controller.handle!=-1) {
								mgr.AddControllerInput(m_Controllers[i].name,m_Controllers[i].Controller);
								}else {
									m_Controllers[i].Controller=null;
								}
							}
							//
							m_Controllers[i].Start();
						}
					}
				}else {
					return -1;
				}
			}
			//
			return 0;
		}

		public virtual int UpdateInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnControllerUpdate();
				}
			}
			return 0;
		}

		public virtual int ExitInput() {
			for(int i=0,imax=m_Controllers.Length;i<imax;++i) {
				if(m_Controllers[i].handedness!=ControllerType.None) {
					m_Controllers[i].OnDestroy();
				}
			}
			return 0;
		}

		#endregion Messages

	}
}