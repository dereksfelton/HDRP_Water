using UnityEngine;
using System.Collections.Generic;

namespace AdvancedWater
{
    /// <summary>
    /// Manages dynamic water surface interactions like splashes, wakes, and ripples.
    /// Uses a combination of particle systems and render textures for surface displacement.
    /// </summary>
    public class WaterInteractionManager : System.IDisposable
    {
        private WaterSystem waterSystem;
        
        // Interaction tracking
        private List<WaterInteraction> activeInteractions;
        private const int MAX_ACTIVE_INTERACTIONS = 64;
        
        // Render texture for dynamic displacement
        private RenderTexture displacementMap;
        private RenderTexture previousDisplacementMap;
        private int displacementResolution = 512;
        
        // Foam system
        private List<FoamParticle> foamParticles;
        private const int MAX_FOAM_PARTICLES = 1000;

        public WaterInteractionManager(WaterSystem system)
        {
            waterSystem = system;
            activeInteractions = new List<WaterInteraction>(MAX_ACTIVE_INTERACTIONS);
            foamParticles = new List<FoamParticle>(MAX_FOAM_PARTICLES);
            
            InitializeDisplacementSystem();
        }

        private void InitializeDisplacementSystem()
        {
            // Create render textures for dynamic displacement
            displacementMap = new RenderTexture(
                displacementResolution,
                displacementResolution,
                0,
                RenderTextureFormat.ARGBFloat
            );
            displacementMap.name = "Water Displacement Map";
            displacementMap.wrapMode = TextureWrapMode.Repeat;
            displacementMap.filterMode = FilterMode.Bilinear;
            displacementMap.enableRandomWrite = true;
            displacementMap.Create();

            previousDisplacementMap = new RenderTexture(
                displacementResolution,
                displacementResolution,
                0,
                RenderTextureFormat.ARGBFloat
            );
            previousDisplacementMap.name = "Water Displacement Map Previous";
            previousDisplacementMap.wrapMode = TextureWrapMode.Repeat;
            previousDisplacementMap.filterMode = FilterMode.Bilinear;
            previousDisplacementMap.Create();
        }

        public void Update(float deltaTime)
        {
            UpdateInteractions(deltaTime);
            UpdateFoam(deltaTime);
            UpdateDisplacementMap(deltaTime);
        }

        private void UpdateInteractions(float deltaTime)
        {
            // Update and remove expired interactions
            for (int i = activeInteractions.Count - 1; i >= 0; i--)
            {
                WaterInteraction interaction = activeInteractions[i];
                interaction.lifetime -= deltaTime;
                
                if (interaction.lifetime <= 0f)
                {
                    activeInteractions.RemoveAt(i);
                }
                else
                {
                    interaction.currentRadius += interaction.expansionRate * deltaTime;
                    interaction.currentAmplitude *= Mathf.Exp(-deltaTime * interaction.decayRate);
                    activeInteractions[i] = interaction;
                }
            }
        }

        private void UpdateFoam(float deltaTime)
        {
            // Update foam particles
            for (int i = foamParticles.Count - 1; i >= 0; i--)
            {
                FoamParticle particle = foamParticles[i];
                particle.lifetime -= deltaTime;
                
                if (particle.lifetime <= 0f)
                {
                    foamParticles.RemoveAt(i);
                }
                else
                {
                    // Update particle position (will add current/wind influence later)
                    particle.position += particle.velocity * deltaTime;
                    particle.alpha *= Mathf.Exp(-deltaTime * particle.decayRate);
                    foamParticles[i] = particle;
                }
            }
        }

        private void UpdateDisplacementMap(float deltaTime)
        {
            // This will be implemented with compute shaders in later stages
            // For now, we'll prepare the structure
        }

        /// <summary>
        /// Register a splash at a world position
        /// </summary>
        public void CreateSplash(Vector3 worldPosition, float intensity, float radius = 1f)
        {
            if (!waterSystem.enableInteractions)
                return;

            if (activeInteractions.Count >= MAX_ACTIVE_INTERACTIONS)
                return;

            WaterInteraction splash = new WaterInteraction
            {
                position = worldPosition,
                interactionType = InteractionType.Splash,
                initialAmplitude = intensity,
                currentAmplitude = intensity,
                initialRadius = radius,
                currentRadius = radius,
                expansionRate = 5f, // Ripple expansion speed
                decayRate = 2f,     // How quickly it fades
                lifetime = 3f       // Total duration
            };

            activeInteractions.Add(splash);

            // Generate foam particles for splash
            CreateFoamAtPosition(worldPosition, intensity * 0.5f, radius);
        }

        /// <summary>
        /// Register a continuous wake (for boats, swimming objects, etc.)
        /// </summary>
        public void CreateWake(Vector3 worldPosition, Vector3 velocity, float intensity)
        {
            if (!waterSystem.enableInteractions)
                return;

            // Wakes are handled differently - they create a trail of interactions
            float speed = velocity.magnitude;
            if (speed < 0.1f)
                return;

            WaterInteraction wake = new WaterInteraction
            {
                position = worldPosition,
                interactionType = InteractionType.Wake,
                initialAmplitude = intensity * 0.5f,
                currentAmplitude = intensity * 0.5f,
                initialRadius = 0.5f,
                currentRadius = 0.5f,
                expansionRate = 3f,
                decayRate = 1.5f,
                lifetime = 2f
            };

            activeInteractions.Add(wake);
            
            // Create foam along wake
            CreateFoamAtPosition(worldPosition, intensity * 0.3f, 0.3f);
        }

        /// <summary>
        /// Create foam particles at a position
        /// </summary>
        private void CreateFoamAtPosition(Vector3 worldPosition, float amount, float spread)
        {
            int particleCount = Mathf.RoundToInt(amount * 20f);
            particleCount = Mathf.Min(particleCount, MAX_FOAM_PARTICLES - foamParticles.Count);

            for (int i = 0; i < particleCount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * spread;
                Vector3 particlePos = worldPosition + new Vector3(randomOffset.x, 0f, randomOffset.y);

                FoamParticle particle = new FoamParticle
                {
                    position = particlePos,
                    velocity = new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        0f,
                        Random.Range(-0.5f, 0.5f)
                    ),
                    size = Random.Range(0.1f, 0.3f),
                    alpha = 1f,
                    decayRate = Random.Range(0.5f, 2f),
                    lifetime = Random.Range(2f, 5f)
                };

                foamParticles.Add(particle);
            }
        }

        /// <summary>
        /// Get all active interactions for shader/rendering
        /// </summary>
        public void GetInteractionsForShader(out Vector4[] positions, out Vector4[] parameters)
        {
            int count = Mathf.Min(activeInteractions.Count, 32); // Shader limit
            positions = new Vector4[32];
            parameters = new Vector4[32];

            for (int i = 0; i < count; i++)
            {
                WaterInteraction interaction = activeInteractions[i];
                positions[i] = new Vector4(
                    interaction.position.x,
                    interaction.position.y,
                    interaction.position.z,
                    (float)interaction.interactionType
                );
                
                parameters[i] = new Vector4(
                    interaction.currentAmplitude,
                    interaction.currentRadius,
                    interaction.lifetime,
                    0f // Reserved
                );
            }
        }

        /// <summary>
        /// Get foam particle data for rendering
        /// </summary>
        public List<FoamParticle> GetFoamParticles()
        {
            return foamParticles;
        }

        public void Dispose()
        {
            if (displacementMap != null)
            {
                displacementMap.Release();
                if (Application.isPlaying)
                    Object.Destroy(displacementMap);
                else
                    Object.DestroyImmediate(displacementMap);
            }

            if (previousDisplacementMap != null)
            {
                previousDisplacementMap.Release();
                if (Application.isPlaying)
                    Object.Destroy(previousDisplacementMap);
                else
                    Object.DestroyImmediate(previousDisplacementMap);
            }

            activeInteractions?.Clear();
            foamParticles?.Clear();
        }

        /// <summary>
        /// Represents a single water surface interaction
        /// </summary>
        private struct WaterInteraction
        {
            public Vector3 position;
            public InteractionType interactionType;
            public float initialAmplitude;
            public float currentAmplitude;
            public float initialRadius;
            public float currentRadius;
            public float expansionRate;
            public float decayRate;
            public float lifetime;
        }

        /// <summary>
        /// Represents a foam particle
        /// </summary>
        public struct FoamParticle
        {
            public Vector3 position;
            public Vector3 velocity;
            public float size;
            public float alpha;
            public float decayRate;
            public float lifetime;
        }

        private enum InteractionType
        {
            Splash = 0,
            Wake = 1,
            Ripple = 2
        }
    }
}