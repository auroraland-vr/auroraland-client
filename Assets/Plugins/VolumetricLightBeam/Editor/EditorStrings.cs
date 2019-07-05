#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    public static class EditorStrings
    {
        public static readonly GUIContent SideThickness = new GUIContent(
            "Side Thickness",
            "Thickness of the beam when looking at it from the side.\n1 = the beam is fully visible (no difference between the center and the edges), but produces hard edges.\nLower values produce softer transition at beam edges.");

        public static readonly GUIContent ColorMode = new GUIContent("Color", "Apply a flat/plain/single color, or a gradient.");
        public static readonly GUIContent ColorGradient = new GUIContent("", "Use the gradient editor to set color and alpha variations along the beam.");
        public static readonly GUIContent ColorFlat = new GUIContent("", "Use the color picker to set a plain RGBA color (takes account of the alpha value).");

        public static readonly GUIContent AlphaInside  = new GUIContent("Alpha (inside)",  "Modulate the beam inside opacity. Is multiplied to Color's alpha.");
        public static readonly GUIContent AlphaOutside = new GUIContent("Alpha (outside)", "Modulate the beam outside opacity. Is multiplied to Color's alpha.");

        public static readonly GUIContent SpotAngle = new GUIContent("Spot Angle", "Define the angle (in degrees) at the base of the beam's cone");

        public static readonly GUIContent GlareFrontal = new GUIContent("Glare (frontal)", "Boost intensity factor when looking at the beam from the inside directly at the source.");
        public static readonly GUIContent GlareBehind  = new GUIContent("Glare (from behind)", "Boost intensity factor when looking at the beam from behind.");

        public static readonly GUIContent TrackChanges = new GUIContent(
            "Track changes during Playtime",
            "Check this box to be able to modify properties during Playtime via Script, Animator and/or Timeline.\nEnabling this feature is at very minor performance cost. So keep it disabled if you don't plan to modify this light beam during playtime.");

        public static readonly GUIContent AttenuationEquation = new GUIContent("Equation", "Attenuation equation used to compute fading between 'Fade Start Distance' and 'Range Distance'.\n- Linear: Simple linear attenuation\n- Quadratic: Quadratic attenuation, which usually gives more realistic results\n- Blend: Custom blending mix between linear (0.0) and quadratic attenuation (1.0)");
        public static readonly GUIContent AttenuationCustomBlending = new GUIContent("", "Blending value between Linear (0.0) and Quadratic (1.0) attenuation equations.");

        public static readonly GUIContent FadeStart = new GUIContent("Fade Start Distance", "Distance from the light source (in units) the beam intensity will start to fall off.");
        public static readonly GUIContent FadeEnd = new GUIContent("Range Distance", "Distance from the light source (in units) the beam is entirely faded out");

        public static readonly GUIContent NoiseEnabled = new GUIContent("Enabled", "Enable 3D Noise effect");
        public static readonly GUIContent NoiseIntensity = new GUIContent("Intensity", "Higher intensity means the noise contribution is stronger and more visible");
        public static readonly GUIContent NoiseScale = new GUIContent("Scale", "3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic");
        public static readonly GUIContent NoiseVelocity = new GUIContent("Velocity", "World Space direction and speed of the noise scrolling, simulating the fog/smoke movement");

        public static readonly GUIContent CameraClippingDistance = new GUIContent("Camera", "Distance from the camera the beam will fade.\n0 = hard intersection\nHigher values produce soft intersection when the camera is near the cone triangles.");
        public static readonly GUIContent DepthBlendDistance = new GUIContent("Opaque geometry", "Distance from the world geometry the beam will fade.\n0 = hard intersection\nHigher values produce soft intersection when the beam intersects other opaque geometry.");

        public static readonly GUIContent ConeRadiusStart = new GUIContent("Truncated Radius", "Radius (in units) at the beam's source (the top of the cone).\n0 will generate a perfect cone geometry.\nHigher values will generate truncated cones.");

        public static readonly GUIContent GeomCap = new GUIContent("Cap Geom", "Generate Cap Geometry (only visible from inside)");
        public static readonly GUIContent GeomSides = new GUIContent("Sides", "Number of Sides of the cone. Higher values give better looking results, but require more memory and graphic performance.");

        public const string SortingLayer = "Sorting Layer";
        public static readonly GUIContent SortingOrder = new GUIContent("Order in Layer", "The overlay priority within its layer. Lower numbers are rendered first and subsequent numbers overlay those below.");

        public static readonly GUIContent ResetProperties = new GUIContent("Default values", "Reset properties to their default values.");
        public static readonly GUIContent GenerateGeometry = new GUIContent("Regenerate geometry", "Force to re-create the Beam Geometry GameObject.");

        public static readonly GUIContent AddDustParticles = new GUIContent("Add Dust Particles", "Add a 'VolumetricDustParticles' component.");
        public static readonly GUIContent AddDynamicOcclusion = new GUIContent("Add Dynamic Occlusion", "Add a 'DynamicOcclusion' component.");

        public const string HelpNoSpotlight = "To bind properties from the Light and the Beam together, this component must be attached to a Light of type 'Spot'";
        public const string HelpNoiseLoadingFailed = "Fail to load 3D noise texture. Please check your Config.";
        public const string HelpAnimatorWarning = "If you want to animate your light beam in real-time, you should enable the 'trackChangesDuringPlaytime' property.";
        public const string HelpTrackChangesEnabled = "This beam will keep track of the changes of its own properties and the spotlight attached to it (if any) during playtime. You can modify every properties except 'geomSides'.";
        public const string HelpDepthTextureMode = "To support 'Soft Intersection with Opaque Geometry', your camera must use 'DepthTextureMode.Depth'.";
        public const string HelpDepthMobile = "On mobile platforms, the depth buffer precision can be pretty low. Try to keep a small depth range on your cameras: the difference between the near and far clip planes should stay as low as possible.";

    }
}
#endif
