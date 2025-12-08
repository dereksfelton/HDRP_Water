#ifndef WATER_WAVES_INCLUDED
#define WATER_WAVES_INCLUDED

// ============================================================================
// STAGE 02: Water Wave Animation - Gerstner Waves & Multi-Octave Noise
// ============================================================================
// This file contains the mathematical implementation for realistic water
// surface animation using Gerstner (trochoidal) waves and Perlin noise.
//
// Compatible with: Unity 6.3, HDRP 17, RTX 5000-series
// ============================================================================

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

// Wave Constants
#define MAX_WAVE_LAYERS 8
#define GRAVITY 9.81
#define TWO_PI 6.28318530718

// ============================================================================
// STRUCTURE DEFINITIONS
// ============================================================================

/// <summary>
/// Single wave layer parameters
/// Represents one Gerstner wave component
/// </summary>
struct WaveLayer
{
    float2 direction;      // Normalized wave direction (XZ plane)
    float amplitude;       // Wave height (meters)
    float wavelength;      // Distance between crests (meters)
    float steepness;       // Wave sharpness (0 = sine wave, 1 = sharp crest)
    float speed;           // Wave propagation speed (meters/second)
    float phase;           // Initial phase offset (radians)
};

/// <summary>
/// Combined wave displacement result
/// Contains position offset and derived normal
/// </summary>
struct WaveResult
{
    float3 position;       // Displaced vertex position (world space)
    float3 normal;         // Surface normal at displaced position
    float3 tangent;        // Surface tangent (for normal mapping)
    float3 binormal;       // Surface binormal (for normal mapping)
};

// ============================================================================
// UTILITY FUNCTIONS
// ============================================================================

/// <summary>
/// Fast sine/cosine calculation for wave animation
/// Uses hardware interpolation for performance
/// </summary>
void FastSinCos(float angle, out float sinOut, out float cosOut)
{
    sinOut = sin(angle);
    cosOut = cos(angle);
}

/// <summary>
/// Calculate wave frequency (omega) from wavelength
/// omega = 2π / wavelength
/// </summary>
float CalculateWaveFrequency(float wavelength)
{
    return TWO_PI / wavelength;
}

/// <summary>
/// Calculate wave phase speed using deep water dispersion relation
/// c = sqrt(g * lambda / 2π)
/// For shallow water: c = sqrt(g * depth)
/// </summary>
float CalculatePhaseSpeed(float wavelength)
{
    float omega = CalculateWaveFrequency(wavelength);
    return sqrt(GRAVITY / omega);
}

/// <summary>
/// Calculate maximum steepness before wave breaks
/// Q_max = 1 / (frequency * amplitude * numWaves)
/// </summary>
float CalculateMaxSteepness(float frequency, float amplitude, int numWaves)
{
    return 1.0 / (frequency * amplitude * numWaves);
}

// ============================================================================
// GERSTNER WAVE IMPLEMENTATION
// ============================================================================

/// <summary>
/// Calculate single Gerstner wave displacement and derivatives
/// 
/// Gerstner waves create realistic ocean motion with:
/// - Circular particle trajectories
/// - Sharp crests and rounded troughs
/// - Proper wave breaking characteristics
/// 
/// Position offset: Q * D * A * sin(dot(D, P) * omega + t * phi)
/// Where Q = steepness, D = direction, A = amplitude,
///       omega = frequency, phi = phase speed
/// </summary>
void CalculateGerstnerWave(
    WaveLayer wave,
    float3 worldPos,
    float time,
    inout float3 positionOffset,
    inout float3 tangent,
    inout float3 binormal)
{
    // Calculate wave parameters
    float omega = CalculateWaveFrequency(wave.wavelength);
    float phaseSpeed = wave.speed;
    
    // Calculate wave phase
    float phase = dot(wave.direction, worldPos.xz) * omega + time * phaseSpeed + wave.phase;
    
    // Calculate sin and cos for efficiency
    float sinPhase, cosPhase;
    FastSinCos(phase, sinPhase, cosPhase);
    
    // Wave amplitude contribution
    float WA = omega * wave.amplitude;
    
    // Position displacement
    // Horizontal displacement (creates circular motion)
    positionOffset.x += wave.steepness * wave.direction.x * wave.amplitude * cosPhase;
    positionOffset.z += wave.steepness * wave.direction.y * wave.amplitude * cosPhase;
    
    // Vertical displacement
    positionOffset.y += wave.amplitude * sinPhase;
    
    // Normal derivatives (for proper lighting)
    // Using partial derivatives of the Gerstner wave equation
    float QWA = wave.steepness * WA;
    
    // Tangent derivatives
    tangent.x += -QWA * wave.direction.x * wave.direction.x * sinPhase;
    tangent.y += wave.direction.x * WA * cosPhase;
    tangent.z += -QWA * wave.direction.x * wave.direction.y * sinPhase;
    
    // Binormal derivatives
    binormal.x += -QWA * wave.direction.x * wave.direction.y * sinPhase;
    binormal.y += wave.direction.y * WA * cosPhase;
    binormal.z += -QWA * wave.direction.y * wave.direction.y * sinPhase;
}

/// <summary>
/// Calculate combined result from multiple Gerstner wave layers
/// Sums contribution from all active waves
/// </summary>
WaveResult CalculateWaves(
    WaveLayer waves[MAX_WAVE_LAYERS],
    int numWaves,
    float3 worldPos,
    float time)
{
    WaveResult result;
    
    // Initialize accumulators
    float3 positionOffset = float3(0, 0, 0);
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);
    
    // Sum all wave contributions
    for (int i = 0; i < numWaves && i < MAX_WAVE_LAYERS; i++)
    {
        CalculateGerstnerWave(
            waves[i],
            worldPos,
            time,
            positionOffset,
            tangent,
            binormal);
    }
    
    // Calculate final position
    result.position = worldPos + positionOffset;
    
    // Calculate normal from tangent space vectors
    result.tangent = normalize(tangent);
    result.binormal = normalize(binormal);
    result.normal = normalize(cross(result.binormal, result.tangent));
    
    return result;
}

// ============================================================================
// NOISE-BASED RIPPLES
// ============================================================================

/// <summary>
/// Simple 2D Perlin-style noise for ripple detail
/// Hash-based pseudo-random generation for GPU efficiency
/// </summary>
float Hash2D(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}

/// <summary>
/// 2D noise function using bilinear interpolation
/// Returns value in range [0, 1]
/// </summary>
float Noise2D(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    
    // Smooth interpolation curve (smoothstep)
    float2 u = f * f * (3.0 - 2.0 * f);
    
    // Four corner values
    float a = Hash2D(i);
    float b = Hash2D(i + float2(1.0, 0.0));
    float c = Hash2D(i + float2(0.0, 1.0));
    float d = Hash2D(i + float2(1.0, 1.0));
    
    // Bilinear interpolation
    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

/// <summary>
/// Multi-octave fractal noise for detailed ripples
/// Each octave adds higher-frequency detail at lower amplitude
/// </summary>
float FractalNoise(float2 p, int octaves, float frequency, float amplitude, float lacunarity, float persistence)
{
    float value = 0.0;
    float currentAmplitude = amplitude;
    float currentFrequency = frequency;
    
    for (int i = 0; i < octaves; i++)
    {
        value += Noise2D(p * currentFrequency) * currentAmplitude;
        currentAmplitude *= persistence;  // Each octave has less influence
        currentFrequency *= lacunarity;   // Each octave has higher frequency
    }
    
    return value;
}

/// <summary>
/// Calculate ripple displacement using animated noise
/// Creates small-scale surface detail like wind chop
/// </summary>
float3 CalculateRipples(
    float3 worldPos,
    float time,
    float2 windDirection,
    float windSpeed,
    float rippleScale,
    float rippleStrength,
    int octaves)
{
    // Animate noise based on wind
    float2 animatedPos = worldPos.xz + windDirection * time * windSpeed;
    
    // Multi-octave noise for detail
    float noise = FractalNoise(
        animatedPos * rippleScale,
        octaves,
        1.0,           // Base frequency
        1.0,           // Base amplitude
        2.0,           // Lacunarity (frequency multiplier)
        0.5);          // Persistence (amplitude multiplier)
    
    // Convert noise to displacement
    // Center around 0 and apply strength
    float displacement = (noise - 0.5) * 2.0 * rippleStrength;
    
    // Return vertical displacement only
    return float3(0, displacement, 0);
}

/// <summary>
/// Calculate normal map from noise for ripple detail
/// Uses finite differences to approximate derivatives
/// </summary>
float3 CalculateRippleNormal(
    float3 worldPos,
    float time,
    float2 windDirection,
    float windSpeed,
    float rippleScale,
    float rippleStrength,
    int octaves,
    float sampleOffset)
{
    // Sample height at three points
    float2 animatedPos = worldPos.xz + windDirection * time * windSpeed;
    
    float h = FractalNoise(animatedPos * rippleScale, octaves, 1.0, 1.0, 2.0, 0.5);
    float hX = FractalNoise((animatedPos + float2(sampleOffset, 0)) * rippleScale, octaves, 1.0, 1.0, 2.0, 0.5);
    float hZ = FractalNoise((animatedPos + float2(0, sampleOffset)) * rippleScale, octaves, 1.0, 1.0, 2.0, 0.5);
    
    // Calculate derivatives
    float dX = (hX - h) / sampleOffset * rippleStrength;
    float dZ = (hZ - h) / sampleOffset * rippleStrength;
    
    // Construct normal
    float3 normal = normalize(float3(-dX, 1.0, -dZ));
    return normal;
}

// ============================================================================
// COMBINED WAVE + RIPPLE CALCULATION
// ============================================================================

/// <summary>
/// Calculate final surface displacement from waves and ripples
/// Combines Gerstner waves (large-scale) with noise ripples (small-scale)
/// </summary>
WaveResult CalculateSurfaceAnimation(
    WaveLayer waves[MAX_WAVE_LAYERS],
    int numWaves,
    float3 worldPos,
    float time,
    float2 windDirection,
    float windSpeed,
    float rippleScale,
    float rippleStrength,
    int rippleOctaves,
    float rippleNormalSampleOffset)
{
    // Calculate large-scale Gerstner waves
    WaveResult result = CalculateWaves(waves, numWaves, worldPos, time);
    
    // Add small-scale ripple detail
    float3 rippleDisplacement = CalculateRipples(
        result.position,  // Use displaced position for ripples
        time,
        windDirection,
        windSpeed,
        rippleScale,
        rippleStrength,
        rippleOctaves);
    
    result.position += rippleDisplacement;
    
    // Blend ripple normals with wave normals
    if (rippleStrength > 0.001)
    {
        float3 rippleNormal = CalculateRippleNormal(
            result.position,
            time,
            windDirection,
            windSpeed,
            rippleScale,
            rippleStrength,
            rippleOctaves,
            rippleNormalSampleOffset);
        
        // Blend normals (weighted average)
        result.normal = normalize(lerp(result.normal, rippleNormal, 0.3));
    }
    
    return result;
}

// ============================================================================
// LEVEL OF DETAIL (LOD) HELPERS
// ============================================================================

/// <summary>
/// Calculate appropriate wave count based on distance from camera
/// Reduces wave layers for distant water to improve performance
/// </summary>
int CalculateLODWaveCount(float distanceToCamera, int maxWaves, float lodDistance0, float lodDistance1)
{
    if (distanceToCamera < lodDistance0)
    {
        return maxWaves; // Full detail
    }
    else if (distanceToCamera < lodDistance1)
    {
        // Interpolate wave count
        float t = (distanceToCamera - lodDistance0) / (lodDistance1 - lodDistance0);
        return (int)lerp((float)maxWaves, (float)max(maxWaves / 2, 1), t);
    }
    else
    {
        return max(maxWaves / 2, 1); // Minimum waves
    }
}

/// <summary>
/// Calculate ripple detail level based on distance
/// Reduces octaves for distant surfaces
/// </summary>
int CalculateLODRippleOctaves(float distanceToCamera, int maxOctaves, float lodDistance)
{
    if (distanceToCamera < lodDistance)
    {
        return maxOctaves;
    }
    else
    {
        return max(maxOctaves / 2, 1);
    }
}

#endif // WATER_WAVES_INCLUDED
