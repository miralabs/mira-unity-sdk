/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730


using UnityEngine;
using UnityEditor;
using System;

namespace FlatLighting {
	[CustomPropertyDrawer(typeof(RangeWithStepAttribute))]
	public class RangeWithStepAttributeDrawer : PropertyDrawer {

		private RangeWithStepAttribute _attributeValue = null;
		private RangeWithStepAttribute attributeValue	{
			get {
				if (_attributeValue == null) {
					_attributeValue = (RangeWithStepAttribute) attribute;
				}
				return _attributeValue;
			}
		}

		private const float SLIDER_HEIGHT = 16.0f;
		private const float SLIDER_SPACE_HEIGHT = 2.0f;

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			return (SLIDER_HEIGHT + SLIDER_SPACE_HEIGHT) + 10.0f;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				throw new Exception("Please use \"RangeWithStep\" annotation only on int types.");
			}
				
			DrawRangeWithStepSlider(position, property, label);
		}

		private void DrawRangeWithStepSlider(Rect position, SerializedProperty property, GUIContent label) {
			Rect rangeWithStepSliderPosition = position;
			rangeWithStepSliderPosition.height = SLIDER_HEIGHT;

			property.intValue = GetNearestMultiple(EditorGUI.IntSlider (rangeWithStepSliderPosition, label, property.intValue, attributeValue.min, attributeValue.max), attributeValue.step);
		}

		int GetNearestMultiple(int n, int step)
		{
			int stepToShift = (int)Math.Log (step, 2.0);
			int a;
			a = n - 1;
			a = a >> stepToShift;
			a = a + 1;
			return a << stepToShift;
		}
	}
}
