using UnityEngine;

namespace AdvancedWater
{
    /// <summary>
    /// Scriptable Object containing reusable water appearance and behavior settings.
    /// Create presets for different water types (tropical, arctic, murky river, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "New Water Profile", menuName = "Advanced Water/Water Profile")]
    public class WaterProfile : ScriptableObject
    {
        [Header("Visual Identity")]
        [Tooltip("Display name for this water profile")]
        public string profileName = "Default Water";
        
        [TextArea(3, 5)]
        [Tooltip("Description of this water profile")]
        public string description = "A general-purpose water configuration";

        [Header("Color Palette")]
        public Color shallowColor = new Color(0.1f, 0.6f, 0.7f, 1f);
        public Color deepColor = new Color(0.0f, 0.1f, 0.3f, 1f);
        public Color foamColor = Color.white;
        public Color causticsColor = new Color(0.5f, 0.8f, 1f, 1f);

        [Header("Optical Properties")]
        [Range(0f, 100f)]
        public float depthFalloff = 10f;
        
        [Range(0f, 50f)]
        public float clarity = 15f;
        
        [Range(0f, 1f)]
        [Tooltip("How much light is absorbed vs scattered")]
        public float absorption = 0.3f;
        
        [Range(0f, 1f)]
        [Tooltip("Strength of subsurface scattering")]
        public float scattering = 0.5f;

        [Header("Surface Appearance")]
        [Range(0f, 1f)]
        public float smoothness = 0.95f;
        
        [Range(0f, 1f)]
        [Tooltip("Metallic property (should typically be 0 for water)")]
        public float metallic = 0f;
        
        [Range(1f, 2f)]
        [Tooltip("Index of refraction (1.333 for pure water)")]
        public float refractionIndex = 1.333f;

        [Header("Wave Configuration")]
        public WavePreset wavePreset = WavePreset.Calm;
        
        [Range(0f, 5f)]
        public float waveAmplitude = 1f;
        
        [Range(0.1f, 10f)]
        public float waveFrequency = 1f;
        
        [Range(0.1f, 10f)]
        public float waveSpeed = 1f;
        
        [Range(1, 8)]
        [Tooltip("Number of wave octaves for detail")]
        public int waveOctaves = 4;

        [Header("Foam Settings")]
        [Range(0f, 1f)]
        public float foamThreshold = 0.5f;
        
        [Range(0f, 5f)]
        public float foamAmount = 1f;
        
        [Range(0f, 10f)]
        public float foamDecayRate = 2f;

        [Header("Reflection & Refraction")]
        [Range(0f, 1f)]
        public float reflectionStrength = 0.8f;
        
        [Range(0f, 1f)]
        public float refractionStrength = 0.5f;
        
        [Tooltip("Use planar reflections (higher quality, more expensive)")]
        public bool usePlanarReflections = true;

        [Header("Caustics")]
        public bool enableCaustics = true;
        
        [Range(0f, 2f)]
        public float causticsStrength = 1f;
        
        [Range(0.1f, 5f)]
        public float causticsScale = 1f;
        
        [Range(0.1f, 5f)]
        public float causticsSpeed = 1f;

        [Header("Underwater Atmosphere")]
        public Color underwaterFogColor = new Color(0.0f, 0.2f, 0.3f, 1f);
        
        [Range(0f, 100f)]
        public float underwaterFogDensity = 0.1f;
        
        [Range(0f, 200f)]
        public float underwaterFogDistance = 50f;
        
        [Tooltip("Color tint applied to underwater view")]
        public Color underwaterTint = new Color(0.5f, 0.7f, 0.8f, 1f);

        [Header("Performance Hints")]
        [Tooltip("Suggested maximum tessellation for this profile")]
        [Range(1f, 64f)]
        public float recommendedTessellation = 32f;
        
        [Tooltip("Suggested LOD distances")]
        public float[] lodDistances = new float[] { 50f, 100f, 200f };

        /// <summary>
        /// Apply this profile's settings to a WaterSystem component
        /// </summary>
        public void ApplyToWaterSystem(WaterSystem waterSystem)
        {
            if (waterSystem == null)
                return;

            waterSystem.shallowColor = shallowColor;
            waterSystem.deepColor = deepColor;
            waterSystem.depthFalloff = depthFalloff;
            waterSystem.clarity = clarity;
            waterSystem.smoothness = smoothness;
            waterSystem.waveStrength = waveAmplitude;
            waterSystem.waveSpeed = waveSpeed;
            waterSystem.maxTessellation = recommendedTessellation;
        }

        /// <summary>
        /// Create a copy of this profile with modifications
        /// </summary>
        public WaterProfile CreateVariant(string variantName)
        {
            WaterProfile variant = Instantiate(this);
            variant.profileName = variantName;
            return variant;
        }
    }

    /// <summary>
    /// Predefined wave behavior patterns
    /// </summary>
    public enum WavePreset
    {
        Calm,           // Minimal wave activity
        Gentle,         // Light ripples
        Moderate,       // Regular waves
        Choppy,         // Irregular, frequent waves
        Rough,          // Strong wave action
        Storm,          // Violent wave patterns
        Custom          // User-defined settings
    }
}
