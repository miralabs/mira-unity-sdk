//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using Ximmerse;
using Ximmerse.InputSystem;
using Ximmerse.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControllerInputGUI:MonoBehaviour {

	#region Nested Types

	/// <summary>
	/// 
	/// </summary>
	public enum AxisType:byte{
		TYPE_1D,
		TYPE_2D,
		TYPE_TOUCHPAD
	}

	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class AxisEntry{
		[System.NonSerialized]public ControllerEntry context;
		public AxisType type=AxisType.TYPE_1D;
		public ControllerAxis[] keys=new ControllerAxis[2];
		[SerializeField]internal GameObject m_GameObject=null;
		[System.NonSerialized]internal Image m_Image=null;
		[System.NonSerialized]internal RectTransform m_Thumb=null;
		[SerializeField]internal Vector3 m_Center=Vector3.zero;
		[SerializeField]internal float m_Radius=128f;

		/// <summary>
		/// 
		/// </summary>
		public void Start(ControllerEntry context){
			this.context=context;
			m_Thumb=m_GameObject.GetComponent<RectTransform>();
			m_Image=m_GameObject.GetComponent<Image>();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Update(){
			switch(type){
				case AxisType.TYPE_1D:
					m_Image.fillAmount=context.controllerInput.GetAxis(keys[0]);
				break;
				case AxisType.TYPE_2D:
					m_Thumb.localPosition=
						m_Center+
						new Vector3(
							m_Radius*context.controllerInput.GetAxis(keys[0]),
							m_Radius*context.controllerInput.GetAxis(keys[1]),
							0.0f
						);
				break;
				case AxisType.TYPE_TOUCHPAD:
					m_Thumb.localPosition=
						m_Center+
						new Vector3(
							m_Radius*(context.controllerInput.touchPos.x-.5f)*2.0f,
							m_Radius*(context.controllerInput.touchPos.y-.5f)*-2.0f,
							0.0f
						);
				break;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class ButtonEntry{
		[System.NonSerialized]public ControllerEntry context;
		public ControllerButton key;
		[SerializeField]internal GameObject m_GameObject=null;
		[System.NonSerialized]internal Transform m_Transform;
		[System.NonSerialized]internal Graphic m_Graphic;
		[System.NonSerialized]internal float m_DefaultScale;

		/// <summary>
		/// 
		/// </summary>
		public void Start(ControllerEntry context){
			this.context=context;
			m_Transform=m_GameObject.transform;
			m_Graphic=m_GameObject.GetComponent<Graphic>();
			m_DefaultScale=m_Transform.localScale.x;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Update(){
			if(m_Graphic==null) return;
			//
			if(!true) {
			}else{
				m_Graphic.enabled=context.controllerInput.GetButton(key);
				/*
				if(context.controllerInput.GetButtonDown(key)){
					m_Graphic.enabled=true;
				} else if(context.controllerInput.GetButtonUp(key)){
					m_Graphic.enabled=false;
				}
				*/
			}
		}

		public void OnHighlight(float value){
			m_Transform.localScale=Vector3.one*(value*m_DefaultScale);
		}
	}

	[System.Serializable]
	public class SliderGroup {
		public Image image;
		public float max;
		public Text text;
		public string format;

		public void Start() {
			format=format.Replace("\\n","\n");
		}

		public void Update(float value) {
			if(image!=null) {
				image.fillAmount=value/max;
			}
			if(text!=null) {
				text.text=string.Format(format,value);
			}
		}
	}

	[System.Serializable]
	public class VibrationEntry:UKeyValuePair<Button,float> {
		public int type;
		[System.NonSerialized]public ControllerEntry context;

		public void Start(ControllerEntry context){
			this.context=context;
			if(key!=null) {
				if(value>=0f)key.onClick.AddListener(OnStartClick);
				else key.onClick.AddListener(OnStopClick);
			}
		}

		protected void OnStartClick() {
			if(context!=null) {
			if(context.controllerInput!=null) {
				// context.controllerInput.StartVibration(type,value);
			}}
		}

		protected void OnStopClick() {
			if(context!=null) {
			if(context.controllerInput!=null) {
				// context.controllerInput.StopVibration();
			}}
		}

	}

#if UNITY_EDITOR
	[UnityEditor.CustomPropertyDrawer(typeof(VibrationEntry))]
	public class VibrationEntryDrawer:UKeyValuePairDrawer<Button,float> {
		public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
			if(fields==null) {
				numFields=3;
				fields=new string[3]{"key","type","value"};
			}
			base.OnGUI(position,property,label);
		}
	}
#endif

	[System.Serializable]
	public class ControllerEntry{
		[System.NonSerialized]public ControllerInput controllerInput;
		[SerializeField]internal Text m_NameText;
		[SerializeField]internal string m_Name="";
		[SerializeField]internal GameObject m_EnableGo,m_DisableGo;
		[SerializeField]internal FPSCounter m_FPSCounter;
		[SerializeField]internal Text m_ConnectionText;
		[SerializeField]internal AxisEntry[] m_Axes=new AxisEntry[0];
		[SerializeField]internal ButtonEntry[] m_Buttons=new ButtonEntry[0];
		[SerializeField]internal VibrationEntry[] m_Vibrations=new VibrationEntry[0];
		[SerializeField]internal UIVector3Field m_Position;
		[System.NonSerialized]internal GameObject m_PositionGo;
		[SerializeField]internal UIVector3Field m_Rotation;
		[SerializeField]internal UIVector3Field m_Accelerometer;
		[SerializeField]internal UIVector3Field m_Gyroscope;
		[SerializeField]internal SliderGroup m_AccelerometerMag=new SliderGroup{max=9.801f*2f*1.73f,format="Accel:{0:f3}m^2/s"};
		[SerializeField]internal SliderGroup m_GyroscopeMag=new SliderGroup{max=1000f,format="Accel:{0:f3}"};

		/// <summary>
		/// 
		/// </summary>
		public void Start(){
			if(m_NameText!=null) {
				m_Name=m_NameText.text;
			}
			//
			controllerInput=ControllerInputManager.instance.GetControllerInput(m_Name);
			if(controllerInput==null) {
				SetActive(false);
				return;
			// Check error code.
			}else if(controllerInput.connectionState==DeviceConnectionState.Error){
				SetActive(false);
				if(m_DisableGo!=null) {
					Text text=m_DisableGo.GetComponentInChildren<Text>();
					if(text!=null) {
						text.text="Error:"+controllerInput.errorCode;
						controllerInput=null;
					}
				}
			}else {
				SetActive(true);
			}
			// TODO : Make it recenterable.
			controllerInput.isAbsRotation=false;
			//
			int i,imax;
			for(i=0,imax=m_Axes.Length;i<imax;++i){
				m_Axes[i].Start(this);
			}
			for(i=0,imax=m_Buttons.Length;i<imax;++i){
				m_Buttons[i].Start(this);
			}
			for(i=0,imax=m_Vibrations.Length;i<imax;++i){
				m_Vibrations[i].Start(this);
			}
			if(m_FPSCounter) {
				m_FPSCounter.getFrameCount=GetFrameCount;
			}
			if(m_AccelerometerMag.image!=null) {
				m_AccelerometerMag.Start();
			}
			if(m_GyroscopeMag.image!=null) {
				m_GyroscopeMag.Start();
			}
			//Log.i("ControllerInputGUI","Create a GUI for "+controllerInput+"@"+controllerInput.name);
			Update();
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetActive(bool value) {
			if(m_EnableGo!=null) {
				m_EnableGo.SetActive(value);
			}
			if(m_DisableGo!=null) {
				m_DisableGo.SetActive(!value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Update(){
			if(controllerInput==null) {
				return;
			}
			//
			if(m_ConnectionText!=null) {
				m_ConnectionText.text=controllerInput.connectionState.ToString();
			}
			//
			int i,imax;
			for(i=0,imax=m_Axes.Length;i<imax;++i){
				m_Axes[i].Update();
			}
			for(i=0,imax=m_Buttons.Length;i<imax;++i){
				m_Buttons[i].Update();
			}
			if(m_Position!=null) {
				if(m_PositionGo==null) {
					m_PositionGo=m_Position.gameObject;
				}
				//
				m_PositionGo.SetActive((controllerInput.trackingResult&TrackingResult.PositionTracked)!=0);
				m_Position.value=controllerInput.GetPosition();
			}
			if(m_Rotation!=null) {
				m_Rotation.value=controllerInput.GetRotation().eulerAngles;
			}
			if(m_Accelerometer!=null) {
				m_Accelerometer.value=controllerInput.GetAccelerometer();
			}
			if(m_Gyroscope!=null) {
				m_Gyroscope.value=controllerInput.GetGyroscope();
			}
			if(m_AccelerometerMag.image!=null) {
				m_AccelerometerMag.Update(controllerInput.GetAccelerometer().magnitude);
			}
			if(m_GyroscopeMag.image!=null) {
				m_GyroscopeMag.Update(controllerInput.GetGyroscope().magnitude);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int GetFrameCount() {
			if(controllerInput==null) {
				return 0;
			}
			return controllerInput.frameCount;
		}
	}

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(ButtonEntry))]
	public class ButtonEntryDrawer:SimplePropertyDrawer {
		public override void OnGUI(Rect position,SerializedProperty property,GUIContent label) {
			numFields=2;
			base.OnGUI(position,property,label);
		}

		protected override void OnDraw(SerializedProperty property,Rect[] rects) {
			int i=0;
			GUI.changed=false;
			int key=(int)((ControllerButton)(EditorGUI.EnumPopup(rects[i++],(ControllerButton)(property.FindPropertyRelative("key").intValue))));
			Object m_GameObject=EditorGUI.ObjectField(rects[i++],property.FindPropertyRelative("m_GameObject").objectReferenceValue,typeof(GameObject),true);

			if(GUI.changed) {
				property.FindPropertyRelative("key").intValue=key;
				property.FindPropertyRelative("m_GameObject").objectReferenceValue=m_GameObject;
			}
		}
	}

#endif

	#endregion Nested Types

	#region Fields

	[SerializeField]protected ControllerEntry[] m_Controllers=new ControllerEntry[0];

	#endregion Fields

	#region Unity Messages

	protected virtual void Start(){
		for(int i=0,imax=m_Controllers.Length;i<imax;++i){
			m_Controllers[i].Start();
		}
	}

	protected virtual void Update(){
		for(int i=0,imax=m_Controllers.Length;i<imax;++i){
			m_Controllers[i].Update();
		}
	}

#if UNITY_EDITOR

	[ContextMenu("Rename Buttons")]
	protected virtual void RenameButtonGameObjects() {
		for(int i=0,imax=m_Controllers.Length;i<imax;++i){
			for(int j=0,jmax=m_Controllers[i].m_Buttons.Length;j<jmax;++j){
				if(m_Controllers[i].m_Buttons[j].m_GameObject!=null) {
					m_Controllers[i].m_Buttons[j].m_GameObject.name=
						"Highlight_"+m_Controllers[i].m_Buttons[j].key.ToString();
				}
			}
		}
	}

#endif

	#endregion Unity Messages

}