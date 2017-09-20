//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using Ximmerse.VR;
using Ximmerse.InputSystem;

/// <summary>
/// This class is for dispatching controller tracked events.
/// </summary>
public class TrackedEventListener:MonoBehaviour {

	#region Static

	public const int
		RESULT_UNKNOWN          = -1,
		RESULT_OUT_OF_RANGE     = -2,
		RESULT_NO_TRACKING_DATA = -3,
		RESULT_NO_TRACKER       = -4,
		RESULT_OK               =  0
	;

	public static TrackedEventListener current;

	#endregion Static

	#region Fields

	[System.NonSerialized]protected int m_LastResult=RESULT_UNKNOWN;

	public ControllerType controller=ControllerType.None;
	[System.NonSerialized]protected ControllerInput m_ControllerInput;
	[System.NonSerialized]protected bool m_IsPositionEmulated;

	[System.NonSerialized]public GameObject uiOutOfRange;
	[System.NonSerialized]protected Ximmerse.UI.UIFade m_FadeOutOfRange;

	/// <summary>
	/// The action when the TrackedObject became visible.
	/// </summary>
	public UnityEvent onBecameVisible=new UnityEvent();

	/// <summary>
	/// The action when the TrackedObject became invisible.
	/// </summary>
	public UnityEvent onBecameInvisible=new UnityEvent();

	/// <summary>
	/// 
	/// </summary>
	public UnityEvent onStartEmulation=new UnityEvent();

	/// <summary>
	/// 
	/// </summary>
	public UnityEvent onStopEmulation=new UnityEvent();
	
	#endregion Fields

	#region Unity Messages & Events
	
	protected virtual void Start() {
		TrackedObject obj=GetComponent<TrackedObject>();
		if(obj!=null) {
			controller=obj.source;
		}
		if(controller==ControllerType.None) {
			m_ControllerInput=ControllerInputManager.instance.GetControllerInput(name);
		}else {
			m_ControllerInput=ControllerInputManager.instance.GetControllerInput(controller);
		}
		//
		if(PlayerPrefsEx.GetBool("UINotification-OutOfRange.enabled",false)) {
			uiOutOfRange=PlayerPrefsEx.GetObject("UINotification-OutOfRange-"+controller.ToString(),null) as GameObject;
			if(uiOutOfRange!=null) {
				Transform uiRoot=VRContext.uiRootVR;
				if(uiRoot!=null) {
					GameObject go=Instantiate(uiOutOfRange);
						Transform t=go.transform,raw=uiOutOfRange.transform;
						t.SetParent(uiRoot);
						t.localPosition=raw.localPosition;
						t.localRotation=raw.localRotation;
						t.localScale=raw.localScale;
					uiOutOfRange=go;
					m_FadeOutOfRange=uiOutOfRange.GetComponent<Ximmerse.UI.UIFade>();
					//
					if(m_FadeOutOfRange!=null) {
						m_FadeOutOfRange.alpha=0.0f;
					}
					SetUiOutOfRange(false);
				}
			}
		}
	}
	
	protected virtual void Update() {
		if(m_ControllerInput!=null) {
			//
			bool isPositionEmulated=m_ControllerInput.positionEmulated;
			if(isPositionEmulated!=m_IsPositionEmulated) {
				m_IsPositionEmulated=isPositionEmulated;
				//
				if(m_IsPositionEmulated) {
					onStartEmulation.Invoke();
				}else {
					onStopEmulation.Invoke();
				}
			}
			//
			if(m_ControllerInput.connectionState==DeviceConnectionState.Connected) {
				OnTrackingResult(
					m_ControllerInput.positionTracked?
					RESULT_OK:RESULT_NO_TRACKING_DATA
				);
			} else {
				OnTrackingResult(RESULT_NO_TRACKER);
			}
		}
	}

	public virtual void OnTrackingResult(int result){
		if(result==m_LastResult) {
			return;
		}
		//
		bool showUI=(result!=RESULT_OK);
		current=this;
			switch(result) {
				case RESULT_OK:
					onBecameVisible.Invoke();
				break;
				case RESULT_OUT_OF_RANGE:
				case RESULT_NO_TRACKING_DATA:
					onBecameInvisible.Invoke();
				break;
				// <!-- TODO: VR Legacy Mode. -->
				case RESULT_NO_TRACKER:
					onBecameInvisible.Invoke();
					//If controller is connected at last time..
					showUI=(m_LastResult==RESULT_OK||m_LastResult==RESULT_NO_TRACKING_DATA);
				break;
				default:
					Ximmerse.Log.w("TrackedEventListener","Unhandled result@"+result);
				break;
			}
		current=null;
		// UI
		SetUiOutOfRange(showUI);
		//
		m_LastResult=result;
	}

	public virtual void SetUiOutOfRange(bool value){
		if(uiOutOfRange!=null) {
			if(m_FadeOutOfRange!=null) {
				m_FadeOutOfRange.Play(value);
			}else {
				uiOutOfRange.SetActive(value);
			}
		}
	}

	#endregion Unity Messages & Events

}
