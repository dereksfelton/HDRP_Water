# QUICK FIX - Stage 0/Stage 1 Integration

**You encountered compilation errors. Here's the fast fix:**

---

## What Happened

The original Stage 1 files conflicted with:
- Unity 6.3 HDRP built-in functions
- Your existing Stage 0 code

---

## Quick Fix (3 Steps)

### Step 1: Remove Old Files
In Unity, delete these files you just copied:
```
Assets/AdvancedWater/Shaders/WaterCore.hlsl
Assets/AdvancedWater/Scripts/WaterProfile.cs
Assets/AdvancedWater/Shaders/WaterSurface.shader
```

### Step 2: Download Updated Files
Download these **UPDATED** versions:
- [WaterCore.hlsl](computer:///mnt/user-data/outputs/WaterCore.hlsl) ← Fixed HLSL conflicts
- [WaterProfile.cs](computer:///mnt/user-data/outputs/WaterProfile.cs) ← Added missing methods
- [WaterSurface.shader](computer:///mnt/user-data/outputs/WaterSurface.shader) ← Fixed function calls

### Step 3: Copy to Unity
```
WaterCore.hlsl    → Assets/AdvancedWater/Shaders/
WaterProfile.cs   → Assets/AdvancedWater/Scripts/
WaterSurface.shader → Assets/AdvancedWater/Shaders/
```

Plus the Editor file (if not already copied):
```
WaterSurfaceShaderGUI.cs → Assets/AdvancedWater/Editor/
```

---

## Verify

After Unity recompiles:
- ✅ Console should be clean (no errors)
- ✅ Water should render when you enter Play Mode
- ✅ Can assign WaterProfile to WaterSystem

---

## What Was Fixed

1. **HLSL conflicts** - Removed duplicate HDRP functions
2. **Missing method** - Added `ApplyToWaterSystem()` for Stage 0 compatibility  
3. **Missing properties** - Added underwater placeholders for Stage 7

**Details**: See [STAGE_01_INTEGRATION_FIXES.md](computer:///mnt/user-data/outputs/STAGE_01_INTEGRATION_FIXES.md) for complete explanation.

---

## Continue Stage 1

Once errors are cleared:
1. Continue following [STAGE_01_CHECKLIST.md](computer:///mnt/user-data/outputs/STAGE_01_CHECKLIST.md)
2. Create your material and profiles
3. Test the water rendering

---

**Status**: Updated files ready to download above ⬆️
