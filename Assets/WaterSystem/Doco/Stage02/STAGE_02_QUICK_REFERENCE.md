# Stage 02: Quick Reference Guide

**Quick setup guide for water surface animation in Unity 6.3 + HDRP 17**

---

## 5-Minute Setup

### Step 1: Import Files (2 min)

Copy these files to your Unity project:

```
Assets/WaterSystem/Scripts/
- WaterSurfaceAnimator.cs
- WaterWaveData.cs  
- WaterProfile.cs (replace)

Assets/WaterSystem/Scripts/Editor/
- WaterProfileEditor.cs (replace)
- WaterShaderGUI.cs

Assets/WaterSystem/Shaders/
- WaterSurface.shader (replace)
- Include/WaterWaves.hlsl
```

### Step 2: Add Component (1 min)

1. Select your water GameObject
2. Add Component → **WaterSurfaceAnimator**
3. Leave default settings

### Step 3: Create Profile (1 min)

1. Right-click in Project → Create → Water System → Water Profile
2. Name it "My Ocean"
3. Click **"Reset to Ocean"** button in Inspector

### Step 4: Apply & Test (1 min)

1. Drag profile to WaterSystem.currentProfile
2. Enable material keyword: **_WAVES_ENABLED**
3. Press Play ▶️
4. Watch waves animate!

---

## Component Quick Reference

### WaterSurfaceAnimator

**Purpose**: Manages wave animation and time synchronization

**Key Properties**:
| Property | Default | Purpose |
|----------|---------|---------|
| animateWaves | true | Enable/disable animation |
| timeScale | 1.0 | Speed multiplier (0.5 = half speed) |
| useGlobalTime | true | Sync across all water instances |
| maxWaveLayers | 8 | Maximum active waves |
| enableLOD | true | Reduce detail at distance |

**Quick Actions**:
- Pause: `animateWaves = false`
- Speed up: `timeScale = 2.0`
- Reset time: Right-click → Reset Time

---

## Profile Quick Reference

### WaterProfile Wave Settings

**Wave Layers** (8 maximum):
| Parameter | Range | What It Does |
|-----------|-------|--------------|
| Direction | Vector2 | Which way wave travels |
| Amplitude | 0-10m | Wave height (crest to trough / 2) |
| Wavelength | 0.1-100m | Distance between crests |
| Steepness | 0-1 | 0=smooth sine, 1=sharp crest |
| Speed | 0-20 m/s | 0=auto-calculate |
| Phase | 0-2π | Starting offset |

**Ripple Settings**:
| Parameter | Range | What It Does |
|-----------|-------|--------------|
| Wind Direction | Vector2 | Ripple movement direction |
| Wind Speed | 0-20 m/s | Ripple animation speed |
| Ripple Scale | 0.1-10 | Size of ripples |
| Ripple Strength | 0-1 | Height of ripples |
| Ripple Octaves | 1-6 | Detail level (higher=more) |

---

## Preset Recipes

### Ocean (Large Rolling Waves)

```
Wave Layer 0:
- Direction: (1, 0)
- Amplitude: 1.5m
- Wavelength: 60m
- Steepness: 0.6

Wave Layer 1:
- Direction: (0.7, 0.7)
- Amplitude: 1.0m
- Wavelength: 40m
- Steepness: 0.5

Wave Layer 2:
- Direction: (-0.5, 0.866)
- Amplitude: 0.5m
- Wavelength: 20m
- Steepness: 0.4

Wave Layer 3:
- Direction: (0.3, -0.954)
- Amplitude: 0.3m
- Wavelength: 10m
- Steepness: 0.3

Ripples:
- Wind Speed: 5 m/s
- Ripple Strength: 0.15
- Ripple Octaves: 4
```

### Lake (Gentle Ripples)

```
Wave Layer 0:
- Direction: (1, 0)
- Amplitude: 0.15m
- Wavelength: 8m
- Steepness: 0.2

Wave Layer 1:
- Direction: (0.6, 0.8)
- Amplitude: 0.1m
- Wavelength: 5m
- Steepness: 0.15

Ripples:
- Wind Speed: 1.5 m/s
- Ripple Strength: 0.05
- Ripple Octaves: 3
```

### River (Directional Flow)

```
Wave Layer 0:
- Direction: (1, 0) [flow direction]
- Amplitude: 0.2m
- Wavelength: 3m
- Steepness: 0.3
- Speed: 2 m/s [override auto]

Wave Layer 1:
- Direction: (1, 0.1)
- Amplitude: 0.15m
- Wavelength: 2m
- Steepness: 0.25
- Speed: 1.8 m/s

Ripples:
- Wind Direction: (1, 0) [same as flow]
- Wind Speed: 3 m/s
- Ripple Strength: 0.08
- Ripple Octaves: 3
```

### Pool (Almost Still)

```
Wave Layer 0:
- Direction: (1, 0)
- Amplitude: 0.02m
- Wavelength: 0.5m
- Steepness: 0.1

Ripples:
- Wind Speed: 0.5 m/s
- Ripple Strength: 0.02
- Ripple Octaves: 2
```

---

## Common Tasks

### Make Waves Faster/Slower

**Method 1 - Time Scale** (affects all waves):
```
WaterSurfaceAnimator.timeScale = 2.0f; // 2x speed
```

**Method 2 - Individual Waves** (Edit Profile):
- Increase wave Speed parameter
- Or decrease Wavelength (shorter = faster)

### Add More Detail

1. Increase Ripple Octaves (2 → 4)
2. Add more wave layers
3. Reduce Ripple Scale (larger scale = less detail)

### Reduce Performance Cost

1. Decrease maxWaveLayers (8 → 4)
2. Enable LOD
3. Reduce Ripple Octaves (4 → 2)
4. Increase LOD distances

### Synchronize Multiple Water Bodies

Set all WaterSurfaceAnimator instances to:
- `useGlobalTime = true`

They'll all animate in sync.

### Make Choppy Water (Stormy)

1. Increase Steepness (0.5 → 0.8)
2. Add more wave layers (4 → 8)
3. Increase Wind Speed (2 → 8 m/s)
4. Increase Ripple Strength (0.1 → 0.3)

### Make Calm Water

1. Decrease Amplitude (1.0 → 0.1)
2. Decrease Steepness (0.5 → 0.2)
3. Reduce Wind Speed (5 → 1 m/s)
4. Use fewer wave layers (4 → 1-2)

---

## Debugging Tips

### Waves Don't Move

✅ Check:
1. `WaterSurfaceAnimator.animateWaves = true`
2. Material shader keyword `_WAVES_ENABLED` enabled
3. Profile has wave layers
4. `timeScale > 0`

### Performance Issues

✅ Optimize:
1. Enable LOD (`enableLOD = true`)
2. Reduce `maxWaveLayers` (8 → 4)
3. Reduce `rippleOctaves` (4 → 2)
4. Increase LOD distances

### Waves Look Wrong

✅ Fix:
1. Reduce Steepness if too spiky
2. Check Direction is normalized
3. Ensure Wavelength > 0.1
4. Validate Amplitude isn't too large

### Shader Errors

✅ Verify:
1. HDRP 17 is installed
2. WaterWaves.hlsl in Shaders/Include/
3. WaterCore.hlsl exists
4. Shader target 4.5+

---

## Keyboard Shortcuts (Editor Scripts)

When WaterProfile is selected:

- **Alt+O**: Reset to Ocean preset
- **Alt+L**: Reset to Lake preset  
- **Alt+V**: Create Variant (must implement in editor)

---

## Script API Reference

### Get Wave Height at Position

```csharp
WaterSystem waterSystem = GetComponent<WaterSystem>();
float height = waterSystem.GetWaveHeightAtPosition(worldPos);
```

### Pause/Resume Animation

```csharp
WaterSurfaceAnimator animator = GetComponent<WaterSurfaceAnimator>();
animator.PauseAnimation();
animator.ResumeAnimation();
```

### Get Current Stats

```csharp
int activeWaves = animator.GetCurrentWaveCount();
int rippleDetail = animator.GetCurrentRippleOctaves();
float totalHeight = waterProfile.GetMaxWaveHeight();
Vector2 direction = waterProfile.GetWaveDirection();
```

---

## Material Keywords

Enable in Material Inspector:

- **_WAVES_ENABLED**: Enable Gerstner wave animation
- **_RIPPLES_ENABLED**: Enable ripple detail

Disable both for flat water (Stage 01 only).

---

## Performance Targets

**Good Performance**:
- WaterSurfaceAnimator: <0.1ms
- GPU Waves: <0.3ms
- Total: <0.5ms

**Check in Profiler**:
1. Window → Analysis → Profiler
2. CPU Usage → Hierarchy
3. Find "WaterSurfaceAnimator.Update"

---

## File Dependencies

**Required Files**:
```
WaterSurfaceAnimator.cs
  ↓ requires
WaterSystem.cs (Stage 0)
  ↓ requires  
WaterProfile.cs
  ↓ contains
WaterWaveData.cs

WaterSurface.shader
  ↓ includes
WaterWaves.hlsl
  ↓ includes
WaterCore.hlsl (Stage 1)
```

**Optional Files**:
- WaterProfileEditor.cs (better Inspector)
- WaterShaderGUI.cs (better Material Inspector)

---

## Next Stage Preview

**Stage 03** will add:
- Screen Space Reflections (SSR)
- Sky/environment reflections
- Planar reflections
- Reflection probe integration

Waves from Stage 02 will affect reflections!

---

## Help & Resources

**If stuck**:
1. Check STAGE_02_TESTING.md
2. Review STAGE_02_DOCUMENTATION.md
3. Verify Unity 6.3 compatibility
4. Check HDRP 17 is installed

**Common Unity versions**:
- ✅ Unity 6.3 + HDRP 17: Full support
- ⚠️ Unity 6.2: May need adjustments
- ❌ Unity 2022 LTS: Different HDRP API

---

**Quick Start Complete!** 🌊

Your water should now have realistic rolling waves and surface animation.
