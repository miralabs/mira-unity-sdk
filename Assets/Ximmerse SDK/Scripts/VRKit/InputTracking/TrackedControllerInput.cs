//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;

namespace Ximmerse.InputSystem{

	public interface IInputTracking {
		bool Emulates(int node);
		bool Exists(int node);
		Vector3 GetLocalPosition(int node);
		Quaternion GetLocalRotation(int node);
	}

	/// <summary>
	/// 
	/// </summary>
	public class TrackedControllerInput:ControllerInput {

		#region Fields

		public IInputTracking inputTracking;
		public int node;

		#endregion Fields

		#region Constructors

		public TrackedControllerInput(string name,IInputTracking inputTracking,int defaultNode):base(name) {
			this.inputTracking=inputTracking;
			this.node=defaultNode;
		}

		#endregion Constructors

		#region TODO : State is HACKED

		/*public override DeviceConnectionState connectionState {
			get {
				if(inputTracking!=null) {
					if(inputTracking.Exists(node)) {
						return DeviceConnectionState.Connected;
					}
				}
				return base.connectionState;
			}
		}*/


		public override TrackingResult trackingResult {
			get {
				if(inputTracking!=null) {
					TrackingResult result=base.trackingResult;
					//
					if(inputTracking.Exists(node)) {
						result|=(TrackingResult.PositionTracked);
					}else {
						result&=(~TrackingResult.PositionTracked);
					}
					if(inputTracking.Emulates(node)) {
						result|=(TrackingResult.PositionEmulated);
					}else {
						result&=(~TrackingResult.PositionEmulated);
					}
					//
					return result;
				}
				return base.trackingResult;
			}
		}

		#endregion TODO : State is HACKED

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateState() {
			if(Time.frameCount!=m_PrevFrameCount){
				base.UpdateState();
				// TODO : Assign blob id dynamically.
				/*if(m_State.state_mask!=0) {
					node=(m_State.state_mask&0xff)-1;
				}*/
			}
		}

		public override Vector3 GetPosition() {
			// Lost tracking...
			if((trackingResult&TrackingResult.PositionTracked)==0) {
				return Vector3.zero;
			}
			if(inputTracking!=null) {
				return inputTracking.GetLocalPosition(node);
			}
			//
			return Vector3.zero;
		}

		public override Quaternion GetRotation() {
			return base.GetRotation();
		}

		#endregion Methods

	}
}
