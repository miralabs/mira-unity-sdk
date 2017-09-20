//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using Ximmerse.InputSystem;

/// <summary>
///  Provides visual feedback for the controller.
/// </summary>
public class ControllerVisual:MonoBehaviour {

	#region Nested Types

	public enum AxisType {
		Joystick,
		Trigger,
	}

	[System.Serializable]
	public class AxisListener {
		public string name;
		public AxisType type=AxisType.Joystick;
		public ControllerAxis[] axes=new ControllerAxis[2];

		public Transform target;
		public float maxValue;
		[System.NonSerialized]protected Vector3 m_CachedVector3;

		public void Awake(ControllerInput input) {
			if(target!=null) {
				switch(type) {
					case AxisType.Joystick:
						m_CachedVector3=target.localPosition;
					break;
					case AxisType.Trigger:
						m_CachedVector3=target.localRotation.eulerAngles;
					break;
				}
			}
		}

		public void Update(ControllerInput input) {
			if(target!=null) {
				switch(type) {
					case AxisType.Joystick:
						target.localPosition=m_CachedVector3+new Vector3(input.GetAxis(axes[0])*maxValue,input.GetAxis(axes[1])*maxValue,0.0f);
					break;
					case AxisType.Trigger:
						target.localRotation=Quaternion.AngleAxis(m_CachedVector3.x+input.GetAxis(axes[0])*maxValue,Vector3.right);
					break;
				}
			}
		}
	}

	[System.Serializable]
	public class ButtonListener {
		public string name;
		public ControllerButton[] unacceptables=new ControllerButton[0];
		public ControllerButton[] acceptables=new ControllerButton[1];
		
		public GameObject target;
		public UnityEngine.Events.UnityEvent onPressed=new UnityEngine.Events.UnityEvent();
		public UnityEngine.Events.UnityEvent onReleased=new UnityEngine.Events.UnityEvent();
		
		[System.NonSerialized]public bool value=false;
		[System.NonSerialized]public uint[] buttonMasks=new uint[2];

		public void Awake(ControllerInput input) {
			buttonMasks[0]=0;
			for(int i=0,imax=unacceptables.Length;i<imax;++i) {
				buttonMasks[0]|=(uint)unacceptables[i];
			}
			buttonMasks[1]=0;
			for(int i=0,imax=acceptables.Length;i<imax;++i) {
				buttonMasks[1]|=(uint)acceptables[i];
			}
		}

		public void Update(ControllerInput input) {
			//
			bool prevValue=value;
			if(buttonMasks[0]!=0) {
				if(input.GetButton(buttonMasks[0])){
					value=false;
				}else {
					value=input.GetButton(buttonMasks[1]);
				}
			}else {
				value=input.GetButton(buttonMasks[1]);
			}
			//
			if(value!=prevValue) {
				if(target!=null) {
					target.SetActive(value);
				}else {
					if(value) {
						onPressed.Invoke();
					}else {
						onReleased.Invoke();
					}
				}
			}
		}
	}

	#endregion Nested Types

	#region Fields
	
	public ControllerType controller=ControllerType.None;

	public AxisListener[] axes=new AxisListener[0];
	public ButtonListener[] buttons=new ButtonListener[0];
	[System.NonSerialized]protected ControllerInput m_Input;

	#endregion Fields

	#region Unity Messages

	protected virtual void Awake() {
		TrackedObject obj=GetComponent<TrackedObject>();
		if(obj!=null) {
			controller=obj.source;
		}
		if(controller==ControllerType.None) {
			m_Input=ControllerInputManager.GetInput(name);
		}else {
			m_Input=ControllerInputManager.GetInput(controller);
		}
		//
		if(m_Input!=null) {
			for(int i=0,imax=axes.Length;i<imax;++i) {
				axes[i].Awake(m_Input);
			}
			for(int i=0,imax=buttons.Length;i<imax;++i) {
				buttons[i].Awake(m_Input);
			}
		}
	}

	protected virtual void Update() {
		if(m_Input!=null) {
			for(int i=0,imax=axes.Length;i<imax;++i) {
				axes[i].Update(m_Input);
			}
			for(int i=0,imax=buttons.Length;i<imax;++i) {
				buttons[i].Update(m_Input);
			}
		}
	}

#if UNITY_EDITOR

	[ContextMenu("Reset As Motion Controller")]
	protected virtual void ResetAsMotionController() {
		axes=new AxisListener[2] {
			new AxisListener{name="Touchpad",type=AxisType.Joystick,axes=new ControllerAxis[2]{ControllerAxis.PrimaryThumbX,ControllerAxis.PrimaryThumbY}},
			new AxisListener{name="Trigger",type=AxisType.Trigger,axes=new ControllerAxis[1]{ControllerAxis.PrimaryTrigger}}
		};
		buttons=new ButtonListener[7] {
			// Touch
			new ButtonListener{name="Touch",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.Touch},unacceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.Click}},
			// Click
			new ButtonListener{name="Click",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.Click}},
			// App
			new ButtonListener{name="App",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.App}},
			// Home
			new ButtonListener{name="Home",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.Home}},
			// GripL
			new ButtonListener{name="GripL",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.GripL}},
			// GripR
			new ButtonListener{name="GripR",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.GripR}},
			// Trigger
			new ButtonListener{name="Trigger",acceptables=new ControllerButton[1]{(ControllerButton)XimmerseButton.Trigger}}
		};
	}

#endif

	#endregion Unity Messages

}
