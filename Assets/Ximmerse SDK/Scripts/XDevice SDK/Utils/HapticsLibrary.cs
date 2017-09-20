using System.Collections.Generic;
using UnityEngine;

public class HapticsLibrary:MonoBehaviour {

	#region Static
	
	protected static HapticsLibrary s_Instance;
	protected static bool s_InstanceCached=false;

	public static HapticsLibrary instance {
		get {
			if(!s_InstanceCached) {//
				s_InstanceCached=true;
				if(s_Instance==null) {
					s_Instance=Resources.Load<HapticsLibrary>("Input/Default Haptics Library");
				}
			}
			return s_Instance;
		}
	}

	public static HapticsDesc GetDesc(string key) {
		HapticsLibrary lib=instance;
		if(lib!=null) {
			if(lib.hapticsDescs.ContainsKey(key)) {
				return lib.hapticsDescs[key];
			}
		}
		return null;
	}

	#endregion Static

	#region Nested Types

	[System.Serializable]
	public class HapticsDesc{
		public string key;
		public int intValue0;
		public int intValue1;
		public Object objValue;
	}

#if UNITY_EDITOR
	[UnityEditor.CustomPropertyDrawer(typeof(HapticsDesc))]
	public class HapticsDescDrawer:SimplePropertyDrawer{
		public override void OnGUI(Rect position,UnityEditor.SerializedProperty property,GUIContent label) {
			if(fields==null) {
				numFields=4;
				fields=new string[4]{"key","objValue","intValue0","intValue1"};
			}
			base.OnGUI(position,property,label);
		}
	}
#endif

	#endregion Nested Types

	#region Fields

	[SerializeField]protected HapticsDesc[] m_HapticsDescs=new HapticsDesc[1]{
		new HapticsDesc{key="Default",intValue0=20}
	};
	public Dictionary<string,HapticsDesc> hapticsDescs;

	#endregion Fields

	#region Unity Messages

	protected virtual void Awake() {
		int i=0,imax=m_HapticsDescs.Length;
		hapticsDescs=new Dictionary<string, HapticsDesc>(imax);
		for(;i<imax;++i) {
			hapticsDescs.Add(m_HapticsDescs[i].key,m_HapticsDescs[i]);
		}
	}

	#endregion Unity Messages

}