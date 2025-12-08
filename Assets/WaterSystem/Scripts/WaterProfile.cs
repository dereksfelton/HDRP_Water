using UnityEngine;

namespace WaterSystem
{
    /// <summary>
    /// Water Profile ScriptableObject
    /// Stores all visual and physical properties for water appearance
    /// 
    /// STAGES:
    /// - Stage 0: Basic structure
    /// - Stage 1: Visual properties (color, depth, lighting)
    /// - Stage 2: Wave animation properties [NEW]
    /// - Future: Reflection, refraction, interactions, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "WaterProfile", menuName = "Water System/Water Profile", order = 1)]
    public class WaterProfile : ScriptableObject
    {
        [Header("Profile Info")]
        [Tooltip("Profile name for identification")]
        public string profileName = "New Water Profile";
        
        [Tooltip("Description of this water type")]
        [TextArea(2, 4)]
        public string description = "";
        
        // ====================================================================
        // STAGE 1: VISUAL PROPERTIES
        // ====================================================================
        
        [Header("Water Colors")]
        [Tooltip("Water color in shallow areas")]
        public Color shallowColor = new Color(0.325f, 0.807f, 0.971f, 0.725f);
        
        [Tooltip("Water color in deep areas")]
        public Color deepColor = new Color(0.086f, 0.407f, 1f, 0.749f);
        
        [Tooltip("Distance at which water reaches full depth color")]
        [Range(0.1f, 100f)]
        public float depthMaxDistance = 20f;
        
        [Header("Surface Properties")]
        [Tooltip("Surface smoothness (0 = rough, 1 = mirror-like)")]
        [Range(0f, 1f)]
        public float smoothness = 0.95f;
        
        [Tooltip("Metallic property (usually 0 for water)")]
        [Range(0f, 1f)]
        public float metallic = 0f;
        
        [Header("Light Interaction")]
        [Tooltip("Water absorption color (RGB absorbs these wavelengths)")]
        public Color absorptionColor = new Color(0.45f, 0.029f, 0.018f, 1f);
        
        [Tooltip("Light scattering color")]
        public Color scatteringColor = new Color(0f, 0.46f, 0.54f, 1f);
        
        [Tooltip("Scattering intensity")]
        [Range(0f, 10f)]
        public float scatteringPower = 3f;
        
        [Header("Fresnel & Refraction")]
        [Tooltip("Fresnel power (controls reflection angle dependency)")]
        [Range(0f, 10f)]
        public float fresnelPower = 5f;
        
        [Tooltip("Refraction distortion strength")]
        [Range(0f, 1f)]
        public float refractionStrength = 0.1f;
        
        [Header("Normal Mapping")]
        [Tooltip("Normal map texture for surface micro-detail")]
        public Texture2D normalMap;
        
        [Tooltip("Normal map strength")]
        [Range(0f, 2f)]
        public float normalStrength = 1f;
        
        [Tooltip("Normal map tiling")]
        public Vector2 normalTiling = Vector2.one;
        
        [Tooltip("Normal map scroll speed (animated surface detail)")]
        [Range(0f, 0.1f)]
        public float normalScrollSpeed = 0.02f;
        
        // ====================================================================
        // STAGE 2: WAVE ANIMATION PROPERTIES [NEW]
        // ====================================================================
        
        [Header("Wave Animation (Stage 2)")]
        [Tooltip("Wave configuration data")]
        public WaterWaveData waveData;
        
        [Tooltip("Enable wave animation")]
        public bool enableAnimation = true;
        
        [Tooltip("Overall wave animation speed multiplier")]
        [Range(0f, 5f)]
        public float animationSpeed = 1f;
        
        // ====================================================================
        // STAGE 0+: FUTURE PROPERTIES (PLACEHOLDERS)
        // ====================================================================
        
        [Header("Future Features")]
        [Tooltip("(Stage 3) Reflection quality")]
        [Range(0f, 1f)]
        public float reflectionQuality = 1f;
        
        [Tooltip("(Stage 4) Underwater visibility distance")]
        [Range(1f, 100f)]
        public float underwaterVisibility = 20f;
        
        [Tooltip("(Stage 4) Underwater tint color")]
        public Color underwaterTint = new Color(0.2f, 0.4f, 0.6f, 1f);
        
        [Tooltip("(Stage 4) Underwater fog distance")]
        [Range(1f, 100f)]
        public float underwaterFogDistance = 30f;
        
        [Tooltip("(Stage 5) Surface interaction response strength")]
        [Range(0f, 2f)]
        public float interactionStrength = 1f;
        
        // ====================================================================
        // INITIALIZATION & VALIDATION
        // ====================================================================
        
        private void OnEnable()
        {
            // Initialize wave data if null
            if (waveData == null)
            {
                waveData = new WaterWaveData();
                
                // Add default wave layer
                waveData.AddLayer(WaveLayer.CreateDefault());
            }
        }
        
        private void OnValidate()
        {
            // Validate wave data
            if (waveData != null)
            {
                waveData.ValidateAllLayers();
            }
            
            // Clamp values
            depthMaxDistance = Mathf.Max(0.1f, depthMaxDistance);
            underwaterVisibility = Mathf.Max(1f, underwaterVisibility);
        }
        
        // ====================================================================
        // FACTORY METHODS
        // ====================================================================
        
        /// <summary>
        /// Create an ocean water profile with rolling waves
        /// </summary>
        public static WaterProfile CreateOceanProfile()
        {
            var profile = CreateInstance<WaterProfile>();
            profile.profileName = "Ocean - Rolling Waves";
            profile.description = "Large ocean with rolling waves and dynamic surface";
            
            // Visual properties
            profile.shallowColor = new Color(0.325f, 0.807f, 0.971f, 0.725f);
            profile.deepColor = new Color(0.086f, 0.407f, 0.8f, 0.85f);
            profile.depthMaxDistance = 50f;
            profile.smoothness = 0.95f;
            profile.fresnelPower = 5f;
            profile.refractionStrength = 0.15f;
            
            // Wave animation
            profile.waveData = WaterWaveData.CreateOceanWaves();
            profile.enableAnimation = true;
            profile.animationSpeed = 1f;
            
            return profile;
        }
        
        /// <summary>
        /// Create a lake water profile with gentle ripples
        /// </summary>
        public static WaterProfile CreateLakeProfile()
        {
            var profile = CreateInstance<WaterProfile>();
            profile.profileName = "Lake - Gentle Ripples";
            profile.description = "Calm lake with subtle surface movement";
            
            // Visual properties
            profile.shallowColor = new Color(0.4f, 0.75f, 0.85f, 0.6f);
            profile.deepColor = new Color(0.15f, 0.3f, 0.5f, 0.8f);
            profile.depthMaxDistance = 20f;
            profile.smoothness = 0.92f;
            profile.fresnelPower = 4.5f;
            profile.refractionStrength = 0.08f;
            
            // Wave animation
            profile.waveData = WaterWaveData.CreateLakeWaves();
            profile.enableAnimation = true;
            profile.animationSpeed = 0.8f;
            
            return profile;
        }
        
        /// <summary>
        /// Create a river water profile with directional flow
        /// </summary>
        public static WaterProfile CreateRiverProfile()
        {
            var profile = CreateInstance<WaterProfile>();
            profile.profileName = "River - Flowing Current";
            profile.description = "River with directional flow and rapids";
            
            // Visual properties
            profile.shallowColor = new Color(0.6f, 0.8f, 0.85f, 0.5f);
            profile.deepColor = new Color(0.2f, 0.4f, 0.5f, 0.75f);
            profile.depthMaxDistance = 8f;
            profile.smoothness = 0.88f;
            profile.fresnelPower = 4f;
            profile.refractionStrength = 0.12f;
            
            // Wave animation
            profile.waveData = WaterWaveData.CreateRiverWaves();
            profile.enableAnimation = true;
            profile.animationSpeed = 1.2f;
            
            return profile;
        }
        
        /// <summary>
        /// Create a pool water profile with minimal movement
        /// </summary>
        public static WaterProfile CreatePoolProfile()
        {
            var profile = CreateInstance<WaterProfile>();
            profile.profileName = "Pool - Still Water";
            profile.description = "Swimming pool with minimal surface movement";
            
            // Visual properties
            profile.shallowColor = new Color(0.5f, 0.85f, 0.95f, 0.4f);
            profile.deepColor = new Color(0.3f, 0.6f, 0.9f, 0.7f);
            profile.depthMaxDistance = 3f;
            profile.smoothness = 0.98f;
            profile.fresnelPower = 5.5f;
            profile.refractionStrength = 0.05f;
            
            // Wave animation
            profile.waveData = WaterWaveData.CreatePoolWaves();
            profile.enableAnimation = true;
            profile.animationSpeed = 0.5f;
            
            return profile;
        }
        
        // ====================================================================
        // UTILITY METHODS
        // ====================================================================
        
        /// <summary>
        /// Create a duplicate of this profile
        /// </summary>
        public WaterProfile CreateVariant(string variantName = "")
        {
            var variant = Instantiate(this);
            
            if (!string.IsNullOrEmpty(variantName))
            {
                variant.profileName = variantName;
            }
            else
            {
                variant.profileName = $"{profileName} (Variant)";
            }
            
            // Deep copy wave data
            if (waveData != null)
            {
                variant.waveData = waveData.Clone();
            }
            
            return variant;
        }
        
        /// <summary>
        /// Apply this profile's settings to a material
        /// Called by WaterSystem when profile changes
        /// </summary>
        public void ApplyToMaterial(Material material)
        {
            if (material == null)
                return;
            
            // Stage 1: Visual properties
            material.SetColor("_ShallowColor", shallowColor);
            material.SetColor("_DeepColor", deepColor);
            material.SetFloat("_DepthMaxDistance", depthMaxDistance);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            material.SetColor("_AbsorptionColor", absorptionColor);
            material.SetColor("_ScatteringColor", scatteringColor);
            material.SetFloat("_ScatteringPower", scatteringPower);
            material.SetFloat("_FresnelPower", fresnelPower);
            material.SetFloat("_RefractionStrength", refractionStrength);
            
            if (normalMap != null)
            {
                material.SetTexture("_BumpMap", normalMap);
                material.SetFloat("_BumpScale", normalStrength);
                material.SetVector("_NormalTiling", normalTiling);
            }
            
            // Normal map scroll speed
            material.SetFloat("_NormalScrollSpeed", normalScrollSpeed);
            
            // Stage 2: Wave animation properties
            // (Handled by WaterSurfaceAnimator component)
        }
        
        /// <summary>
        /// Apply this profile to a WaterSystem component
        /// Called by WaterSystem when profile changes
        /// </summary>
        public void ApplyToWaterSystem(WaterSystem waterSystem)
        {
            if (waterSystem == null)
                return;
            
            // Get the material from the WaterSystem's renderer
            Renderer renderer = waterSystem.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                ApplyToMaterial(renderer.sharedMaterial);
            }
            
            // If WaterSurfaceAnimator exists, force it to update
            WaterSurfaceAnimator animator = waterSystem.GetComponent<WaterSurfaceAnimator>();
            if (animator != null)
            {
                animator.ForceUpdate();
            }
        }
        
        /// <summary>
        /// Get estimated total wave height for gameplay queries
        /// </summary>
        public float GetMaxWaveHeight()
        {
            if (waveData == null)
                return 0f;
            
            return waveData.GetTotalWaveHeight();
        }
        
        /// <summary>
        /// Get dominant wave direction for gameplay
        /// </summary>
        public Vector2 GetWaveDirection()
        {
            if (waveData == null)
                return Vector2.right;
            
            return waveData.GetDominantDirection();
        }
    }
}
