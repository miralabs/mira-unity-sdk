/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LightingManager : MonoBehaviour {

	public SceneLigtingSetup day;
	public SceneLigtingSetup night;
	public GameObject root;

	void Start() {
		SetLighting(0);
	}

	public void SetLighting(int option) {
		if (option == 0) {
			//day
			night.DisableObjects();
			day.Apply(root);
		} else {
			//night
			night.Apply(root);
			day.DisableObjects();
		} 
	}
}
