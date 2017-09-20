//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.VR;
using Ximmerse.InputSystem;

/// <summary>
/// Displays the control points of camera frustum,and draws a play areas for tracker device.
/// </summary>
public class PlayAreaHelper:MonoBehaviour{

	#region Statics

	/// <summary>
	/// Taken from SteamVR_Frustum.UpdateModel() in SteamVR SDK.
	/// </summary>
	public static void UpdatePoints(float fovLeft,float fovRight,float fovTop,float fovBottom,float nearZ,float  farZ,Transform[] points,int start){
		fovLeft = Mathf.Clamp(fovLeft, 1, 89.5f);
		fovRight = Mathf.Clamp(fovRight, 1, 89.5f);
		fovTop = Mathf.Clamp(fovTop, 1, 89.5f);
		fovBottom = Mathf.Clamp(fovBottom, 1, 89.5f);
		farZ = Mathf.Max(farZ, nearZ + 0.01f);
		nearZ = Mathf.Clamp(nearZ, 0.01f, farZ - 0.01f);

		float lsin = Mathf.Sin(-fovLeft * Mathf.Deg2Rad);
		float rsin = Mathf.Sin(fovRight * Mathf.Deg2Rad);
		float tsin = Mathf.Sin(fovTop * Mathf.Deg2Rad);
		float bsin = Mathf.Sin(-fovBottom * Mathf.Deg2Rad);

		float lcos = Mathf.Cos(-fovLeft * Mathf.Deg2Rad);
		float rcos = Mathf.Cos(fovRight * Mathf.Deg2Rad);
		float tcos = Mathf.Cos(fovTop * Mathf.Deg2Rad);
		float bcos = Mathf.Cos(-fovBottom * Mathf.Deg2Rad);

		//float corners = new Vector3[] {
		points[start++].localPosition=new  Vector3(lsin * nearZ / lcos, tsin * nearZ / tcos, nearZ); //tln
		points[start++].localPosition=new  Vector3(rsin * nearZ / rcos, tsin * nearZ / tcos, nearZ); //trn
		points[start++].localPosition=new  Vector3(lsin * nearZ / lcos, bsin * nearZ / bcos, nearZ); //bln
		points[start++].localPosition=new  Vector3(rsin * nearZ / rcos, bsin * nearZ / bcos, nearZ); //brn
		points[start++].localPosition=new  Vector3(lsin * farZ  / lcos, tsin * farZ  / tcos, farZ ); //tlf
		points[start++].localPosition=new  Vector3(rsin * farZ  / rcos, tsin * farZ  / tcos, farZ ); //trf
		points[start++].localPosition=new  Vector3(lsin * farZ  / lcos, bsin * farZ  / bcos, farZ ); //blf
		points[start++].localPosition=new  Vector3(rsin * farZ  / rcos, bsin * farZ  / bcos, farZ ); //brf
		//};
	}

	#endregion Statics

	#region Nested Types
	
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundaryTestResult{
		// Returns true if the queried test would violate and/or trigger the tested boundary types.
		public bool isTriggering;
		//
		public int cornerId0;
		public int cornerId1;
		// Returns the distance between the queried test object and the closest tested boundary type.
		public float distance;
		// Returns the closest point to the queried test object.
		public Vector3 point;
		// Returns the normal of the closest point to the queried test object.
		public Vector3 normal;
	};

	internal static class NativeMethods {
		const string pluginName=XDevicePlugin.LIB_XDEVICE;

		[DllImport(pluginName,CallingConvention=CallingConvention.Cdecl)]
		public static extern int XDeviceTestNode(int which,int node,ref BoundaryTestResult result);
	}

	#endregion Nested Types

	#region Fields
	
	
	[Header("Frustum")]
	[Range(1f,179f)]
	public float fovX=90f;
	[Range(1f,179f)]
	public float fovY=90f;
	public float nearZ=0.5f, farZ=2.5f;
	/*
	public Vector2 orthographicSize;
	*/
	
	[Header("Misc")]
	public float warningDistance=1.0f;
	public Transform[] controlPoints=new Transform[0];
    public bool showCameraModel = true;
    [SerializeField]protected GameObject m_cameraModel;
	public bool autoCreatePlayArea=false;
	[SerializeField]protected PlayAreaRenderer m_PlayArea;
	[SerializeField]protected Vector3[] m_Corners=new Vector3[0];

	[Header("XDevice")]
	public string trackerName;
	[System.NonSerialized]protected int m_TrackerHandle;
	public bool showOnTrackingLost=true;
	public int[] trackedNodes=new int[4]{0,1,2,3};
	
	[System.NonSerialized]protected Transform m_Transform;
	[System.NonSerialized]protected bool m_ReadyForModel=false;
	[System.NonSerialized]protected int m_GroundAlpha,m_WallAlpha,m_PlaneAlpha;
	/*
	[System.NonSerialized]protected float m_CosFovX,m_CosFovY,m_SqrNearZ,m_SqrFarZ;
	[System.NonSerialized]protected Vector2 m_SqrOrthographicSize;
	*/
	[System.NonSerialized]protected Vector3[] m_CachedCorners=null;

	#endregion Fields

	#region Unity Messages

	protected virtual void Start() {
		m_Transform=transform;
		//
		m_TrackerHandle=-1;
		if(showOnTrackingLost) {
			XDevicePlugin.Init();
			m_TrackerHandle=XDevicePlugin.GetInputDeviceHandle(trackerName);
			if(m_TrackerHandle<0) {
				showOnTrackingLost=false;
			}
		}
		//
		m_ReadyForModel=(controlPoints.Length==8);
		// Cache values.
		/*
		m_CosFovX=Mathf.Cos(Mathf.Deg2Rad*fovX*.5f);
		m_CosFovY=Mathf.Cos(Mathf.Deg2Rad*fovY*.5f);
		m_SqrNearZ=nearZ*nearZ;
		m_SqrFarZ=farZ*farZ;

		m_SqrOrthographicSize=orthographicSize*.5f;
		m_SqrOrthographicSize=Vector2.Scale(m_SqrOrthographicSize,m_SqrOrthographicSize);
		*/
		//
		if(m_Corners!=null&&m_Corners.Length!=0) {
			m_CachedCorners=m_Corners;
		}
		m_Corners=null;
		//
		m_GroundAlpha=PlayerPrefsEx.GetInt("PlayArea.drawGround",1);
		m_WallAlpha=PlayerPrefsEx.GetInt("PlayArea.drawWall",1);
		m_PlaneAlpha=PlayerPrefsEx.GetInt("PlayArea.drawPlane",1);
		autoCreatePlayArea=PlayerPrefsEx.GetBool("PlayArea.enabled",autoCreatePlayArea);
        showCameraModel=PlayerPrefsEx.GetBool("PlayArea.showCameraModel",showCameraModel);
        if (m_cameraModel!=null){
            m_cameraModel.SetActive(showCameraModel);
        }
        warningDistance=PlayerPrefsEx.GetFloat("PlayArea.warningDistance",0.5f);
		//
		if(autoCreatePlayArea) {
			Transform t=null;
            Transform trackingSpaceAnchor = VRContext.GetAnchor(VRNode.TrackingSpace);

			if(m_PlayArea==null) {
				if(trackingSpaceAnchor!=null) {
					m_PlayArea =trackingSpaceAnchor.GetComponentInChildren<PlayAreaRenderer>();
				}
				if(m_PlayArea==null) {
					GameObject go=PlayerPrefsEx.GetObject("PlayAreaRenderer") as GameObject;
					m_PlayArea=Instantiate(go).GetComponent<PlayAreaRenderer>();
					m_PlayArea.name=go.name;
				}
			}
			m_PlayArea.handedness=-1;
			//
			t=m_PlayArea.transform;
            t.SetParent(trackingSpaceAnchor);
			t.localPosition=(VRContext.trackingOrigin==TrackingOrigin.FloorLevel) ? Vector3.zero :
				new Vector3(0f,-XDevicePlugin.GetFloat(m_TrackerHandle,XDevicePlugin.kField_TrackerHeightFloat,0f),0f);
			t.localRotation=Quaternion.identity;
			t.localScale=Vector3.one;
		}
		//
		UpdateModel();
	}

	protected virtual void Update() {
		if(showOnTrackingLost) {
			CheckBounds();
		}
	}

	protected virtual void OnDestroy() {
	}

#if UNITY_EDITOR

	[ContextMenu("Get Control Points")]
	protected virtual void GetControlPoints() {
		if(UnityEditor.Selection.activeGameObject!=null) {
			Transform t=UnityEditor.Selection.activeGameObject.transform;
			int i=t.childCount;
			controlPoints=new Transform[i];
			while(i-->0) {
				controlPoints[i]=t.GetChild(i);
			}
		}
	}

#endif

	#endregion Unity Messages

	#region Methods

	public virtual Vector3 TransformPoint(Vector3 point) {
		point=m_Transform.TransformPoint(point);
		//
		Transform trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
		if(trackingSpace!=null) {
			return trackingSpace.InverseTransformPoint(point);
		}
		return point;
	}

	public virtual Vector3 TransformPoint(Transform point) {
		Transform trackingSpace=VRContext.GetAnchor(VRNode.TrackingSpace);
		if(trackingSpace!=null) {
			return trackingSpace.InverseTransformPoint(point.position);
		}
		return point.position;
	}

	public virtual void SetNumCorners(int num) {
		if(m_Corners==null||m_Corners.Length!=num) {
			m_Corners=new Vector3[num];
			//
			float y=m_PlayArea==null?0.0f:m_PlayArea.transform.localPosition.y;
		}
	}

	public virtual void UpdateModel() {
		if(!m_ReadyForModel) {
			return;
		}
		//
		UpdatePoints(fovX*.5f,fovX*.5f,fovY*.5f,fovY*.5f,nearZ,farZ,controlPoints,0);
		//
		if(m_CachedCorners!=null) {
			int i=m_CachedCorners.Length;
			SetNumCorners(i);
			while(i-->0) {
				m_Corners[i]=TransformPoint(m_CachedCorners[i]);
				m_Corners[i].y=0.01f;
			}
		}else {
			SetNumCorners(4);
			m_Corners[0]=Vector3.Lerp(TransformPoint(controlPoints[0]),TransformPoint(controlPoints[2]),.5f);
			m_Corners[1]=Vector3.Lerp(TransformPoint(controlPoints[4]),TransformPoint(controlPoints[6]),.5f);
			m_Corners[2]=Vector3.Lerp(TransformPoint(controlPoints[5]),TransformPoint(controlPoints[7]),.5f);
			m_Corners[3]=Vector3.Lerp(TransformPoint(controlPoints[1]),TransformPoint(controlPoints[3]),.5f);
		}
		//
		if(m_PlayArea!=null) {
			m_PlayArea.corners=m_Corners;
			m_PlayArea.wallAlpha=.25f;
			m_PlayArea.cameraPos=m_PlayArea.transform.InverseTransformPoint(m_Transform.position);
			m_PlayArea.cameraFovY=Vector2.one*m_Transform.localRotation.eulerAngles.x+new Vector2(-fovY*.5f,fovY*.5f);
			//m_PlayArea.BuildMesh();
		}

	}
	
	public virtual void OnTrackerRecenter() {
		if(m_PlayArea!=null) {
			Transform t=m_PlayArea.transform;
			float yaw=180.0f+m_Transform.localRotation.eulerAngles.y;
			t.localRotation=Quaternion.Euler(0.0f,yaw,0.0f);
			Vector3 newPos=m_Transform.localPosition-t.localRotation*new Vector3(0.0f,0.0f,XDevicePlugin.GetFloat(m_TrackerHandle,XDevicePlugin.kField_TrackerDepthFloat,0f));
			newPos.y=t.localPosition.y;
			t.localPosition=newPos;
		}
		UpdateModel();
	}

	public virtual void CheckBounds() {
		//
		if(m_TrackerHandle==-1||
			XDevicePlugin.GetInt(m_TrackerHandle,XDevicePlugin.kField_ConnectionStateInt,0)!=(int)DeviceConnectionState.Connected
		){
			return;
		}
		//
		BoundaryTestResult hitInfo=new BoundaryTestResult();
		Vector3 position=Vector3.zero;
		float minDistance=float.MaxValue;
		int intRet;
		for(int i=0,imax=trackedNodes.Length;i<imax;++i) {
			intRet=NativeMethods.XDeviceTestNode(m_TrackerHandle,trackedNodes[i],ref hitInfo);
			if(intRet>=0) {
				if(intRet==0){// False => outside
					if(hitInfo.distance!=float.MaxValue&&hitInfo.distance>0.0f) {
						hitInfo.distance*=-1;
					}
				}
				if(hitInfo.distance<minDistance) {
					minDistance=hitInfo.distance;
				}
			}
		}  
		if(m_PlayArea!=null) {
			float alpha=1.0f-Mathf.Clamp01(minDistance/warningDistance);
			//
			m_PlayArea.groundAlpha=m_GroundAlpha;
			m_PlayArea.wallAlpha=alpha*m_WallAlpha;
			m_PlayArea.planeAlpha=alpha*m_PlaneAlpha;
		}
	}

	#endregion Methods

}
