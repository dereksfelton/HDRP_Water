using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace AdvancedWater
{
    /// <summary>
    /// Main water system component. Attach to a plane/mesh representing water surface.
    /// This component orchestrates all water subsystems and exposes parameters to Inspector.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WaterSystem : MonoBehaviour
    {
        [Header("Water Body Configuration")]
        [Tooltip("Defines the type and behavior of this water body")]
        public WaterBodyType waterBodyType = WaterBodyType.Ocean;
        
        [Tooltip("Reference to the water profile containing all visual settings")]
        public WaterProfile profile;

        [Header("Surface Properties")]
        [Range(0f, 10f)]
        [Tooltip("Overall scale of water surface details")]
        public float surfaceScale = 1f;
        
        [Range(0f, 1f)]
        [Tooltip("Smoothness of the water surface (0 = rough, 1 = mirror-like)")]
        public float smoothness = 0.95f;

        [Header("Depth & Transparency")]
        [Tooltip("Color of shallow water")]
        public Color shallowColor = new Color(0.1f, 0.6f, 0.7f, 1f);
        
        [Tooltip("Color of deep water")]
        public Color deepColor = new Color(0.0f, 0.1f, 0.3f, 1f);
        
        [Range(0f, 100f)]
        [Tooltip("Depth at which water reaches maximum opacity (meters)")]
        public float depthFalloff = 10f;
        
        [Range(0f, 50f)]
        [Tooltip("Maximum visibility depth underwater (meters)")]
        public float clarity = 15f;

        [Header("Wave System")]
        [Tooltip("Enable procedural wave simulation")]
        public bool enableWaves = true;
        
        [Range(0f, 5f)]
        [Tooltip("Overall amplitude multiplier for waves")]
        public float waveStrength = 1f;
        
        [Range(0.1f, 10f)]
        [Tooltip("Speed multiplier for wave animation")]
        public float waveSpeed = 1f;

        [Header("Interaction System")]
        [Tooltip("Enable dynamic surface interactions (splashes, wakes, etc.)")]
        public bool enableInteractions = true;
        
        [Tooltip("Layer mask for objects that can interact with water")]
        public LayerMask interactionLayers = -1;

        [Header("Underwater Effects")]
        [Tooltip("Enable underwater rendering and transitions")]
        public bool enableUnderwater = true;
        
        [Tooltip("Custom underwater volume override (optional)")]
        public Volume underwaterVolume;

        [Header("Performance")]
        [Tooltip("Maximum tessellation factor for surface mesh")]
        [Range(1f, 64f)]
        public float maxTessellation = 32f;
        
        [Tooltip("Distance at which tessellation reaches minimum")]
        [Range(10f, 500f)]
        public float tessellationFalloffDistance = 200f;
        
        [Tooltip("Enable GPU instancing for foam particles")]
        public bool useGPUInstancing = true;

        [Header("Debug")]
        [Tooltip("Show debug visualizations in Scene view")]
        public bool showDebug = false;
        
        [Tooltip("Display performance metrics")]
        public bool showPerformanceStats = false;

        // Internal references
        private MeshRenderer waterRenderer;
        private MeshFilter waterMeshFilter;
        private MaterialPropertyBlock propertyBlock;
        private WaterWaveSimulator waveSimulator;
        private WaterInteractionManager interactionManager;
        private WaterUnderwaterRenderer underwaterRenderer;

        // Cached property IDs for performance
        private static readonly int ShallowColorID = Shader.PropertyToID("_ShallowColor");
        private static readonly int DeepColorID = Shader.PropertyToID("_DeepColor");
        private static readonly int DepthFalloffID = Shader.PropertyToID("_DepthFalloff");
        private static readonly int ClarityID = Shader.PropertyToID("_Clarity");
        private static readonly int WaveStrengthID = Shader.PropertyToID("_WaveStrength");
        private static readonly int WaveSpeedID = Shader.PropertyToID("_WaveSpeed");
        private static readonly int SurfaceScaleID = Shader.PropertyToID("_SurfaceScale");
        private static readonly int SmoothnessID = Shader.PropertyToID("_Smoothness");

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void OnValidate()
        {
            // Called when values change in Inspector (Edit mode)
            // Apply profile settings when profile is changed
            if (profile != null)
            {
                profile.ApplyToWaterSystem(this);
            }
        }

        private void Initialize()
        {
            waterRenderer = GetComponent<MeshRenderer>();
            waterMeshFilter = GetComponent<MeshFilter>();
            propertyBlock = new MaterialPropertyBlock();

            // Initialize subsystems
            InitializeWaveSimulator();
            InitializeInteractionManager();
            InitializeUnderwaterRenderer();

            // Apply profile settings if available
            if (profile != null)
            {
                profile.ApplyToWaterSystem(this);
            }

            // Apply initial settings
            UpdateMaterialProperties();
        }

        private void Cleanup()
        {
            if (waveSimulator != null)
            {
                waveSimulator.Dispose();
                waveSimulator = null;
            }

            if (interactionManager != null)
            {
                interactionManager.Dispose();
                interactionManager = null;
            }

            if (underwaterRenderer != null)
            {
                underwaterRenderer.Dispose();
                underwaterRenderer = null;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying && !showDebug)
                return;

            UpdateMaterialProperties();
            UpdateSubsystems(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (showPerformanceStats)
            {
                DisplayPerformanceStats();
            }
        }

        private void UpdateSubsystems(float deltaTime)
        {
            if (enableWaves && waveSimulator != null)
            {
                waveSimulator.Update(deltaTime);
            }

            if (enableInteractions && interactionManager != null)
            {
                interactionManager.Update(deltaTime);
            }

            if (enableUnderwater && underwaterRenderer != null)
            {
                underwaterRenderer.Update(deltaTime);
            }
        }

        private void UpdateMaterialProperties()
        {
            if (waterRenderer == null || propertyBlock == null)
                return;

            waterRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor(ShallowColorID, shallowColor);
            propertyBlock.SetColor(DeepColorID, deepColor);
            propertyBlock.SetFloat(DepthFalloffID, depthFalloff);
            propertyBlock.SetFloat(ClarityID, clarity);
            propertyBlock.SetFloat(WaveStrengthID, waveStrength);
            propertyBlock.SetFloat(WaveSpeedID, waveSpeed);
            propertyBlock.SetFloat(SurfaceScaleID, surfaceScale);
            propertyBlock.SetFloat(SmoothnessID, smoothness);

            waterRenderer.SetPropertyBlock(propertyBlock);
        }

        private void InitializeWaveSimulator()
        {
            waveSimulator = new WaterWaveSimulator(this);
        }

        private void InitializeInteractionManager()
        {
            interactionManager = new WaterInteractionManager(this);
        }

        private void InitializeUnderwaterRenderer()
        {
            underwaterRenderer = new WaterUnderwaterRenderer(this);
        }

        private void DisplayPerformanceStats()
        {
            // Performance stats will be implemented with the profiling framework
        }

        /// <summary>
        /// Get the height of the water surface at a world position
        /// </summary>
        public float GetWaterHeightAtPosition(Vector3 worldPosition)
        {
            if (waveSimulator != null && enableWaves)
            {
                return waveSimulator.GetHeightAtPosition(worldPosition);
            }
            return transform.position.y;
        }

        /// <summary>
        /// Check if a world position is underwater
        /// </summary>
        public bool IsUnderwater(Vector3 worldPosition)
        {
            return worldPosition.y < GetWaterHeightAtPosition(worldPosition);
        }

        /// <summary>
        /// Get the surface normal at a world position
        /// </summary>
        public Vector3 GetSurfaceNormal(Vector3 worldPosition)
        {
            if (waveSimulator != null && enableWaves)
            {
                return waveSimulator.GetNormalAtPosition(worldPosition);
            }
            return Vector3.up;
        }

        private void OnDrawGizmos()
        {
            if (!showDebug)
                return;

            // Draw water bounds
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
            if (waterMeshFilter != null && waterMeshFilter.sharedMesh != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireMesh(waterMeshFilter.sharedMesh);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw interaction range
            if (enableInteractions && showDebug)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, tessellationFalloffDistance);
            }
        }
    }

    /// <summary>
    /// Defines the type of water body and influences default behavior
    /// </summary>
    public enum WaterBodyType
    {
        Ocean,      // Large open water with long waves
        Lake,       // Calm enclosed water with gentle ripples
        River,      // Flowing water with current
        Pool,       // Still artificial water
        Waterfall   // Cascading water
    }
}