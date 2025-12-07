// WaterCore.hlsl
// Core shader functions and utilities for the Water System
// Unity 6.3 HDRP Compatible

#ifndef WATER_CORE_INCLUDED
#define WATER_CORE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

// ============================================================================
// STAGE 0: Core Utilities
// ============================================================================
// Note: Many utility functions are now provided by HDRP includes
// We only define custom water-specific utilities here

/// <summary>
/// Get the odd negative scale factor for correct tangent space transformation
/// Only define if not already defined by HDRP
/// </summary>
#ifndef GetOddNegativeScale
float GetOddNegativeScale()
{
    return unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0;
}
#endif

/// <summary>
/// Get normalized view direction in world space
/// Custom implementation for water system
/// </summary>
float3 GetWaterViewDir(float3 positionWS)
{
    if (unity_OrthoParams.w == 0)
    {
        // Perspective
        return normalize(_WorldSpaceCameraPos - positionWS);
    }
    else
    {
        // Orthographic
        float4x4 viewMat = GetWorldToViewMatrix();
        return -viewMat[2].xyz;
    }
}

/// <summary>
/// Compute screen position from clip space position
/// </summary>
float4 ComputeWaterScreenPos(float4 positionCS)
{
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = positionCS.zw;
    return o;
}

// ============================================================================
// STAGE 1: Visual Appearance Utilities
// ============================================================================

/// <summary>
/// Calculate water depth from scene and surface depth
/// Returns depth in world units
/// </summary>
float GetWaterDepth(float sceneDepth, float surfaceDepth)
{
    return max(0.0, sceneDepth - surfaceDepth);
}

/// <summary>
/// Blend between shallow and deep water colors based on depth
/// </summary>
float3 BlendWaterColor(float3 shallowColor, float3 deepColor, float depth, float maxDepth)
{
    float factor = saturate(depth / maxDepth);
    return lerp(shallowColor, deepColor, factor);
}

/// <summary>
/// Calculate Fresnel effect using Schlick's approximation
/// </summary>
float CalculateFresnel(float3 normal, float3 viewDir, float power)
{
    float NdotV = saturate(dot(normal, viewDir));
    return pow(1.0 - NdotV, power);
}

/// <summary>
/// Calculate Fresnel with custom F0 (reflectance at normal incidence)
/// Typical F0 for water is ~0.02
/// </summary>
float CalculateFresnelF0(float3 normal, float3 viewDir, float F0)
{
    float NdotV = saturate(dot(normal, viewDir));
    return F0 + (1.0 - F0) * pow(1.0 - NdotV, 5.0);
}

/// <summary>
/// Apply water absorption based on depth
/// Simulates light absorption as it travels through water
/// </summary>
float3 ApplyAbsorption(float3 color, float3 absorptionColor, float depth)
{
    float3 absorption = exp(-absorptionColor * depth);
    return color * absorption;
}

/// <summary>
/// Calculate light scattering in water
/// </summary>
float3 CalculateScattering(float3 scatteringColor, float depth, float power)
{
    float scatterAmount = pow(saturate(depth * 0.1), power);
    return scatteringColor * scatterAmount;
}

/// <summary>
/// Simple specular calculation using Blinn-Phong
/// </summary>
float CalculateSpecular(float3 normal, float3 lightDir, float3 viewDir, float smoothness)
{
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normal, halfDir));
    float specPower = smoothness * 100.0;
    return pow(NdotH, specPower);
}

/// <summary>
/// Unpack normal map with scale
/// </summary>
float3 UnpackNormalScale(float4 packedNormal, float scale)
{
    #if defined(UNITY_NO_DXT5nm)
        float3 normal = packedNormal.xyz * 2.0 - 1.0;
        normal.xy *= scale;
        return normalize(normal);
    #else
        // DXT5nm format (AG channels contain normal data)
        packedNormal.w *= packedNormal.x;
        float3 normal;
        normal.xy = (packedNormal.wy * 2.0 - 1.0) * scale;
        normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
        return normal;
    #endif
}

// ============================================================================
// FUTURE STAGES: Placeholders
// ============================================================================

// Stage 2: Wave animation functions will be added here
// Stage 3: Reflection functions will be added here
// Stage 4: Refraction and underwater visibility functions will be added here
// Stage 5: Surface interaction functions will be added here

#endif // WATER_CORE_INCLUDED
