/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

public class SpinAnimation : MonoBehaviour {

	void Update () {
		transform.RotateAround(transform.position, transform.up + transform.right, Time.deltaTime * 90f);
	}
}
