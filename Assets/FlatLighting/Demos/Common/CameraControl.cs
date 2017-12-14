/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {

	public Transform pointOfInterest;
	public bool shouldRotateAutomatic;
	[Range(10.0f, 30.0f)] 
	public float rotationSpeed = 20.0f; //a speed modifier

	[Space]
	public float minRadius = 20.0f;
	public float maxRadius = 60.0f;

	[Space]
	public float minZoom = 10.0f;
	public float maxZoom = 50.0f;

	private Camera myCamera;
	private Vector3 lastNormalizedPosition;
	private float radius;


	void Start () {
		myCamera = GetComponent<Camera>();
		radius = Vector3.Distance(transform.position, pointOfInterest.position);
		transform.LookAt(pointOfInterest.position);
	}
	
	void Update () {
		if (!shouldRotateAutomatic) {
			return;
		}

		RotateAutomatic();
	}

	private void RotateAutomatic() {
		transform.RotateAround (pointOfInterest.position, Vector3.up, Time.deltaTime * rotationSpeed);
		transform.LookAt(pointOfInterest.position);
	}

	public void ToggleAutomaticRotation(bool toggle) {
		shouldRotateAutomatic = toggle;
	}

	private Vector3 GetNormalizedCircularPosition(float degrees) {
		float value = Mathf.Lerp(0.0f, 360.0f*Mathf.Deg2Rad, degrees);
		float newZ = Mathf.Sin(value);
		float newX = Mathf.Cos(value);
		return new Vector3(newX, 0.0f, newZ);
	}

	public void RotateHorizontally(float amount) {
		lastNormalizedPosition = GetNormalizedCircularPosition(amount);
		Vector3 newPosition = lastNormalizedPosition * radius;
		newPosition.y = transform.position.y;

		transform.position = newPosition;
		transform.LookAt(pointOfInterest.position);
	}

	public void SetRadius(float amount) {
		radius = Mathf.Lerp(minRadius, maxRadius, amount);
		Vector3 newPosition = new Vector3(lastNormalizedPosition.x, 0.0f, lastNormalizedPosition.z) * radius;
		newPosition.y = transform.position.y;

		transform.position = newPosition;
		transform.LookAt(pointOfInterest.position);
	}

	public void Zoom(float amount) {
		float normalizedZoom = Mathf.Lerp(minZoom, maxZoom, amount);
		myCamera.orthographicSize = normalizedZoom;
	}
}
