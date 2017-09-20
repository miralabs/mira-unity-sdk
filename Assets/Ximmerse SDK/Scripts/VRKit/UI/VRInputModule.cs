//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Ximmerse.InputSystem;

namespace Ximmerse.UI {

	public class VRPointerEventData:PointerEventData {
		public ControllerInput controller;
		public VRPointerEventData(EventSystem eventSystem):base(eventSystem){
		}
	}
	
	/// <summary>
	/// An input module for UGUI,which helps you to manage multiple ui inputs.
	/// </summary>
	public class VRInputModule:BaseInputModule{

		#region Nested Types

		[System.Serializable]
		public class SubInputModule{

			#region Fields

			/// <summary>
			/// 
			/// </summary>
			public ControllerType controller=ControllerType.Hmd;
			protected ControllerInput m_Controller;
			protected string m_ControllerName;

			/// <summary>
			// Based on default time for a button to animate to Pressed.
			/// </summary>
			public float clickTime=0.1f;

			/// <summary>
			/// 
			/// </summary>
			public ControllerButton buttonClick;

			#endregion Fields

			#region Fields For uGUI

			[System.NonSerialized]public EventSystem eventSystem;
			[System.NonSerialized]public VRInputModule main;
			[System.NonSerialized]protected List<RaycastResult> m_RaycastResultCache=new List<RaycastResult>();

			[System.NonSerialized]public Vector2 hotspot=new Vector2(0.5f,0.5f);

			[System.NonSerialized]public VRPointerEventData pointerData;

			#endregion Fields For uGUI

			#region Methods

			public void ActivateModule(VRInputModule main) {
				this.eventSystem=main.eventSystem;
				this.main=main;
				//
				m_ControllerName=controller.ToString();
				m_Controller=ControllerInputManager.instance.GetControllerInput(controller);
			}

			public void DeactivateModule() {
				DisableGazePointer();
				//base.DeactivateModule();
				if(pointerData!=null) {
					HandlePendingClick();
					main.HandlePointerExitAndEnter(pointerData,null);
					pointerData=null;
				}
				eventSystem.SetSelectedGameObject(null,main.GetBaseEventData());
			}

			#endregion Methods

			#region Modify from "GoogleVR/Scripts/GazeInputModule.cs"

			public void Process() {
				// Save the previous Game Object
				GameObject gazeObjectPrevious = GetCurrentGameObject();

				CastRayFromGaze();
				UpdateCurrentObject();
				UpdateReticle(gazeObjectPrevious);

				// Handle input
				//if(!Input.GetMouseButtonDown(0)&&Input.GetMouseButton(0)) {
				//	HandleDrag();
				//} else if(Time.unscaledTime-pointerData.clickTime<clickTime) {
				//	// Delay new events until clickTime has passed.
				//} else if(!pointerData.eligibleForClick&&
				//		   (GvrViewer.Instance.Triggered||Input.GetMouseButtonDown(0)||
				//			GvrController.ClickButtonDown)) {
				//	// New trigger action.
				//	HandleTrigger();
				//} else if(!GvrViewer.Instance.Triggered&&!Input.GetMouseButton(0)&&
				//		   !GvrController.ClickButton) {
				//	// Check if there is a pending click to handle.
				//	HandlePendingClick();
				//}

				// Handle input
				if(!m_Controller.GetButtonDown(buttonClick)&&m_Controller.GetButton(buttonClick)) {
					HandleDrag();
				} else if(Time.unscaledTime-pointerData.clickTime<clickTime) {
					// Delay new events until clickTime has passed.
				} else if(!pointerData.eligibleForClick&&
						   (m_Controller.GetButtonDown(buttonClick))) {
					// New trigger action.
					HandleTrigger();
				} else if(!m_Controller.GetButton(buttonClick)) {
					// Check if there is a pending click to handle.
					HandlePendingClick();
				}
			}
			/// @endcond

			private void CastRayFromGaze() {
				//Vector2 headPose = NormalizedCartesianToSpherical(GvrViewer.Instance.HeadPose.Orientation*Vector3.forward);

				if(pointerData==null) {
					pointerData=new VRPointerEventData(eventSystem);
					pointerData.controller=m_Controller;
					//lastHeadPose=headPose;
				}

				// Cast a ray into the scene
				pointerData.Reset();
				pointerData.position=new Vector2(hotspot.x*Screen.width,hotspot.y*Screen.height);
				//eventSystem.RaycastAll(pointerData,m_RaycastResultCache);
				VRRaycaster caster=VRRaycaster.TryRaycastAll(m_ControllerName,pointerData,m_RaycastResultCache);
				pointerData.pointerCurrentRaycast=FindFirstRaycast(m_RaycastResultCache);
				m_RaycastResultCache.Clear();
				//
				if(caster!=null) {
					RaycastResult rr=pointerData.pointerCurrentRaycast;
					if(rr.gameObject==null) {
						caster.SetLaserLineDepth(100f);
					}else {
						caster.SetLaserLineDepth(rr.distance);
					}
				}
				//pointerData.delta=headPose-lastHeadPose;
				//lastHeadPose=headPose;
			}

			private void UpdateCurrentObject() {
				// Send enter events and update the highlight.
				var go = pointerData.pointerCurrentRaycast.gameObject;
				main.HandlePointerExitAndEnter(pointerData,go);
				// Update the current selection, or clear if it is no longer the current object.
				var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
				if(selected==eventSystem.currentSelectedGameObject) {
					ExecuteEvents.Execute(eventSystem.currentSelectedGameObject,main.GetBaseEventData(),
										  ExecuteEvents.updateSelectedHandler);
				} else {
					eventSystem.SetSelectedGameObject(null,pointerData);
				}
			}

			void UpdateReticle(GameObject previousGazedObject) {
				/*if(gazePointer==null) {
					return;
				}

				Camera camera = pointerData.enterEventCamera; // Get the camera
				GameObject gazeObject = GetCurrentGameObject(); // Get the gaze target
				Vector3 intersectionPosition = GetIntersectionPosition();
				bool isInteractive = pointerData.pointerPress!=null||
					ExecuteEvents.GetEventHandler<IPointerClickHandler>(gazeObject)!=null;

				if(gazeObject==previousGazedObject) {
					if(gazeObject!=null) {
						gazePointer.OnGazeStay(camera,gazeObject,intersectionPosition,isInteractive);
					}
				} else {
					if(previousGazedObject!=null) {
						gazePointer.OnGazeExit(camera,previousGazedObject);
					}

					if(gazeObject!=null) {
						gazePointer.OnGazeStart(camera,gazeObject,intersectionPosition,isInteractive);
					}
				}*/
			}

			private void HandleDrag() {
				bool moving = pointerData.IsPointerMoving();

				if(moving&&pointerData.pointerDrag!=null&&!pointerData.dragging) {
					ExecuteEvents.Execute(pointerData.pointerDrag,pointerData,
						ExecuteEvents.beginDragHandler);
					pointerData.dragging=true;
				}

				// Drag notification
				if(pointerData.dragging&&moving&&pointerData.pointerDrag!=null) {
					// Before doing drag we should cancel any pointer down state
					// And clear selection!
					if(pointerData.pointerPress!=pointerData.pointerDrag) {
						ExecuteEvents.Execute(pointerData.pointerPress,pointerData,ExecuteEvents.pointerUpHandler);

						pointerData.eligibleForClick=false;
						pointerData.pointerPress=null;
						pointerData.rawPointerPress=null;
					}
					ExecuteEvents.Execute(pointerData.pointerDrag,pointerData,ExecuteEvents.dragHandler);
				}
			}

			private void HandlePendingClick() {
				if(!pointerData.eligibleForClick&&!pointerData.dragging) {
					return;
				}

				/*if(gazePointer!=null) {
					Camera camera = pointerData.enterEventCamera;
					gazePointer.OnGazeTriggerEnd(camera);
				}*/

				var go = pointerData.pointerCurrentRaycast.gameObject;

				// Send pointer up and click events.
				ExecuteEvents.Execute(pointerData.pointerPress,pointerData,ExecuteEvents.pointerUpHandler);
				if(pointerData.eligibleForClick) {
					ExecuteEvents.Execute(pointerData.pointerPress,pointerData,ExecuteEvents.pointerClickHandler);
				} else if(pointerData.dragging) {
					ExecuteEvents.ExecuteHierarchy(go,pointerData,ExecuteEvents.dropHandler);
					ExecuteEvents.Execute(pointerData.pointerDrag,pointerData,ExecuteEvents.endDragHandler);
				}

				// Clear the click state.
				pointerData.pointerPress=null;
				pointerData.rawPointerPress=null;
				pointerData.eligibleForClick=false;
				pointerData.clickCount=0;
				pointerData.clickTime=0;
				pointerData.pointerDrag=null;
				pointerData.dragging=false;
			}

			private void HandleTrigger() {
				var go = pointerData.pointerCurrentRaycast.gameObject;

				// Send pointer down event.
				pointerData.pressPosition=pointerData.position;
				pointerData.pointerPressRaycast=pointerData.pointerCurrentRaycast;
				pointerData.pointerPress=
				  ExecuteEvents.ExecuteHierarchy(go,pointerData,ExecuteEvents.pointerDownHandler)
					??ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

				// Save the drag handler as well
				pointerData.pointerDrag=ExecuteEvents.GetEventHandler<IDragHandler>(go);
				if(pointerData.pointerDrag!=null) {
					ExecuteEvents.Execute(pointerData.pointerDrag,pointerData,ExecuteEvents.initializePotentialDrag);
				}

				// Save the pending click state.
				pointerData.rawPointerPress=go;
				pointerData.eligibleForClick=true;
				pointerData.delta=Vector2.zero;
				pointerData.dragging=false;
				pointerData.useDragThreshold=true;
				pointerData.clickCount=1;
				pointerData.clickTime=Time.unscaledTime;

				/*if(gazePointer!=null) {
					gazePointer.OnGazeTriggerStart(pointerData.enterEventCamera);
				}*/
			}

			private Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords) {
				cartCoords.Normalize();
				if(cartCoords.x==0)
					cartCoords.x=Mathf.Epsilon;
				float outPolar = Mathf.Atan(cartCoords.z/cartCoords.x);
				if(cartCoords.x<0)
					outPolar+=Mathf.PI;
				float outElevation = Mathf.Asin(cartCoords.y);
				return new Vector2(outPolar,outElevation);
			}

			GameObject GetCurrentGameObject() {
				if(pointerData!=null&&pointerData.enterEventCamera!=null) {
					return pointerData.pointerCurrentRaycast.gameObject;
				}

				return null;
			}

			Vector3 GetIntersectionPosition() {
				// Check for camera
				Camera cam = pointerData.enterEventCamera;
				if(cam==null) {
					return Vector3.zero;
				}

				float intersectionDistance = pointerData.pointerCurrentRaycast.distance+cam.nearClipPlane;
				Vector3 intersectionPosition = cam.transform.position+cam.transform.forward*intersectionDistance;

				return intersectionPosition;
			}

			void DisableGazePointer() {
				/*if(gazePointer==null) {
					return;
				}

				GameObject currentGameObject = GetCurrentGameObject();
				if(currentGameObject) {
					Camera camera = pointerData.enterEventCamera;
					gazePointer.OnGazeExit(camera,currentGameObject);
				}

				gazePointer.OnGazeDisabled();*/
			}

			#endregion Modify from "GoogleVR/Scripts/GazeInputModule.cs"

		}

		#endregion Nested Types

		#region Fields

		public SubInputModule[] subInputModules=new SubInputModule[1];

		#endregion Fields

		#region Methods

		public static void TriggerVibration(PointerEventData e,float duration) {
			if(e is VRPointerEventData){
				ControllerInput c=(e as VRPointerEventData).controller;
				if(c!=null) {
					c.StartVibration(0,duration);
				}
			}
		}

		// Active state
		private bool isActive = false;

		public override bool ShouldActivateModule() {
			bool activeState = base.ShouldActivateModule();

			activeState=activeState;//&&(GvrViewer.Instance.VRModeEnabled||!vrModeOnly);

			if(activeState!=isActive) {
				isActive=activeState;

				// Activate gaze pointer
				//if(gazePointer!=null) {
				//	if(isActive) {
				//		gazePointer.OnGazeEnabled();
				//	}
				//}
			}

			return activeState;
		}

		public override void ActivateModule() {
			base.ActivateModule();
			for(int i=0,imax=subInputModules.Length;i<imax;++i) {
				subInputModules[i].ActivateModule(this);
			}
		}

		public override void DeactivateModule() {
			//DisableGazePointer();
			base.DeactivateModule();
			//if(pointerData!=null) {
			//	HandlePendingClick();
			//	HandlePointerExitAndEnter(pointerData,null);
			//	pointerData=null;
			//}
			for(int i=0,imax=subInputModules.Length;i<imax;++i) {
				subInputModules[i].DeactivateModule();
			}
			eventSystem.SetSelectedGameObject(null,GetBaseEventData());
		}

		public override void Process() {
			for(int i=0,imax=subInputModules.Length;i<imax;++i) {
				subInputModules[i].Process();
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			VRRaycaster.s_AllCanvases.Clear();
		}

		#endregion Methods

	}

}