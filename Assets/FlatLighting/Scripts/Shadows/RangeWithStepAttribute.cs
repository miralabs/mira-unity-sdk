/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;

namespace FlatLighting {
	public class RangeWithStepAttribute : PropertyAttribute {

		public readonly int min;
		public readonly int max;
		public readonly int step;

		public RangeWithStepAttribute(int min, int max, int step) {
			this.min = min;
			this.max = max;
			this.step = step;
		}

		public RangeWithStepAttribute(int min, int max) {
			this.step = 1;
			this.min = min;
			this.max = max;
		}
	}
}
