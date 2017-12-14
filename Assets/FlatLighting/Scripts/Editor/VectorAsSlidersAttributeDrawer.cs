/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using UnityEditor;
using System;

namespace FlatLighting {
	[CustomPropertyDrawer(typeof(VectorAsSlidersAttribute))]
	public class VectorAsSlidersAttributeDrawer : PropertyDrawer {

		private VectorAsSlidersAttribute _attributeValue = null;
		private VectorAsSlidersAttribute attributeValue	{
			get {
				if (_attributeValue == null) {
					_attributeValue = (VectorAsSlidersAttribute) attribute;
				}
				return _attributeValue;
			}
		}

		private const float TITLE_HEIGHT = 16.0f;
		private const float SLIDER_HEIGHT = 16.0f;
		private const float SLIDER_SPACE_HEIGHT = 2.0f;
		private const float SLIDER_LABEL_WITDH = 100.0f;
		private int VECTOR_DIMENSIONS = 3;

		private GUIStyle sliderLabelStyle = new GUIStyle(EditorStyles.label);

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return TITLE_HEIGHT + VECTOR_DIMENSIONS * (SLIDER_HEIGHT + SLIDER_SPACE_HEIGHT) + 10.0f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			SerializedProperty vector = property;
			Vector4 targetVectorValue = CreateVectorWithProperDimensions(vector);

			SetupSliderLabelStyle();

			DrawPropertyHeader(position);

			for (int i=0 ; i < VECTOR_DIMENSIONS; i++) {
				DrawVectorSlider(position, ref targetVectorValue, i);
			}

			vector.vector4Value = targetVectorValue;
		}

		private Vector4 CreateVectorWithProperDimensions(SerializedProperty vector) {
			Vector4 targetVectorValue;
			int vectorTypeDimensions = 0;

			if (vector.propertyType == SerializedPropertyType.Vector2) {
				vectorTypeDimensions = 2;
			} else if (vector.propertyType == SerializedPropertyType.Vector3) {
				vectorTypeDimensions = 3;
			} else if (vector.propertyType == SerializedPropertyType.Vector4) {
				vectorTypeDimensions = 4;
			} else {
				throw new Exception("Please use \"VectorAsSliders\" annotation only on Vector2, Vector3 or Vector4 types.");
			}
				
			if (vectorTypeDimensions == 2) {
				targetVectorValue = new Vector4(vector.vector2Value.x, vector.vector2Value.y, 0.0f, 0.0f);
			} else if (vectorTypeDimensions == 3) {
				if (attributeValue.dimensions == 2) {
					VECTOR_DIMENSIONS = 2;
					targetVectorValue = new Vector4(vector.vector3Value.x, vector.vector3Value.y, 0.0f, 0.0f);
				} else {
					VECTOR_DIMENSIONS = 3;
					targetVectorValue = new Vector4(vector.vector3Value.x, vector.vector3Value.y, vector.vector3Value.z, 0.0f);
				}
			} else {
				if (attributeValue.dimensions == 2) {
					VECTOR_DIMENSIONS = 2;
					targetVectorValue = new Vector4(vector.vector4Value.x, vector.vector4Value.y, 0.0f, 0.0f);
				} else if (attributeValue.dimensions == 3) {
					VECTOR_DIMENSIONS = 3;
					targetVectorValue = new Vector4(vector.vector4Value.x, vector.vector4Value.y, vector.vector4Value.z, 0.0f);
				} else {
					VECTOR_DIMENSIONS = 4;
					targetVectorValue = vector.vector4Value;
				}
			}

			return targetVectorValue;
		}
			
		private void DrawPropertyHeader (Rect position) {
			Rect textFieldPosition = position;
			textFieldPosition.height = TITLE_HEIGHT;
			EditorGUI.LabelField(textFieldPosition, new GUIContent(attributeValue.label));
		}

		private void SetupSliderLabelStyle() {
			sliderLabelStyle.alignment = TextAnchor.MiddleRight;
			sliderLabelStyle.stretchWidth = true;
			sliderLabelStyle.richText = true;
			sliderLabelStyle.padding = new RectOffset(0, 10, 0, 0);
		}

		private void DrawVectorSlider(Rect position, ref Vector4 targetVectorValue, int index) {
			Rect rowPosition = position;
			rowPosition.y += ((1 + index) * SLIDER_HEIGHT) + index * SLIDER_SPACE_HEIGHT;
			rowPosition.height = SLIDER_HEIGHT;

			Rect labelSliderPosition = rowPosition;
			labelSliderPosition.width = SLIDER_LABEL_WITDH;
			EditorGUI.LabelField(labelSliderPosition, new GUIContent("<b>" + (1+index).ToString() + "</b>"), sliderLabelStyle);

			Rect vectorSliderPosition = rowPosition;
			vectorSliderPosition.x += SLIDER_LABEL_WITDH;
			vectorSliderPosition.width = vectorSliderPosition.width - SLIDER_LABEL_WITDH;

			targetVectorValue[index] = EditorGUI.Slider(vectorSliderPosition, targetVectorValue[index], attributeValue.min, attributeValue.max);				
		}
	}
}
