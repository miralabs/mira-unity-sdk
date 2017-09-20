//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;

namespace Ximmerse {

/// <summary>
/// 
/// </summary>
public partial class Log {

		#region Interface

		/// <summary>
		/// 
		/// </summary>
		public interface ILogger {
			int v(string tag,string msg);
			int i(string tag,string msg);
			int d(string tag,string msg);
			int w(string tag,string msg);
			int e(string tag,string msg);
		}

		#endregion Interface

		#region Fields

		public static ILogger s_Logger;
		public static int s_Filter;

		#endregion Fields

		#region Constraints

		public const int
			k_Filter_v           =1<<0,
			k_Filter_i           =1<<1,
			k_Filter_d           =1<<2,
			k_Filter_w           =1<<3,
			k_Filter_e           =1<<4,
			//
			k_Error_No_Logger    =  -1,
			k_Error_Fail_Filter  =  -2,
			k_Error_OK           =   0
		;

		#endregion Constraints

		#region Static Methods

		static Log(){
			s_Logger=new Logger(null);
			s_Filter=-1;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int v(string tag,string msg){
			if(s_Logger==null) return k_Error_No_Logger;
			if((s_Filter&k_Filter_v)==0) return k_Error_Fail_Filter;
			return s_Logger.v(tag,msg);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static int i(string tag,string msg){
			if(s_Logger==null) return k_Error_No_Logger;
			if((s_Filter&k_Filter_i)==0) return k_Error_Fail_Filter;
			return s_Logger.i(tag,msg);
		}

		/// <summary>
		/// 
		/// </summary>
		public static int d(string tag,string msg){
			if(s_Logger==null) return k_Error_No_Logger;
			if((s_Filter&k_Filter_d)==0) return k_Error_Fail_Filter;
			return s_Logger.d(tag,msg);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static int w(string tag,string msg){
			if(s_Logger==null) return k_Error_No_Logger;
			if((s_Filter&k_Filter_w)==0) return k_Error_Fail_Filter;
			return s_Logger.w(tag,msg);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static int e(string tag,string msg){
			if(s_Logger==null) return k_Error_No_Logger;
			if((s_Filter&k_Filter_e)==0) return k_Error_Fail_Filter;
			return s_Logger.e(tag,msg);
		}

		#endregion Static Methods
	
	}

	public class Logger:Log.ILogger {
		public string format="tag={0},msg={1}";
		public Logger(string format=null) {
			if(!string.IsNullOrEmpty(format)) {
				this.format=format;
			}
		}

		public int v(string tag,string msg) {
			Debug.LogFormat(format,tag,msg);
			return 0;
		}

		public int i(string tag,string msg) {
			Debug.LogFormat(format,tag,msg);
			return 0;
		}

		public int d(string tag,string msg) {
			Debug.LogFormat(format,tag,msg);
			return 0;
		}

		public int w(string tag,string msg) {
			Debug.LogWarningFormat(format,tag,msg);
			return 0;
		}

		public int e(string tag,string msg) {
			Debug.LogErrorFormat(format,tag,msg);
			return 0;
		}

	}
}
