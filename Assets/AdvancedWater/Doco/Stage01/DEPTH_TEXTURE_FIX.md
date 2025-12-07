# Depth Texture Redefinition Fix

**Issue**: `redefinition of '_CameraDepthTexture'`  
**Line**: 68  
**Status**: ✅ FIXED

---

## Problem

HDRP's `ShaderVariables.hlsl` already declares `_CameraDepthTexture`. We were trying to declare it again, causing a redefinition error.

---

## Solution

**[WaterSurface.shader](computer:///mnt/user-data/outputs/WaterSurface.shader)** ← v4 - Uses HDRP built-ins

### What Changed:

**Removed our declaration:**
```hlsl
// ❌ Removed - already provided by HDRP
TEXTURE2D_X_FLOAT(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);
```

**Updated depth sampling to use HDRP function:**
```hlsl
// ✅ Use HDRP's built-in function
float rawDepth = LoadCameraDepth(screenUV * _ScreenSize.xy);
```

---

## Install

Replace:
```
Assets/AdvancedWater/Shaders/WaterSurface.shader
```

---

## Verify

After Unity recompiles:
- ✅ No redefinition errors
- ✅ Shader compiles successfully  
- ✅ Appears in shader dropdown
- ✅ Material works correctly

---

## Technical Note

HDRP provides these built-in depth functions in `ShaderVariables.hlsl`:
- `LoadCameraDepth(uint2 pixelCoords)` - Load depth at pixel coordinates
- `_CameraDepthTexture` - Depth texture (already declared)
- `_ScreenSize` - Screen dimensions

We now use the built-in function instead of trying to sample the texture ourselves.

---

**Updated**: December 6, 2025  
**Version**: v4  
**Status**: ✅ Ready to download
