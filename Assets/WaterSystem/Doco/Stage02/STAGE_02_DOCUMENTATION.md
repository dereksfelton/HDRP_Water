# Stage 02: Regular Surface Movement - Implementation Guide

**Target**: Unity 6.3 with HDRP 17  
**Hardware**: NVIDIA RTX 5000-series / AMD equivalent  
**Dependencies**: Stage 01 Complete (Still Water)

---

## Overview

Stage 02 adds realistic surface animation to the water system through:
- **Gerstner waves**: Physically-based wave motion with proper displacement and normals
- **Multi-octave noise**: Layered detail for ripples and small-scale movement
- **Time synchronization**: Global and local animation timing
- **Performance optimization**: GPU-accelerated calculations, LOD support

### What This Stage Adds

**Visual Enhancements**:
- Smooth, rolling wave motion
- Small-scale ripples and surface detail
- Realistic water surface dynamics
- Customizable wave patterns per profile

**Technical Features**:
- Vertex displacement for wave geometry
- Normal map animation for detail
- Multiple wave layer compositing
- Wind-driven surface animation
- Time-based animation controls

**Performance Targets**:
- GPU vertex displacement: <0.3ms
- Wave calculations: <0.1ms CPU
- Total frame impact: <0.5ms

---

## Architecture

### New Components

1. **WaterWaveData.cs** (Extended)
   - Gerstner wave parameters
   - Multi-layer wave configuration
   - Animation timing controls

2. **WaterSurfaceAnimator.cs** (New)
   - GPU wave calculation coordination
   - Time management and synchronization
   - LOD-based animation scaling

3. **WaterCore.hlsl** (Extended)
   - Gerstner wave HLSL functions
   - Multi-octave noise functions
   - Normal calculation from waves

4. **WaterSurface.shader** (Extended)
   - Vertex displacement
   - Animated normal mapping
   - Wave-based UV scrolling

### Modified Components

- **WaterProfile**: Add wave animation properties
- **WaterSurfaceRenderer**: Connect to animation system
- **WaterProfileEditor**: Add wave preview controls

---

## Implementation Phases

### Phase 1: Gerstner Wave Mathematics (60 min)
Implement the core wave algorithm in HLSL

### Phase 2: Surface Animation System (90 min)
Create C# infrastructure for wave management

### Phase 3: Shader Integration (45 min)
Update vertex and fragment shaders for animation

### Phase 4: Multi-Layer Ripples (45 min)
Add noise-based detail layers

### Phase 5: Profile Configuration (30 min)
Expose controls and create preset variants

**Total Estimated Time**: 4.5 hours

---

## Mathematical Foundation

### Gerstner Waves

Gerstner (trochoidal) waves provide realistic ocean wave motion with:
- Circular particle motion
- Sharp crests, rounded troughs
- Proper wave breaking characteristics

**Position Formula**:
```
P(x,z,t) = P₀ + Σᵢ Qᵢ * Dᵢ * Aᵢ * sin(dot(Dᵢ, P₀) * ωᵢ + t * φᵢ)
```

Where:
- `P₀` = Original position
- `Qᵢ` = Steepness (0-1)
- `Dᵢ` = Direction (normalized 2D vector)
- `Aᵢ` = Amplitude (wave height)
- `ωᵢ` = Frequency (2π / wavelength)
- `φᵢ` = Phase speed (√(g * ωᵢ))
- `g` = Gravity (9.81 m/s²)

**Normal Calculation**:
Binormal and tangent vectors derived from wave partial derivatives:
```
Binormal = (-Σ WA * direction.x * cos(...), 1 - Σ Q * WA * sin(...), -Σ WA * direction.y * cos(...))
Tangent = (1 - Σ Q * WA * sin(...), -Σ WA * direction.x * cos(...), -Σ WA * direction.y * cos(...))
Normal = normalize(cross(Binormal, Tangent))
```

### Multi-Octave Noise

Layered Perlin noise for detail:
```
height = Σᵢ (amplitude / i) * noise(position * frequency * i + time * speed)
```

Provides:
- Fine-scale ripples
- Wind-driven chop
- Surface texture variation

---

## Performance Considerations

### GPU Optimization
- Vertex displacement in vertex shader (parallel processing)
- Shared wave calculations across vertices
- LOD-based wave count reduction
- Texture-based noise lookups

### CPU Optimization
- Pre-calculate wave parameters
- Update only on property changes
- Cache frequently used values
- Minimal per-frame overhead

### Memory Usage
- Wave data: ~1KB per profile
- Animation state: ~512 bytes
- Shader constants: ~256 bytes
- Total overhead: <2KB

---

## Testing Checklist

### Visual Verification
- [ ] Waves move smoothly with realistic motion
- [ ] Multiple wave layers blend naturally
- [ ] Normal maps animate correctly
- [ ] Detail ripples visible at close range
- [ ] No visible seams or discontinuities
- [ ] Profile presets look distinct (ocean vs lake vs river)

### Technical Verification
- [ ] Performance <0.5ms total
- [ ] No vertex swimming or artifacts
- [ ] LOD transitions are smooth
- [ ] Time synchronization works across instances
- [ ] Wave parameters update in Inspector immediately
- [ ] Animation can be paused/resumed

### Profile Testing
- [ ] Ocean: Large, rolling waves
- [ ] Lake: Gentle ripples, minimal movement
- [ ] River: Directional flow, consistent motion
- [ ] Pool: Almost still, tiny surface tension waves

---

## Known Limitations & Future Improvements

### Current Limitations
- Wave count limited to 8 for performance
- No shore interaction (Stage 6)
- No object-generated waves (Stage 5)
- Fixed wave direction per layer

### Stage 3+ Dependencies
- **Stage 3**: Reflection will be affected by animated normals
- **Stage 4**: Refraction needs wave displacement
- **Stage 5**: Interactions will modify wave parameters
- **Stage 6**: Shorelines will dampen/redirect waves

---

## File Structure

```
Assets/WaterSystem/
├── Scripts/
│   ├── WaterSurfaceAnimator.cs          [NEW]
│   ├── WaterWaveData.cs                 [MODIFIED]
│   ├── WaterProfile.cs                  [MODIFIED]
│   └── WaterSurfaceRenderer.cs          [MODIFIED]
│
├── Shaders/
│   ├── WaterCore.hlsl                   [MODIFIED]
│   ├── WaterSurface.shader              [MODIFIED]
│   └── Include/
│       └── WaterWaves.hlsl              [NEW]
│
└── Profiles/
    ├── Ocean_Rolling.asset              [NEW]
    ├── Lake_Rippled.asset               [NEW]
    ├── River_Flowing.asset              [NEW]
    └── Pool_Calm.asset                  [NEW]
```

---

## Next Steps

After Stage 02 completion:
1. Test with different camera angles
2. Verify performance on target hardware
3. Create additional profile variants
4. Tag commit as `STAGE_02_COMPLETE`
5. Proceed to Stage 03: Environmental Reflections

---

**Stage 02 Status**: Ready for Implementation  
**Estimated Completion**: 4.5 hours  
**Risk Level**: Low (well-understood algorithms)
