/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730



using UnityEngine;
using System.Collections;

namespace FlatLighting {
	[RequireComponent(typeof(Renderer))]
	public class MaterialBlender : MonoBehaviour {

		public float durationSeconds;
		public bool loop;
		public Material[] materials;

		Material internalMaterial;

		void Start () {
			GenerateInternalMaterial();	
			StartCoroutine(UpdateBlending());
		}

		void GenerateInternalMaterial() {
			internalMaterial = new Material(Shader.Find(Constants.FlatLightingShaderPath));
			Renderer myRenderer = GetComponent<Renderer>();
			myRenderer.material = internalMaterial;
		}
		
		IEnumerator UpdateBlending() {
			if(materials.Length < 2)
			{
				Debug.Log("Less than 2 materials. Cannot interpolate");
				yield break;
			}

			int currentMaterialIndex = 0;
			int nextMaterialIndex = 1;
			for(int i = nextMaterialIndex; i < materials.Length; i = nextMaterialIndex)
			{
				Material currentMaterial = materials[currentMaterialIndex];
				Material nextMaterial = materials[nextMaterialIndex];

				CheckMaterialCompatibility(currentMaterial, nextMaterial);
//				Debug.Log("interpolating between : " + currentMaterial.name + " and " + nextMaterial.name);

				for(float t = 0.0f; t <= 1; t += Time.deltaTime / durationSeconds)
				{
					InterpolateMaterial(currentMaterial, nextMaterial, t);
					yield return null;
				}

				DisableUnusedKeywords(nextMaterial);

				currentMaterialIndex = nextMaterialIndex;
				nextMaterialIndex++;

				if(loop && nextMaterialIndex == materials.Length)
				{
					nextMaterialIndex = 0;
				}
			}
		}

		void CheckMaterialCompatibility(Material currentMaterial, Material nextMaterial)
		{
			//Lighting Axis
			if(currentMaterial.IsKeywordEnabled(Constants.Shader.AXIS_COLORS_LOCAL) &&
			    nextMaterial.IsKeywordEnabled(Constants.Shader.AXIS_COLORS_GLOBAL))
			{
				Debug.Log("Material " + currentMaterial.name + " has Lighting space set to local, but material " + 
					nextMaterial.name + " has Lighting space set to Global. Interpolation may be wrong.");
			}

			if(currentMaterial.IsKeywordEnabled(Constants.Shader.AXIS_COLORS_GLOBAL) &&
				nextMaterial.IsKeywordEnabled(Constants.Shader.AXIS_COLORS_LOCAL))
			{
				Debug.Log("Material " + currentMaterial.name + " has Lighting space set to global, but material " + 
					nextMaterial.name + " has Lighting space set to Local. Interpolation may be wrong.");
			}

			//Gradient Fog
			if(currentMaterial.IsKeywordEnabled(Constants.Shader.GRADIENT_LOCAL_KEYWORD) &&
				nextMaterial.IsKeywordEnabled(Constants.Shader.GRADIENT_WORLD_KEYWORD))
			{
				Debug.Log("Material " + currentMaterial.name + " has Gradient space set to local, but material " + 
					nextMaterial.name + " has Gradient space set to Global. Interpolation may be wrong.");
			}

			if(currentMaterial.IsKeywordEnabled(Constants.Shader.GRADIENT_WORLD_KEYWORD) &&
				nextMaterial.IsKeywordEnabled(Constants.Shader.GRADIENT_LOCAL_KEYWORD))
			{
				Debug.Log("Material " + currentMaterial.name + " has Gradient space set to global, but material " + 
					nextMaterial.name + " has Gradient space set to Local. Interpolation may be wrong.");
			}

			if(!currentMaterial.shader.name.Contains(Constants.FlatLightingShaderPath))
			{
				Debug.Log("Material " + currentMaterial.name + ": Currently only shader " + Constants.FlatLightingShaderPath + " is supported");
			}

			if(!nextMaterial.shader.name.Contains(Constants.FlatLightingShaderPath))
			{
				Debug.Log("Material " + currentMaterial.name + ": Currently only shader " + Constants.FlatLightingShaderPath + " is supported");
			}
		}

		void DisableUnusedKeywords(Material nextMaterial) {

			if(nextMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD))
			{
				internalMaterial.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
				internalMaterial.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);
			} else
			{
				internalMaterial.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
				internalMaterial.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);
			}

			if(!nextMaterial.IsKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD))
			{
				internalMaterial.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD);
			}

			if(!nextMaterial.IsKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD))
			{
				internalMaterial.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD);
			}

			if(!nextMaterial.IsKeywordEnabled(Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD))
			{
				internalMaterial.DisableKeyword(Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD);
			}
		}

		void InterpolateMaterial(Material currentMaterial, Material nextMaterial, float t) {
			InterpolateLightAxis(currentMaterial, nextMaterial, t);

			//TODO: space keyword local/world is special. Consider interpolating between local and world
			InterpolateMaterialColorWithKeyword(currentMaterial, nextMaterial, t, "_GradienColorGoal", Constants.Shader.GRADIENT_WORLD_KEYWORD);
			InterpolateMaterialColorWithKeyword(currentMaterial, nextMaterial, t, "_GradienColorGoal", Constants.Shader.GRADIENT_LOCAL_KEYWORD);
		}

		void InterpolateLightAxis(Material currentMaterial, Material nextMaterial, float t)
		{
			InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveX);
			InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveY);
			InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveZ);

			InterpolateSymmetricAxisLights(currentMaterial, nextMaterial, t);
			InterpolateGradientAxisLights(currentMaterial, nextMaterial, t); //Note: time order important
		}

		void InterpolateSymmetricAxisLights(Material currentMaterial, Material nextMaterial, float t)
		{
			if(nextMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
			{
				internalMaterial.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
				internalMaterial.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);

				if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
				{
					InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeX);
					InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeY);
					InterpolateMaterialColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeZ);
				}
				else
				{
					InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveX, Constants.Shader.LightNegativeX);
					InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveY, Constants.Shader.LightNegativeY);
					InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightPositiveZ, Constants.Shader.LightNegativeZ);
				}
			}
			else
			{
				if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
				{
					internalMaterial.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
					internalMaterial.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);

					InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeX, Constants.Shader.LightPositiveX);
					InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeY, Constants.Shader.LightPositiveY);
					InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, Constants.Shader.LightNegativeZ, Constants.Shader.LightPositiveZ);
				}
				else
				{
					internalMaterial.DisableKeyword(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD);
					internalMaterial.EnableKeyword(Constants.Shader.SYMETRIC_COLORS_ON_KEYWORD);
				}
			}
		}

		void InterpolateGradientAxisLights(Material currentMaterial, Material nextMaterial, float t)
		{
			InterpolateGradientAxis(currentMaterial, nextMaterial, t, 
				Constants.Shader.AXIS_GRADIENT_ON_X_KEYWORD,
				Constants.Shader.LightPositiveX,
				Constants.Shader.LightPositive2X,
				Constants.Shader.LightNegativeX,
				Constants.Shader.LightNegative2X,
				Constants.Shader.GradientWidthPositiveX,
				Constants.Shader.GradientWidthNegativeX,
				Constants.Shader.GradientOriginOffsetPositiveX,
				Constants.Shader.GradientOriginOffsetNegativeX
			);

			InterpolateGradientAxis(currentMaterial, nextMaterial, t, 
				Constants.Shader.AXIS_GRADIENT_ON_Y_KEYWORD,
				Constants.Shader.LightPositiveY,
				Constants.Shader.LightPositive2Y,
				Constants.Shader.LightNegativeY,
				Constants.Shader.LightNegative2Y,
				Constants.Shader.GradientWidthPositiveY,
				Constants.Shader.GradientWidthNegativeY,
				Constants.Shader.GradientOriginOffsetPositiveY,
				Constants.Shader.GradientOriginOffsetNegativeY
			);

			InterpolateGradientAxis(currentMaterial, nextMaterial, t, 
				Constants.Shader.AXIS_GRADIENT_ON_Z_KEYWORD,
				Constants.Shader.LightPositiveZ,
				Constants.Shader.LightPositive2Z,
				Constants.Shader.LightNegativeZ,
				Constants.Shader.LightNegative2Z,
				Constants.Shader.GradientWidthPositiveZ,
				Constants.Shader.GradientWidthNegativeZ,
				Constants.Shader.GradientOriginOffsetPositiveZ,
				Constants.Shader.GradientOriginOffsetNegativeZ
			);
		}

		void InterpolateGradientAxis(Material currentMaterial, Material nextMaterial, float t, 
			string gradientKeyword,
			string LightPositive,
			string LightPositive2,
			string LightNegative,
			string LightNegative2,
			string GradientWidthPositive,
			string GradientWidthNegative,
			string GradientOriginOffsetPositive,
			string GradientOriginOffsetNegative)
		{
			if(nextMaterial.IsKeywordEnabled(gradientKeyword))
			{
				internalMaterial.EnableKeyword(gradientKeyword);

				if(currentMaterial.IsKeywordEnabled(gradientKeyword))
				{
					InterpolateMaterialColor(currentMaterial, nextMaterial, t, LightPositive2);
					InterpolateMaterialFloat(currentMaterial, nextMaterial, t, GradientWidthPositive);
					InterpolateMaterialFloat(currentMaterial, nextMaterial, t, GradientOriginOffsetPositive);

					if(nextMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							InterpolateMaterialColor(currentMaterial, nextMaterial, t, LightNegative2);
							InterpolateMaterialFloat(currentMaterial, nextMaterial, t, GradientWidthNegative);
							InterpolateMaterialFloat(currentMaterial, nextMaterial, t, GradientOriginOffsetNegative);
						} else
						{
							InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, LightPositive2, LightNegative2);
							internalMaterial.SetFloat(GradientWidthNegative, nextMaterial.GetFloat(GradientWidthNegative));
							internalMaterial.SetFloat(GradientOriginOffsetNegative, nextMaterial.GetFloat(GradientOriginOffsetNegative));

						}
					} else
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, LightNegative2, LightPositive2);
						}
					}
				}
				else
				{
					InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, LightPositive, LightPositive2);
					internalMaterial.SetFloat(GradientWidthPositive, nextMaterial.GetFloat(GradientWidthPositive));
					internalMaterial.SetFloat(GradientOriginOffsetPositive, nextMaterial.GetFloat(GradientOriginOffsetPositive));

					if(nextMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, LightNegative, LightNegative2);
							internalMaterial.SetFloat(GradientWidthNegative, nextMaterial.GetFloat(GradientWidthNegative));
							internalMaterial.SetFloat(GradientOriginOffsetNegative, nextMaterial.GetFloat(GradientOriginOffsetNegative));
						} else
						{
							InterpolateNextPropertyColor(currentMaterial, nextMaterial, t, LightPositive, LightNegative2);
							internalMaterial.SetFloat(GradientWidthNegative, nextMaterial.GetFloat(GradientWidthNegative));
							internalMaterial.SetFloat(GradientOriginOffsetNegative, nextMaterial.GetFloat(GradientOriginOffsetNegative));
						}
					} else
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							Color current = currentMaterial.GetColor(LightNegative);
							Color next = nextMaterial.GetColor(LightPositive2);
							internalMaterial.SetColor(LightNegative2, Interpolate(current, next, t));

							internalMaterial.SetFloat(GradientWidthNegative, nextMaterial.GetFloat(GradientWidthPositive));
							internalMaterial.SetFloat(GradientOriginOffsetNegative, nextMaterial.GetFloat(GradientOriginOffsetPositive));
						} 
					}
				}
			}
			else
			{
				if(currentMaterial.IsKeywordEnabled(gradientKeyword))
				{
					internalMaterial.EnableKeyword(gradientKeyword);

					InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, LightPositive2, LightPositive);

					if(nextMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, LightNegative2, LightNegative);
						} else
						{
							InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, LightPositive2, LightNegative);
							Color current = currentMaterial.GetColor(LightPositive2);
							Color next = nextMaterial.GetColor(LightNegative);
							internalMaterial.SetColor(LightNegative2, Interpolate(current, next, t));
						}
					} else
					{
						if(currentMaterial.IsKeywordEnabled(Constants.Shader.SYMETRIC_COLORS_OFF_KEYWORD))
						{
							InterpolateCurrentPropertyColor(currentMaterial, nextMaterial, t, LightNegative2, LightPositive);
						}
					}
				}
				else
				{
					internalMaterial.DisableKeyword(gradientKeyword);
				}
			}
		}

		void InterpolateNextPropertyColor(Material currentMaterial, Material nextMaterial, float t , string currentProperty, string nextProperty)
		{
			Color currentColor = currentMaterial.GetColor(currentProperty);
			Color nextColor = nextMaterial.GetColor(nextProperty);
			Color interpolatedColor = Interpolate(currentColor, nextColor, t);
			internalMaterial.SetColor(nextProperty, interpolatedColor);
		}

		void InterpolateCurrentPropertyColor(Material currentMaterial, Material nextMaterial, float t , string currentProperty, string nextProperty)
		{
			Color currentColor = currentMaterial.GetColor(currentProperty);
			Color nextColor = nextMaterial.GetColor(nextProperty);
			Color interpolatedColor = Interpolate(currentColor, nextColor, t);
			internalMaterial.SetColor(currentProperty, interpolatedColor);
		}

		void InterpolateFromFloat(Material nextMaterial, float t, float initValue, string toProperty)
		{
			float nextFloat = nextMaterial.GetFloat(toProperty);
			float interpolatedFloat = Interpolate(initValue, nextFloat, t);
			internalMaterial.SetFloat(toProperty, interpolatedFloat);
		}

		void InterpolateMaterialColorWithKeyword(Material currentMaterial, Material nextMaterial, float t, string propertyName, string keyword) {
			if(nextMaterial.IsKeywordEnabled(keyword))
			{
				internalMaterial.EnableKeyword(keyword);
				InterpolateNextMaterialColor(currentMaterial, nextMaterial, t, propertyName, keyword);
			} else
			{
				if(currentMaterial.IsKeywordEnabled(keyword))
				{
					InterpolateCurrentMaterialColor(currentMaterial, nextMaterial, t, propertyName, keyword);
				} else
				{
					internalMaterial.DisableKeyword(keyword);
				}
			}
		}

		void InterpolateCurrentMaterialColor(Material mat1, Material mat2, float t, string propertyName, string keyword) {
			Color interpolatedColor;
			Color color1 = mat1.GetColor(propertyName);
			if(mat2.IsKeywordEnabled(keyword))
			{
				Color color2 = mat2.GetColor(propertyName);
				interpolatedColor = Interpolate(color1, color2, t);
			} else
			{
				interpolatedColor = color1;
				interpolatedColor.a = Interpolate(color1.a, 0, t);
			}

			internalMaterial.SetColor(propertyName, interpolatedColor);
		}

		void InterpolateNextMaterialColor(Material mat1, Material mat2, float t, string propertyName, string keyword) {
			Color interpolatedColor;
			Color color2 = mat2.GetColor(propertyName);
			if(mat1.IsKeywordEnabled(keyword))
			{
				Color color1 = mat1.GetColor(propertyName);
				interpolatedColor = Interpolate(color1, color2, t);
			} else
			{
				interpolatedColor = color2;
				interpolatedColor.a = Interpolate(0, color2.a, t);
			}

			internalMaterial.SetColor(propertyName, interpolatedColor);
		}

		void InterpolateMaterialColor(Material mat1, Material mat2, float t, string propertyName) {
			Color color1 = mat1.GetColor(propertyName);
			Color color2 = mat2.GetColor(propertyName);
			Color interpolatedColor = Interpolate(color1, color2, t);
			internalMaterial.SetColor(propertyName, interpolatedColor);
		}

		Color Interpolate(Color a, Color b, float t) {
			return Color.Lerp(a,b,t);
		}

		void InterpolateMaterialFloat(Material mat1, Material mat2, float t, string propertyName) {
			float float1 = mat1.GetFloat(propertyName);
			float float2 = mat2.GetFloat(propertyName);
			float interpolatedFloat = Interpolate(float1, float2, t);
			internalMaterial.SetFloat(propertyName, interpolatedFloat);
		}

		float Interpolate(float a, float b, float t) {
			return (1 - t) * a + t * b;
		}
	}
}