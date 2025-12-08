# Advanced Water System for Unity HDRP - Stage 0 Documentation

## Overview

This foundation package establishes the complete architecture for an advanced water rendering system in Unity 6.3+ using HDRP. It provides the scaffolding for all subsequent stages of development.

---

## Package Contents

### Core Components

1. **WaterSystem.cs** - Main component managing all water behavior
2. **WaterProfile.cs** - ScriptableObject for reusable water configurations
3. **WaterWaveSimulator.cs** - Procedural wave generation and queries
4. **WaterInteractionManager.cs** - Surface interactions (splashes, wakes, foam)
5. **WaterUnderwaterRenderer.cs** - Underwater effects and transitions
6. **WaterCustomPass.cs** - HDRP custom render pass for advanced effects
7. **WaterShaderIncludes.hlsl** - Shader utilities and functions

---

## Installation Instructions

### Step 1: Project Setup

1. **Create/Open Unity 6.3+ Project** with HDRP template
2. **Verify HDRP Installation**:
   - Window → Package Manager
   - Ensure "High Definition RP" is installed (version 17.0+)

### Step 2: Import Core Scripts

1. **Create folder structure**:
   ```
   Assets/
   └── WaterSystem/
       ├── Scripts/
       │   ├── WaterSystem.cs
       │   ├── WaterProfile.cs
       │   ├── WaterWaveSimulator.cs
       │   ├── WaterInteractionManager.cs
       │   ├── WaterUnderwaterRenderer.cs
       │   └── WaterCustomPass.cs
       └── Shaders/
           └── WaterShaderIncludes.hlsl
   ```

2. **Copy files** from this package to the appropriate folders

3. **Wait for compilation** (check Console for errors)

### Step 3: Configure HDRP Asset

1. **Locate your Active HDRP Asset**:
   - Go to: **Edit → Project Settings → Graphics**
   - Look for: **"Default Render Pipeline"** (Unity 6.3) or "Scriptable Render Pipeline Settings" (older versions)
   - Click on the assigned asset name to highlight it in the Project window
   - Common locations: `Assets/Settings/HDRPDefaultResources/`
   - You may see multiple assets (HDRP Balanced, HDRP High Fidelity, HDRP Performant)
   - **Configure the one that's currently assigned** in Graphics settings
   - **Recommended**: Switch to **HDRP High Fidelity** for RTX 5000-series hardware

2. **Enable Required Features**:
   - Select your active HDRP Asset in Project window
   - Open in Inspector
   
   **Lighting Section**:
   - Expand **"Reflections"** subsection:
     - ✓ **Screen Space Reflection** (critical for water reflections)
     - Settings should show high-quality values (your defaults are likely fine)
   - Expand **"Volumetrics"** subsection:
     - ✓ **Volumetric Fog** (critical for underwater effects)
     - ☐ **Volumetric Clouds** (optional, not required for water)
     - **Max Local Fog On Screen**: 16+ (default is fine)
   
   **Material Section**:
   - ✓ **Distortion** (critical for water refraction)
   - ✓ **Subsurface Scattering** (bonus - improves shallow water translucency)
   - **Default Material Quality Level**: High (recommended for RTX 5000-series)
   
   **Post-processing Section**:
   - ✓ Post-processing features are **enabled by default** in Unity 6.3 HDRP
   - The settings you see (Depth of Field tiers, Color Grading LUT) are correct
   - Individual effects are controlled at the Volume level (not here)
   - **No changes needed** in this section

3. **Verification**:
   - All three critical settings enabled:
     - ✓ Volumetric Fog
     - ✓ Screen Space Reflection
     - ✓ Distortion
   
4. **Custom Pass Configuration**:
   - The WaterCustomPass will be added per-scene (see Scene Setup below)

### Step 4: Create Water Profile Assets

1. **Create your first Water Profile**:
   - Right-click in Project window
   - Create → Advanced Water → Water Profile
   - Name it "Ocean_Default"

2. **Configure the profile** (Inspector):
   ```
   Profile Name: "Ocean Default"
   Shallow Color: Light blue (0.1, 0.6, 0.7)
   Deep Color: Dark blue (0.0, 0.1, 0.3)
   Depth Falloff: 10
   Clarity: 15
   Smoothness: 0.95
   Wave Amplitude: 1.0
   Wave Speed: 1.0
   ```

3. **Create additional profiles** for different water types:
   - Lake, River, Pool, etc.
   - Use `Create Variant` in profile Inspector for quick iterations

---

## Scene Setup

### Creating Your First Water Body

1. **Create Water Plane**:
   - GameObject → 3D Object → Plane
   - Rename to "Ocean"
   - Scale: (100, 1, 100) for large ocean
   - Position: (0, 0, 0)

2. **Add WaterSystem Component**:
   - Select the plane
   - Add Component → Advanced Water → Water System
   - Assign your Water Profile to the `profile` field

3. **Configure Water Settings**:
   ```
   Water Body Type: Ocean
   Surface Scale: 1.0
   Smoothness: 0.95
   Enable Waves: ✓
   Enable Interactions: ✓
   Enable Underwater: ✓
   Max Tessellation: 32
   Show Debug: ✓ (for testing)
   ```

### Setting Up Custom Render Pass

1. **Create Custom Pass Volume**:
   - GameObject → Volume → Custom Pass
   - Name it "Water Custom Pass Volume"

2. **Configure Custom Pass**:
   - Select the Custom Pass Volume
   - Click "Add Custom Pass" button in Inspector
   - Choose "WaterCustomPass"

3. **Assign Water System**:
   - In the WaterCustomPass settings:
     - Target Water System: Drag your Ocean object here
     - Enable Planar Reflections: ✓
     - Enable Refraction: ✓
     - Reflection Resolution Scale: 0.5

4. **Set Injection Point**:
   - Injection Point: "Before Transparent"
   - This ensures water renders with proper reflections/refractions

---

## Creating the Water Material

Since we haven't built the Shader Graph yet, we'll create a placeholder:

### Step 1: Create Basic Material

1. **Create HDRP Lit Material**:
   - Right-click in Project
   - Create → Material
   - Name it "Water_Material"

2. **Configure for Water** (temporary - will be replaced with Shader Graph):
   - Surface Type: Transparent
   - Rendering Pass: Before Refraction
   - Smoothness: 0.95
   - Metallic: 0.0
   - Base Color: Light blue (0.3, 0.6, 0.8, 0.5)

3. **Assign to Water Plane**:
   - Select your Ocean plane
   - Drag Water_Material to Mesh Renderer

### Step 2: Prepare for Shader Graph (Next Stage)

When we build the actual water shader in Stage 1, we'll:
- Create Shader Graph using HDRP/Lit template
- Integrate WaterShaderIncludes.hlsl functions
- Add custom nodes for wave displacement
- Implement color grading, foam, and other effects

---

## Testing the Foundation

### Verification Checklist

Follow these steps to verify your Stage 0 setup is working correctly:

#### 1. **Scene View Verification**
- [ ] **Water plane is visible**
  - In Scene view, you should see your water plane (large blue/transparent plane)
  - Camera icon in Scene view should show the plane
  
- [ ] **Debug visualization is active** (if enabled)
  - Select your water plane GameObject
  - In WaterSystem component, check "Show Debug" is enabled
  - Scene view should show:
    - Blue wireframe around water bounds
    - Yellow sphere when water GameObject is selected (interaction range)
  
- [ ] **No console errors**
  - Open Console window: **Window → General → Console**
  - Filter to "Errors" (red icon)
  - Should show: **0 Errors**
  - Warnings are okay, but should be none if you applied all fixes

#### 2. **Play Mode Testing**

**Enter Play Mode**:
- Click the Play button ▶ at top of Unity
- Wait for scene to load
- **Do NOT exit Play mode yet**

**While in Play Mode, verify**:

- [ ] **Water component initializes**
  - Console shows no errors
  - No exceptions related to WaterSystem, WaveSimulator, etc.
  
- [ ] **WaterSystem Update runs**
  - Open **Window → Analysis → Profiler**
  - Click on CPU Usage module
  - In the timeline, look for "WaterSystem.Update"
  - Should appear every frame (see screenshot below for reference)
  
- [ ] **Material property block updates**
  - Select water plane while in Play mode
  - In Inspector, find WaterSystem component
  - Try changing "Shallow Color" - color should update in Scene view
  - Try changing "Wave Strength" slider - value should update

**Exit Play Mode**:
- Click Stop button ■

#### 3. **Inspector Testing**

**With water plane selected in Hierarchy**:

- [ ] **Adjust Shallow/Deep colors**
  - In WaterSystem component, click on "Shallow Color"
  - Change to bright green (for testing)
  - *Note: Visual change won't be obvious yet (no shader in Stage 0)*
  - Change back to light blue (0.1, 0.6, 0.7)
  
- [ ] **Change Wave Strength**
  - Drag "Wave Strength" slider
  - Values should change smoothly (0 to 5 range)
  - No console errors when adjusting
  
- [ ] **Toggle Show Debug**
  - Check "Show Debug" - Scene view should show wireframe gizmos
  - Uncheck "Show Debug" - Gizmos should disappear

#### 4. **Custom Pass Verification**

**Locate Custom Pass Volume**:
- In Hierarchy, find "Custom Pass Volume" GameObject
- Select it

**Verify settings**:
- [ ] Custom Pass Volume component visible in Inspector
- [ ] "Custom Passes" list shows "WaterCustomPass"
- [ ] WaterCustomPass settings show:
  - Target Water System: Your water plane assigned
  - Enable Planar Reflections: ✓
  - Enable Refraction: ✓
- [ ] Injection Point: "Before Transparent"

#### 5. **Profile System Test**

**Test profile application**:
- [ ] Select water plane
- [ ] In WaterSystem, change "Profile" dropdown
- [ ] Try each profile: Ocean_Default, Lake_Calm, River_Fast, Pool_Still
- [ ] Each profile should apply different settings
- [ ] No errors when switching profiles

**Verify profile variants exist**:
- [ ] In Project window, locate your water profiles
- [ ] Should have at least 4 profiles created
- [ ] Each profile should have different settings when inspected

#### 6. **Material Assignment Check**

**Select water plane**:
- [ ] Look at Mesh Renderer component in Inspector
- [ ] Materials → Element 0 should show "Water_Material"
- [ ] Material should show in Project when clicked
- [ ] Material Inspector shows:
  - Surface Type: Transparent ✓
  - Base Color: Light blue with some transparency

### Performance Baseline

**Open Profiler**: Window → Analysis → Profiler

**Enter Play Mode and check**:

| System | Location in Profiler | Expected Time | Status |
|--------|---------------------|---------------|--------|
| WaterSystem.Update | CPU Usage → Scripts | < 0.1ms | [ ] |
| WaterWaveSimulator | Under WaterSystem | < 0.05ms | [ ] |
| Total Water Overhead | Combined | < 0.2ms | [ ] |

**How to check**:
1. Open Profiler
2. Click "CPU Usage" module
3. Enter Play mode
4. Click on any frame in the timeline
5. In the lower panel, expand hierarchy to find WaterSystem.Update
6. Time shown on right side should be under 0.1ms

**Expected Performance (RTX 5000-series, 1080p)**:
- WaterSystem.Update: ~0.05ms ✓
- WaterWaveSimulator: ~0.02ms ✓
- Rendering: N/A (no shader yet - Stage 1)
- **Total**: ~0.07ms CPU overhead

**If performance is higher than expected**:
- Reduce Max Tessellation to 16
- Disable "Show Performance Stats" if enabled
- Check that only one WaterSystem exists in scene

### Common Verification Issues

**Issue**: "Can't see water plane in Scene view"
- **Check**: Camera position - move camera to see origin (0,0,0)
- **Check**: Water plane scale - should be (100, 1, 100)
- **Check**: Layers - ensure water is on visible layer

**Issue**: "WaterSystem.Update not appearing in Profiler"
- **Check**: Profiler is recording (red circle at top)
- **Check**: "Deep Profile" is enabled (may be needed for script details)
- **Check**: You're in Play mode when checking

**Issue**: "Changing colors doesn't update anything"
- **Expected**: In Stage 0, visual changes won't be obvious (no shader yet)
- **Check**: Console for errors
- **Wait for**: Stage 1 shader implementation for visual updates

**Issue**: "Debug gizmos not showing"
- **Check**: "Show Debug" is checked in WaterSystem
- **Check**: Gizmos are enabled in Scene view (Gizmos button at top)
- **Check**: Water GameObject is selected (some gizmos only show when selected)

**Issue**: "Custom Pass shows errors"
- **Check**: Water GameObject is assigned to "Target Water System"
- **Check**: Injection Point is set correctly
- **Check**: HDRP asset has custom passes enabled

### Visual Checklist (What You Should See)

**In Scene View**:
```
✓ Large plane at origin (if looking from above)
✓ Blue wireframe when "Show Debug" enabled
✓ Water_Material assigned (visible in Mesh Renderer)
✓ No pink/magenta shader errors
```

**In Inspector (with water selected)**:
```
✓ WaterSystem component with all settings
✓ Mesh Renderer with Water_Material
✓ Profile assigned in dropdown
✓ All checkboxes for Enable Waves/Interactions/Underwater
```

**In Console**:
```
✓ No red errors
✓ No warnings (after applying fixes)
✓ Clean console on Play mode entry
```

**In Profiler (Play mode)**:
```
✓ WaterSystem.Update appears in CPU timeline
✓ Time is under 0.1ms
✓ No allocation spikes in Memory profiler
```

### Screenshot Reference Points

**Where to look**:
1. **Scene View**: Should show water plane with optional wireframe
2. **Inspector**: WaterSystem component with all settings visible
3. **Console**: Empty (no errors/warnings)
4. **Profiler → CPU**: WaterSystem.Update visible and fast
5. **Hierarchy**: Water plane + Custom Pass Volume present

---

### All Checks Passed? ✓

If you can check all the boxes above:
- ✅ **Stage 0 is complete and verified**
- ✅ **Foundation is solid**
- ✅ **Ready to proceed to Stage 1**

If any checks fail, review the troubleshooting sections or the relevant setup steps in this documentation.

---

## API Reference

### WaterSystem Public Methods

```csharp
// Get water surface height at world position
float height = waterSystem.GetWaterHeightAtPosition(worldPos);

// Check if position is underwater
bool underwater = waterSystem.IsUnderwater(worldPos);

// Get surface normal at position
Vector3 normal = waterSystem.GetSurfaceNormal(worldPos);
```

### WaterProfile Methods

```csharp
// Apply profile to water system
profile.ApplyToWaterSystem(waterSystem);

// Create variant
WaterProfile tropical = oceanProfile.CreateVariant("Tropical Ocean");
```

### WaterInteractionManager (accessed via WaterSystem internals)

```csharp
// Create splash (will be public in later stages)
interactionManager.CreateSplash(position, intensity: 1.0f, radius: 2.0f);

// Create wake
interactionManager.CreateWake(position, velocity, intensity: 0.5f);
```

---

## Troubleshooting

### Common Issues

**Issue**: "WaterSystem component won't add"
- **Solution**: Ensure scripts compiled successfully. Check Console for errors.

**Issue**: "Can't find HDRP Asset settings mentioned in documentation"
- **Solution**: Unity 6.3 reorganized the interface:
  - "Scriptable Render Pipeline Settings" is now "Default Render Pipeline"
  - Post-processing effects are enabled by default (no checkboxes)
  - Screen Space Reflection is under Lighting → Reflections subsection
  - Volumetric Fog is under Lighting → Volumetrics subsection

**Issue**: "Multiple HDRP assets in Settings folder (Balanced, High Fidelity, Performant)"
- **Solution**: 
  - Only ONE is active at a time
  - Check Edit → Project Settings → Graphics → Default Render Pipeline
  - Configure whichever one is assigned there
  - For RTX 5000-series: recommend using HDRP High Fidelity

**Issue**: "Custom Pass not appearing"
- **Solution**: 
  1. Verify HDRP asset is assigned in Project Settings → Graphics
  2. Check Custom Pass Volume is in scene
  3. Ensure water object is assigned to pass

**Issue**: "Water is invisible"
- **Solution**: 
  1. Check material is assigned to mesh renderer
  2. Verify Surface Type is Transparent
  3. Check camera is rendering transparent layer

**Issue**: "Performance is poor"
- **Solution**: 
  1. Reduce Max Tessellation
  2. Lower Reflection Resolution Scale
  3. Disable Show Performance Stats if enabled

### Debug Visualization

Enable "Show Debug" in WaterSystem to see:
- Water bounds (blue wireframe)
- Interaction ranges (yellow sphere when selected)
- Wave simulation gizmos (future stages)

---

## Next Steps: Stage 1

With the foundation complete, Stage 1 will implement:

1. **Water Surface Shader** (Shader Graph)
   - Base color and depth blending
   - Fresnel-based reflectivity
   - Soft intersection with geometry
   - Specular highlights

2. **Normal Map Integration**
   - Surface detail without vertex displacement
   - Animated normal maps
   - Blending multiple normal layers

3. **Improved Material System**
   - Full profile integration
   - Runtime material updates
   - LOD system preparation

---

## Architecture Notes

### Design Decisions

**Why separate components?**
- Modularity: Each system can be tested/optimized independently
- Performance: Only enabled systems incur overhead
- Extensibility: Easy to add new features without breaking existing code

**Why MaterialPropertyBlock?**
- Avoids material duplication
- Enables per-instance customization
- Better for instancing and performance

**Why custom HDRP pass?**
- Required for planar reflections
- Needed for underwater transitions (Stage 7)
- Enables advanced rendering techniques

### Thread Safety

Currently, all systems run on main thread. Future optimizations:
- Move wave calculations to Job System
- GPU-based interaction rendering (compute shaders)
- Async profile loading

---

## Support & Feedback

This is Stage 0 of a multi-stage water system. Each subsequent stage will:
- Build upon this foundation
- Add new features incrementally
- Maintain backward compatibility with earlier stages

**Ready to proceed to Stage 1?** Ensure all verification tests pass before continuing.

---

## File Reference Summary

| File | Purpose | Dependencies |
|------|---------|--------------|
| WaterSystem.cs | Main orchestrator | All other components |
| WaterProfile.cs | Configuration data | None |
| WaterWaveSimulator.cs | Wave mathematics | Unity.Mathematics |
| WaterInteractionManager.cs | Surface dynamics | RenderTexture, Lists |
| WaterUnderwaterRenderer.cs | Underwater FX | HDRP Volume, Fog |
| WaterCustomPass.cs | HDRP rendering | CustomPass, RTHandles |
| WaterShaderIncludes.hlsl | Shader utilities | HDRP shader lib |

---

**Foundation Complete** ✓

All systems are in place to begin visual development in Stage 1.
