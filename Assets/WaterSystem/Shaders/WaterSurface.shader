Shader "HDRP/Water/Surface"
{
    Properties
    {
        // ====================================================================
        // STAGE 1: VISUAL PROPERTIES
        // ====================================================================
        
        // Water Colors
        _ShallowColor("Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepColor("Deep Color", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance", Float) = 20
        
        // Surface Properties
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.95
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        
        // Normal Mapping
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0
        _NormalTiling("Normal Tiling", Vector) = (1, 1, 0, 0)
        _NormalScrollSpeed("Normal Scroll Speed", Range(0, 0.1)) = 0.02
        
        // Light Interaction
        _AbsorptionColor("Absorption Color", Color) = (0.45, 0.029, 0.018, 1.0)
        _ScatteringColor("Scattering Color", Color) = (0.0, 0.46, 0.54, 1.0)
        _ScatteringPower("Scattering Power", Range(0.0, 10.0)) = 3.0
        
        // Fresnel and Refraction
        _FresnelPower("Fresnel Power", Range(0.0, 10.0)) = 5.0
        _RefractionStrength("Refraction Strength", Range(0.0, 1.0)) = 0.1
        
        // ====================================================================
        // STAGE 2: WAVE ANIMATION PROPERTIES [NEW]
        // ====================================================================
        
        // Wave Animation - Managed by WaterSurfaceAnimator
        
        // Time
        _WaterTime("Water Time", Float) = 0
        
        // Wave count
        _WaveCount("Wave Count", Float) = 0
        
        // Wave Layer 0
        _Wave0_Direction("Wave 0 Direction", Vector) = (1, 0, 0, 0)
        _Wave0_Amplitude("Wave 0 Amplitude", Float) = 0.5
        _Wave0_Wavelength("Wave 0 Wavelength", Float) = 10
        _Wave0_Steepness("Wave 0 Steepness", Float) = 0.5
        _Wave0_Speed("Wave 0 Speed", Float) = 0
        _Wave0_Phase("Wave 0 Phase", Float) = 0
        
        // Wave Layer 1
        _Wave1_Direction("Wave 1 Direction", Vector) = (0.7, 0.7, 0, 0)
        _Wave1_Amplitude("Wave 1 Amplitude", Float) = 0.3
        _Wave1_Wavelength("Wave 1 Wavelength", Float) = 8
        _Wave1_Steepness("Wave 1 Steepness", Float) = 0.4
        _Wave1_Speed("Wave 1 Speed", Float) = 0
        _Wave1_Phase("Wave 1 Phase", Float) = 1.57
        
        // Wave Layers 2-7 (properties defined but values set by C#)
        _Wave2_Direction("Wave 2 Direction", Vector) = (0, 0, 0, 0)
        _Wave2_Amplitude("Wave 2 Amplitude", Float) = 0
        _Wave2_Wavelength("Wave 2 Wavelength", Float) = 1
        _Wave2_Steepness("Wave 2 Steepness", Float) = 0
        _Wave2_Speed("Wave 2 Speed", Float) = 0
        _Wave2_Phase("Wave 2 Phase", Float) = 0
        
        _Wave3_Direction("Wave 3 Direction", Vector) = (0, 0, 0, 0)
        _Wave3_Amplitude("Wave 3 Amplitude", Float) = 0
        _Wave3_Wavelength("Wave 3 Wavelength", Float) = 1
        _Wave3_Steepness("Wave 3 Steepness", Float) = 0
        _Wave3_Speed("Wave 3 Speed", Float) = 0
        _Wave3_Phase("Wave 3 Phase", Float) = 0
        
        _Wave4_Direction("Wave 4 Direction", Vector) = (0, 0, 0, 0)
        _Wave4_Amplitude("Wave 4 Amplitude", Float) = 0
        _Wave4_Wavelength("Wave 4 Wavelength", Float) = 1
        _Wave4_Steepness("Wave 4 Steepness", Float) = 0
        _Wave4_Speed("Wave 4 Speed", Float) = 0
        _Wave4_Phase("Wave 4 Phase", Float) = 0
        
        _Wave5_Direction("Wave 5 Direction", Vector) = (0, 0, 0, 0)
        _Wave5_Amplitude("Wave 5 Amplitude", Float) = 0
        _Wave5_Wavelength("Wave 5 Wavelength", Float) = 1
        _Wave5_Steepness("Wave 5 Steepness", Float) = 0
        _Wave5_Speed("Wave 5 Speed", Float) = 0
        _Wave5_Phase("Wave 5 Phase", Float) = 0
        
        _Wave6_Direction("Wave 6 Direction", Vector) = (0, 0, 0, 0)
        _Wave6_Amplitude("Wave 6 Amplitude", Float) = 0
        _Wave6_Wavelength("Wave 6 Wavelength", Float) = 1
        _Wave6_Steepness("Wave 6 Steepness", Float) = 0
        _Wave6_Speed("Wave 6 Speed", Float) = 0
        _Wave6_Phase("Wave 6 Phase", Float) = 0
        
        _Wave7_Direction("Wave 7 Direction", Vector) = (0, 0, 0, 0)
        _Wave7_Amplitude("Wave 7 Amplitude", Float) = 0
        _Wave7_Wavelength("Wave 7 Wavelength", Float) = 1
        _Wave7_Steepness("Wave 7 Steepness", Float) = 0
        _Wave7_Speed("Wave 7 Speed", Float) = 0
        _Wave7_Phase("Wave 7 Phase", Float) = 0
        
        // Ripple properties
        _RippleOctaves("Ripple Octaves", Float) = 3
        _WindDirection("Wind Direction", Vector) = (1, 0.5, 0, 0)
        _WindSpeed("Wind Speed", Float) = 2
        _RippleScale("Ripple Scale", Float) = 1
        _RippleStrength("Ripple Strength", Float) = 0.1
        _RippleNormalSampleOffset("Ripple Normal Sample Offset", Float) = 0.1
        
        // LOD
        _LODDistance0("LOD Distance 0", Float) = 100
        _LODDistance1("LOD Distance 1", Float) = 500
        _RippleLODDistance("Ripple LOD Distance", Float) = 75
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "HDRenderPipeline"
        }
        
        LOD 300
        
        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode" = "ForwardOnly" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
            
            // Shader features
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            
            // Shader variants
            #pragma shader_feature_local _WAVES_ENABLED
            #pragma shader_feature_local _RIPPLES_ENABLED
            
            #pragma vertex WaterVertex
            #pragma fragment WaterFragment
            
            // Define shadow quality levels (required by HDRP before including shadow files)
            #define PUNCTUAL_SHADOW_MEDIUM
            #define DIRECTIONAL_SHADOW_MEDIUM
            #define AREA_SHADOW_MEDIUM
            
            // HDRP includes - CORRECT ORDER IS CRITICAL
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
            
            // Custom water includes
            #include "Include/WaterCore.hlsl"
            #include "Include/WaterWaves.hlsl"
            
            // ================================================================
            // SHADER PROPERTIES
            // ================================================================
            
            // Stage 1: Visual properties
            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float _DepthMaxDistance;
                float _Smoothness;
                float _Metallic;
                float4 _AbsorptionColor;
                float4 _ScatteringColor;
                float _ScatteringPower;
                float _FresnelPower;
                float _RefractionStrength;
                float _BumpScale;
                float4 _NormalTiling;
                float _NormalScrollSpeed;
                
                // Stage 2: Wave animation properties
                float _WaterTime;
                int _WaveCount;
                
                // Wave layers
                float4 _Wave0_Direction;
                float _Wave0_Amplitude;
                float _Wave0_Wavelength;
                float _Wave0_Steepness;
                float _Wave0_Speed;
                float _Wave0_Phase;
                
                float4 _Wave1_Direction;
                float _Wave1_Amplitude;
                float _Wave1_Wavelength;
                float _Wave1_Steepness;
                float _Wave1_Speed;
                float _Wave1_Phase;
                
                float4 _Wave2_Direction;
                float _Wave2_Amplitude;
                float _Wave2_Wavelength;
                float _Wave2_Steepness;
                float _Wave2_Speed;
                float _Wave2_Phase;
                
                float4 _Wave3_Direction;
                float _Wave3_Amplitude;
                float _Wave3_Wavelength;
                float _Wave3_Steepness;
                float _Wave3_Speed;
                float _Wave3_Phase;
                
                float4 _Wave4_Direction;
                float _Wave4_Amplitude;
                float _Wave4_Wavelength;
                float _Wave4_Steepness;
                float _Wave4_Speed;
                float _Wave4_Phase;
                
                float4 _Wave5_Direction;
                float _Wave5_Amplitude;
                float _Wave5_Wavelength;
                float _Wave5_Steepness;
                float _Wave5_Speed;
                float _Wave5_Phase;
                
                float4 _Wave6_Direction;
                float _Wave6_Amplitude;
                float _Wave6_Wavelength;
                float _Wave6_Steepness;
                float _Wave6_Speed;
                float _Wave6_Phase;
                
                float4 _Wave7_Direction;
                float _Wave7_Amplitude;
                float _Wave7_Wavelength;
                float _Wave7_Steepness;
                float _Wave7_Speed;
                float _Wave7_Phase;
                
                // Ripple properties
                int _RippleOctaves;
                float4 _WindDirection;
                float _WindSpeed;
                float _RippleScale;
                float _RippleStrength;
                float _RippleNormalSampleOffset;
                
                // LOD
                float _LODDistance0;
                float _LODDistance1;
                float _RippleLODDistance;
            CBUFFER_END
            
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            
            // ================================================================
            // VERTEX/FRAGMENT STRUCTURES
            // ================================================================
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // ================================================================
            // HELPER: BUILD WAVE LAYER ARRAY
            // ================================================================
            
            void BuildWaveLayers(out WaveLayer waves[MAX_WAVE_LAYERS])
            {
                waves[0].direction = _Wave0_Direction.xy;
                waves[0].amplitude = _Wave0_Amplitude;
                waves[0].wavelength = _Wave0_Wavelength;
                waves[0].steepness = _Wave0_Steepness;
                waves[0].speed = _Wave0_Speed;
                waves[0].phase = _Wave0_Phase;
                
                waves[1].direction = _Wave1_Direction.xy;
                waves[1].amplitude = _Wave1_Amplitude;
                waves[1].wavelength = _Wave1_Wavelength;
                waves[1].steepness = _Wave1_Steepness;
                waves[1].speed = _Wave1_Speed;
                waves[1].phase = _Wave1_Phase;
                
                waves[2].direction = _Wave2_Direction.xy;
                waves[2].amplitude = _Wave2_Amplitude;
                waves[2].wavelength = _Wave2_Wavelength;
                waves[2].steepness = _Wave2_Steepness;
                waves[2].speed = _Wave2_Speed;
                waves[2].phase = _Wave2_Phase;
                
                waves[3].direction = _Wave3_Direction.xy;
                waves[3].amplitude = _Wave3_Amplitude;
                waves[3].wavelength = _Wave3_Wavelength;
                waves[3].steepness = _Wave3_Steepness;
                waves[3].speed = _Wave3_Speed;
                waves[3].phase = _Wave3_Phase;
                
                waves[4].direction = _Wave4_Direction.xy;
                waves[4].amplitude = _Wave4_Amplitude;
                waves[4].wavelength = _Wave4_Wavelength;
                waves[4].steepness = _Wave4_Steepness;
                waves[4].speed = _Wave4_Speed;
                waves[4].phase = _Wave4_Phase;
                
                waves[5].direction = _Wave5_Direction.xy;
                waves[5].amplitude = _Wave5_Amplitude;
                waves[5].wavelength = _Wave5_Wavelength;
                waves[5].steepness = _Wave5_Steepness;
                waves[5].speed = _Wave5_Speed;
                waves[5].phase = _Wave5_Phase;
                
                waves[6].direction = _Wave6_Direction.xy;
                waves[6].amplitude = _Wave6_Amplitude;
                waves[6].wavelength = _Wave6_Wavelength;
                waves[6].steepness = _Wave6_Steepness;
                waves[6].speed = _Wave6_Speed;
                waves[6].phase = _Wave6_Phase;
                
                waves[7].direction = _Wave7_Direction.xy;
                waves[7].amplitude = _Wave7_Amplitude;
                waves[7].wavelength = _Wave7_Wavelength;
                waves[7].steepness = _Wave7_Steepness;
                waves[7].speed = _Wave7_Speed;
                waves[7].phase = _Wave7_Phase;
            }
            
            // ================================================================
            // VERTEX SHADER
            // ================================================================
            
            Varyings WaterVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // Get original world position
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                
                // ============================================================
                // STAGE 2: WAVE DISPLACEMENT [NEW]
                // ============================================================
                
                #if defined(_WAVES_ENABLED)
                    // Build wave layer array
                    WaveLayer waves[MAX_WAVE_LAYERS];
                    BuildWaveLayers(waves);
                    
                    // Calculate distance to camera for LOD
                    float distanceToCamera = length(_WorldSpaceCameraPos - positionWS);
                    
                    // Calculate LOD wave count
                    int lodWaveCount = CalculateLODWaveCount(
                        distanceToCamera,
                        _WaveCount,
                        _LODDistance0,
                        _LODDistance1);
                    
                    // Calculate ripple octaves
                    int lodRippleOctaves = CalculateLODRippleOctaves(
                        distanceToCamera,
                        _RippleOctaves,
                        _RippleLODDistance);
                    
                    // Calculate surface animation
                    WaveResult waveResult = CalculateSurfaceAnimation(
                        waves,
                        lodWaveCount,
                        positionWS,
                        _WaterTime,
                        _WindDirection.xy,
                        _WindSpeed,
                        _RippleScale,
                        _RippleStrength,
                        lodRippleOctaves,
                        _RippleNormalSampleOffset);
                    
                    // Apply wave displacement
                    positionWS = waveResult.position;
                    
                    // Use wave normal (will be further modified by normal map in fragment)
                    output.normalWS = waveResult.normal;
                    
                    // Calculate tangent space from wave derivatives
                    float3 bitangent = cross(output.normalWS, waveResult.tangent);
                    output.tangentWS = float4(waveResult.tangent, 1.0);
                    
                #else
                    // No wave animation - use original geometry
                    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                    output.tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
                #endif
                
                // Transform to clip space
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                
                // Calculate view direction
                output.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                
                // Pass through UVs
                output.uv = input.uv;
                
                return output;
            }
            
            // ================================================================
            // FRAGMENT SHADER
            // ================================================================
            
            float4 WaterFragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Normalize interpolated vectors
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // ============================================================
                // NORMAL MAPPING (with animated tiling)
                // ============================================================
                
                float2 normalUV = input.uv * _NormalTiling.xy;
                
                // Animate normal map UVs (subtle scrolling) - speed from profile
                #if defined(_WAVES_ENABLED)
                    normalUV += _WindDirection.xy * _WaterTime * _NormalScrollSpeed;
                #endif
                
                float3 normalTS = UnpackNormalScale(
                    SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, normalUV),
                    _BumpScale);
                
                // Transform normal to world space
                float3 tangentWS = normalize(input.tangentWS.xyz);
                float3 bitangentWS = cross(normalWS, tangentWS) * input.tangentWS.w;
                float3x3 tangentToWorld = float3x3(tangentWS, bitangentWS, normalWS);
                normalWS = normalize(mul(normalTS, tangentToWorld));
                
                // ============================================================
                // STAGE 1: WATER APPEARANCE (from previous stage)
                // ============================================================
                
                // Depth-based color (placeholder - will use proper depth in Stage 4)
                float depth = 1.0; // Temporary
                float3 waterColor = lerp(_ShallowColor.rgb, _DeepColor.rgb, 
                    saturate(depth / _DepthMaxDistance));
                
                // Fresnel effect
                float fresnel = CalculateFresnel(normalWS, viewDirWS, _FresnelPower);
                
                // Simple lighting - fixed directional light from above
                // For now, assume sunlight from above-forward direction
                // In Stage 3 we'll add proper HDRP light integration
                float3 lightDir = normalize(float3(0.5, 0.8, 0.3)); // Sun direction
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 lighting = float3(1, 1, 1) * NdotL;
                
                // Combine color and lighting
                float3 finalColor = waterColor * (lighting + 0.3); // Ambient
                
                // Apply fresnel (increases reflection at grazing angles)
                finalColor = lerp(finalColor, float3(1, 1, 1), fresnel * 0.3);
                
                // Alpha (with fresnel)
                float alpha = lerp(_ShallowColor.a, _DeepColor.a, saturate(depth / _DepthMaxDistance));
                alpha = lerp(alpha, 1.0, fresnel * 0.5);
                
                // Note: HDRP applies volumetric fog in post-processing, not per-pixel
                
                return float4(finalColor, alpha);
            }
            
            ENDHLSL
        }
    }
}
