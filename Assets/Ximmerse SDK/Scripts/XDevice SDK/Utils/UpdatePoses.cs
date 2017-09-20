//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.InputSystem;

/// <summary>
/// Update poses in Camera.OnPreCull() to reduce the motion-to-photon latency.
/// </summary>
[RequireComponent(typeof(Camera))]
public class UpdatePoses:MonoBehaviour {

	#region Static

	protected static UpdatePoses s_Instance;
	protected static bool s_InstanceCached;

	public static bool existsInstance{
		get {
			return s_Instance!=null;
		}
	}

	/// <summary>
	/// Gets the singleton instance.
	/// </summary>
	/// <returns> UpdatePoses Singleton</returns>
	public static UpdatePoses instance {
		get {
			if(!s_InstanceCached) {//
				s_InstanceCached=true;
				if(s_Instance==null) {
					//
					GameObject go=new GameObject("poseUpdater",typeof(UpdatePoses));
					DontDestroyOnLoad(go);
					s_Instance=go.GetComponent<UpdatePoses>();
				}
			}
			return s_Instance;
		}
	}

	#endregion Static

	#region Fields

	public System.Action onUpdatePoses;

	//
	protected int m_PrevFrameCount;

	#endregion Fields

	#region Unity Messages

	protected virtual void Awake() {
		// Taken from SteamVR_UpdatePoses.Awake() in SteamVR SDK.
		Camera camera = GetComponent<Camera>();
#if UNITY_5&&(!(UNITY_5_3||UNITY_5_2||UNITY_5_1||UNITY_5_0))
		camera.stereoTargetEye=StereoTargetEyeMask.None;
#endif
		camera.clearFlags=CameraClearFlags.Nothing;
		camera.useOcclusionCulling=false;
		camera.cullingMask=0;
		camera.depth=-9999;
	}

	protected virtual void Start() {
		//StartCoroutine("UpdateEof");
	}

	//protected virtual void Update() {
	//	TryUpdatePoses();
	//}

	//protected virtual void LateUpdate() {
	//	TryUpdatePoses();
	//}

	protected virtual void OnPreCull() {
		TryUpdatePoses();
	}

	protected virtual System.Collections.IEnumerator UpdateEof() {
		var wait = new WaitForEndOfFrame();
		while(true) {
			yield return wait;
			//
			if(m_PrevFrameCount==Time.frameCount){
			}else{
				//Ximmerse.Log.d("UpdatePoses","Not new data at this UnityFrame.");
			}
		}
	}

	protected virtual void OnDestroy() {
		StopAllCoroutines();
		//
		s_Instance=null;
		s_InstanceCached=false;
	}

	#endregion Unity Messages

	#region Methods

	public virtual void TryUpdatePoses() {
		if(Time.frameCount!=m_PrevFrameCount){
			//
			m_PrevFrameCount=Time.frameCount;
			//
			if(onUpdatePoses!=null) {
				onUpdatePoses();
			}
		}
	}

	#endregion Methods

}