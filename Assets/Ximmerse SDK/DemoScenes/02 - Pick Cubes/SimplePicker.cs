//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;
using Ximmerse.InputSystem;

public class SimplePicker:MonoBehaviour {

	#region Nested Types

	public class PickObject {

		#region Static

		// <!-- Pool Manager

		protected static List<PickObject> s_InstancePool=new List<PickObject>();
		protected static int s_NumInstancePool=0;

		public static PickObject Pop(Transform transform) {
			PickObject obj=null;
			if(s_NumInstancePool>0) {
				obj=s_InstancePool[--s_NumInstancePool];
				s_InstancePool.RemoveAt(s_NumInstancePool);
			}else {
				obj=new PickObject();
			}
			obj.Set(transform);
			return obj;
		}

		public static void Push(PickObject obj) {
			obj.Reset();
			//int i=s_InstancePool.IndexOf(obj);
			//if(i==-1) {
			s_InstancePool.Add(obj);
			++s_NumInstancePool;
			//}
		}

		// Pool Manager -->

		#endregion Static

		#region Fields

		public Transform transform;
		public Transform parent;
		public Rigidbody rigidbody;
		public bool isKinematic;
		public bool useGravity;

		#endregion Fields

		#region Methods

		public virtual void SetParent(Transform parent) {
			if(parent==null) {
				this.transform.SetParent(this.parent,true);
				if(this.rigidbody!=null) {
					this.rigidbody.isKinematic=this.isKinematic;
					this.rigidbody.useGravity=this.useGravity;
				}
			}else {
				this.transform.SetParent(parent,true);
				if(this.rigidbody!=null) {
					this.rigidbody.isKinematic=true;
					this.rigidbody.useGravity=false;
				}
			}
		}

		public virtual void Reset() {
			this.transform=null;
			this.parent=null;
			this.rigidbody=null;
			this.isKinematic=false;
			this.useGravity=false;
		}

		public virtual void Set(Transform transform) {
			//Reset();
			//
			this.transform=transform;
			this.parent=this.transform.parent;
			if(this.transform!=null) {
				this.rigidbody=this.transform.GetComponent<Rigidbody>();
				if(this.rigidbody!=null) {
					this.isKinematic=this.rigidbody.isKinematic;
					this.useGravity=this.rigidbody.useGravity;
				}
			}
		}

		public virtual void Recycle() {
			Push(this);
		}

		#endregion Methods

	}

	#endregion Nested Types

	#region Fields

	[SerializeField]protected ControllerType m_ControllerType=ControllerType.LeftController;
	[System.NonSerialized]protected ControllerInput m_ControllerInput;
	[SerializeField]protected Transform m_Container,m_Point;
	[SerializeField]protected LayerMask m_LayerMask=Physics.DefaultRaycastLayers;
	[System.NonSerialized]public int layerMask;
	[SerializeField]protected float m_Radius=.5f;
	[SerializeField]protected int m_MaxObjects=1;
	[System.NonSerialized]protected Collider[] m_Colliders=new Collider[32];
	[System.NonSerialized]protected List<PickObject> m_PickObjects=new List<PickObject>();

	#endregion Fields

	#region Unity Messages

	protected virtual void Start() {
		if(m_Container==null){m_Container=transform;}
		if(m_Point==null){m_Point=transform;}
		layerMask=m_LayerMask.value;

		m_ControllerInput=ControllerInputManager.instance.GetControllerInput(m_ControllerType);
	}

	protected virtual void Update() {
		if(m_ControllerInput!=null) {
			if(m_ControllerInput.GetButtonDown(ControllerButton.PrimaryTrigger)) {
				int i=0,imax;
#if UNITY_5&&!UNITY_5_0&&!UNITY_5_1
				imax=Physics.OverlapSphereNonAlloc(m_Point.position,m_Radius,m_Colliders,layerMask);
#else
				m_Colliders=Physics.OverlapSphere(m_Point.position,m_Radius,layerMask);
				imax=m_Colliders.Length;
#endif
				if(imax>0) {
					if(m_MaxObjects>=0) {
						imax=Mathf.Min(imax,m_MaxObjects);
					}
					for(;i<imax;++i) {
						AddPickObject(m_Colliders[i].transform);
					}
				}
			}else if(!m_ControllerInput.GetButton(ControllerButton.PrimaryTrigger)) {
				if(m_PickObjects.Count>0) {
					ClearPickObjects();
				}
			}
		}
	}

	#region DrawGizmos

	protected virtual void OnDrawGizmos() {
		DrawGizmos(false);
    }

	protected virtual void OnDrawGizmosSelected() {
		DrawGizmos(true);
	}

	protected virtual void DrawGizmos(bool isSelected) {
		Gizmos.color=(isSelected)?Color.green:Color.gray;
		//
		Transform t=m_Point==null?transform:m_Point;
		Gizmos.DrawWireSphere(t.position,m_Radius);
	}

	#endregion DrawGizmos

	#endregion Unity Messages

	#region Methods

	public virtual void AddPickObject(Transform t) {
		if(t!=null) {
			PickObject obj=PickObject.Pop(t);
			obj.SetParent(m_Container);
			m_PickObjects.Add(obj);
		}
	}

	public virtual void ClearPickObjects() {
		PickObject obj;for(int i=0,imax=m_PickObjects.Count;i<imax;++i) {obj=m_PickObjects[i];
			if(obj!=null) {
				obj.SetParent(null);
				obj.Recycle();
			}
		}
		m_PickObjects.Clear();
	}

	#endregion Methods

}
