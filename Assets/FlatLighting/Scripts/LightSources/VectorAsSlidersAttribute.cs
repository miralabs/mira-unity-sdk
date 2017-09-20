/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

namespace FlatLighting {
	public class VectorAsSlidersAttribute : PropertyAttribute {

		public readonly string label;
		public readonly float min;
		public readonly float max;
		public readonly int dimensions;

		public VectorAsSlidersAttribute(string label, float min, float max) {
			this.label = label;
			this.min = min;
			this.max = max;
			this.dimensions = -1;
		}

		public VectorAsSlidersAttribute(string label, int dimensions, float min, float max) {
			this.label = label;
			this.min = min;
			this.max = max;
			this.dimensions = dimensions;
		}
	}
}
