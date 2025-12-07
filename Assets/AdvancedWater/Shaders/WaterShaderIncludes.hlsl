// WaterShaderIncludes.hlsl
// Common functions and utilities for water shaders in HDRP
// Place this file in your project's Shaders folder

#ifndef WATER_SHADER_INCLUDES
#define WATER_SHADER_INCLUDES

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

// ============================================================================
// WATER CONSTANTS
// ============================================================================

#define MAX_WAVE_COUNT 8
#define PI 3.14159265359
#define TWO_PI 6.28318530718

// ============================================================================
// WATER PROPERTIES
// ============================================================================

// Colors
CBUFFER_START(WaterColors)
    float4 _ShallowColor;
    float4 _DeepColor;
    float4 _FoamColor;
    float4 _CausticsColor;
CBUFFER_END

// Optical properties
CBUFFER_START(WaterOptical)
    float _DepthFalloff;
    float _Clarity;
    float _Absorption;
    float _Scattering;
    float _RefractionIndex;
CBUFFER_END

// Surface properties
CBUFFER_START(WaterSurface)
    float _Smoothness;
    float _Metallic;
    float _SurfaceScale;
CBUFFER_END

// Wave parameters (packed for efficiency)
CBUFFER_START(WaterWaves)
    float4 _WaveA[MAX_WAVE_COUNT]; // xy: direction, z: amplitude, w: wavelength
    float4 _WaveB[MAX_WAVE_COUNT]; // x: steepness, y: speed, zw: reserved
    float _WaveCount;
    float _WaveStrength;
    float _WaveSpeed;
    float _Time;
CBUFFER_END

// Textures
TEXTURE2D(_NormalMap);
SAMPLER(sampler_NormalMap);

TEXTURE2D(_FoamTexture);
SAMPLER(sampler_FoamTexture);

TEXTURE2D(_CausticsTexture);
SAMPLER(sampler_CausticsTexture);

TEXTURE2D(_PlanarReflectionTexture);
SAMPLER(sampler_PlanarReflectionTexture);

TEXTURE2D(_RefractionTexture);
SAMPLER(sampler_RefractionTexture);

// ============================================================================
// UTILITY FUNCTIONS
// ============================================================================

// Convert world position to water local space
float3 WorldToWaterLocal(float3 worldPos, float4x4 waterMatrix)
{
    return mul(waterMatrix, float4(worldPos, 1.0)).xyz;
}

// Sample scene depth at screen UV
float SampleSceneDepth(float2 screenUV)
{
    return SAMPLE_TEXTURE2D_X_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV, 0).r;
}

// Calculate water depth from scene depth
float CalculateWaterDepth(float3 worldPos, float2 screenUV)
{
    float sceneDepth = SampleSceneDepth(screenUV);
    float3 sceneWorldPos = ComputeWorldSpacePosition(screenUV, sceneDepth, UNITY_MATRIX_I_VP);
    return sceneWorldPos.y - worldPos.y;
}

// ============================================================================
// GERSTNER WAVE FUNCTIONS
// ============================================================================

struct GerstnerWaveResult
{
    float3 position;
    float3 normal;
    float3 tangent;
    float3 binormal;
};

// Evaluate a single Gerstner wave
GerstnerWaveResult EvaluateGerstnerWave(
    float2 position,
    float2 direction,
    float amplitude,
    float wavelength,
    float steepness,
    float speed,
    float time)
{
    GerstnerWaveResult result;
    
    float k = TWO_PI / wavelength; // Wave number
    float c = speed;
    float a = amplitude;
    float Q = steepness / (k * a * MAX_WAVE_COUNT);
    
    float d = k * (dot(direction, position) - c * time);
    float cosD = cos(d);
    float sinD = sin(d);
    
    // Position offset
    result.position = float3(
        Q * a * direction.x * cosD,
        a * sinD,
        Q * a * direction.y * cosD
    );
    
    // Derivatives for normal calculation
    float wa = k * a;
    float S = sinD;
    float C = cosD;
    
    result.tangent = float3(
        1.0 - Q * wa * direction.x * direction.x * S,
        wa * direction.x * C,
        -Q * wa * direction.x * direction.y * S
    );
    
    result.binormal = float3(
        -Q * wa * direction.x * direction.y * S,
        wa * direction.y * C,
        1.0 - Q * wa * direction.y * direction.y * S
    );
    
    result.normal = normalize(cross(result.binormal, result.tangent));
    
    return result;
}

// Evaluate all Gerstner waves at a position
GerstnerWaveResult EvaluateAllWaves(float3 worldPos, float time)
{
    GerstnerWaveResult result;
    result.position = float3(0, 0, 0);
    result.tangent = float3(1, 0, 0);
    result.binormal = float3(0, 0, 1);
    result.normal = float3(0, 1, 0);
    
    float2 pos = worldPos.xz;
    
    for (int i = 0; i < (int)_WaveCount && i < MAX_WAVE_COUNT; i++)
    {
        float2 direction = _WaveA[i].xy;
        float amplitude = _WaveA[i].z * _WaveStrength;
        float wavelength = _WaveA[i].w;
        float steepness = _WaveB[i].x;
        float speed = _WaveB[i].y * _WaveSpeed;
        
        GerstnerWaveResult wave = EvaluateGerstnerWave(
            pos, direction, amplitude, wavelength, steepness, speed, time
        );
        
        result.position += wave.position;
        result.tangent += wave.tangent;
        result.binormal += wave.binormal;
    }
    
    result.normal = normalize(cross(result.binormal, result.tangent));
    
    return result;
}

// ============================================================================
// WATER COLOR & DEPTH FUNCTIONS
// ============================================================================

// Calculate water color based on depth
float4 CalculateWaterColor(float depth, float3 viewDir, float3 normal)
{
    // Depth-based color interpolation
    float depthFactor = saturate(depth / _DepthFalloff);
    float4 waterColor = lerp(_ShallowColor, _DeepColor, depthFactor);
    
    // Fresnel effect
    float fresnel = pow(1.0 - saturate(dot(viewDir, normal)), 5.0);
    
    return waterColor;
}

// Calculate transparency based on depth
float CalculateTransparency(float depth)
{
    return 1.0 - saturate(depth / _Clarity);
}

// ============================================================================
// FOAM FUNCTIONS
// ============================================================================

// Calculate foam intensity based on wave crest and depth
float CalculateFoamIntensity(float3 worldPos, float depth, float waveHeight)
{
    // Foam on wave crests
    float crestFoam = saturate(waveHeight * 2.0);
    
    // Foam in shallow water
    float shallowFoam = saturate(1.0 - depth / 2.0);
    
    return max(crestFoam, shallowFoam);
}

// Sample foam texture with animation
float SampleFoam(float2 uv, float time, float intensity)
{
    float2 animatedUV1 = uv * 2.0 + float2(time * 0.1, time * 0.05);
    float2 animatedUV2 = uv * 3.0 - float2(time * 0.08, time * 0.12);
    
    float foam1 = SAMPLE_TEXTURE2D(_FoamTexture, sampler_FoamTexture, animatedUV1).r;
    float foam2 = SAMPLE_TEXTURE2D(_FoamTexture, sampler_FoamTexture, animatedUV2).r;
    
    return (foam1 + foam2) * 0.5 * intensity;
}

// ============================================================================
// REFLECTION & REFRACTION
// ============================================================================

// Calculate reflection UV from view direction and normal
float2 CalculateReflectionUV(float3 viewDir, float3 normal, float2 screenUV)
{
    float3 reflection = reflect(-viewDir, normal);
    return screenUV + reflection.xy * 0.1;
}

// Calculate refraction UV with chromatic aberration
float2 CalculateRefractionUV(float3 normal, float2 screenUV, float strength)
{
    return screenUV + normal.xz * strength * 0.05;
}

// ============================================================================
// CAUSTICS
// ============================================================================

// Sample animated caustics
float SampleCaustics(float3 worldPos, float time, float depth)
{
    float2 uv = worldPos.xz * 0.1;
    
    float2 uv1 = uv + float2(time * 0.02, time * 0.03);
    float2 uv2 = uv * 1.5 - float2(time * 0.025, time * 0.02);
    
    float caustics1 = SAMPLE_TEXTURE2D(_CausticsTexture, sampler_CausticsTexture, uv1).r;
    float caustics2 = SAMPLE_TEXTURE2D(_CausticsTexture, sampler_CausticsTexture, uv2).r;
    
    float caustics = min(caustics1, caustics2);
    
    // Fade with depth
    float depthFade = saturate(1.0 - depth / 10.0);
    
    return caustics * depthFade;
}

#endif // WATER_SHADER_INCLUDES
