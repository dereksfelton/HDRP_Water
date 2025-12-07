using UnityEngine;

namespace AdvancedWater
{
    /// <summary>
    /// Water Profile - Configuration asset for water appearance and behavior
    /// Stores all settings that define how a water body looks and behaves
    /// Stage 1: Extended with visual appearance properties
    /// </summary>
    [CreateAssetMenu(fileName = "WaterProfile", menuName = "Water System/Water Profile", order = 1)]
    public class WaterProfile : ScriptableObject
    {
        [Header("Profile Identity")]
        [Tooltip("Name identifier for this water profile")]
        public string profileName = "Default Water";
        
        [TextArea(2, 4)]
        [Tooltip("Description of this water profile's intended use")]
        public string description = "A general-purpose water profile";
        
        // ====================================================================
        // STAGE 1: VISUAL APPEARANCE - Still Water from Above
        // ====================================================================
        
        [Header("Stage 1: Water Color")]
        [ColorUsage(true, true)]
        [Tooltip("Color of water in shallow areas (near shore, over light-colored bottom)")]
        public Color shallowColor = new Color(0.325f, 0.807f, 0.971f, 0.725f);
        
        [ColorUsage(true, true)]
        [Tooltip("Color of water in deep areas (far from shore, over dark bottom)")]
        public Color deepColor = new Color(0.086f, 0.407f, 1.0f, 0.749f);
        
        [Range(0.1f, 100f)]
        [Tooltip("Distance over which water transitions from shallow to deep color (in meters)")]
        public float depthMaxDistance = 10f;
        
        [Space(10)]
        [Header("Stage 1: Surface Properties")]
        [Range(0f, 1f)]
        [Tooltip("Surface smoothness - controls reflection sharpness (0=rough, 1=mirror-like)")]
        public float smoothness = 0.95f;
        
        [Range(0f, 1f)]
        [Tooltip("Metallic property - should typically stay at 0 for water")]
        public float metallic = 0.0f;
        
        [Space(10)]
        [Header("Stage 1: Normal Mapping")]
        [Tooltip("Normal map texture for surface micro-detail (bumps, small ripples)")]
        public Texture2D normalMap;
        
        [Range(0f, 2f)]
        [Tooltip("Strength of the normal map effect (0=flat, 2=very bumpy)")]
        public float normalScale = 1.0f;
        
        [Space(10)]
        [Header("Stage 1: Light Absorption")]
        [ColorUsage(false)]
        [Tooltip("Color absorbed as light travels through water - affects underwater tint")]
        public Color absorptionColor = new Color(0.45f, 0.029f, 0.018f, 1.0f);
        
        [Space(10)]
        [Header("Stage 1: Light Scattering")]
        [ColorUsage(false)]
        [Tooltip("Color of light scattered by particles suspended in water")]
        public Color scatteringColor = new Color(0.0f, 0.46f, 0.54f, 1.0f);
        
        [Range(0f, 10f)]
        [Tooltip("How strongly light scatters - higher values = more concentrated near surface")]
        public float scatteringPower = 3.0f;
        
        [Space(10)]
        [Header("Stage 1: Fresnel & Refraction")]
        [Range(0f, 10f)]
        [Tooltip("Fresnel power - controls edge brightness (higher = brighter edges)")]
        public float fresnelPower = 5.0f;
        
        [Range(0f, 1f)]
        [Tooltip("Strength of light refraction through water surface")]
        public float refractionStrength = 0.1f;
        
        // ====================================================================
        // STAGE 0: CORE SETTINGS
        // ====================================================================
        
        [Space(20)]
        [Header("Stage 0: Mesh Generation")]
        [Tooltip("Water mesh configuration")]
        public WaterMeshSettings meshSettings = new WaterMeshSettings();
        
        [Header("Stage 0: Performance")]
        [Tooltip("Performance and optimization settings")]
        public PerformanceSettings performanceSettings = new PerformanceSettings();
        
        // ====================================================================
        // PUBLIC METHODS
        // ====================================================================
        
        /// <summary>
        /// Applies this profile's settings to a material
        /// Updates all shader properties based on current profile values
        /// </summary>
        /// <param name="material">Target material to update</param>
        public void ApplyToMaterial(Material material)
        {
            if (material == null)
            {
                Debug.LogError("[WaterProfile] Cannot apply profile to null material");
                return;
            }
            
            // Stage 1: Water Color
            material.SetColor("_ShallowColor", shallowColor);
            material.SetColor("_DeepColor", deepColor);
            material.SetFloat("_DepthMaxDistance", depthMaxDistance);
            
            // Stage 1: Surface Properties
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            
            // Stage 1: Normal Mapping
            if (normalMap != null)
            {
                material.SetTexture("_BumpMap", normalMap);
            }
            material.SetFloat("_BumpScale", normalScale);
            
            // Stage 1: Absorption & Scattering
            material.SetColor("_AbsorptionColor", absorptionColor);
            material.SetColor("_ScatteringColor", scatteringColor);
            material.SetFloat("_ScatteringPower", scatteringPower);
            
            // Stage 1: Fresnel & Refraction
            material.SetFloat("_FresnelPower", fresnelPower);
            material.SetFloat("_RefractionStrength", refractionStrength);
        }
        
        /// <summary>
        /// Validates profile settings and returns any warnings or errors
        /// </summary>
        /// <returns>Array of warning/error messages (empty if valid)</returns>
        public string[] Validate()
        {
            var warnings = new System.Collections.Generic.List<string>();
            
            // Stage 1 Validations
            if (depthMaxDistance < 0.1f)
            {
                warnings.Add("Depth max distance is very small - may cause harsh color transitions");
            }
            
            if (smoothness < 0.5f)
            {
                warnings.Add("Low smoothness may make water look unrealistic for calm water");
            }
            
            if (metallic > 0.1f)
            {
                warnings.Add("Metallic should typically be 0 for water (non-metallic surface)");
            }
            
            if (normalMap == null)
            {
                warnings.Add("No normal map assigned - surface will lack micro-detail");
            }
            
            if (shallowColor.a > 0.95f || deepColor.a > 0.95f)
            {
                warnings.Add("Water alpha is very high - may appear too opaque");
            }
            
            // Stage 0 Validations
            if (meshSettings.gridResolution < 10)
            {
                warnings.Add("Grid resolution is very low - may produce blocky water surface");
            }
            
            if (meshSettings.gridSize < 1f)
            {
                warnings.Add("Grid size is very small - may not cover intended area");
            }
            
            return warnings.ToArray();
        }
        
        /// <summary>
        /// Creates a deep copy of this profile
        /// </summary>
        public WaterProfile Clone()
        {
            WaterProfile clone = CreateInstance<WaterProfile>();
            
            // Copy all fields
            clone.profileName = profileName + " (Copy)";
            clone.description = description;
            
            // Stage 1
            clone.shallowColor = shallowColor;
            clone.deepColor = deepColor;
            clone.depthMaxDistance = depthMaxDistance;
            clone.smoothness = smoothness;
            clone.metallic = metallic;
            clone.normalMap = normalMap;
            clone.normalScale = normalScale;
            clone.absorptionColor = absorptionColor;
            clone.scatteringColor = scatteringColor;
            clone.scatteringPower = scatteringPower;
            clone.fresnelPower = fresnelPower;
            clone.refractionStrength = refractionStrength;
            
            // Stage 0
            clone.meshSettings = new WaterMeshSettings
            {
                gridResolution = meshSettings.gridResolution,
                gridSize = meshSettings.gridSize,
                generateAtRuntime = meshSettings.generateAtRuntime
            };
            
            clone.performanceSettings = new PerformanceSettings
            {
                lodLevels = performanceSettings.lodLevels,
                lodDistance = performanceSettings.lodDistance,
                enableInstancing = performanceSettings.enableInstancing
            };
            
            return clone;
        }
        
        // ====================================================================
        // BACKWARD COMPATIBILITY (for Stage 0 components)
        // ====================================================================
        
        /// <summary>
        /// Applies profile settings to the WaterSystem component (Stage 0 compatibility)
        /// This method bridges Stage 0 and Stage 1 - updates both material and system
        /// </summary>
        public void ApplyToWaterSystem(WaterSystem waterSystem)
        {
            if (waterSystem == null)
            {
                Debug.LogError("[WaterProfile] Cannot apply to null WaterSystem");
                return;
            }
            
            // Apply to material if available
            if (waterSystem.GetComponent<MeshRenderer>() != null)
            {
                Material material = waterSystem.GetComponent<MeshRenderer>().sharedMaterial;
                if (material != null)
                {
                    ApplyToMaterial(material);
                }
            }
            
            // Future: Additional system-level settings will be applied here in later stages
        }
        
        // ====================================================================
        // FUTURE STAGES: Placeholder properties
        // These will be properly implemented in their respective stages
        // ====================================================================
        
        // Stage 7: Underwater Rendering (placeholders for now)
        [Header("Stage 7: Underwater (Coming Soon)")]
        [HideInInspector]
        public Color underwaterTint = new Color(0.0f, 0.3f, 0.4f, 1.0f);
        
        [HideInInspector]
        public float underwaterFogDistance = 50f;
    }
    
    // ========================================================================
    // SUPPORTING CLASSES
    // ========================================================================
    
    /// <summary>
    /// Settings for water mesh generation
    /// </summary>
    [System.Serializable]
    public class WaterMeshSettings
    {
        [Range(10, 500)]
        [Tooltip("Number of vertices along each axis of the water grid")]
        public int gridResolution = 100;
        
        [Range(1f, 1000f)]
        [Tooltip("Physical size of the water grid in world units")]
        public float gridSize = 100f;
        
        [Tooltip("Generate mesh at runtime instead of using pre-saved asset")]
        public bool generateAtRuntime = true;
    }
    
    /// <summary>
    /// Performance optimization settings
    /// </summary>
    [System.Serializable]
    public class PerformanceSettings
    {
        [Range(1, 4)]
        [Tooltip("Number of LOD (Level of Detail) levels for distance-based quality reduction")]
        public int lodLevels = 3;
        
        [Range(10f, 200f)]
        [Tooltip("Distance at which each LOD transition occurs")]
        public float lodDistance = 50f;
        
        [Tooltip("Enable GPU instancing for rendering multiple water bodies efficiently")]
        public bool enableInstancing = true;
    }
}
