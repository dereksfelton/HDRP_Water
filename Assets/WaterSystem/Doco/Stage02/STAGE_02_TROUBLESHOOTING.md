# Stage 2 Troubleshooting Guide

**Unity Version:** 6000.3.0f1 (Unity 6.3)  
**HDRP Version:** 17.3.0  
**Document Date:** December 8, 2025

This guide documents all issues encountered during Stage 2 implementation and their solutions.

---

## Table of Contents

1. [Shader Compilation Errors](#shader-compilation-errors)
2. [Rendering Issues](#rendering-issues)
3. [Animation Issues](#animation-issues)
4. [Performance Issues](#performance-issues)
5. [Profile Issues](#profile-issues)

---

## Shader Compilation Errors

### Error: "Syntax error, unexpected $undefined at line 9"

**Full Error:**
```
Shader error in '': syntax error, unexpected $undefined, 
expecting TVAL_ID or TVAL_VARREF at line 9
```

**Symptoms:**
- Shader fails to compile
- Empty shader name in error message
- Error points to Properties block

**Cause:**
Unity 6000.3.0f1 shader compiler bug - cannot parse `[Header("...")]` attributes

**Solution:**
Remove all `[Header()]` and `[Space()]` attributes from shader Properties:

```hlsl
// BEFORE (BROKEN):
[Header("Water Colors")]
_ShallowColor("Shallow Color", Color) = (...)

// AFTER (WORKS):
// Water Colors
_ShallowColor("Shallow Color", Color) = (...)
```

**Notes:**
- This is a Unity bug, not a code issue
- Bug exists in fresh HDRP projects
- Reported to Unity
- Likely fixed in Unity 6000.3.1f1+
- Header attributes are cosmetic only (organize Inspector UI)
- Zero functional impact from removing them

**Verification:**
1. Remove all `[Header()]` tags
2. Replace with comments: `// Section Name`
3. Reimport shader
4. Console should show 0 errors

---

### Error: "Undefined punctual shadow filter algorithm"

**Full Error:**
```
Shader error in 'HDRP/Water/Surface': "Undefined punctual shadow filter algorithm" 
at /Library/PackageCache/.../HDShadowAlgorithms.hlsl(67)
```

**Symptoms:**
- Shader compilation fails
- Multiple shadow-related errors
- Mentions PUNCTUAL_FILTER_ALGORITHM

**Cause:**
HDRP shadow system requires quality level defines before includes

**Solution:**
Add shadow quality defines BEFORE HDRP includes:

```hlsl
#pragma vertex WaterVertex
#pragma fragment WaterFragment

// Define shadow quality levels (required by HDRP)
#define PUNCTUAL_SHADOW_MEDIUM
#define DIRECTIONAL_SHADOW_MEDIUM
#define AREA_SHADOW_MEDIUM

// HDRP includes - CORRECT ORDER IS CRITICAL
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
```

**Options:**
- `_ULTRA_LOW` - Lowest quality
- `_LOW` - Low quality
- `_MEDIUM` - Recommended for most use cases
- `_HIGH` - High quality (more expensive)

**Notes:**
- Must be defined BEFORE includes
- Required for all three shadow types
- MEDIUM is good balance of quality/performance

---

### Error: "unrecognized identifier 'GLOBAL_CBUFFER_START'"

**Full Error:**
```
Shader error in 'HDRP/Water/Surface': unrecognized identifier 'GLOBAL_CBUFFER_START' 
at /Library/PackageCache/.../ShaderVariablesGlobal.cs.hlsl(14)
```

**Symptoms:**
- Shader fails to compile
- Error in HDRP package file
- Mentions CBUFFER macros

**Cause:**
Missing `Material.hlsl` include in HDRP include sequence

**Solution:**
Ensure correct include order with Material.hlsl:

```hlsl
// HDRP includes - CORRECT ORDER IS CRITICAL
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"  // THIS WAS MISSING!
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
```

**Critical:**
- Include order matters!
- Material.hlsl MUST come before Lighting.hlsl
- Material.hlsl defines CBUFFER macros

---

### Error: "redefinition of 'GetOddNegativeScale'"

**Full Error:**
```
Shader error in 'HDRP/Water/Surface': redefinition of 'GetOddNegativeScale' 
at Assets/WaterSystem/Shaders/Include/WaterCore.hlsl(23)
```

**Also Affects:**
- `UnpackNormalScale`
- Other utility functions

**Symptoms:**
- Shader compilation fails
- Error points to WaterCore.hlsl
- Function already defined by HDRP

**Cause:**
HDRP 17 now provides these utility functions

**Solution:**
Remove duplicate functions from WaterCore.hlsl:

```hlsl
// DELETE THESE (now provided by HDRP):
float GetOddNegativeScale() { ... }
float3 UnpackNormalScale(...) { ... }
```

**HDRP Now Provides:**
- `GetOddNegativeScale()` - In Common.hlsl
- `UnpackNormalScale()` - In Packing.hlsl
- Many other utility functions

---

### Error: "undeclared identifier 'ComputeFogFactor'"

**Full Error:**
```
Shader error in 'HDRP/Water/Surface': undeclared identifier 'ComputeFogFactor' 
at Assets/WaterSystem/Shaders/WaterSurface.shader(435)
```

**Also Affects:**
- `MixFog()`

**Symptoms:**
- Shader compilation fails
- Error in fragment shader
- Fog-related functions missing

**Cause:**
Built-in pipeline fog functions don't exist in HDRP

**Solution:**
Remove all fog code from shader:

```hlsl
// DELETE from Varyings struct:
float fogFactor : TEXCOORD5;

// DELETE from vertex shader:
output.fogFactor = ComputeFogFactor(output.positionCS.z);

// DELETE from fragment shader:
finalColor = MixFog(finalColor, input.fogFactor);
```

**Why:**
HDRP applies volumetric fog in post-processing, not per-pixel

**Alternative:**
Enable volumetric fog in HDRP settings:
1. Select HDRP Asset
2. Lighting → Volumetrics → Enable
3. Adjust fog density/distance in Volume Profile

---

### Error: "undeclared identifier '_MainLightPosition'"

**Full Error:**
```
Shader error in 'HDRP/Water/Surface': undeclared identifier '_MainLightPosition' 
at Assets/WaterSystem/Shaders/WaterSurface.shader(484)
```

**Also Fails:**
- `DirectionalLightDatas[0]`
- `GetMainLight()`

**Symptoms:**
- Shader compilation fails
- Lighting code broken
- Built-in pipeline functions used

**Cause:**
HDRP has different lighting system than built-in pipeline

**Temporary Solution (Stage 2):**
Use hardcoded directional light:

```hlsl
// Temporary fixed light direction
float3 lightDir = normalize(float3(0.5, 0.8, 0.3));
float3 lightColor = float3(1, 1, 1);

float NdotL = saturate(dot(normalWS, lightDir));
float3 lighting = lightColor * NdotL;
```

**Proper Solution (Stage 3):**
Will implement HDRP light probes and proper light sampling

**Note:**
This is temporary for Stage 2 only - Stage 3 adds full lighting

---

### Warning: "Property _WaveCount already exists with different type"

**Full Warning:**
```
Property _WaveCount already exists in the property sheet with a different type: 1
```

**Also Affects:**
- `_RippleOctaves`

**Symptoms:**
- Yellow warning in Console
- Property type mismatch
- Material may not update correctly

**Cause:**
Shader declares property as `Integer` but material has it as `Float`

**Solution:**
Change shader property type from Integer to Float:

```hlsl
// BEFORE:
_WaveCount("Wave Count", Integer) = 0

// AFTER:
_WaveCount("Wave Count", Float) = 0
```

**Why:**
Unity materials store all numeric properties as Float internally

**Impact:**
Cosmetic only - functionality works either way

---

### Warning: "Could not create custom UI for shader"

**Full Warning:**
```
Could not create a custom UI for the shader 'HDRP/Water/Surface'. 
The shader has the following: 'CustomEditor = WaterSystem.WaterShaderGUI'. 
Does the custom editor specified include its namespace? 
And does the class either derive from ShaderGUI or MaterialEditor?
```

**Symptoms:**
- Yellow warning in Console
- Material Inspector shows default UI
- Custom shader GUI not loading

**Cause:**
CustomEditor reference in shader but class not found

**Solution:**
Remove CustomEditor line from shader:

```hlsl
// DELETE:
CustomEditor "WaterSystem.WaterShaderGUI"
```

**Alternative:**
Fix WaterShaderGUI.cs namespace and ensure it's in Editor folder

**Impact:**
Material Inspector uses default UI instead of custom GUI

---

## Rendering Issues

### Issue: Water Renders Completely White

**Symptoms:**
- Game view shows solid white surface
- Scene view may show correct colors
- No errors in Console

**Possible Causes & Solutions:**

**Cause 1: Shader Compilation Failed Silently**
- Check Console for warnings
- Reimport WaterSurface.shader
- Verify all includes present

**Cause 2: Lighting Issue**
- Check scene has Directional Light
- Verify light is enabled
- Check light intensity > 0

**Cause 3: Material Not Applied**
- Select WaterSystem GameObject
- Check Mesh Renderer → Materials
- Should be M_Water_Stage1

**Cause 4: Camera Issue**
- Check camera Clear Flags
- Verify Culling Mask includes water layer
- Check HD Additional Camera Data component

**Debug Test:**
Replace fragment shader with:
```hlsl
return float4(_ShallowColor.rgb, 1.0);
```

If this shows cyan, shader pipeline works - issue is in lighting code.

---

### Issue: Water Renders Magenta/Pink

**Symptoms:**
- Game view shows hot pink/magenta surface
- Classic Unity "broken shader" color
- Console shows shader errors

**Cause:**
Shader compilation failed

**Solution:**
1. Check Console for error messages
2. Fix shader errors (see compilation errors section above)
3. Reimport shader
4. Material should update automatically

**Common Shader Errors:**
- Missing include files
- Undefined identifiers
- Function redefinitions
- Syntax errors

---

### Issue: Cube Also Renders Cyan

**Symptoms:**
- Water is cyan (correct)
- Other objects also turn cyan
- Materials seem to be shared

**Cause:**
Water material accidentally assigned to other objects

**Solution:**
1. Select affected object (e.g., Cube)
2. Mesh Renderer → Materials
3. Change from M_Water_Stage1 to correct material
4. Create new HDRP/Lit material if needed

**Prevention:**
Keep water material separate from other object materials

---

### Issue: Objects Sink Below Waterline

**Symptoms:**
- Objects that should float appear submerged
- Waterline too high on objects
- Objects partially invisible

**Cause:**
Object Y position below water surface level

**Solution:**
1. Move object upward on Y axis
2. Ensure object.position.y > waterSystem.position.y
3. Add half of object height if needed

**Formula:**
```
objectY = waterY + (objectHeight / 2)
```

---

### Issue: Normal Map Warning Persists

**Symptoms:**
- Warning: "Normal mapped shader without a normal map"
- Performance warning in Inspector
- Appears on material

**Cause:**
No normal map assigned to material

**Solution:**
1. Assign T_WaterNormal.png to material
2. Ensure texture import type is "Normal map"
3. Warning should disappear

**Steps:**
1. Select T_WaterNormal in Project
2. Inspector → Texture Type → Normal map
3. Click Apply
4. Drag to M_Water_Stage1 Normal Map slot

---

## Animation Issues

### Issue: Waves Not Animating

**Symptoms:**
- Water surface appears static
- No wave movement in Play Mode
- Surface is flat or fixed shape

**Diagnostic Steps:**

**Step 1: Check WaterSurfaceAnimator**
- Select WaterSystem GameObject
- Verify WaterSurfaceAnimator component attached
- Check "Animate Waves" checkbox is enabled
- Ensure Time Scale > 0

**Step 2: Check Shader Keywords**
- Select M_Water_Stage1 material
- In Project window, right-click → Properties
- Look for "Shader Keywords" section
- Should show: `_WAVES_ENABLED _RIPPLES_ENABLED`

**Step 3: Check Wave Count**
- Select M_Water_Stage1
- Inspector → Wave Count should be > 0
- If 0, profile not initialized

**Step 4: Check Profile Has Wave Data**
- Select current water profile
- Inspector → Wave Layers section
- Should show wave layers (not empty)

**Solutions:**

**If Keywords Missing:**
```csharp
// Create script in Editor folder:
Material mat = /* your material */;
mat.EnableKeyword("_WAVES_ENABLED");
mat.EnableKeyword("_RIPPLES_ENABLED");
```

**If Profile Empty:**
Run `Water → Initialize All Profiles` menu command

**If Component Missing:**
Add WaterSurfaceAnimator component to WaterSystem GameObject

---

### Issue: Wave Animation in Edit Mode

**Symptoms:**
- Water animates when NOT in Play Mode
- Water Time increases in Edit Mode
- Can slow down Editor

**Cause:**
WaterSurfaceAnimator has `[ExecuteAlways]` attribute

**Intended Behavior:**
This allows previewing water animation without entering Play Mode

**To Disable:**
Uncheck "Animate Waves" in WaterSurfaceAnimator component

**To Permanently Disable:**
Remove `[ExecuteAlways]` attribute from WaterSurfaceAnimator.cs

---

### Issue: All Profiles Look the Same

**Symptoms:**
- Switching profiles doesn't change wave appearance
- Ocean looks like Lake looks like River
- No visual difference

**Cause:**
Profiles not properly initialized with wave data

**Solution:**
Run profile initialization:

1. Create InitializeAllWaterProfiles.cs in Editor folder
2. Menu: Water → Initialize All Profiles
3. Check Console for success messages
4. Test profile switching again

**Verification:**
Each profile should show different wave parameters:
- Ocean: 4 waves, large amplitude
- Lake: 2 waves, small amplitude
- River: 3 waves, directional
- Pool: 1 wave, minimal

---

### Issue: Normal Map Not Scrolling

**Symptoms:**
- Surface detail appears static
- No texture animation visible
- Normal map frozen

**Diagnostic Steps:**

**Step 1: Check Keyword**
Material must have `_WAVES_ENABLED` keyword active

**Step 2: Check Property**
Material should have `_NormalScrollSpeed` > 0

**Step 3: Check Profile**
Profile should have `normalScrollSpeed` > 0

**Solutions:**

**If Keyword Missing:**
Enable with EnableWaterKeywords script

**If Property Missing:**
Update WaterProfile.cs and WaterSurface.shader

**If Speed is Zero:**
Change profile.normalScrollSpeed to 0.02 (or appropriate value)

---

### Issue: Profile Switching Doesn't Update Material

**Symptoms:**
- Change profile in WaterSystem
- Material properties don't change
- Waves stay the same

**Cause:**
ApplyToMaterial() not being called

**Solution:**
Check WaterSystem.cs OnValidate() method calls:
```csharp
void OnValidate()
{
    if (currentProfile != null)
    {
        currentProfile.ApplyToWaterSystem(this);
    }
}
```

**Manual Update:**
Select WaterSystem, change any property to force OnValidate()

---

## Performance Issues

### Issue: Low Frame Rate

**Symptoms:**
- FPS drops below 60
- Stuttering during gameplay
- Profiler shows high frame time

**Diagnostic:**
Open Unity Profiler, check:
- CPU: WaterSurfaceAnimator.Update time
- GPU: Render.Mesh time for water
- Memory: GC allocations

**Solutions:**

**If CPU Bound:**
1. Disable "Animate Waves" when not visible
2. Use LOD system (already enabled)
3. Reduce update frequency

**If GPU Bound:**
1. Reduce Max Wave Layers (6 instead of 8)
2. Reduce Max Ripple Octaves (2 instead of 4)
3. Reduce mesh subdivision (20x20 instead of 40x40)
4. Increase LOD distances

**If GC Issues:**
Should be 0 - if not, report bug

**Recommended Settings:**
```
Max Wave Layers: 6
Max Ripple Octaves: 3
LOD Distance 0: 50
LOD Distance 1: 100
Mesh: 40x40 vertices
```

---

### Issue: High GPU Time

**Symptoms:**
- Profiler shows water mesh render > 1ms
- GPU bound (CPU waiting)
- High vertex/fragment shader time

**Solutions:**

**Reduce Mesh Complexity:**
- Use 20x20 plane instead of 40x40
- Fewer vertices = faster computation

**Optimize LOD:**
- Increase LOD Distance 0 (50 → 75)
- Increase LOD Distance 1 (100 → 150)
- Earlier quality reduction

**Reduce Wave Layers:**
- Max Wave Layers: 6 → 4
- Fewer waves = less computation

**Reduce Ripple Octaves:**
- Max Ripple Octaves: 4 → 2
- Less noise calculation

---

### Issue: Memory Leaks / GC Spikes

**Symptoms:**
- Profiler shows GC allocations
- Memory usage increases over time
- Periodic frame hitches

**Expected:**
WaterSurfaceAnimator should produce 0 GC allocations

**If Allocations Occur:**
This is a bug - check:
1. No temporary arrays created in Update()
2. No string concatenation
3. No LINQ queries
4. All arrays pre-allocated

**Report:**
If allocations occur, this is a regression - please report

---

## Profile Issues

### Issue: Ocean Profile Has No Waves

**Symptoms:**
- Ocean_Default shows empty Wave Layers
- Wave Count = 0 in material
- Surface is flat

**Cause:**
Profile not initialized with wave data

**Solution:**
```csharp
// Run initialization script
Water → Initialize All Profiles

// Or manually:
Ocean_Default.waveData = WaterWaveData.CreateOceanWaves();
```

**Verification:**
Ocean_Default should show 4 wave layers in Inspector

---

### Issue: Profile Changes Ignored

**Symptoms:**
- Change profile dropdown
- Nothing happens visually
- Material stays the same

**Cause:**
OnValidate() not triggering or ApplyToMaterial() not working

**Debug:**
Add to WaterSystem.cs:
```csharp
void OnValidate()
{
    Debug.Log("OnValidate called");
    if (currentProfile != null)
    {
        Debug.Log($"Applying profile: {currentProfile.name}");
        currentProfile.ApplyToWaterSystem(this);
    }
}
```

Check Console for debug messages when changing profile

**Solution:**
Ensure WaterSystem.cs has proper OnValidate() implementation

---

### Issue: Cannot Create New Profile

**Symptoms:**
- Right-click → Create → Water System → Water Profile
- Nothing happens or error

**Cause:**
Missing CreateAssetMenu attribute

**Solution:**
Check WaterProfile.cs has:
```csharp
[CreateAssetMenu(fileName = "WaterProfile", 
                 menuName = "Water System/Water Profile", 
                 order = 1)]
```

**Alternative:**
Duplicate existing profile and modify

---

## Deep Clean Procedures

### Full Shader Reimport

**When:** Shader changes not taking effect

**Steps:**
1. Close Unity
2. Delete `Library/` folder
3. Delete `Temp/` folder
4. Reopen Unity (will take time to reimport)
5. Reimport WaterSurface.shader

**WARNING:** This clears all import caches

---

### Material Reset

**When:** Material in broken state

**Steps:**
1. Note current material settings
2. Select M_Water_Stage1
3. Inspector → Shader dropdown → None
4. Shader dropdown → HDRP/Water/Surface
5. Re-apply all settings
6. Run EnableWaterKeywords script

---

### Profile Reinitialization

**When:** Profiles corrupted or inconsistent

**Steps:**
1. Run `Water → Initialize All Profiles`
2. Check Console for success
3. Manually verify each profile
4. Test profile switching

---

## Emergency Procedures

### If Nothing Works

**Nuclear Option:**
1. Export working files from Stage 1
2. Delete entire WaterSystem folder
3. Reimport Stage 1 files
4. Add Stage 2 files one by one
5. Test after each file

**Before Nuclear Option:**
1. Commit to Git
2. Tag as "pre-nuclear"
3. Can rollback if needed

---

### Getting Help

**Information to Provide:**
1. Unity version (exact build number)
2. HDRP version
3. Full Console error messages
4. Screenshots of Inspector
5. What you tried already
6. Steps to reproduce

**Where to Ask:**
1. Unity Forums - HDRP section
2. Unity Answers
3. Stack Overflow - unity3d tag
4. Discord - Unity HDRP channels

---

## Preventive Measures

### Before Making Changes

1. **Commit to Git**
2. **Tag current state**
3. **Test in separate scene**
4. **One change at a time**

### After Making Changes

1. **Check Console immediately**
2. **Test in Play Mode**
3. **Check Profiler**
4. **Commit if working**

### Regular Maintenance

1. **Clear Console regularly**
2. **Check for Unity updates**
3. **Reimport assets occasionally**
4. **Monitor performance**

---

**Document Version:** 1.0  
**Last Updated:** December 8, 2025  
**Based On:** Actual Stage 2 implementation issues  
