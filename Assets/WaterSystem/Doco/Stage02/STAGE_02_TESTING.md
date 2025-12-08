# Stage 02: Testing & Verification Guide

**Unity Version**: 6.3  
**HDRP Version**: 17  
**Stage**: Regular Surface Movement

---

## Pre-Testing Setup

### 1. File Installation

Ensure all Stage 02 files are properly located:

```
Assets/WaterSystem/
├── Scripts/
│   ├── WaterSurfaceAnimator.cs          [NEW - Stage 02]
│   ├── WaterWaveData.cs                 [NEW - Stage 02]
│   ├── WaterProfile.cs                  [MODIFIED - Stage 02]
│   └── Editor/
│       ├── WaterProfileEditor.cs        [MODIFIED - Stage 02]
│       └── WaterShaderGUI.cs            [NEW - Stage 02]
│
└── Shaders/
    ├── WaterSurface.shader              [MODIFIED - Stage 02]
    └── Include/
        └── WaterWaves.hlsl              [NEW - Stage 02]
```

### 2. Compilation Check

1. Open Unity 6.3
2. Wait for script compilation to complete
3. Check Console for errors (there should be none)
4. If errors appear, verify:
   - All files are in correct locations
   - Namespace declarations match
   - HDRP 17 is properly installed

### 3. Shader Compilation

1. Select `WaterSurface.shader` in Project window
2. Check Inspector - should show "Compiled successfully"
3. If shader errors appear:
   - Verify WaterCore.hlsl path
   - Verify WaterWaves.hlsl path
   - Check HDRP include paths

---

## Test Suite 1: Component Integration

### Test 1.1: Add WaterSurfaceAnimator Component

**Steps**:
1. Select existing water GameObject from Stage 01
2. Add Component → WaterSurfaceAnimator
3. Verify component appears with default settings

**Expected Results**:
- Component adds without errors
- Default values:
  - `animateWaves = true`
  - `timeScale = 1`
  - `maxWaveLayers = 8`
  - `enableLOD = true`
  - `lodDistance0 = 100`
  - `lodDistance1 = 500`

**Pass/Fail**: ___________

### Test 1.2: Component Dependencies

**Steps**:
1. Verify WaterSystem component still present
2. Check Inspector for RequireComponent warning
3. Try removing WaterSystem (should fail)

**Expected Results**:
- WaterSystem is required
- Cannot remove WaterSystem while WaterSurfaceAnimator exists
- No console errors

**Pass/Fail**: ___________

---

## Test Suite 2: Wave Profile Configuration

### Test 2.1: Create Ocean Profile

**Steps**:
1. Right-click in Project window
2. Create → Water System → Water Profile
3. Name it "Test_Ocean_Waves"
4. In Inspector, click "Reset to Ocean" button

**Expected Results**:
- Profile creates successfully
- Wave Data initialized
- Contains 4 wave layers
- Ripple settings configured
- Preview graph shows wave pattern

**Pass/Fail**: ___________

### Test 2.2: Edit Wave Layers

**Steps**:
1. Open Test_Ocean_Waves profile
2. Expand "Wave Animation"
3. Expand "Wave Layers"
4. Modify first wave:
   - Set Amplitude to 2.0
   - Set Wavelength to 30
   - Set Steepness to 0.7

**Expected Results**:
- Changes apply immediately
- "Effective Speed" updates
- "Max Safe Steepness" warning appears if steepness too high
- Wave preview updates

**Pass/Fail**: ___________

### Test 2.3: Add/Remove Wave Layers

**Steps**:
1. Click "Add Wave Layer" button
2. Configure new layer
3. Click "Remove Layer" on middle layer
4. Use ↑/↓ buttons to reorder layers

**Expected Results**:
- Can add up to 8 layers
- "Add Wave Layer" disables at 8 layers
- Remove works correctly
- Reordering preserves layer data

**Pass/Fail**: ___________

### Test 2.4: Ripple Configuration

**Steps**:
1. Expand "Ripple Detail Settings"
2. Enable Ripples checkbox
3. Set Wind Direction to (1, 0.5)
4. Set Wind Speed to 5 m/s
5. Set Ripple Strength to 0.2

**Expected Results**:
- Settings save correctly
- Wind direction normalizes automatically
- Wave Statistics update

**Pass/Fail**: ___________

---

## Test Suite 3: Runtime Animation

### Test 3.1: Basic Wave Animation

**Steps**:
1. Apply Test_Ocean_Waves to water GameObject
2. Ensure WaterSurfaceAnimator is attached
3. Enable shader keyword "_WAVES_ENABLED" on material
4. Enter Play Mode
5. Observe water surface

**Expected Results**:
- Water surface animates smoothly
- Rolling wave motion visible
- No stuttering or artifacts
- Performance <0.5ms (check Profiler)

**Pass/Fail**: ___________

### Test 3.2: Time Control

**Steps**:
1. In Play Mode, pause animation (animateWaves = false)
2. Resume animation
3. Change timeScale to 2.0
4. Change timeScale to 0.5

**Expected Results**:
- Pause freezes animation
- Resume continues smoothly
- 2.0x speed doubles animation rate
- 0.5x speed halves animation rate

**Pass/Fail**: ___________

### Test 3.3: Global vs Local Time

**Steps**:
1. Create two water planes
2. Set first to useGlobalTime = true
3. Set second to useGlobalTime = false
4. Enter Play Mode
5. Observe both surfaces

**Expected Results**:
- Global time: both surfaces synchronized
- Local time: surfaces animate independently
- Both animate smoothly

**Pass/Fail**: ___________

---

## Test Suite 4: LOD System

### Test 4.1: Distance-Based LOD

**Steps**:
1. Enable showDebugInfo on WaterSurfaceAnimator
2. Position camera close to water (<100m)
3. Move camera to 200m distance
4. Move camera to 600m distance
5. Observe debug UI

**Expected Results**:
- Close (<100m): Full wave count (8 layers)
- Medium (100-500m): Reduced waves (4-8 layers interpolated)
- Far (>500m): Minimum waves (4 layers)
- Debug UI shows current wave count

**Pass/Fail**: ___________

### Test 4.2: LOD Gizmo Visualization

**Steps**:
1. Enable showDebugInfo
2. Select water GameObject in Hierarchy
3. View Scene window (not Game)
4. Observe LOD distance spheres

**Expected Results**:
- Yellow sphere at lodDistance0 (100m)
- Orange sphere at lodDistance1 (500m)
- Cyan sphere at rippleLODDistance (75m)
- Spheres centered on water GameObject

**Pass/Fail**: ___________

---

## Test Suite 5: Visual Quality

### Test 5.1: Wave Shape Quality

**Steps**:
1. Create Test_Ocean_Waves profile
2. Set single wave layer:
   - Amplitude: 1.0
   - Wavelength: 10
   - Steepness: 0.0 (sine wave)
3. Enter Play Mode, observe wave shape
4. Increase Steepness to 0.8 (sharp crests)
5. Observe wave shape change

**Expected Results**:
- Steepness 0.0: Smooth, rounded waves
- Steepness 0.8: Sharp crests, rounded troughs
- Realistic Gerstner wave appearance
- No vertex swimming or artifacts

**Pass/Fail**: ___________

### Test 5.2: Multi-Layer Blending

**Steps**:
1. Create profile with 4 waves:
   - Layer 0: 60m wavelength, 1.5m amplitude, direction (1,0)
   - Layer 1: 40m wavelength, 1.0m amplitude, direction (0.7,0.7)
   - Layer 2: 20m wavelength, 0.5m amplitude, direction (-0.5,0.866)
   - Layer 3: 10m wavelength, 0.3m amplitude, direction (0.3,-0.954)
2. Enter Play Mode
3. Observe water from multiple angles

**Expected Results**:
- Complex, natural-looking wave patterns
- Multiple wave directions visible
- No obvious repetition
- Waves blend smoothly

**Pass/Fail**: ___________

### Test 5.3: Ripple Detail

**Steps**:
1. Enable ripples in profile
2. Set Ripple Strength to 0.15
3. Set Ripple Octaves to 4
4. Enter Play Mode
5. Position camera close to surface (5-10m)

**Expected Results**:
- Fine-scale ripples visible
- Ripples animate with wind direction
- Detail adds to large waves naturally
- No flickering or aliasing

**Pass/Fail**: ___________

### Test 5.4: Normal Animation

**Steps**:
1. Add directional light to scene
2. Position to create clear specular highlights
3. Enter Play Mode
4. Observe specular reflections on animated water

**Expected Results**:
- Specular highlights move with waves
- Highlights follow wave crests
- Normals appear correct (pointing upward on peaks)
- No lighting artifacts

**Pass/Fail**: ___________

---

## Test Suite 6: Profile Presets

### Test 6.1: Ocean Profile

**Steps**:
1. Create profile, click "Reset to Ocean"
2. Apply to water GameObject
3. Enter Play Mode

**Expected Results**:
- Large rolling waves
- 4 wave layers active
- Wave height ~3-4 meters total
- Ripple detail visible
- Realistic ocean appearance

**Pass/Fail**: ___________

### Test 6.2: Lake Profile

**Steps**:
1. Create profile, click "Reset to Lake"
2. Apply to water GameObject
3. Enter Play Mode

**Expected Results**:
- Gentle ripples only
- 2 wave layers active
- Wave height <0.5 meters
- Minimal movement
- Calm lake appearance

**Pass/Fail**: ___________

### Test 6.3: River Profile

**Steps**:
1. Create profile, create river preset (use factory method)
2. Apply to water GameObject
3. Enter Play Mode

**Expected Results**:
- Directional waves
- Waves aligned with flow
- 2 wave layers active
- Visible current movement
- River-like appearance

**Pass/Fail**: ___________

### Test 6.4: Pool Profile

**Steps**:
1. Create profile, create pool preset
2. Apply to water GameObject
3. Enter Play Mode

**Expected Results**:
- Almost still surface
- 1 wave layer, tiny amplitude
- Minimal ripples
- Surface tension waves only
- Pool-like appearance

**Pass/Fail**: ___________

---

## Test Suite 7: Performance Verification

### Test 7.1: Profiler Analysis

**Steps**:
1. Open Unity Profiler (Window → Analysis → Profiler)
2. Enter Play Mode
3. Select CPU Usage module
4. Expand Hierarchy
5. Locate "WaterSurfaceAnimator.Update"

**Expected Results**:
- WaterSurfaceAnimator.Update <0.1ms
- Total water rendering <0.5ms
- No GC allocations
- Consistent frame time

**Pass/Fail**: ___________

### Test 7.2: Multiple Water Instances

**Steps**:
1. Create 5 water plane GameObjects
2. Add WaterSystem + WaterSurfaceAnimator to each
3. Set all to useGlobalTime = true
4. Enter Play Mode
5. Check Profiler

**Expected Results**:
- All instances synchronized
- Performance scales linearly
- Total overhead <2.5ms for 5 instances
- No performance degradation over time

**Pass/Fail**: ___________

### Test 7.3: Build Test

**Steps**:
1. Create standalone build (Development Build)
2. Run build
3. Check performance
4. Verify animation works

**Expected Results**:
- Waves animate in build
- No console errors
- Performance matches editor
- Visual quality maintained

**Pass/Fail**: ___________

---

## Test Suite 8: Edge Cases & Error Handling

### Test 8.1: Zero Wave Layers

**Steps**:
1. Create profile
2. Remove all wave layers
3. Apply to water
4. Enter Play Mode

**Expected Results**:
- Water renders flat (no animation)
- No errors
- Shader handles gracefully

**Pass/Fail**: ___________

### Test 8.2: Extreme Values

**Steps**:
1. Set wave amplitude to 10m
2. Set wavelength to 0.1m (very small)
3. Set steepness to 1.0 (maximum)
4. Enter Play Mode

**Expected Results**:
- No crashes
- Validation warnings appear
- Wave breaks naturally or clamps

**Pass/Fail**: ___________

### Test 8.3: Missing Components

**Steps**:
1. Remove WaterSystem component
2. Try to add WaterSurfaceAnimator
3. Observe behavior

**Expected Results**:
- RequireComponent prevents removal
- OR automatically adds WaterSystem
- Clear error message if fails

**Pass/Fail**: ___________

### Test 8.4: Null Profile

**Steps**:
1. Remove profile from WaterSystem
2. Enter Play Mode

**Expected Results**:
- No null reference exceptions
- Warning logged
- Water renders flat or default

**Pass/Fail**: ___________

---

## Performance Targets

### Frame Time Budget
- **WaterSurfaceAnimator**: <0.1ms CPU
- **GPU Vertex Displacement**: <0.3ms
- **Total Water Rendering**: <0.5ms
- **GC Allocations**: 0 bytes/frame

### Quality Metrics
- **Wave Smoothness**: No visible stepping at 60 FPS
- **Normal Quality**: Smooth specular highlights
- **LOD Transitions**: Imperceptible
- **Ripple Detail**: Visible at <20m distance

---

## Known Issues & Limitations

### Current Limitations
1. Maximum 8 wave layers for performance
2. Gerstner waves only (no FFT-based ocean)
3. No shore interaction yet (Stage 6)
4. No object-generated waves yet (Stage 5)

### Unity 6.3 Specific Notes
1. HDRP 17 shader syntax required
2. Ensure using GetMainLight() for lighting
3. Volumetric fog compatibility verified

---

## Regression Testing (Stage 01)

Ensure Stage 01 features still work:

- [ ] Water color gradient renders correctly
- [ ] Depth-based coloring works
- [ ] Fresnel effect visible
- [ ] Normal mapping applies
- [ ] Profile switching works
- [ ] Custom editor functions

---

## Final Checklist

Before marking Stage 02 complete:

- [ ] All test suites passed
- [ ] Performance within targets
- [ ] No console errors or warnings
- [ ] Documentation complete
- [ ] Example profiles created (Ocean, Lake, River, Pool)
- [ ] Code comments comprehensive
- [ ] Git commit ready with tag: `STAGE_02_COMPLETE`

---

## Troubleshooting Common Issues

### Issue: Waves don't animate

**Solution**:
1. Check WaterSurfaceAnimator.animateWaves is true
2. Verify shader keyword "_WAVES_ENABLED" is enabled
3. Ensure profile has wave layers
4. Check time scale isn't zero

### Issue: Weird vertex artifacts

**Solution**:
1. Reduce wave steepness
2. Check for NaN values in wave parameters
3. Ensure wavelength > 0.1m
4. Verify normal map is assigned

### Issue: Performance too slow

**Solution**:
1. Enable LOD
2. Reduce maxWaveLayers
3. Reduce rippleOctaves
4. Check for multiple lights

### Issue: Waves look wrong in Scene view

**Solution**:
1. Enter Play Mode (animation requires runtime)
2. Check Scene view shading mode
3. Verify lighting is set up

---

**Testing Completed By**: ___________  
**Date**: ___________  
**Overall Pass/Fail**: ___________  
**Notes**: ___________
