using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace WaterSystem
{
    /// <summary>
    /// STAGE 02: Water Surface Animation Controller
    /// Manages time-based wave animation and coordinates GPU calculations
    /// 
    /// Responsibilities:
    /// - Global time management for wave synchronization
    /// - GPU shader property updates
    /// - LOD-based animation scaling
    /// - Performance monitoring
    /// 
    /// Compatible with: Unity 6.3, HDRP 17
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(WaterSystem))]
    public class WaterSurfaceAnimator : MonoBehaviour
    {
        // ====================================================================
        // INSPECTOR PROPERTIES
        // ====================================================================
        
        [Header("Animation Control")]
        [Tooltip("Enable/disable water surface animation")]
        public bool animateWaves = true;
        
        [Tooltip("Time scale for animation speed (1.0 = normal, 0.5 = half speed)")]
        [Range(0f, 5f)]
        public float timeScale = 1f;
        
        [Tooltip("Use global time (synchronized across all water) or local time")]
        public bool useGlobalTime = true;
        
        [Header("Performance")]
        [Tooltip("Maximum number of wave layers to calculate (lower = better performance)")]
        [Range(1, 8)]
        public int maxWaveLayers = 8;
        
        [Tooltip("Enable LOD (Level of Detail) based on camera distance")]
        public bool enableLOD = true;
        
        [Tooltip("Distance where LOD reduction begins (meters)")]
        [Range(10f, 500f)]
        public float lodDistance0 = 100f;
        
        [Tooltip("Distance where LOD reduction completes (meters)")]
        [Range(50f, 1000f)]
        public float lodDistance1 = 500f;
        
        [Header("Ripple Detail")]
        [Tooltip("Maximum noise octaves for ripple detail")]
        [Range(1, 6)]
        public int maxRippleOctaves = 4;
        
        [Tooltip("Distance where ripple detail is reduced (meters)")]
        [Range(10f, 200f)]
        public float rippleLODDistance = 75f;
        
        [Header("Debug")]
        [Tooltip("Show animation statistics in Inspector")]
        public bool showDebugInfo = false;
        
        // ====================================================================
        // PRIVATE FIELDS
        // ====================================================================
        
        private WaterSystem waterSystem;
        private Material waterMaterial;
        private float localTime = 0f;
        private static float globalTime = 0f;
        
        // Shader property IDs (cached for performance)
        private static readonly int TimeID = Shader.PropertyToID("_WaterTime");
        private static readonly int WaveCountID = Shader.PropertyToID("_WaveCount");
        private static readonly int RippleOctavesID = Shader.PropertyToID("_RippleOctaves");
        private static readonly int LODDistance0ID = Shader.PropertyToID("_LODDistance0");
        private static readonly int LODDistance1ID = Shader.PropertyToID("_LODDistance1");
        private static readonly int RippleLODDistanceID = Shader.PropertyToID("_RippleLODDistance");
        
        // Wave layer property arrays (up to 8 layers)
        private static readonly int[] WaveDirectionIDs = new int[8];
        private static readonly int[] WaveAmplitudeIDs = new int[8];
        private static readonly int[] WaveWavelengthIDs = new int[8];
        private static readonly int[] WaveSteepnessIDs = new int[8];
        private static readonly int[] WaveSpeedIDs = new int[8];
        private static readonly int[] WavePhaseIDs = new int[8];
        
        // Performance tracking
        private int currentWaveCount = 0;
        private int currentRippleOctaves = 0;
        private float lastUpdateTime = 0f;
        
        // ====================================================================
        // UNITY LIFECYCLE
        // ====================================================================
        
        static WaterSurfaceAnimator()
        {
            // Cache all wave property IDs on static initialization
            for (int i = 0; i < 8; i++)
            {
                WaveDirectionIDs[i] = Shader.PropertyToID($"_Wave{i}_Direction");
                WaveAmplitudeIDs[i] = Shader.PropertyToID($"_Wave{i}_Amplitude");
                WaveWavelengthIDs[i] = Shader.PropertyToID($"_Wave{i}_Wavelength");
                WaveSteepnessIDs[i] = Shader.PropertyToID($"_Wave{i}_Steepness");
                WaveSpeedIDs[i] = Shader.PropertyToID($"_Wave{i}_Speed");
                WavePhaseIDs[i] = Shader.PropertyToID($"_Wave{i}_Phase");
            }
        }
        
        private void Awake()
        {
            waterSystem = GetComponent<WaterSystem>();
        }
        
        private void OnEnable()
        {
            // Reset time when enabled
            if (!useGlobalTime)
            {
                localTime = 0f;
            }
        }
        
        private void Update()
        {
            if (!animateWaves)
                return;
            
            UpdateAnimationTime();
            UpdateShaderProperties();
        }
        
        private void OnValidate()
        {
            // Clamp values
            lodDistance1 = Mathf.Max(lodDistance1, lodDistance0 + 1f);
            
            // Force update when properties change in editor
            if (Application.isPlaying)
            {
                UpdateShaderProperties();
            }
        }
        
        // ====================================================================
        // ANIMATION TIME MANAGEMENT
        // ====================================================================
        
        /// <summary>
        /// Update animation time based on settings
        /// Global time is synchronized across all water instances
        /// </summary>
        private void UpdateAnimationTime()
        {
            float deltaTime = Time.deltaTime * timeScale;
            
            if (useGlobalTime)
            {
                // Update global time (static variable)
                globalTime += deltaTime;
            }
            else
            {
                // Update local time (per-instance)
                localTime += deltaTime;
            }
        }
        
        /// <summary>
        /// Get current animation time value
        /// </summary>
        public float GetAnimationTime()
        {
            return useGlobalTime ? globalTime : localTime;
        }
        
        /// <summary>
        /// Reset animation time to zero
        /// Useful for synchronizing water instances or debugging
        /// </summary>
        public void ResetTime()
        {
            if (useGlobalTime)
            {
                globalTime = 0f;
            }
            else
            {
                localTime = 0f;
            }
        }
        
        // ====================================================================
        // SHADER PROPERTY UPDATES
        // ====================================================================
        
        /// <summary>
        /// Update all shader properties for wave animation
        /// Called every frame when animation is enabled
        /// </summary>
        private void UpdateShaderProperties()
        {
            if (waterSystem == null || waterSystem.currentProfile == null)
                return;
            
            // Get or create material
            if (waterMaterial == null)
            {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    waterMaterial = renderer.sharedMaterial;
                }
                
                if (waterMaterial == null)
                    return;
            }
            
            // Update time
            waterMaterial.SetFloat(TimeID, GetAnimationTime());
            
            // Update wave layers
            UpdateWaveProperties();
            
            // Update LOD settings
            if (enableLOD)
            {
                UpdateLODProperties();
            }
            
            lastUpdateTime = Time.time;
        }
        
        /// <summary>
        /// Update wave layer properties from current profile
        /// </summary>
        private void UpdateWaveProperties()
        {
            var profile = waterSystem.currentProfile;
            if (profile == null || profile.waveData == null)
                return;
            
            var waveData = profile.waveData;
            int waveCount = Mathf.Min(waveData.layers.Count, maxWaveLayers);
            currentWaveCount = waveCount;
            
            // Set wave count
            waterMaterial.SetInt(WaveCountID, waveCount);
            
            // Set individual wave properties
            for (int i = 0; i < 8; i++)
            {
                if (i < waveCount)
                {
                    var layer = waveData.layers[i];
                    waterMaterial.SetVector(WaveDirectionIDs[i], new Vector4(layer.direction.x, layer.direction.y, 0, 0));
                    waterMaterial.SetFloat(WaveAmplitudeIDs[i], layer.amplitude);
                    waterMaterial.SetFloat(WaveWavelengthIDs[i], layer.wavelength);
                    waterMaterial.SetFloat(WaveSteepnessIDs[i], layer.steepness);
                    waterMaterial.SetFloat(WaveSpeedIDs[i], layer.speed);
                    waterMaterial.SetFloat(WavePhaseIDs[i], layer.phase);
                }
                else
                {
                    // Zero out unused layers
                    waterMaterial.SetFloat(WaveAmplitudeIDs[i], 0f);
                }
            }
            
            // Update ripple settings
            currentRippleOctaves = Mathf.Min(waveData.rippleOctaves, maxRippleOctaves);
            waterMaterial.SetInt(RippleOctavesID, currentRippleOctaves);
        }
        
        /// <summary>
        /// Update LOD distance properties
        /// </summary>
        private void UpdateLODProperties()
        {
            waterMaterial.SetFloat(LODDistance0ID, lodDistance0);
            waterMaterial.SetFloat(LODDistance1ID, lodDistance1);
            waterMaterial.SetFloat(RippleLODDistanceID, rippleLODDistance);
        }
        
        // ====================================================================
        // PUBLIC API
        // ====================================================================
        
        /// <summary>
        /// Pause wave animation
        /// </summary>
        public void PauseAnimation()
        {
            animateWaves = false;
        }
        
        /// <summary>
        /// Resume wave animation
        /// </summary>
        public void ResumeAnimation()
        {
            animateWaves = true;
        }
        
        /// <summary>
        /// Set animation time scale
        /// </summary>
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(0f, scale);
        }
        
        /// <summary>
        /// Get current wave count being rendered
        /// </summary>
        public int GetCurrentWaveCount()
        {
            return currentWaveCount;
        }
        
        /// <summary>
        /// Get current ripple octave count being rendered
        /// </summary>
        public int GetCurrentRippleOctaves()
        {
            return currentRippleOctaves;
        }
        
        /// <summary>
        /// Force immediate shader property update
        /// Useful after changing profile or settings
        /// </summary>
        public void ForceUpdate()
        {
            UpdateShaderProperties();
        }
        
        // ====================================================================
        // DEBUG & VISUALIZATION
        // ====================================================================
        
        private void OnGUI()
        {
            if (!showDebugInfo || !Application.isPlaying)
                return;
            
            GUI.Box(new Rect(10, 10, 250, 150), "Water Animation Debug");
            
            int y = 35;
            GUI.Label(new Rect(20, y, 230, 20), $"Animation Time: {GetAnimationTime():F2}s");
            y += 20;
            GUI.Label(new Rect(20, y, 230, 20), $"Time Scale: {timeScale:F2}x");
            y += 20;
            GUI.Label(new Rect(20, y, 230, 20), $"Active Waves: {currentWaveCount}/{maxWaveLayers}");
            y += 20;
            GUI.Label(new Rect(20, y, 230, 20), $"Ripple Octaves: {currentRippleOctaves}/{maxRippleOctaves}");
            y += 20;
            GUI.Label(new Rect(20, y, 230, 20), $"LOD Enabled: {enableLOD}");
            y += 20;
            
            if (waterSystem != null && waterSystem.currentProfile != null)
            {
                GUI.Label(new Rect(20, y, 230, 20), $"Profile: {waterSystem.currentProfile.name}");
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugInfo || !enableLOD)
                return;
            
            // Draw LOD distance spheres
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, lodDistance0);
            
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, lodDistance1);
            
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, rippleLODDistance);
        }
#endif
    }
}
