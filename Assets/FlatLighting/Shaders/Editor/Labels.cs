/// Credit to Bogdan Gochev for licensing this shader code to us. 
/// Usage of the Flat Lighting Shader from this SDK is licensed for Mira applications only.
/// Flat lighting asset: https://www.assetstore.unity3d.com/en/#!/content/67730

using UnityEngine;
using System.Collections;

namespace FlatLighting {
	public static class Labels  {
		public static string EmptyText = "";

		public static string LightmapHeader = "<b>Lightmapping</b>";
		public static string CustomLightmap = "Custom";
		public static string UnityLightmap = "Unity";
		public static string[] LightmapUVChannels =  {"UV 0", "UV 1"};
		public static string LightmapUVChannelsLabel = "UV channel: ";
		public static GUIContent UseLightmap = new GUIContent("Use Lightmapping", "Use a custom generated lightmap from an external tool or unity baked lightmap");
		public static string LightmapTexture = "Lightmap Texture";
		public static string ShadowTint = "Shadow Tint";
		public static string ShadowBoost = "Shadow Boost";

		public static GUIContent VertexColor = new GUIContent("Use Vertex Colors", "Use the vertex color channel to paint the model.");

		public static string LightingHeader = "<b>Lighting</b>";
		public static GUIContent UseSymmetrcialColors = new GUIContent("Use Symmetrical Colors", "Use only 3 Axis : +/-X, +/-Y, +/-Z");
		public static GUIContent UseAxisGradientXColors = new GUIContent("Use Gradient X Axis", "Use gradient between 2 colors instead of a single flat color.");
		public static GUIContent UseAxisGradientYColors = new GUIContent("Use Gradient Y Axis", "Use gradient between 2 colors instead of a single flat color.");
		public static GUIContent UseAxisGradientZColors = new GUIContent("Use Gradient Z Axis", "Use gradient between 2 colors instead of a single flat color.");
		public static string LightNegativeX = "Light -X";
		public static string LightPositiveX = "Light +X";
		public static string LightNegativeZ = "Light -Z";
		public static string LightPositiveZ = "Light +Z";
		public static string LightNegativeY = "Light -Y";
		public static string LightPositiveY = "Light +Y";
		public static string LightSecondColor = "Second Color";
		public static string LightAxisGradientOffset = "Origin Offset";
		public static string LightAxisGradientWidth = "Width";

		public static string MainTextureLabel = "Main Texture";

		public static string LightSourcesHeader = "<b>Light Sources</b>";
		public static GUIContent LightSourceBlend = new GUIContent("Light Source Blend", "How each Light Sources reacts when approaches another Light Source.");
		public static GUIContent[] LightSourceBlendEnumLabels =  { 
			new GUIContent("Individual", "Every Light Source has its own color,intensity and smoothness property"), 
			new GUIContent("Multiple", "Use same color,intensity and smoothness properies for every Light Source to create smooth blending ")
		};
		public static string BlendedLightColor = "Color";
		public static GUIContent BlendedLightIntensities = new GUIContent("Intensities", "The Light Source intensities (the same as in the per-light configuration).");
		public static GUIContent BlendedLightIntensities0 = new GUIContent("Intensity 0", "The inner most circle intensity.");
		public static GUIContent BlendedLightIntensities1 = new GUIContent("Intensity 1", "Middle circle intensity.");
		public static GUIContent BlendedLightIntensities2 = new GUIContent("Intensity 2", "Outer cyrcle intensity.");
		public static string BlendedLightSmoothness = "Smoothness";
		public static GUIContent AmbientLight = new GUIContent("Ambient Light");
		public static GUIContent DirectLight = new GUIContent("Directional Light");
		public static string GlobalDirectLight = "Global Directional Light";
		public static string GlobalDirectLightHelp = "Use Global Direct Light Source in the scene.";
		public static GUIContent SpotLight = new GUIContent("Spot Light");
		public static GUIContent PointLight = new GUIContent("Point Light");

		public static string GlobalGradientHeader = "<b>Global Gradient/Fog</b>";
		public static GUIContent UseGlobalGradient = new GUIContent("Use Global Gradient/Fog");
		public static string GlobalGradientColor = "Color";
		public static string GlobalGradientBlending = "Blending (Fog Effect)";
		public static GUIContent FogRange = new GUIContent("Range");
		public static string GlobalGradientOffset = "Origin Offset";
		public static string GlobalGradientWidth = "Width";
		public static string GlobalGradientAxis= "Axis";
		public static string[] GradientAxisEnumLabels =  {"X", "Y", "Z", "Free"};
		public static string GradientAxisX= "X";
		public static string GradientAxisY= "Y";
		public static string GradientAxisZ= "Z";

		public static GUIContent Space = new GUIContent("Space", "The Model/Object space or World that this settings is appliend.");
		public static GUIContent[] SpaceEnumLabels =  { new GUIContent("Local", "Local/Model/Object space"), new GUIContent("World", "World space")};

		public static GUIContent ToggleOn = new GUIContent("ON");
		public static GUIContent ToggleOff = new GUIContent("OFF");

		public static string ShadowsHeader = "<b>Custom Shadows</b>";
		public static GUIContent ReceiveShadowsHeader = new GUIContent("Receive", "Receive and show shadows from the custom shadow projector.");
		public static GUIContent CastShadowsHeader = new GUIContent("Cast", "Cast shadows that will be received by materials affected by the shadow projector.");

		public static GUIContent Alpha = new GUIContent("Alpha", "Transparency amount of this material.");
		public static GUIContent Wave = new GUIContent("Wave", "The wave settings.");

		public static GUIContent CelThreshold = new GUIContent("Cel Threshold");

		public static string MainTexture = "Main Texture";

		public static string HelpUseFlatLightingLightSources = "Use FlatLighting light sources in order to view results.";
		public static string HelpReceiveCustomShadows = "Use FlatLighting Shadow Projector to get shadows.";
	}
}
