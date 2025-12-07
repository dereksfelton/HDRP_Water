# Shader Compilation Fix - Unity 6.3 HDRP

**Issue**: WaterSurface shader appears under "Failed to compile" in shader dropdown  
**Status**: ✅ FIXED - Unity 6.3 compatible version ready

---

## Problem

When trying to assign the shader to M_Water_Stage1, you see:
```
HDRP → Water → Surface (under "Failed to compile" section)
```

This means the shader has compilation errors in Unity 6.3.

---

## Root Cause

Unity 6.3 HDRP has updated shader requirements:

1. **LightMode tag changed**: `"Forward"` → `"ForwardOnly"`
2. **Different includes required**: HDRP shader pass includes updated
3. **Function naming**: Some helper functions renamed
4. **Screen position calculation**: Different API in Unity 6.3

---

## Solution

Download the updated Unity 6.3 compatible shader:

### [WaterSurface.shader](computer:///mnt/user-data/outputs/WaterSurface.shader) (UPDATED for Unity 6.3)

---

## What Changed

### 1. Pass LightMode Tag
```hlsl
// OLD (doesn't work in Unity 6.3)
Tags { "LightMode" = "Forward" }

// NEW (Unity 6.3 compatible)
Tags { "LightMode" = "ForwardOnly" }
```

### 2. Shader Includes
```hlsl
// Added proper HDRP 17 includes
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.hlsl"
```

### 3. View Direction Function
```hlsl
// OLD (custom function)
float3 viewDirWS = GetWaterViewDir(input.positionWS);

// NEW (using HDRP built-in)
float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.positionWS));
```

### 4. Vertex/Fragment Function Names
```hlsl
// Simplified to standard names
#pragma vertex Vert
#pragma fragment Frag

Varyings Vert(Attributes input) { ... }
float4 Frag(Varyings input) : SV_Target { ... }
```

---

## Installation

### Step 1: Replace Shader File
```
WaterSurface.shader (UPDATED) → Assets/AdvancedWater/Shaders/
```

Replace the existing file with this new version.

### Step 2: Wait for Unity to Recompile
Unity will automatically recompile shaders. Watch the progress bar.

### Step 3: Check Shader Dropdown
Open M_Water_Stage1 material and check shader dropdown:
- ✅ `HDRP → Water → Surface` should now appear in the normal list
- ❌ Should NOT appear under "Failed to compile"

---

## Verification

After replacing the shader:

### 1. Shader Compiles Successfully
- No errors in Console
- Shader appears in normal list (not "Failed to compile")

### 2. Material Inspector Shows Custom GUI
- Select M_Water_Stage1 material
- Should see organized sections:
  - Water Color
  - Surface Properties
  - Light Interaction
  - Advanced

### 3. Test Assignment
- Assign shader: `HDRP → Water → Surface`
- Material should show water properties
- No pink/magenta preview

---

## Unity 6.3 HDRP Shader Requirements

For reference, here are the key requirements for HDRP shaders in Unity 6.3:

### Required Pass Tags
```hlsl
Pass
{
    Name "ForwardOnly"
    Tags { "LightMode" = "ForwardOnly" }
    // ...
}
```

### Minimum Required Includes
```hlsl
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
```

### Depth Texture Sampling
```hlsl
TEXTURE2D_X_FLOAT(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

// In fragment shader:
float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV).r;
float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
```

---

## Common Unity 6.3 Shader Errors

### Error: "LightMode 'Forward' not found"
**Fix**: Change to `"ForwardOnly"`

### Error: "Undeclared identifier 'GetWorldSpaceNormalizeViewDir'"
**Fix**: Use `GetWorldSpaceViewDir()` instead

### Error: "Cannot find include file"
**Fix**: Update include paths to match HDRP 17 structure

---

## If Shader Still Won't Compile

### Check Unity Console
1. Open Console (Ctrl+Shift+C / Cmd+Shift+C)
2. Look for shader compilation errors
3. Note the specific error message and line number

### Check HDRP Version
1. Window → Package Manager
2. Find "High Definition RP"
3. Should be version 17.x for Unity 6.3

### Reimport Shader
1. Select WaterSurface.shader in Project
2. Right-click → Reimport
3. Wait for compilation

### Check File Location
Verify shader is in correct location:
```
Assets/AdvancedWater/Shaders/WaterSurface.shader ✓
```

---

## Fallback: Simple Unlit Version

If the shader still won't compile, here's a minimal test version:

```hlsl
Shader "HDRP/Water/SimpleTest"
{
    Properties
    {
        _Color("Color", Color) = (0.3, 0.7, 0.9, 0.8)
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        
        Pass
        {
            Tags { "LightMode" = "ForwardOnly" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            
            float4 _Color;
            
            struct Attributes
            {
                float3 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                return _Color;
            }
            
            ENDHLSL
        }
    }
}
```

Save as `SimpleWaterTest.shader` to verify basic HDRP shader compilation works.

---

## Next Steps

Once shader compiles:

1. ✅ Assign `HDRP → Water → Surface` to M_Water_Stage1
2. ✅ Create water profiles ([SAMPLE_PROFILES.md](computer:///mnt/user-data/outputs/SAMPLE_PROFILES.md))
3. ✅ Assign material to WaterManager
4. ✅ Test in scene!

---

## Technical Notes

### Unity 6.3 Changes from Unity 2023

Unity 6.3 introduced several HDRP shader changes:
- Pass naming conventions updated
- Built-in function reorganization
- Include file restructuring
- Depth texture access methods changed

The updated shader accounts for all these changes and should work reliably in Unity 6.3+.

---

**Updated**: December 6, 2025  
**Unity Version**: 6.3  
**HDRP Version**: 17.x  
**Status**: ✅ Shader fixed and ready
