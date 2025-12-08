using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace WaterSystem
{
    /// <summary>
    /// Manages underwater rendering, fog, post-processing, and surface transition effects.
    /// Integrates with HDRP's volumetric system and custom passes.
    /// </summary>
    public class WaterUnderwaterRenderer : System.IDisposable
    {
        private WaterSystem waterSystem;
        
        // Underwater state tracking
        private Camera mainCamera;
        private bool isUnderwater;
        private float submersionDepth;
        private float transitionProgress; // 0 = fully above, 1 = fully below
        
        // Volume system
        private Volume underwaterVolume;
        private Fog fog;
        private ColorAdjustments colorAdjustments;
        
        // Transition settings
        private const float TRANSITION_SPEED = 3f;
        private const float SURFACE_THICKNESS = 0.5f; // Vertical distance considered "at surface"

        public WaterUnderwaterRenderer(WaterSystem system)
        {
            waterSystem = system;
            InitializeUnderwaterEffects();
        }

        private void InitializeUnderwaterEffects()
        {
            mainCamera = Camera.main;
            
            // Create or get underwater volume
            if (waterSystem.underwaterVolume != null)
            {
                underwaterVolume = waterSystem.underwaterVolume;
            }
            else
            {
                CreateUnderwaterVolume();
            }

            SetupVolumeComponents();
        }

        private void CreateUnderwaterVolume()
        {
            GameObject volumeObject = new GameObject("Underwater Volume");
            volumeObject.transform.SetParent(waterSystem.transform);
            volumeObject.transform.localPosition = Vector3.zero;

            underwaterVolume = volumeObject.AddComponent<Volume>();
            underwaterVolume.isGlobal = false;
            underwaterVolume.priority = 10;
            underwaterVolume.weight = 0f; // Start disabled

            // Create a box collider for volume bounds
            BoxCollider volumeBounds = volumeObject.AddComponent<BoxCollider>();
            volumeBounds.isTrigger = true;
            volumeBounds.center = new Vector3(0f, -50f, 0f); // Below water surface
            volumeBounds.size = new Vector3(1000f, 100f, 1000f); // Large underwater area
        }

        private void SetupVolumeComponents()
        {
            if (underwaterVolume.profile == null)
            {
                underwaterVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            }

            // Add fog (handles both volumetric and exponential fog in Unity 6.3+)
            if (!underwaterVolume.profile.TryGet(out fog))
            {
                fog = underwaterVolume.profile.Add<Fog>();
            }
            ConfigureFog();

            // Add color adjustments
            if (!underwaterVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = underwaterVolume.profile.Add<ColorAdjustments>();
            }
            ConfigureColorAdjustments();
        }

        private void ConfigureFog()
        {
            if (fog == null)
                return;

            fog.active = true;
            fog.enabled.value = true;
            
            // Unity 6.3 HDRP Fog settings for underwater
            fog.colorMode.value = FogColorMode.SkyColor;
            fog.meanFreePath.value = 50f; // Visibility distance underwater
            fog.baseHeight.value = -10f; // Below water surface
            fog.maximumHeight.value = 100f;
            
            // Enable volumetric if available
            // Note: Volumetric fog is controlled at HDRP asset level in Unity 6.3+
        }

        private void ConfigureColorAdjustments()
        {
            if (colorAdjustments == null)
                return;

            colorAdjustments.active = true;
            colorAdjustments.colorFilter.value = new Color(0.6f, 0.8f, 0.9f); // Underwater tint
            colorAdjustments.colorFilter.overrideState = true;
        }

        public void Update(float deltaTime)
        {
            if (!waterSystem.enableUnderwater || mainCamera == null)
                return;

            UpdateSubmersionState(deltaTime);
            UpdateVolumeEffects(deltaTime);
        }

        private void UpdateSubmersionState(float deltaTime)
        {
            Vector3 cameraPos = mainCamera.transform.position;
            float waterHeight = waterSystem.GetWaterHeightAtPosition(cameraPos);
            
            submersionDepth = waterHeight - cameraPos.y;
            
            // Determine if we're underwater
            bool wasUnderwater = isUnderwater;
            isUnderwater = submersionDepth > SURFACE_THICKNESS;

            // Smooth transition
            float targetProgress = isUnderwater ? 1f : 0f;
            
            // Special handling for surface zone
            if (Mathf.Abs(submersionDepth) < SURFACE_THICKNESS)
            {
                targetProgress = Mathf.InverseLerp(-SURFACE_THICKNESS, SURFACE_THICKNESS, submersionDepth);
            }

            transitionProgress = Mathf.Lerp(
                transitionProgress,
                targetProgress,
                deltaTime * TRANSITION_SPEED
            );

            // Trigger events on transition
            if (isUnderwater != wasUnderwater)
            {
                OnUnderwaterStateChanged(isUnderwater);
            }
        }

        private void UpdateVolumeEffects(float deltaTime)
        {
            if (underwaterVolume == null)
                return;

            // Update volume weight based on transition progress
            underwaterVolume.weight = transitionProgress;

            // Adjust fog density based on depth
            if (fog != null && isUnderwater)
            {
                float depthFactor = Mathf.Clamp01(submersionDepth / waterSystem.clarity);
                fog.meanFreePath.value = Mathf.Lerp(
                    waterSystem.clarity,
                    waterSystem.clarity * 0.5f,
                    depthFactor
                );
            }
        }

        private void OnUnderwaterStateChanged(bool underwater)
        {
            if (underwater)
            {
                // Entering water
                Debug.Log("Camera submerged");
                // Future: Trigger audio effects, particle systems, etc.
            }
            else
            {
                // Exiting water
                Debug.Log("Camera surfaced");
                // Future: Trigger splash effects
            }
        }

        /// <summary>
        /// Get current underwater state for gameplay systems
        /// </summary>
        public bool IsUnderwater => isUnderwater;

        /// <summary>
        /// Get transition progress (0 = above, 1 = below)
        /// </summary>
        public float TransitionProgress => transitionProgress;

        /// <summary>
        /// Get submersion depth (positive = underwater)
        /// </summary>
        public float SubmersionDepth => submersionDepth;

        /// <summary>
        /// Apply custom underwater color grading (will be used with profile system)
        /// </summary>
        public void ApplyUnderwaterProfile(WaterProfile profile)
        {
            if (profile == null || colorAdjustments == null)
                return;

            colorAdjustments.colorFilter.value = profile.underwaterTint;
            
            if (fog != null)
            {
                // Note: In Unity 6.3+, fog color is controlled by colorMode (SkyColor/ConstantColor)
                // For custom underwater fog color, we'd need to set colorMode to ConstantColor
                // and then set the color parameter
                fog.meanFreePath.value = profile.underwaterFogDistance;
            }
        }

        public void Dispose()
        {
            // Cleanup volume if we created it
            if (underwaterVolume != null && waterSystem.underwaterVolume == null)
            {
                if (underwaterVolume.gameObject != null)
                {
                    if (Application.isPlaying)
                        Object.Destroy(underwaterVolume.gameObject);
                    else
                        Object.DestroyImmediate(underwaterVolume.gameObject);
                }
            }
        }
    }
}