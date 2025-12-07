Shader "HDRP/Water/Surface"
{
    Properties
    {
        // Water Color Properties
        _ShallowColor("Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepColor("Deep Color", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Depth Maximum Distance", Float) = 10
        
        // Surface Properties
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.95
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        
        // Normal Map
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0
        
        // Absorption & Scattering
        _AbsorptionColor("Absorption Color", Color) = (0.45, 0.029, 0.018, 1.0)
        _ScatteringColor("Scattering Color", Color) = (0.0, 0.46, 0.54, 1.0)
        _ScatteringPower("Scattering Power", Range(0.0, 10.0)) = 3.0
        
        // Fresnel
        _FresnelPower("Fresnel Power", Range(0.0, 10.0)) = 5.0
        
        // Refraction
        _RefractionStrength("Refraction Strength", Range(0.0, 1.0)) = 0.1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "HDRenderPipeline"
        }
        
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
            
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ DOTS_INSTANCING_ON
            
            #pragma vertex Vert
            #pragma fragment Frag
            
            // Minimal includes - only what's guaranteed to exist
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            
            // Material properties
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            // Note: _CameraDepthTexture is provided by HDRP ShaderVariables.hlsl
            
            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float _DepthMaxDistance;
                float _Smoothness;
                float _Metallic;
                float4 _BumpMap_ST;
                float _BumpScale;
                float4 _AbsorptionColor;
                float4 _ScatteringColor;
                float _ScatteringPower;
                float _FresnelPower;
                float _RefractionStrength;
            CBUFFER_END
            
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float3 viewDirWS : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // Helper function for screen position
            float4 ComputeWaterScreenPos(float4 positionCS)
            {
                float4 o = positionCS * 0.5f;
                o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
                o.zw = positionCS.zw;
                return o;
            }
            
            // Helper function for view direction
            float3 GetWaterViewDir(float3 positionWS)
            {
                return normalize(_WorldSpaceCameraPos - positionWS);
            }
            
            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                // Transform positions
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                output.positionWS = positionWS;
                output.positionCS = TransformWorldToHClip(positionWS);
                
                // Transform normals
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                // Transform tangent
                float3 tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
                float sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = float4(tangentWS, sign);
                
                // UVs
                output.uv = input.uv0 * _BumpMap_ST.xy + _BumpMap_ST.zw;
                
                // Screen position for depth sampling
                output.screenPos = ComputeWaterScreenPos(output.positionCS);
                
                // View direction
                output.viewDirWS = GetWaterViewDir(positionWS);
                
                return output;
            }
            
            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Sample normal map
                float4 normalSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                float3 normalTS = UnpackNormalScale(normalSample, _BumpScale);
                
                // Transform normal to world space
                float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w;
                float3x3 TBN = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, TBN));
                
                // Calculate depth
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                // Use HDRP's built-in depth texture (provided by ShaderVariables.hlsl)
                float rawDepth = LoadCameraDepth(screenUV * _ScreenSize.xy);
                
                // Linear depth calculation
                float sceneDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceDepth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
                float depthDifference = max(0, sceneDepth - surfaceDepth);
                
                // Water color based on depth
                float depthFactor = saturate(depthDifference / _DepthMaxDistance);
                float3 waterColor = lerp(_ShallowColor.rgb, _DeepColor.rgb, depthFactor);
                
                // View direction
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Fresnel effect
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                
                // Apply scattering
                float3 scattering = _ScatteringColor.rgb * pow(saturate(depthDifference * 0.1), _ScatteringPower);
                waterColor += scattering;
                
                // Simple directional lighting
                float3 lightDir = normalize(float3(0.5, 1.0, 0.3));
                float NdotL = saturate(dot(normalWS, lightDir));
                
                // Specular (Blinn-Phong)
                float3 halfDir = normalize(lightDir + viewDirWS);
                float specular = pow(saturate(dot(normalWS, halfDir)), _Smoothness * 100.0) * _Smoothness;
                
                // Combine lighting
                waterColor = waterColor * (0.7 + 0.3 * NdotL) + specular * 0.5;
                
                // Apply Fresnel
                waterColor = lerp(waterColor, waterColor * 1.5, fresnel * 0.3);
                
                // Final alpha
                float alpha = lerp(_ShallowColor.a, _DeepColor.a, depthFactor);
                alpha = saturate(alpha + fresnel * 0.15);
                
                return float4(waterColor, alpha);
            }
            
            ENDHLSL
        }
    }
    
    FallBack "Hidden/HDRP/FallbackError"
    CustomEditor "AdvancedWater.Editor.WaterSurfaceShaderGUI"
}
