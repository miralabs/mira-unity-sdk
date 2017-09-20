//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Ximmerse.InputSystem;

namespace Ximmerse.UI {

	/// <summary>
	///  A BaseRaycaster which can be attached to any GameObject.
	/// <para>NOTE:It hacks GraphicRaycaster,so using it may cause some issues.</para>
	/// </summary>
	public class VRRaycaster:GraphicRaycaster{

		#region Static

		public static Dictionary<string,VRRaycaster> s_InstanceMap=new Dictionary<string,VRRaycaster>();
		public static List<Canvas> s_AllCanvases=new List<Canvas>();

		public static VRRaycaster TryRaycastAll(string name,PointerEventData eventData,List<RaycastResult> raycastResults){
			raycastResults.Clear();
			VRRaycaster raycaster=null;
			if(s_InstanceMap.TryGetValue(name,out raycaster)) {
				int i=0,imax=s_AllCanvases.Count;
				if(imax==0) {
					s_AllCanvases.AddRange(FindObjectsOfType<Canvas>());
					s_AllCanvases.RemoveAll((x)=>{
						BaseRaycaster br=x.GetComponent<BaseRaycaster>();
						return br==null||br is VRRaycaster;
					});
					imax=s_AllCanvases.Count;
				}
				if(imax==0) {
					raycaster.PhysicsRaycast(eventData,raycastResults);
				}else {
					for(;i<imax;++i) {
						raycaster.SetCanvas(s_AllCanvases[i]);
						raycaster.Raycast(eventData,raycastResults);
					}
				}
			}else{
				//Log.w("VRRaycaster","No such a VRRaycaster(name="+name+") at TryRaycastAll().");
			}
			return raycaster;
		}

		#endregion Static

		#region Fields

		/// <summary>
		/// 
		/// </summary>
		public ControllerType controller=ControllerType.None;
		
		public Transform laserLine;

		public bool usePhysics;
		public LayerMask eventMask;

		/// <summary>
		/// Cache for eventCamera.
		/// </summary>
		[System.NonSerialized]protected Camera m_EventCamera;
		[System.NonSerialized]protected Transform m_Transform;

		[System.NonSerialized]protected System.Reflection.FieldInfo m_CanvasRef;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected override void Awake() {
			base.Awake();
			// Firstly,we need to disable the Canvas which attached to this GameObject.
			GetComponent<Canvas>().enabled=false;
			m_Transform=transform;
			// Assign the controller type automatically.
			if(controller==ControllerType.None) {
				Transform t=transform;
				TrackedObject obj=null;
				while(t!=null) {
					obj=t.GetComponent<TrackedObject>();
					if(obj!=null) {
						controller=obj.source;
						break;
					}
					//
					t=t.parent;
				}
				if(controller==ControllerType.None) {
					controller=ControllerType.Hmd;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void Start() {
			base.Start();
			// Finally,register it into s_InstanceMap.
			string key=controller.ToString();
			if(s_InstanceMap.ContainsKey(key)) {
				s_InstanceMap[key]=this;
			}else {
				s_InstanceMap.Add(key,this);
			}
		}
		
		protected override void OnDestroy() {
			base.OnDestroy();
			string key=controller.ToString();
			if(s_InstanceMap.ContainsKey(key)) {
				s_InstanceMap.Remove(key);
			}
		}

		protected override void OnDisable() {
		}

		protected override void OnEnable() {
		}

		#endregion Unity Messages

		#region Methods

		public virtual void SetLaserLineDepth(float value) {
			if(laserLine!=null) {
				Vector3 s=laserLine.localScale;
				s.z=value;
				laserLine.localScale=s;
			}
		}

		public override void Raycast(PointerEventData eventData,List<RaycastResult> resultAppendList) {
			base.Raycast(eventData,resultAppendList);
			if(usePhysics) {
				PhysicsRaycast(eventData,resultAppendList);
			}
		}

		public virtual void PhysicsRaycast(PointerEventData eventData,List<RaycastResult> resultAppendList) {
			if(this.eventCamera!=null) {
				Ray ray=this.eventCamera.ScreenPointToRay((Vector3)eventData.position);
				float maxDistance=this.eventCamera.farClipPlane-this.eventCamera.nearClipPlane;
				RaycastHit[] array=Physics.RaycastAll(ray,maxDistance,this.finalEventMask);
				if(array.Length>1) {
					System.Array.Sort<RaycastHit>(array,(r1,r2)=>(r1.distance.CompareTo(r2.distance)));
				}
				if(array.Length!=0) {
					int index=0;
					int length=array.Length;
					while(index<length) {
						RaycastResult result2=new RaycastResult();
						result2.gameObject=array[index].collider.gameObject;
						result2.module=this;
						result2.distance=array[index].distance;
						result2.worldPosition=array[index].point;
						result2.worldNormal=array[index].normal;
#if UNITY_5&&!UNITY_5_0&&!UNITY_5_1
						result2.screenPosition=eventData.position;
#endif
						result2.index=resultAppendList.Count;
						result2.sortingLayer=0;
						result2.sortingOrder=0;
						RaycastResult item=result2;
						resultAppendList.Add(item);
						index++;
					}
				}
			}
		}

		#endregion Methods

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public override Camera eventCamera {
			get {
				if(m_EventCamera==null) {
					m_EventCamera=GetComponent<Camera>();
					if(m_EventCamera==null) {
						m_EventCamera=gameObject.AddComponent<Camera>();
						//
#if UNITY_5&&(!(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0))
						m_EventCamera.stereoTargetEye=StereoTargetEyeMask.None;
#endif
						m_EventCamera.cullingMask=(LayerMask)0;
						m_EventCamera.enabled=false;
						//Transform t=m_EventCamera.transform;
					}
				}
				if(m_EventCamera==null) {
					return Camera.main;
				}
				return m_EventCamera;
			}
		}

		public int finalEventMask {
			get {
				return ((this.eventCamera==null)?-1:(/*this.eventCamera.cullingMask&*/(int)this.eventMask));
			}
		}

		public void SetCanvas(Canvas canvas) {
			if(m_CanvasRef==null) {
				System.Type clazz=typeof(GraphicRaycaster);
				m_CanvasRef=clazz.GetField("m_Canvas",(System.Reflection.BindingFlags)(-1));
				if(m_CanvasRef==null) {
					return;
				}
			}
			m_CanvasRef.SetValue(this,canvas);
		}

		#endregion Properties

	}

}