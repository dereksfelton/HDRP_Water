using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSystem
{
    /// <summary>
    /// STAGE 02: Individual Gerstner wave layer configuration
    /// Defines parameters for one component of the multi-wave system
    /// </summary>
    [Serializable]
    public class WaveLayer
    {
        [Header("Wave Direction")]
        [Tooltip("Direction the wave travels (will be normalized)")]
        public Vector2 direction = new Vector2(1f, 0f);
        
        [Header("Wave Size")]
        [Tooltip("Wave height in meters (peak to trough distance / 2)")]
        [Range(0f, 10f)]
        public float amplitude = 0.5f;
        
        [Tooltip("Distance between wave crests in meters")]
        [Range(0.1f, 100f)]
        public float wavelength = 10f;
        
        [Header("Wave Shape")]
        [Tooltip("Wave sharpness: 0 = smooth sine wave, 1 = sharp crests (max before breaking)")]
        [Range(0f, 1f)]
        public float steepness = 0.5f;
        
        [Header("Wave Motion")]
        [Tooltip("Wave propagation speed in meters/second (0 = auto-calculate from wavelength)")]
        [Range(0f, 20f)]
        public float speed = 0f;
        
        [Tooltip("Initial phase offset in radians")]
        [Range(0f, 6.28318f)] // 2π
        public float phase = 0f;
        
        /// <summary>
        /// Create a wave layer with default ocean parameters
        /// </summary>
        public static WaveLayer CreateDefault()
        {
            return new WaveLayer
            {
                direction = new Vector2(1f, 0f),
                amplitude = 0.5f,
                wavelength = 10f,
                steepness = 0.5f,
                speed = 0f,
                phase = 0f
            };
        }
        
        /// <summary>
        /// Normalize direction vector
        /// Called automatically before use
        /// </summary>
        public void NormalizeDirection()
        {
            if (direction.magnitude > 0.001f)
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector2(1f, 0f);
            }
        }
        
        /// <summary>
        /// Calculate optimal wave speed from wavelength using dispersion relation
        /// For deep water: c = sqrt(g * lambda / 2π)
        /// </summary>
        public float CalculatePhaseSpeed()
        {
            const float gravity = 9.81f;
            const float twoPi = 6.28318530718f;
            float omega = twoPi / wavelength;
            return Mathf.Sqrt(gravity / omega);
        }
        
        /// <summary>
        /// Get effective speed (auto-calculated if speed is 0)
        /// </summary>
        public float GetEffectiveSpeed()
        {
            return speed > 0.001f ? speed : CalculatePhaseSpeed();
        }
        
        /// <summary>
        /// Calculate maximum safe steepness before wave breaks
        /// Q_max = 1 / (omega * A * N) where N is number of waves
        /// </summary>
        public float CalculateMaxSteepness(int totalWaves)
        {
            const float twoPi = 6.28318530718f;
            float omega = twoPi / wavelength;
            return 1f / (omega * amplitude * totalWaves);
        }
        
        /// <summary>
        /// Validate and clamp parameters to safe ranges
        /// </summary>
        public void Validate(int totalWaves)
        {
            amplitude = Mathf.Max(0f, amplitude);
            wavelength = Mathf.Max(0.1f, wavelength);
            
            // Ensure steepness doesn't cause wave breaking
            float maxSteepness = CalculateMaxSteepness(totalWaves);
            steepness = Mathf.Clamp01(Mathf.Min(steepness, maxSteepness));
            
            NormalizeDirection();
        }
    }
    
    /// <summary>
    /// STAGE 02: Complete wave animation configuration
    /// Extended from Stage 0 to include Gerstner waves and ripple detail
    /// </summary>
    [Serializable]
    public class WaterWaveData
    {
        [Header("Gerstner Wave Layers")]
        [Tooltip("Individual wave components (sum creates final surface)")]
        public List<WaveLayer> layers = new List<WaveLayer>();
        
        [Header("Ripple Detail")]
        [Tooltip("Enable small-scale noise-based ripples")]
        public bool enableRipples = true;
        
        [Tooltip("Wind direction for ripple animation")]
        public Vector2 windDirection = new Vector2(1f, 0.5f);
        
        [Tooltip("Wind speed in meters/second")]
        [Range(0f, 20f)]
        public float windSpeed = 2f;
        
        [Tooltip("Ripple size scale (higher = larger ripples)")]
        [Range(0.1f, 10f)]
        public float rippleScale = 1f;
        
        [Tooltip("Ripple height multiplier")]
        [Range(0f, 1f)]
        public float rippleStrength = 0.1f;
        
        [Tooltip("Noise detail octaves (higher = more detail, more expensive)")]
        [Range(1, 6)]
        public int rippleOctaves = 3;
        
        [Header("Advanced")]
        [Tooltip("Sample offset for ripple normal calculation (smaller = sharper normals)")]
        [Range(0.01f, 1f)]
        public float rippleNormalSampleOffset = 0.1f;
        
        // ====================================================================
        // FACTORY METHODS
        // ====================================================================
        
        /// <summary>
        /// Create ocean wave configuration (large rolling waves)
        /// </summary>
        public static WaterWaveData CreateOceanWaves()
        {
            var data = new WaterWaveData
            {
                enableRipples = true,
                windDirection = new Vector2(1f, 0.3f),
                windSpeed = 5f,
                rippleScale = 0.5f,
                rippleStrength = 0.15f,
                rippleOctaves = 4
            };
            
            // Large primary wave
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(1f, 0f),
                amplitude = 1.5f,
                wavelength = 60f,
                steepness = 0.6f,
                speed = 0f,
                phase = 0f
            });
            
            // Medium secondary wave (different direction)
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(0.7f, 0.7f),
                amplitude = 1f,
                wavelength = 40f,
                steepness = 0.5f,
                speed = 0f,
                phase = 1.57f // π/2
            });
            
            // Smaller detail waves
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(-0.5f, 0.866f),
                amplitude = 0.5f,
                wavelength = 20f,
                steepness = 0.4f,
                speed = 0f,
                phase = 3.14f // π
            });
            
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(0.3f, -0.954f),
                amplitude = 0.3f,
                wavelength = 10f,
                steepness = 0.3f,
                speed = 0f,
                phase = 4.71f // 3π/2
            });
            
            data.ValidateAllLayers();
            return data;
        }
        
        /// <summary>
        /// Create lake wave configuration (gentle ripples)
        /// </summary>
        public static WaterWaveData CreateLakeWaves()
        {
            var data = new WaterWaveData
            {
                enableRipples = true,
                windDirection = new Vector2(1f, 0.2f),
                windSpeed = 1.5f,
                rippleScale = 1.5f,
                rippleStrength = 0.05f,
                rippleOctaves = 3
            };
            
            // Small gentle waves
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(1f, 0f),
                amplitude = 0.15f,
                wavelength = 8f,
                steepness = 0.2f,
                speed = 0f,
                phase = 0f
            });
            
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(0.6f, 0.8f),
                amplitude = 0.1f,
                wavelength = 5f,
                steepness = 0.15f,
                speed = 0f,
                phase = 2f
            });
            
            data.ValidateAllLayers();
            return data;
        }
        
        /// <summary>
        /// Create river wave configuration (directional flow)
        /// </summary>
        public static WaterWaveData CreateRiverWaves()
        {
            var data = new WaterWaveData
            {
                enableRipples = true,
                windDirection = new Vector2(1f, 0f), // Flow direction
                windSpeed = 3f,
                rippleScale = 2f,
                rippleStrength = 0.08f,
                rippleOctaves = 3
            };
            
            // Directional waves aligned with flow
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(1f, 0f),
                amplitude = 0.2f,
                wavelength = 3f,
                steepness = 0.3f,
                speed = 2f, // Override for faster flow
                phase = 0f
            });
            
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(1f, 0.1f),
                amplitude = 0.15f,
                wavelength = 2f,
                steepness = 0.25f,
                speed = 1.8f,
                phase = 1f
            });
            
            data.ValidateAllLayers();
            return data;
        }
        
        /// <summary>
        /// Create pool wave configuration (minimal movement)
        /// </summary>
        public static WaterWaveData CreatePoolWaves()
        {
            var data = new WaterWaveData
            {
                enableRipples = true,
                windDirection = new Vector2(1f, 0.5f),
                windSpeed = 0.5f,
                rippleScale = 3f,
                rippleStrength = 0.02f,
                rippleOctaves = 2
            };
            
            // Tiny surface tension waves
            data.layers.Add(new WaveLayer
            {
                direction = new Vector2(1f, 0f),
                amplitude = 0.02f,
                wavelength = 0.5f,
                steepness = 0.1f,
                speed = 0f,
                phase = 0f
            });
            
            data.ValidateAllLayers();
            return data;
        }
        
        // ====================================================================
        // VALIDATION & UTILITY
        // ====================================================================
        
        /// <summary>
        /// Validate all wave layers
        /// Ensures parameters are within safe ranges
        /// </summary>
        public void ValidateAllLayers()
        {
            // Normalize wind direction
            if (windDirection.magnitude > 0.001f)
            {
                windDirection.Normalize();
            }
            else
            {
                windDirection = new Vector2(1f, 0f);
            }
            
            // Validate each layer
            int count = layers.Count;
            for (int i = 0; i < count; i++)
            {
                layers[i].Validate(count);
            }
        }
        
        /// <summary>
        /// Add a new wave layer with validation
        /// </summary>
        public void AddLayer(WaveLayer layer)
        {
            layers.Add(layer);
            ValidateAllLayers();
        }
        
        /// <summary>
        /// Remove wave layer at index
        /// </summary>
        public void RemoveLayer(int index)
        {
            if (index >= 0 && index < layers.Count)
            {
                layers.RemoveAt(index);
                ValidateAllLayers();
            }
        }
        
        /// <summary>
        /// Get total wave energy (sum of amplitudes)
        /// Useful for gameplay queries
        /// </summary>
        public float GetTotalWaveHeight()
        {
            float total = 0f;
            foreach (var layer in layers)
            {
                total += layer.amplitude;
            }
            return total;
        }
        
        /// <summary>
        /// Get dominant wave direction (weighted by amplitude)
        /// </summary>
        public Vector2 GetDominantDirection()
        {
            Vector2 weighted = Vector2.zero;
            float totalAmplitude = 0f;
            
            foreach (var layer in layers)
            {
                weighted += layer.direction * layer.amplitude;
                totalAmplitude += layer.amplitude;
            }
            
            if (totalAmplitude > 0.001f)
            {
                return weighted / totalAmplitude;
            }
            
            return new Vector2(1f, 0f);
        }
        
        /// <summary>
        /// Create a deep copy of this wave data
        /// </summary>
        public WaterWaveData Clone()
        {
            var clone = new WaterWaveData
            {
                enableRipples = this.enableRipples,
                windDirection = this.windDirection,
                windSpeed = this.windSpeed,
                rippleScale = this.rippleScale,
                rippleStrength = this.rippleStrength,
                rippleOctaves = this.rippleOctaves,
                rippleNormalSampleOffset = this.rippleNormalSampleOffset
            };
            
            foreach (var layer in layers)
            {
                clone.layers.Add(new WaveLayer
                {
                    direction = layer.direction,
                    amplitude = layer.amplitude,
                    wavelength = layer.wavelength,
                    steepness = layer.steepness,
                    speed = layer.speed,
                    phase = layer.phase
                });
            }
            
            return clone;
        }
    }
}
