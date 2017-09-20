/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

public class SceneLigtingSetup : MonoBehaviour {

	public Color cameraBackground;
	[Space]
	public Material globalMaterial;
	[Space]
	public Material vegetationMaterial;
	public Transform vegetationRoot;
	[Space]
	public Material bridgeMaterial;
	public Transform bridgeRoot;
	[Space]
	public Material lookoutMaterial;
	public Transform lookoutRoot;
	[Space]
	public Material towerMaterial;
	public Transform towerRoot;
	[Space]
	public Material deadTreeMaterial;
	public Transform deadTreeRoot;
	[Space]
	public Material rocksMaterial;
	public Transform rocksTreeRoot;
	[Space]
	public GameObject[] objectsToEnable;

	private Renderer[] sceneRenderers;
	private Renderer[] vegetationRenderers;
	private Renderer[] bridgeRenderers;
	private Renderer[] lookoutRenderers;
	private Renderer[] towerRenderers;
	private Renderer[] deadTreeRenderers;
	private Renderer[] boatRenderers;
	private Renderer[] rocksRenderers;

	public void Apply (GameObject root) {
		SetupCameraBG ();
		EnableObjects ();
		ApplyGlobalMaterial (root);
		ApplyMaterial (vegetationRoot,ref vegetationRenderers, vegetationMaterial);
		ApplyMaterial (bridgeRoot,ref bridgeRenderers, bridgeMaterial);
		ApplyMaterial (lookoutRoot,ref lookoutRenderers, lookoutMaterial);
		ApplyMaterial (towerRoot ,ref towerRenderers, towerMaterial);
		ApplyMaterial (deadTreeRoot,ref deadTreeRenderers, deadTreeMaterial);
		ApplyMaterial (rocksTreeRoot,ref rocksRenderers, rocksMaterial);
	}

	private void SetupCameraBG() {
		Camera.main.clearFlags = CameraClearFlags.Color;
		Camera.main.backgroundColor = cameraBackground;
	}

	private void EnableObjects() {
		foreach (GameObject objectToEnable in objectsToEnable) 
			objectToEnable.SetActive (true);
	}

	private void ApplyGlobalMaterial(GameObject root) {
		if (sceneRenderers == null) 
			sceneRenderers = root.GetComponentsInChildren<Renderer> ();

		foreach (Renderer sceneRenderer in sceneRenderers) 
			sceneRenderer.sharedMaterial = globalMaterial;
	}

	private void ApplyMaterial(Transform root, ref Renderer[] renderers, Material material) {
		if (root == null)
			return;

		if (root != null && renderers == null) 
			renderers = root.GetComponentsInChildren<Renderer> ();
		
		foreach (Renderer objRenderer in renderers) 
			objRenderer.sharedMaterial = material;
		
	}
	
	public void DisableObjects () {
		foreach (GameObject objectToEnable in objectsToEnable)
			objectToEnable.SetActive (false);
	}
}
