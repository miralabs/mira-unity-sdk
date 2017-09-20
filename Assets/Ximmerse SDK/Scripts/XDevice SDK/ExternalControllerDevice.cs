//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
#if UNITY_IOS
using AOT;
#endif

namespace Ximmerse.InputSystem {

	/// <summary>
	/// 
	/// </summary>
	//public partial class XDevicePlugin {

		/// <summary>
		/// 
		/// </summary>
		public class ExternalControllerDevice {

			#if UNITY_IOS
			public delegate int GetInputStateDelegate(int which, ref XDevicePlugin.ControllerState state);
			public delegate int SendMessageDelegate(int which, int Msg, int wParam, int lParam) ;
			#endif

			#region Static
			
			public static Dictionary<int,ExternalControllerDevice> instances=new Dictionary<int,ExternalControllerDevice>();

			/// <summary>
			/// 
			/// </summary>
			public static void SetInputDevice(int which,ExternalControllerDevice device) {
				if(instances.ContainsKey(which)) {
					instances[which]=device;
				}else {
					instances.Add(which,device);
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public static void RemoveInputDevice(ExternalControllerDevice device) {
				if(device==null) {
					return;
				}
				instances.Remove(device.handle);
				device.handle=-1;
			}

			/// <summary>
			/// 
			/// </summary>
			public static ExternalControllerDevice GetInputDevice(int which) {
				ExternalControllerDevice device;
				instances.TryGetValue(which,out device);
				if(device!=null) {if(device.handle!=which) {
					Log.e("ExternalControllerDevice","device.handle!=which@GetInputDevice");
					return null;
				}}
				return device;
			}

			/// <summary>
			/// 
			/// </summary>
			public static ExternalControllerDevice GetInputDevice(string name) {
				foreach(ExternalControllerDevice device in instances.Values) {
					if(device.name==name) {
						return device;
					}
				}
				return null;
			}

			/// <summary>
			/// 
			/// </summary>

		 	#if UNITY_IOS
			[MonoPInvokeCallback(typeof(GetInputStateDelegate))]
			#endif
			public static int GetInputState(int which,ref XDevicePlugin.ControllerState state) {
				ExternalControllerDevice device=ExternalControllerDevice.GetInputDevice(which);
				if(device==null) {
					return -1;
				}
				//
				return device.GetInputState(ref state);
			}

			/// <summary>
			/// 
			/// </summary>
			#if UNITY_IOS
			[MonoPInvokeCallback(typeof(SendMessageDelegate))]
			#endif
			public static int SendMessage(int which, int Msg, int wParam, int lParam) {
				ExternalControllerDevice device=ExternalControllerDevice.GetInputDevice(which);
				if(device==null) {
					return -1;
				}
				//
				return device.SendMessage(Msg, wParam, lParam);
			}


			#endregion Static

			#region Fields

			public string name;

			/// <summary>
			/// The unique id created from XDevicePlugin.AddExternalControllerDevice(...)
			/// </summary>
			public int handle;

			#endregion Fields

			#region Constructors

			/// <summary>
			/// 
			/// </summary>
			public ExternalControllerDevice() {
				handle=-1;
			}

			/// <summary>
			/// 
			/// </summary>
			public ExternalControllerDevice(string name):this() {
				this.name=name;
				handle=GetInputDeviceHandle();
			}
		
			#endregion Constructors

			#region Methods

			/// <summary>
			/// 
			/// </summary>
			public virtual int GetInputDeviceHandle() {
				//
				if(handle>=0) {
					return handle;
				}
				// Natives
				int ret=XDevicePlugin.GetInputDeviceHandle(name);
				if(ret>=0){XDevicePlugin.RemoveInputDeviceAt(ret);}// Remove the same name device...
				//
				ret=XDevicePlugin.AddExternalControllerDevice(name,ExternalControllerDevice.GetInputState,ExternalControllerDevice.SendMessage);
				XDevicePlugin.SetInt(ret,XDevicePlugin.kField_ConnectionStateInt,(int)DeviceConnectionState.Connected);
				// Managed
				ExternalControllerDevice.RemoveInputDevice(ExternalControllerDevice.GetInputDevice(name));// Remove the same name device...
				ExternalControllerDevice.SetInputDevice(handle=ret,this);
				return ret;
			}

			/// <summary>
			/// 
			/// </summary>
			public virtual int UpdateInputState() {
				return XDevicePlugin.UpdateInputState(handle);
			}

			/// <summary>
			/// 
			/// </summary>
			public virtual int GetInputState(ref XDevicePlugin.ControllerState state) {
				return -1;
			}

			/// <summary>
			/// 
			/// </summary>
			public virtual int SendMessage(int Msg, int wParam, int lParam) {
				return -1;
			}

			/// <summary>
			/// 
			/// </summary>
			public virtual void ResetStatePose(ref XDevicePlugin.ControllerState state) {
				int i;
				// Motion ???
				i=0;
				state.position[i++]=0f;
				state.position[i++]=0f;
				state.position[i++]=0f;
				i=0;
				state.rotation[i++]=0f;
				state.rotation[i++]=0f;
				state.rotation[i++]=0f;
				state.rotation[i++]=1f;
			}

			#endregion Methods

		}

	//}

}