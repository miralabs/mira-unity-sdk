/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

namespace FlatLighting {
	[RequireComponent(typeof(Camera))]
	public class MouseOrbit : MonoBehaviour {

		[Header ("General")]
		[Tooltip("The initial target.")]
		public Transform target;
		public float yPosClamp = 0.2f;
		[Tooltip("The speed to change the focus of the camera when a new target is selected.")]
		public float targetFocusLerpSpeed = 12;
		public float initialDistance = 40.0f;

		[Header ("Rotation")]
		[Tooltip("The rotation smooth factor.")]
		public float smooth = 8f;
		public float xSpeed = 250;
		public float ySpeed = 120;

		[Header ("Movement")]
		public float wasdSpeed = 0.5f;
		public float wasdSmooth = 6f;
		[Tooltip("The speed when moving with the mouse.")]
		public float panSpeed = 0.3f;

		[Header ("Zoom")]
		public float zoomSmooth = 10f;
		public float minZoom = 1.25f;
		public float maxZoom = 100;
		public float mouseSensitivityScaler = 1;
		public float scrollSensitivityScaler = 1;

		private float distance = 10;
		private float yMaxLimit = 80f;
		private float yMinLimit = -20f;

		private float smoothY;
		private float smoothX;
		private Vector3 movementPOS;
		private Quaternion rotation;
		private Vector3 position;
		private float clickTimey;
		private Transform myTransform;
		private Vector3 posToBe;
		private float zoomSmoothDelegate;
		private float x;
		private float y;
		private Vector3 movementPosOffset = Vector3.zero;

		private bool isOrthographic;
		private Camera myCamera;

		public static float ClampAngle (float angle, float min, float max) {
			while (angle < -360f) {
				angle += 360f;
			}
			while (angle > 360f) {
				angle -= 360f;
			}
			return Mathf.Clamp (angle, min, max);
		}
			
		void Awake () {
			this.myTransform = this.transform;
			myCamera = GetComponent<Camera>();
			isOrthographic = myCamera.orthographic;

			this.distance = initialDistance;
		}

		public void LateUpdate () {
			this.clickTimey -= Time.deltaTime;
			if (this.target == null) {
				Debug.LogWarning("There is no target setup.");
			}
			if (Input.GetKey(KeyCode.Mouse2)) {
				this.clickTimey = 0.3f;
			}
			else {
				if (Input.GetKey(KeyCode.Mouse2) && this.clickTimey > 0f) {
					this.GetTarget ();
				}
			}

			if (this.target) {
				if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse0)) { //left and right mouse buttons
					CalculateRotation();
				}
				if (Input.GetKey(KeyCode.Mouse2)) { // middle button
					CalculatePan();
				}

				SmoothRotation();
				CalculateZoom();
				CalculateMovement();

				this.myTransform.rotation = this.rotation;
				this.myTransform.position = this.position;
			}
		}

		private void GetTarget () {
			Vector3 mousePosition = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (mousePosition.x, mousePosition.y, (float)0));
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast (ray, out raycastHit, Mathf.Infinity)) {
				Renderer targetRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (targetRenderer) {
					this.target = raycastHit.collider.transform;
					this.movementPosOffset = Vector3.zero;
				}
			}
		}

		private void CalculateRotation() {
			this.x += Input.GetAxis ("Mouse X") * this.xSpeed * 0.02f * this.mouseSensitivityScaler;
			this.y -= Input.GetAxis ("Mouse Y") * this.ySpeed * 0.02f * this.mouseSensitivityScaler;
			this.y = Mathf.Clamp (this.y, this.yMinLimit + 0.01f, this.yMaxLimit - 0.01f);
		}

		private void SmoothRotation() {
			this.smoothX = Mathf.Lerp (this.smoothX, this.x, Time.deltaTime * this.smooth); //smoothing
			this.smoothY = Mathf.Lerp (this.smoothY, this.y, Time.deltaTime * this.smooth);
			this.smoothY = MouseOrbit.ClampAngle (this.smoothY, this.yMinLimit, this.yMaxLimit);
			this.rotation = Quaternion.Euler (this.smoothY, this.smoothX, 0f);

//			this.y = MouseOrbit.ClampAngle (this.y, this.yMinLimit, this.yMaxLimit); //without smooothing
//			this.rotation = Quaternion.Euler (this.y, this.x, 0f);
		}

		private void CalculatePan() {
			this.movementPosOffset -= this.myTransform.right * Input.GetAxis ("Mouse X") * this.panSpeed * this.mouseSensitivityScaler;
			this.movementPosOffset -= this.myTransform.up * Input.GetAxis ("Mouse Y") * this.panSpeed * this.mouseSensitivityScaler;
		}

		private void CalculateMovement() {
			bool rightKey = Input.GetKey ("d") || Input.GetKey("right");
			bool leftKey = Input.GetKey ("a") || Input.GetKey("left");
			bool upKey = Input.GetKey ("w") || Input.GetKey("up");
			bool downKey = Input.GetKey ("s") || Input.GetKey("down");
			Vector3 right = this.myTransform.right;
			if (rightKey || leftKey) {
				this.movementPosOffset += right * this.wasdSpeed * (float)((!rightKey || !leftKey) ? ((!rightKey) ? -1 : 1) : 0);
			}
			if (upKey || downKey) {
				this.movementPosOffset += Vector3.Cross (right, Vector3.up) * this.wasdSpeed * (float)((!upKey || !downKey) ? ((!upKey) ? -1 : 1) : 0);
			}

			this.posToBe = Vector3.Lerp (this.posToBe, this.target.position, Time.deltaTime * this.targetFocusLerpSpeed);
			this.movementPOS = Vector3.Lerp (this.movementPOS, this.movementPosOffset, Time.deltaTime * this.wasdSmooth);
			this.position = this.rotation * new Vector3 (0f, 0f, -this.distance) + this.posToBe + this.movementPOS; //smooth
//			this.position = this.rotation * new Vector3 (0f, 0f, -this.distance) + this.posToBe + this.movementPosOffset; // without smoothing
		}

		private void CalculateZoom() {
			this.zoomSmoothDelegate = Mathf.Lerp (this.zoomSmoothDelegate, Input.GetAxis ("Mouse ScrollWheel") * 10f * (this.distance / 10f) * this.scrollSensitivityScaler, Time.deltaTime * this.zoomSmooth);
			this.distance = Mathf.Clamp (this.distance - this.zoomSmoothDelegate, this.minZoom, this.maxZoom);

			if (isOrthographic) {
				myCamera.orthographicSize = this.distance*0.5f;
			}
		}
	}
}