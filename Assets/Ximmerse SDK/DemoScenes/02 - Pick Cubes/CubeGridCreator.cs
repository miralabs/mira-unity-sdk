//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;

public class CubeGridCreator:MonoBehaviour {

	#region Fields

	public Transform root,defaultPrefab;
	public bool createOnAwake=true;
	public int count_x=1,count_y=1,count_z=1;
	public Vector3 offset=Vector3.one;

	#endregion Fields

	#region Methods

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake() {
		if(createOnAwake) {
			CreateCubes();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual Transform GetPrefab(int x,int y,int z) {
		return defaultPrefab;
	}

	/// <summary>
	/// 
	/// </summary>
	[ContextMenu("Create Cubes")]
	protected virtual void CreateCubes() {
		Transform t,prefab;
		Vector3 start=new Vector3(-(count_x-1)/2f*offset.x,-(count_y-1)/2f*offset.y,-(count_z-1)/2f*offset.z);

		int x,y,z;
		for(z=0;z<count_z;++z) {
		for(y=0;y<count_y;++y) {
		for(x=0;x<count_x;++x) {
			prefab=GetPrefab(x,y,z);
			if(prefab==null) continue;
			t=Object.Instantiate(prefab);
			t.SetParent(root);
			t.localPosition=start+new Vector3(offset.x*x,offset.y*y,offset.z*z);
			t.localRotation=prefab.localRotation;
			t.localScale=prefab.localScale;
		}}}
	}

	#endregion Methods

}