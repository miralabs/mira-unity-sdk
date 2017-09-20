/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class MaterialSwapper : MonoBehaviour {

	public Material[] materials;
	public GameObject UnityLightsRoot;
	public int UnityLightsMaterialIndex;
	public GameObject FlatLightsRoot;
	public int FlatLightsMaterialIndex;

	private Renderer myRenderer;

	void Start () {
		myRenderer= GetComponent<Renderer>();

		if (materials.Length > 0)
			myRenderer.sharedMaterial = materials[0];
	}
	
	public void SetMaterial(int option) {
		if (option > materials.Length || option < 0) {
			return;
		}

		myRenderer.sharedMaterial = materials[option];

		UnityLightsRoot.SetActive(UnityLightsMaterialIndex == option);
		FlatLightsRoot.SetActive(FlatLightsMaterialIndex == option);
	}
}
