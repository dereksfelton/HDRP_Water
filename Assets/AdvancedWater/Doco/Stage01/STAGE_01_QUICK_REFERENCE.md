# Stage 1 Quick Reference

**Unity Water System - Stage 1: Still Water Appearance**

---

## Material Property Reference

### Water Color Properties

| Property | Range | Default | Purpose |
|----------|-------|---------|---------|
| **Shallow Color** | RGBA (HDR) | (0.325, 0.807, 0.971, 0.725) | Color near shore/over light bottom |
| **Deep Color** | RGBA (HDR) | (0.086, 0.407, 1.0, 0.749) | Color in deep areas/over dark bottom |
| **Depth Max Distance** | 0.1 - 100m | 10m | Distance for color transition |

**Tips**:
- Alpha channel controls transparency (0.6-0.9 typical)
- Keep colors in same hue family for natural look
- Larger depth distance = more gradual transition

---

### Surface Properties

| Property | Range | Default | Purpose |
|----------|-------|---------|---------|
| **Smoothness** | 0.0 - 1.0 | 0.95 | Surface reflectivity (0=rough, 1=mirror) |
| **Metallic** | 0.0 - 1.0 | 0.0 | Metallic property (keep at 0 for water) |
| **Normal Scale** | 0.0 - 2.0 | 1.0 | Strength of normal map bumps |

**Recommendations**:
- **Smoothness**: 0.85-0.95 for calm water, 0.7-0.85 for rough
- **Metallic**: Always keep at 0 (water is dielectric)
- **Normal Scale**: 0.5-1.0 for subtle detail, 1.0-2.0 for choppy

---

### Light Interaction

| Property | Range | Default | Purpose |
|----------|-------|---------|---------|
| **Absorption Color** | RGB | (0.45, 0.029, 0.018) | Colors absorbed underwater |
| **Scattering Color** | RGB | (0.0, 0.46, 0.54) | Color of scattered light |
| **Scattering Power** | 0.0 - 10.0 | 3.0 | How concentrated scattering is |
| **Fresnel Power** | 0.0 - 10.0 | 5.0 | Edge brightness strength |
| **Refraction Strength** | 0.0 - 1.0 | 0.1 | Light bending amount |

**Color Notes**:
- **Absorption**: Warm colors (reds) absorb first → underwater looks blue/green
- **Scattering**: Cool colors (blues) scatter more → surface looks blue
- Higher scattering power = more surface-concentrated effect

---

## Preset Configurations

### Clear Pool Water
```yaml
Shallow Color:      (0.4, 0.85, 0.95, 0.6)
Deep Color:         (0.1, 0.5, 0.9, 0.85)
Depth Max:          5m
Smoothness:         0.98
Normal Scale:       0.3
Absorption:         (0.4, 0.01, 0.01)  # Very clear
Scattering:         (0.0, 0.3, 0.4)
Scattering Power:   2.0                # Gentle
Fresnel Power:      5.0
```
**Use for**: Swimming pools, fountains, clear tanks

---

### Ocean Water
```yaml
Shallow Color:      (0.2, 0.7, 0.8, 0.7)
Deep Color:         (0.05, 0.3, 0.6, 0.95)
Depth Max:          25m
Smoothness:         0.92
Normal Scale:       0.8
Absorption:         (0.45, 0.03, 0.02)
Scattering:         (0.0, 0.46, 0.54)
Scattering Power:   3.0                # Moderate
Fresnel Power:      5.0
```
**Use for**: Oceans, seas, large bodies of water

---

### Murky Lake
```yaml
Shallow Color:      (0.4, 0.6, 0.5, 0.75)
Deep Color:         (0.1, 0.3, 0.2, 0.9)
Depth Max:          8m
Smoothness:         0.85
Normal Scale:       1.2
Absorption:         (0.5, 0.1, 0.05)   # More absorption
Scattering:         (0.1, 0.4, 0.3)    # Green tint
Scattering Power:   4.0                # Strong
Fresnel Power:      4.0
```
**Use for**: Lakes, ponds, rivers with sediment

---

### Tropical Shallow
```yaml
Shallow Color:      (0.3, 0.9, 0.85, 0.5)  # Turquoise
Deep Color:         (0.0, 0.5, 0.7, 0.85)
Depth Max:          15m
Smoothness:         0.95
Normal Scale:       0.5
Absorption:         (0.4, 0.02, 0.01)
Scattering:         (0.0, 0.5, 0.6)
Scattering Power:   2.5
Fresnel Power:      5.5
```
**Use for**: Caribbean seas, tropical lagoons

---

### Stylized/Cartoon
```yaml
Shallow Color:      (0.5, 0.9, 1.0, 0.6)  # Bright cyan
Deep Color:         (0.2, 0.5, 0.9, 0.8)
Depth Max:          10m
Smoothness:         0.98               # Very shiny
Normal Scale:       0.2                # Minimal detail
Absorption:         (0.3, 0.01, 0.01)
Scattering:         (0.0, 0.4, 0.5)
Scattering Power:   1.5                # Softer
Fresnel Power:      6.0                # Pronounced
```
**Use for**: Stylized games, cartoon aesthetics

---

## Color Theory for Water

### Natural Water Colors

**Shallow Water** (over light sand):
- Cyan to turquoise: (0.3-0.5, 0.7-0.9, 0.8-1.0)
- High saturation, medium-high brightness

**Deep Water** (over dark bottom):
- Deep blue: (0.0-0.2, 0.3-0.5, 0.6-0.9)
- Lower saturation, medium brightness

**Murky/Sediment**:
- Green-brown: (0.2-0.5, 0.4-0.6, 0.3-0.5)
- Lower saturation, medium brightness

### Color Relationships

**Temperature**:
- Warm water (tropical): Shift toward cyan/turquoise
- Cool water (arctic): Shift toward pure blue
- Temperate: Mix of both

**Clarity**:
- Clear: High color saturation, visible depth gradient
- Murky: Low saturation, rapid color transition
- Stained (tannins): Brown/amber tint

**Depth Indicators**:
- Shallow: Lighter, more cyan
- Medium: True blue
- Deep: Darker, more navy

---

## Lighting Scenarios

### Bright Sunny Day
```yaml
Smoothness:         0.95    # Sharp highlights
Fresnel Power:      5.0     # Normal edge brightness
Scattering Power:   2.5     # Visible scattering
```
Strong directional light creates clear specular highlights.

---

### Overcast/Cloudy
```yaml
Smoothness:         0.90    # Softer highlights
Fresnel Power:      4.0     # Reduced edge brightness
Scattering Power:   3.5     # More diffuse
```
Diffuse lighting, less pronounced highlights.

---

### Sunset/Sunrise
```yaml
Shallow Color:      Add warm tint (+0.2 red)
Smoothness:         0.93
Fresnel Power:      6.0     # Enhanced for dramatic effect
```
Warm light colors, long shadows, strong Fresnel.

---

### Night/Moonlight
```yaml
Deep Color:         Very dark blue (0.02, 0.1, 0.2)
Smoothness:         0.92
Fresnel Power:      5.5     # Moon reflection
Scattering Power:   1.0     # Minimal
```
Dark overall, moon creates single bright highlight.

---

## Performance Guidelines

### Draw Calls
- **1 water plane**: 1-2 draw calls ✅
- **Multiple water bodies**: Use GPU instancing
- **LOD enabled**: +1 draw call per LOD level

### Texture Sizes

| Use Case | Normal Map Resolution |
|----------|----------------------|
| Mobile/Low | 512x512 |
| Desktop/Medium | 1024x1024 ✅ |
| High-end/Close-up | 2048x2048 |

### Shader Complexity
- **Current Stage 1**: ~50 ALU instructions
- **Target frame time**: < 0.5ms
- **GPU overhead**: Minimal with instancing

---

## Troubleshooting Quick Fixes

### Water looks flat/boring
- ✅ Add normal map
- ✅ Increase normal scale to 0.8-1.2
- ✅ Ensure smoothness is 0.9+

### Colors don't transition
- ✅ Check depth max distance (try 5-15m)
- ✅ Verify camera depth texture enabled
- ✅ Place objects underwater to test

### Too reflective/mirror-like
- ✅ Reduce smoothness to 0.85-0.90
- ✅ Increase normal scale for more bumps
- ✅ Check Fresnel power isn't too high

### Not transparent enough
- ✅ Lower alpha on shallow/deep colors
- ✅ Check render queue is "Transparent"
- ✅ Verify material isn't set to opaque

### Harsh color banding
- ✅ Increase depth max distance
- ✅ Use more similar colors
- ✅ Check color space is Linear (not Gamma)

### No specular highlights
- ✅ Increase smoothness to 0.95+
- ✅ Add/enable directional light
- ✅ Check light intensity > 0

---

## Inspector Layout

```
┌─ Water Surface Shader ─────────────────┐
│                                         │
│ ▼ Water Color                          │
│   • Shallow Color                      │
│   • Deep Color                         │
│   • Depth Transition Distance          │
│   [Color gradient preview]             │
│                                         │
│ ▼ Surface Properties                   │
│   • Smoothness                         │
│   • Metallic                           │
│   • Normal Map                         │
│   • Normal Strength                    │
│                                         │
│ ▼ Light Interaction                    │
│   • Absorption Color                   │
│   • Scattering Color                   │
│   • Scattering Power                   │
│                                         │
│ ▶ Advanced Settings                    │
│   • Fresnel Power                      │
│   • Refraction Strength                │
│                                         │
└─────────────────────────────────────────┘
```

---

## File Locations

```
Assets/AdvancedWater/
├── Shaders/
│   ├── WaterSurface.shader          ← Main shader
│   └── WaterCore.hlsl                ← Utilities
│
├── Materials/
│   └── Water_Stage1.mat              ← Your material
│
└── Profiles/
    ├── Pool_Clear.asset              ← Presets
    ├── Ocean_Deep.asset
    └── Lake_Murky.asset
```

---

## Version Info

- **Stage**: 1 - Still Water Appearance
- **Unity**: 6.3+
- **HDRP**: 17.x
- **Render Pipeline**: High Definition (HDRP)
- **Color Space**: Linear (required)
- **Target Hardware**: RTX 5000-series / AMD equivalent

---

## Next Stage Preview

**Stage 2: Surface Animation**
- Gerstner waves
- Ripple systems
- Wind-driven movement
- Time-based animation

**Coming Soon!**
