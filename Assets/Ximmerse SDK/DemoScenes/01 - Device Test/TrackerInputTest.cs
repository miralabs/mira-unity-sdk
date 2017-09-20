//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;
using UnityEngine.UI;
using Ximmerse.InputSystem;

/// <summary>
/// A script for testing TrackingInput with UGUI Text.
/// </summary>
public class TrackerInputTest:MonoBehaviour {

	#region Fields
	
	[SerializeField]protected FPSCounter m_FPSCounter;
	[SerializeField]protected Text m_Text;
	[SerializeField]protected int[] m_Nodes=new int[3]{0,1,2};
	[System.NonSerialized]protected TrackerInput m_TrackerInput;

	#endregion Fields

	#region Unity Messages

	protected virtual void Start() {
		m_TrackerInput=TrackerInput.Find(null);
		if(m_FPSCounter!=null) {
			m_FPSCounter.getFrameCount=GetFrameCount;
		}
	}

	protected virtual void Update() {
		if(m_Text!=null){if(m_TrackerInput!=null){
			//System.Text.StringBuilder sb=new System.Text.StringBuilder();
			m_Text.text=m_TrackerInput.ToString(m_Nodes);
		}}
	}

	protected virtual int GetFrameCount() {
		if(m_TrackerInput==null){
			return 0;
		}
		return m_TrackerInput.GetFrameCount();
	}

	#endregion Unity Messages

}