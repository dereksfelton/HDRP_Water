# Stage 02 Integration Guide: Adding Wave Animation

**Unity Version**: 6.3  
**HDRP Version**: 17  
**Prerequisites**: Stage 01 Complete

This guide walks you through integrating the Stage 02 wave animation system into your existing Unity water project.

---

## Quick Start Checklist

- [ ] Stage 01 is working correctly
- [ ] Unity 6.3 with HDRP 17 installed
- [ ] Stage 02 files downloaded
- [ ] No compilation errors
- [ ] Backup created (optional but recommended)

**Estimated Time**: 30-45 minutes

---

## Part 1: File Installation (10 minutes)

### Step 1.1: Create Directory Structure

If you don't already have these folders, create them:

```
Assets/WaterSystem/
├── Scripts/
└── Shaders/
    └── Include/
```

### Step 1.2: Copy Script Files

Copy these files to `Assets/WaterSystem/Scripts/`:

1. **WaterWaveData.cs** - Wave configuration data structures
2. **WaterSurfaceAnimator.cs** - Animation controller component

**Verification**: Check Unity Console for compilation. Should complete with no errors.

### Step 1.3: Copy Shader Files

Copy this file to `Assets/WaterSystem/Shaders/Include/`:

1. **WaterWaves.hlsl** - HLSL wave calculation functions

**Verification**: 
- File appears in Project window under Shaders/Include
- No shader compilation errors

### Step 1.4: Update Existing Files

You need to modify these existing Stage 01 files:

**WaterProfile.cs** - Add wave animation properties
**WaterSurface.shader** - Add vertex displacement
**WaterProfileEditor.cs** - Add wave editing UI (optional but recommended)

We'll update these step-by-step below.

---

## Part 2: Updating WaterProfile.cs (5 minutes)

### Step 2.1: Add Wave Data Field

Open `Assets/WaterSystem/Scripts/WaterProfile.cs`

Find the class properties section and add:

```csharp
[Header("Animation (Stage 02)")]
[Tooltip("Wave animation configuration")]
public WaterWaveData waveData;
```

### Step 2.2: Update CreateDefault Method

Find the `CreateDefault()` or `Reset()` method and add wave initialization:

```csharp
public void SetDefaultValues()
{
    // Existing Stage 01 properties...
    
    // Stage 02: Initialize wave data
    if (waveData == null)
    {
        waveData = WaterWaveData.CreateOceanWaves();
    }
}
```

### Step 2.3: Add Profile Preset Methods

Add these helper methods to WaterProfile class:

```csharp
// Stage 02: Profile preset initializers
public void InitializeAsOcean()
{
    waveData = WaterWaveData.CreateOceanWaves();
}

public void InitializeAsLake()
{
    waveData = WaterWaveData.CreateLakeWaves();
}

public void InitializeAsRiver()
{
    waveData = WaterWaveData.CreateRiverWaves();
}

public void InitializeAsPool()
{
    waveData = WaterWaveData.CreatePoolWaves();
}
```

**Save the file** and wait for Unity to recompile.

---

## Part 3: Updating WaterSurface.shader (10 minutes)

### Step 3.1: Add Wave Include

Open `Assets/WaterSystem/Shaders/WaterSurface.shader`

Find the include section at the top of the shader and add:

```hlsl
#include "Include/WaterWaves.hlsl"
```

### Step 3.2: Add Wave Properties

In the Properties block, add:

```hlsl
Properties
{
    // ... existing Stage 01 properties ...
    
    [Header(Wave Animation)]
    _WaterTime("Water Time", Float) = 0.0
    _WaveCount("Wave Count", Int) = 0
    _RippleOctaves("Ripple Octaves", Int) = 3
    
    // LOD Properties
    _LODDistance0("LOD Distance 0", Float) = 100.0
    _LODDistance1("LOD Distance 1", Float) = 500.0
    _RippleLODDistance("Ripple LOD Distance", Float) = 75.0
    
    // Individual wave properties (up to 8 layers)
    _Wave0_Direction("Wave 0 Direction", Vector) = (1, 0, 0, 0)
    _Wave0_Amplitude("Wave 0 Amplitude", Float) = 0.5
    _Wave0_Wavelength("Wave 0 Wavelength", Float) = 10.0
    _Wave0_Steepness("Wave 0 Steepness", Float) = 0.5
    _Wave0_Speed("Wave 0 Speed", Float) = 5.0
    _Wave0_Phase("Wave 0 Phase", Float) = 0.0
    
    // ... repeat for waves 1-7 ...
    // (Copy the above 6 lines and change "0" to 1, 2, 3, 4, 5, 6, 7)
}
```

### Step 3.3: Add Wave Uniforms

In the shader code section (after CBUFFER), add:

```hlsl
// Wave animation uniforms
float _WaterTime;
int _WaveCount;
int _RippleOctaves;
float _LODDistance0;
float _LODDistance1;
float _RippleLODDistance;

// Wave layer data (8 layers max)
float4 _Wave0_Direction, _Wave1_Direction, _Wave2_Direction, _Wave3_Direction;
float4 _Wave4_Direction, _Wave5_Direction, _Wave6_Direction, _Wave7_Direction;
float _Wave0_Amplitude, _Wave1_Amplitude, _Wave2_Amplitude, _Wave3_Amplitude;
float _Wave4_Amplitude, _Wave5_Amplitude, _Wave6_Amplitude, _Wave7_Amplitude;
float _Wave0_Wavelength, _Wave1_Wavelength, _Wave2_Wavelength, _Wave3_Wavelength;
float _Wave4_Wavelength, _Wave5_Wavelength, _Wave6_Wavelength, _Wave7_Wavelength;
float _Wave0_Steepness, _Wave1_Steepness, _Wave2_Steepness, _Wave3_Steepness;
float _Wave4_Steepness, _Wave5_Steepness, _Wave6_Steepness, _Wave7_Steepness;
float _Wave0_Speed, _Wave1_Speed, _Wave2_Speed, _Wave3_Speed;
float _Wave4_Speed, _Wave5_Speed, _Wave6_Speed, _Wave7_Speed;
float _Wave0_Phase, _Wave1_Phase, _Wave2_Phase, _Wave3_Phase;
float _Wave4_Phase, _Wave5_Phase, _Wave6_Phase, _Wave7_Phase;
```

### Step 3.4: Add Wave Layer Helper Function

Add this function to convert uniforms to WaveLayer array:

```hlsl
void GetWaveLayers(out WaveLayer waves[MAX_WAVE_LAYERS])
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
    
    // ... repeat for waves 2-7 ...
    // (Copy the above 6 lines and change indices)
}
```

### Step 3.5: Update Vertex Shader

Find your vertex shader function (usually called `Vert` or similar) and modify it:

```hlsl
Varyings Vert(Attributes input)
{
    Varyings output;
    
    // Get world position
    float3 worldPos = TransformObjectToWorld(input.positionOS);
    
    // STAGE 02: Calculate wave displacement
    WaveLayer waves[MAX_WAVE_LAYERS];
    GetWaveLayers(waves);
    
    WaveResult waveResult = CalculateSurfaceAnimation(
        waves,
        _WaveCount,
        worldPos,
        _WaterTime,
        float2(1, 0),  // Will be from profile later
        2.0,           // Wind speed
        1.0,           // Ripple scale
        0.15,          // Ripple strength
        _RippleOctaves,
        0.1            // Normal sample offset
    );
    
    // Use displaced position
    worldPos = waveResult.position;
    
    // Transform back to clip space
    output.positionCS = TransformWorldToHClip(worldPos);
    
    // Use wave-generated normal
    output.normalWS = waveResult.normal;
    
    // ... rest of vertex shader ...
    
    return output;
}
```

**Save the shader file** and check Console for shader compilation.

---

## Part 4: Adding WaterSurfaceAnimator to Scene (5 minutes)

### Step 4.1: Select Water GameObject

1. Open your Unity scene with water
2. Select the water GameObject in Hierarchy
3. Verify it has WaterSystem component from Stage 01

### Step 4.2: Add Animator Component

1. Click "Add Component" button in Inspector
2. Type "WaterSurfaceAnimator"
3. Click to add component

### Step 4.3: Configure Animator

Set initial values:
- **Animate Waves**: ✓ (checked)
- **Time Scale**: 1.0
- **Use Global Time**: ✓ (checked)
- **Max Wave Layers**: 8
- **Enable LOD**: ✓ (checked)
- **LOD Distance 0**: 100
- **LOD Distance 1**: 500
- **Max Ripple Octaves**: 4

### Step 4.4: Verify Setup

Check that:
- No red error messages in Inspector
- Component shows all properties correctly
- WaterSystem component still present

---

## Part 5: Creating Wave Profiles (5 minutes)

### Step 5.1: Create Ocean Profile

1. In Project window, right-click
2. Create → Water System → Water Profile
3. Name it "Ocean_Rolling"
4. In Inspector, find the profile asset
5. Manually create wave data:
   - Expand "Wave Data" section
   - Add 4 wave layers
   - Configure as shown in WaterWaveData.cs CreateOceanWaves() method

### Step 5.2: Test Ocean Profile

1. Drag "Ocean_Rolling" profile onto water GameObject
2. Enter Play Mode
3. Water should now animate with rolling waves

### Step 5.3: Create Additional Profiles

Repeat the above for:
- **Lake_Calm** - Use CreateLakeWaves() parameters
- **River_Fast** - Use CreateRiverWaves() parameters  
- **Pool_Still** - Use CreatePoolWaves() parameters

---

## Part 6: Testing & Verification (10 minutes)

### Test 1: Basic Animation

**Steps**:
1. Select water GameObject
2. Apply "Ocean_Rolling" profile
3. Enter Play Mode
4. Observe water surface

**Expected**: Smooth rolling wave animation

### Test 2: Performance Check

**Steps**:
1. Open Profiler (Window → Analysis → Profiler)
2. Enter Play Mode
3. Select CPU Usage module
4. Find WaterSurfaceAnimator.Update in hierarchy

**Expected**: <0.1ms execution time

### Test 3: Profile Switching

**Steps**:
1. In Play Mode, switch between profiles
2. Try Ocean → Lake → River → Pool

**Expected**: Wave animation changes smoothly

### Test 4: Animation Control

**Steps**:
1. In Play Mode, uncheck "Animate Waves"
2. Water should freeze
3. Check "Animate Waves" again
4. Animation resumes

**Expected**: Animation toggles correctly

---

## Part 7: Optional Enhancements

### Enhancement 1: Custom Profile Editor (Optional)

If you want the nice wave editor UI:

1. Copy `WaterProfileEditor.cs` to `Assets/WaterSystem/Scripts/Editor/`
2. Copy `WaterShaderGUI.cs` to `Assets/WaterSystem/Scripts/Editor/`
3. Create Editor folder if it doesn't exist
4. Unity will recompile
5. Select any WaterProfile asset
6. Inspector now shows enhanced editor with wave preview

### Enhancement 2: Debug Visualization (Optional)

Enable debug info to see animation stats:

1. Select water GameObject
2. In WaterSurfaceAnimator component
3. Check "Show Debug Info"
4. Enter Play Mode
5. Debug overlay appears showing animation data

### Enhancement 3: LOD Gizmos (Optional)

Visualize LOD distances in Scene view:

1. Select water GameObject
2. Enable "Show Debug Info"
3. Scene view shows colored spheres for LOD distances
4. Yellow = LOD Distance 0
5. Orange = LOD Distance 1
6. Cyan = Ripple LOD Distance

---

## Troubleshooting

### Issue: "WaterWaveData not found" error

**Solution**: 
- Verify WaterWaveData.cs is in correct folder
- Check that namespace is `WaterSystem`
- Recompile (Assets → Reimport All)

### Issue: Shader compilation errors

**Solution**:
- Verify WaterWaves.hlsl is in Shaders/Include/
- Check #include path matches folder structure
- Ensure using HDRP 17 compatible syntax

### Issue: Waves don't move

**Solution**:
- Check "Animate Waves" is enabled
- Verify profile has wave layers configured
- Ensure Time Scale is not 0
- Check WaterSurfaceAnimator is on GameObject

### Issue: Performance too slow

**Solution**:
- Enable LOD
- Reduce Max Wave Layers to 4
- Reduce Ripple Octaves to 2
- Check Profiler for bottlenecks

### Issue: Weird visual artifacts

**Solution**:
- Reduce wave steepness (<0.8)
- Check wave amplitude isn't too large
- Verify normals are calculated correctly
- Ensure mesh has enough vertices

---

## Verification Checklist

Before proceeding to Stage 03:

- [ ] All scripts compile without errors
- [ ] Shader compiles without errors
- [ ] WaterSurfaceAnimator adds to GameObject successfully
- [ ] Ocean profile animates smoothly
- [ ] Performance <0.5ms total
- [ ] Can switch between profiles
- [ ] Animation pause/resume works
- [ ] No console errors or warnings
- [ ] Stage 01 features still work (color, depth, fresnel)

---

## Next Steps

Once Stage 02 is working:

1. Create additional profile variants for your needs
2. Test with different camera angles and distances
3. Verify performance on target hardware (RTX 5000-series)
4. Commit to Git with tag: `STAGE_02_COMPLETE`
5. Proceed to Stage 03: Environmental Reflections

---

## Support & References

**Documentation Files**:
- `STAGE_02_DOCUMENTATION.md` - Technical details
- `STAGE_02_QUICK_REFERENCE.md` - API reference
- `STAGE_02_TESTING.md` - Comprehensive test suite

**Key Files**:
- `WaterWaveData.cs` - Wave configuration structures
- `WaterSurfaceAnimator.cs` - Animation controller
- `WaterWaves.hlsl` - GPU wave calculations

**Stage Dependencies**:
- Stage 01 (Still Water) - Required
- Stage 03 (Reflections) - Will use animated normals from Stage 02

---

**Integration Status**: Ready for Implementation  
**Estimated Time**: 30-45 minutes  
**Difficulty**: Moderate (requires shader understanding)

Good luck with your water animation integration!
