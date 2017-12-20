// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Downloading and/or using this MIRA SDK is under license from MIRA,
// and subject to all terms and conditions of the Mira SDK License Agreement,
// found here: https://www.mirareality.com/Mira_SDK_License_Agreement.pdf
//
// By downloading this SDK, you agree to the Mira SDK License Agreement.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.

using UnityEngine;

namespace Mira
{
    /// <summary>
    /// Creates the counter-distortion
    /// </summary>
    public class MiraPostRender : MonoBehaviour
    {
        /// <summary>
        /// Enum representing which eye this image corresponds to
        /// </summary>
        public enum Eye { Left, Right };

        /// <summary>
        /// The field of view of the Stereo Camera Rig
        /// </summary>
        private float stereoCamFov = 50f;

        [SerializeField]
        private
        /// <summary>
        /// The option to turn off distortion rendering (to see how messed up everything is)
        /// </summary>
        bool noDistortion = false;

        [SerializeField]
        private
        /// <summary>
        /// A tweakable to reduce the distortion effect
        /// </summary>
        bool postReduceDistortion = false;

        [SerializeField]
        private
        /// <summary>
        ///  Tweakable of the amount of distortion in X
        /// </summary>
        float xDistAmount = 1f;

        [SerializeField]
        private
        /// <summary>
        /// Tweakable of the amount of distortion in Y
        /// </summary>
        float yDistAmount = 1f;

        public bool tanCoordinates = false;
        public Eye eye = Eye.Right;

        // Resolution of mesh in X
        private int xSize = 20;

        // Resolution of mesh in Y
        private int ySize = 20;

        private float zOffset = 35;

        // Dimensions of mesh in X (total length, not halved)
        private float xScalar;

        // Dimensions of mesh in Y (total length, not halved)
        private float yScalar;

        private float tanConstant;

        public float desiredParallaxDist = 1.5f;
        private Mesh mesh;
        private Material renderTextureMaterial;
        private float IPD;

        // public float ParallaxShift = 1.89f;
        public float ParallaxShift = 0f;

        private double[] coefficientsXLeft;
        private double[] coefficientsYLeft;
        private double[] coefficientsXRight;
        private double[] coefficientsYRight;


        private DistortionEquation distortion;

        

        public void InitializeDistortion(float fieldOfView, float ipd)
        {
            distortion = new DistortionEquation();
            renderTextureMaterial = new Material(Shader.Find("Unlit/Texture"));
            if (eye == Eye.Left)
            {
                renderTextureMaterial.SetTexture("_MainTex", MiraArController.Instance.leftCam.GetComponent<Camera>().targetTexture);
            }
            else
            {
                renderTextureMaterial.SetTexture("_MainTex", MiraArController.Instance.rightCam.GetComponent<Camera>().targetTexture);
            }
            renderTextureMaterial.mainTextureScale = new Vector2(-1, 1);
            renderTextureMaterial.mainTextureOffset = new Vector2(1, 0);

            // Debug.Log("Field of view being set to: " + fieldOfView);
            stereoCamFov = fieldOfView;
            // Debug.Log("IPD is being set as: " + ipd);
            IPD = ipd;


            xScalar = stereoCamFov;
            yScalar = stereoCamFov;

            tanConstant = (0.5f * stereoCamFov) / (Mathf.Tan(0.5f * stereoCamFov * Mathf.Deg2Rad));

            CorrectParallax(stereoCamFov);
        }

        private void CorrectParallax(float stereoFov)
        {
            float defaultParallaxDist = 0.6096f;
            // For maximum convergence at infinity:
            float offsetAngle = 90 - Mathf.Atan(defaultParallaxDist / (IPD * 0.5f * 0.001f)) * Mathf.Rad2Deg;
            // Debug.Log("Offset Angle: " + offsetAngle);

            // For a custom maximum convergence planes:
            float convergenceAngle = 90 - Mathf.Atan(desiredParallaxDist / (IPD * 0.5f * 0.001f)) * Mathf.Rad2Deg;
            float hybridAngle = offsetAngle - convergenceAngle;
            // Debug.Log("Hybrid Angle: " + hybridAngle);

            ParallaxShift = hybridAngle;
        }

        public void RecalculateDistortion()
        {
            IPD = MiraArController.Instance.IPD;
            CorrectParallax(MiraArController.Instance.fieldOfView);
            DistortionMesh();
            
        }

        private void OnValidate()
        {
            // CorrectParallax(stereoCamFov);
            // DistortionMesh();
        }

        public void OnPostRender()
        {
            // set first shader pass of the material
            renderTextureMaterial.SetPass(0);
            // draw mesh
            if (eye == Eye.Left)
            {
                if (noDistortion == false)
                    Graphics.DrawMeshNow(mesh, new Vector3(0, 0, zOffset * 2), Quaternion.Euler(0, 180, 0));
                else
                    Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
            }
        }

        public void DistortionMesh()
        {
            // Debug.Log("Field of View: " + stereoCamFov);
            mesh = GetComponent<MeshFilter>().mesh = new Mesh();
            mesh.Clear();

            // CorrectParallax();

            Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
            Vector3[] verticesDistort = new Vector3[vertices.Length];
            Vector2[] uv = new Vector2[vertices.Length];

            float stepX = xScalar / xSize;
            float stepY = yScalar / ySize;

            int i = 0;
            for (int y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    // x coordinate = (stepX * x) - (half total mesh size)
                    // This centers the mesh so it matches input coordinates of the equation
                    vertices[i] = new Vector3((x * stepX) - (xScalar / 2),
                        (y * stepY) - (yScalar / 2),
                        zOffset);
                    if (eye == Eye.Left)
                    {
                        verticesDistort[i] = DistortionL(vertices[i].x, vertices[i].y);
                        vertices[i] = new Vector3(-vertices[i].x, vertices[i].y, zOffset); ;
                    }
                    if (eye == Eye.Right)
                    {
                        verticesDistort[i] = DistortionR(vertices[i].x, vertices[i].y);
                    }

                    vertices[i].z = zOffset;
                    verticesDistort[i].z = zOffset;
                    if (postReduceDistortion)
                    {
                        verticesDistort[i].x = Mathf.Lerp(vertices[i].x, verticesDistort[i].x, xDistAmount);
                        verticesDistort[i].y = Mathf.Lerp(vertices[i].y, verticesDistort[i].y, yDistAmount);
                    }

                    if (noDistortion)
                    {
                        if (eye == Eye.Left)
                            verticesDistort[i] = new Vector3(-vertices[i].x, vertices[i].y, vertices[i].z);
                        else
                            verticesDistort[i] = vertices[i];
                    }
                    // Debug.Log(vertices[i] + ", " + verticesDistort[i]);


                    uv[i] = new Vector2((float)x / xSize, (float)y / ySize);

                    i += 1;
                }
            }

            mesh.vertices = verticesDistort;
            mesh.uv = uv;

            int[] triangles = new int[xSize * ySize * 6];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }
            }
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        private Vector3 DistortionL(float theta, float phi)
        {
            theta = theta + ParallaxShift;
            if (tanCoordinates)
            {
                Debug.Log("Xcoord: " + theta + ", YCoord" + phi);
                // This assumes the render texture is square, you will need a different equation that takes into account aspect ratio for theta if not square
                theta = Mathf.Atan(theta / tanConstant) * Mathf.Rad2Deg;
                phi = Mathf.Atan(phi / tanConstant) * Mathf.Rad2Deg;
                Debug.Log("Theta: " + theta + ", Phi" + phi);
            }

            // Reference Compiled Function here:

            double[] result = distortion.RightDistortionCoordinate(-theta, phi, IPD);

            return new Vector3((float)result[0], (float)result[1]);
        }

        private Vector3 DistortionR(float theta, float phi)
        {
            theta = theta - ParallaxShift;
            if (tanCoordinates)
            {
                theta = Mathf.Atan(theta / tanConstant) * Mathf.Rad2Deg;
                phi = Mathf.Atan(phi / tanConstant) * Mathf.Rad2Deg;
            }

            double[] result = distortion.RightDistortionCoordinate(theta, phi, IPD);

            return new Vector3((float)result[0], (float)result[1]);
        }
    }
}