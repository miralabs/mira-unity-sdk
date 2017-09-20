//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System;
using System.Reflection;

public class ReflectUtil {

	public const string
		 FLAG_TYPE="[Type]"
		,FLAG_METHOD="[Method]"
		,FLAG_PROPERTY="[Property]"
	;

	public const BindingFlags sBindingFlags=unchecked((BindingFlags)0xffffffff);

	public class MemberEntry {
		public Type type;
		public string memberType;
		// Value
		public Object objectValue;
		public MethodInfo methodValue;
		public PropertyInfo propertyValue;
		public Delegate delegateValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	public static T ParseDelegate<T>(string s) where T :class{
		MemberEntry member=ParseMember(s);
		if(member!=null) {
			if(member.delegateValue==null) {
				if(member.memberType==FLAG_PROPERTY){
					if(member.propertyValue==null){return null;}
					member.methodValue=member.propertyValue.GetGetMethod(true);
				}
				try {
					return (Delegate.CreateDelegate(typeof(T),member.objectValue,member.methodValue,true)) as T;
				}catch(Exception e) {
					/*...*/
				}
			} else {
				return member.delegateValue as T;
			}
		}
		return default(T);
	}

	/// <summary>
	/// 
	/// </summary>
	public static MemberEntry ParseMember(string s) {
		string[] args=s.Split('.');
		int i=0,imax=args.Length;
		return ParseMember(args,ref i,imax);
	}
	
	/// <summary>
	/// 
	/// </summary>
	public static string GetFullType(string[] args,ref int offset,int count) {
		System.Text.StringBuilder sb=new System.Text.StringBuilder();
		bool isFirst=true;
		while(offset<count) {
			if(!args[offset].StartsWith("[")) {
				if(isFirst) {
					isFirst=false;
				}else {
					sb.Append(".");
				}
				sb.Append(args[offset]);
				++offset;
			}else {
				break;
			}
		}
		return sb.ToString();
	}

	/// <summary>
	/// 
	/// </summary>
	public static MemberEntry ParseMember(string[] args,ref int offset,int count) {
		string lastCmd="";
		Type currentType=null;
		Object currentObject=null;
		MethodInfo currentMethod=null;
		PropertyInfo currentProperty=null;
		Delegate currentDelegate=null;
		while(offset<count) {
			switch(args[offset++]) {
				case FLAG_TYPE:
					try {
						currentType=Type.GetType(GetFullType(args,ref offset,count),true);
						lastCmd=FLAG_TYPE;
					}catch(Exception e) {
						/*...*/
						goto label_fail_get_member;
						//break;
					}
				break;
				case FLAG_METHOD:
					try {
						switch(lastCmd) {
							case FLAG_TYPE:
								currentMethod=currentType.GetMethod(args[offset++],sBindingFlags);
							break;
							case FLAG_METHOD:
								currentObject = currentMethod.Invoke(currentObject,null);
								currentType   = currentObject.GetType();
								currentMethod = currentType.GetMethod(args[offset++],sBindingFlags);
							break;
							case FLAG_PROPERTY:
								currentObject = currentProperty.GetValue(currentObject,null);
								currentType   = currentObject.GetType();
								currentMethod = currentType.GetMethod(args[offset++],sBindingFlags);
							break;
						}
						lastCmd=FLAG_METHOD;
					}catch(Exception e) {
						/*...*/
						goto label_fail_get_member;
						//break;
					}
				break;
				case FLAG_PROPERTY:
					try {
						switch(lastCmd) {
							case FLAG_TYPE:
								currentProperty=currentType.GetProperty(args[offset++],sBindingFlags);
							break;
							case FLAG_METHOD:
								currentObject   = currentMethod.Invoke(currentObject,null);
								currentType     = currentObject.GetType();
								currentProperty = currentType.GetProperty(args[offset++],sBindingFlags);
							break;
							case FLAG_PROPERTY:
								currentObject   = currentProperty.GetValue(currentObject,null);
								currentType     = currentObject.GetType();
								currentProperty = currentType.GetProperty(args[offset++],sBindingFlags);
							break;
						}
						lastCmd=FLAG_PROPERTY;
					}catch(Exception e) {
						/*...*/
						goto label_fail_get_member;
						//break;
					}
				break;
				// 
				case "==null":
					switch(lastCmd) {
						case FLAG_METHOD:
							currentDelegate=new Func<bool>(()=>(currentMethod.Invoke(currentObject,null)==null));
						break;
						case FLAG_PROPERTY:
							currentDelegate=new Func<bool>(()=>(currentProperty.GetValue(currentObject,null)==null));
						break;
					}
					lastCmd="";
				break;
				case "!=null":
					switch(lastCmd) {
						case FLAG_METHOD:
							currentDelegate=new Func<bool>(()=>(currentMethod.Invoke(currentObject,null)!=null));
						break;
						case FLAG_PROPERTY:
							currentDelegate=new Func<bool>(()=>(currentProperty.GetValue(currentObject,null)!=null));
						break;
					}
					lastCmd="";
				break;
			}
		}
		return new MemberEntry{
			memberType     = lastCmd,
			type           = currentType,
			objectValue    = currentObject,
			methodValue    = currentMethod,
			propertyValue  = currentProperty,
			delegateValue  = currentDelegate,
		};
label_fail_get_member:
		return null;
	}
}
