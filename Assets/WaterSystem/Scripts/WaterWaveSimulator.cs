using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace WaterSystem
{
    /// <summary>
    /// Handles procedural wave generation using Gerstner waves and FFT-based ocean simulation.
    /// Provides height and normal queries for gameplay systems.
    /// </summary>
    public class WaterWaveSimulator : System.IDisposable
    {
        private WaterSystem waterSystem;
        private GerstnerWave[] gerstnerWaves;
        private float time;

        // Cached wave parameters
        private const int MAX_GERSTNER_WAVES = 8;

        // Future optimization: Track when wave parameters change to avoid unnecessary GPU uploads
        #pragma warning disable 0414 // Disable "assigned but never used" warning - will be used in Stage 2+
        private bool isDirty = true;
        #pragma warning restore 0414
        
        public WaterWaveSimulator(WaterSystem system)
        {
            waterSystem = system;
            InitializeWaves();
        }

        private void InitializeWaves()
        {
            // Initialize default Gerstner wave configuration
            gerstnerWaves = new GerstnerWave[MAX_GERSTNER_WAVES];
            
            // Create waves with varying directions, wavelengths, and amplitudes
            // These will be replaced by profile-based configuration later
            for (int i = 0; i < MAX_GERSTNER_WAVES; i++)
            {
                float angle = (i / (float)MAX_GERSTNER_WAVES) * 360f;
                float wavelength = 10f + i * 5f;
                float amplitude = 0.5f / (i + 1);
                
                gerstnerWaves[i] = new GerstnerWave
                {
                    direction = new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad)
                    ),
                    wavelength = wavelength,
                    amplitude = amplitude,
                    steepness = 0.5f,
                    speed = Mathf.Sqrt(9.8f * (2f * Mathf.PI / wavelength)) // Deep water dispersion
                };
            }

            isDirty = true;
        }

        public void Update(float deltaTime)
        {
            time += deltaTime * waterSystem.waveSpeed;
            isDirty = true;
        }

        /// <summary>
        /// Get the water surface height at a world position using Gerstner waves
        /// </summary>
        public float GetHeightAtPosition(Vector3 worldPosition)
        {
            Vector3 localPos = worldPosition - waterSystem.transform.position;
            float height = 0f;

            for (int i = 0; i < gerstnerWaves.Length; i++)
            {
                height += EvaluateGerstnerWave(gerstnerWaves[i], localPos.x, localPos.z, time).y;
            }

            return waterSystem.transform.position.y + height * waterSystem.waveStrength;
        }

        /// <summary>
        /// Get the surface normal at a world position
        /// </summary>
        public Vector3 GetNormalAtPosition(Vector3 worldPosition)
        {
            Vector3 localPos = worldPosition - waterSystem.transform.position;
            Vector3 normal = Vector3.up;
            Vector3 tangent = Vector3.zero;
            Vector3 binormal = Vector3.zero;

            for (int i = 0; i < gerstnerWaves.Length; i++)
            {
                GerstnerWaveResult result = EvaluateGerstnerWaveWithDerivatives(
                    gerstnerWaves[i], localPos.x, localPos.z, time
                );
                
                tangent += result.tangent;
                binormal += result.binormal;
            }

            // Cross product to get normal
            normal = Vector3.Cross(binormal, tangent).normalized;
            
            return normal;
        }

        /// <summary>
        /// Evaluate a single Gerstner wave at a position
        /// </summary>
        private Vector3 EvaluateGerstnerWave(GerstnerWave wave, float x, float z, float t)
        {
            float k = 2f * Mathf.PI / wave.wavelength; // Wave number
            float c = wave.speed;
            float a = wave.amplitude;
            float q = wave.steepness / (k * a * gerstnerWaves.Length);
            
            float d = k * (wave.direction.x * x + wave.direction.y * z - c * t);
            float cosD = Mathf.Cos(d);
            float sinD = Mathf.Sin(d);

            return new Vector3(
                q * a * wave.direction.x * cosD,  // Horizontal displacement X
                a * sinD,                          // Vertical displacement Y
                q * a * wave.direction.y * cosD   // Horizontal displacement Z
            );
        }

        /// <summary>
        /// Evaluate Gerstner wave with tangent and binormal for normal calculation
        /// </summary>
        private GerstnerWaveResult EvaluateGerstnerWaveWithDerivatives(
            GerstnerWave wave, float x, float z, float t)
        {
            float k = 2f * Mathf.PI / wave.wavelength;
            float c = wave.speed;
            float a = wave.amplitude;
            float q = wave.steepness / (k * a * gerstnerWaves.Length);
            
            float d = k * (wave.direction.x * x + wave.direction.y * z - c * t);
            float cosD = Mathf.Cos(d);
            float sinD = Mathf.Sin(d);
            
            float wa = k * a;
            float dx = wave.direction.x;
            float dz = wave.direction.y;
            
            return new GerstnerWaveResult
            {
                position = new Vector3(
                    q * a * dx * cosD,
                    a * sinD,
                    q * a * dz * cosD
                ),
                tangent = new Vector3(
                    1 - q * wa * dx * dx * sinD,
                    wa * dx * cosD,
                    -q * wa * dx * dz * sinD
                ),
                binormal = new Vector3(
                    -q * wa * dx * dz * sinD,
                    wa * dz * cosD,
                    1 - q * wa * dz * dz * sinD
                )
            };
        }

        /// <summary>
        /// Update wave parameters from profile (to be implemented with profile system)
        /// </summary>
        public void UpdateFromProfile(WaterProfile profile)
        {
            if (profile == null)
                return;

            // This will be expanded when we implement full profile support
            isDirty = true;
        }

        /// <summary>
        /// Get wave data for shader (will be used for GPU displacement)
        /// </summary>
        public void GetWaveDataForShader(out Vector4[] waveA, out Vector4[] waveB)
        {
            waveA = new Vector4[MAX_GERSTNER_WAVES];
            waveB = new Vector4[MAX_GERSTNER_WAVES];

            for (int i = 0; i < gerstnerWaves.Length; i++)
            {
                GerstnerWave wave = gerstnerWaves[i];
                
                // Pack wave parameters into vectors for shader
                waveA[i] = new Vector4(
                    wave.direction.x,
                    wave.direction.y,
                    wave.amplitude,
                    wave.wavelength
                );
                
                waveB[i] = new Vector4(
                    wave.steepness,
                    wave.speed,
                    0f, // Reserved for future use
                    0f  // Reserved for future use
                );
            }
        }

        public void Dispose()
        {
            // Cleanup will be needed when we add compute buffer support
        }

        /// <summary>
        /// Structure defining a single Gerstner wave
        /// </summary>
        private struct GerstnerWave
        {
            public Vector2 direction;  // Wave propagation direction (normalized)
            public float wavelength;    // Distance between wave peaks
            public float amplitude;     // Wave height
            public float steepness;     // How peaked the wave is (0-1)
            public float speed;         // Phase speed
        }

        /// <summary>
        /// Result of Gerstner wave evaluation including derivatives
        /// </summary>
        private struct GerstnerWaveResult
        {
            public Vector3 position;
            public Vector3 tangent;
            public Vector3 binormal;
        }
    }
}
