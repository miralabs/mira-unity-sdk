//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
	
namespace Ximmerse.UI{

	/// <summary>
	/// The graphic to draw Vector3 data to screen,with a group of InputFields.
	/// </summary>
	public class UIVector3Field:MonoBehaviour {

		#region Nested Types

		[System.Serializable]
		public class Vector3Event:UnityEvent<Vector3>{}

		#endregion Nested Types

		#region Fields

		[Header("Data")]
		[SerializeField]protected string m_Field="";
		[SerializeField]protected string m_Format="0.000";
		[SerializeField]protected Vector3 m_Value=Vector3.zero;
		[Header("UI")]
		[SerializeField]protected Text m_Label;
		[SerializeField]protected InputField[] m_Texts=new InputField[3];
		[SerializeField]protected Vector3Event m_OnValueChanged;
		[System.NonSerialized]protected bool m_IsRefreshing=false;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Start(){
			Refresh();
			//
			int i=3;
			while(i-->0) {
				if(m_Texts[i]!=null) {
					m_Texts[i].onValueChange.AddListener(OnTextChanged);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnTextChanged(string text){
			if(!m_IsRefreshing) {
				int i=3;
				Vector3 value=Vector3.zero;
				float f;
				while(i-->0){
					if(m_Texts[i]!=null) {
						if(float.TryParse(m_Texts[i].text,out f)) {
							value[i]=f;
						}
					}
				}
				this.value=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Refresh(){
			m_Label.text=m_Field;
			int i=3;
			m_IsRefreshing=true;
				while(i-->0){
					if(m_Texts[i]!=null) {
						m_Texts[i].text=m_Value[i].ToString(m_Format);
					}
				}
			m_IsRefreshing=false;
		}

		public virtual string field{
			get{
				return m_Field;
			}
			set{
				if(value==m_Field){
					return;
				}
				m_Field=value;
				Refresh();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual Vector3 value{
			get{
				return m_Value;
			}
			set{
				if(value==m_Value){
					return;
				}
				m_Value=value;
				Refresh();
				//
				m_OnValueChanged.Invoke(value);
			}
		}

		#endregion Methods

	}

}