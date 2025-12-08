# Water Profile Configurations

This document contains sample configurations for creating Water Profile assets.
Use these as templates when creating new WaterProfile ScriptableObjects in Unity.

---

## Creating a Profile in Unity

1. Right-click in Project window: `Assets/WaterSystem/Profiles/`
2. Create → Water System → Water Profile
3. Name the profile (e.g., "Pool_Clear")
4. Copy values from the configurations below

---

## Profile 1: Clear Pool Water

**Use Case**: Swimming pools, fountains, clear water features

```
Profile Identity
├─ Profile Name: "Clear Pool"
└─ Description: "Crystal-clear pool water with high visibility and sharp reflections"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.4, G: 0.85, B: 0.95, A: 0.6
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.1, G: 0.5, B: 0.9, A: 0.85
│  └─ HDR: No
└─ Depth Max Distance: 5.0

Stage 1: Surface Properties
├─ Smoothness: 0.98
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 0.3

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.4, G: 0.01, B: 0.01, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.0, G: 0.3, B: 0.4, A: 1.0
└─ Scattering Power: 2.0

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 5.0
└─ Refraction Strength: 0.1

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Very clear, high visibility
- Bright cyan in shallow areas
- Deep blue in deep areas
- Minimal surface distortion
- Sharp, mirror-like reflections

---

## Profile 2: Ocean Water (Deep)

**Use Case**: Oceans, seas, large open water bodies

```
Profile Identity
├─ Profile Name: "Ocean Deep"
└─ Description: "Deep ocean water with moderate clarity and realistic coloring"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.2, G: 0.7, B: 0.8, A: 0.7
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.05, G: 0.3, B: 0.6, A: 0.95
│  └─ HDR: No
└─ Depth Max Distance: 25.0

Stage 1: Surface Properties
├─ Smoothness: 0.92
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 0.8

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.45, G: 0.03, B: 0.02, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.0, G: 0.46, B: 0.54, A: 1.0
└─ Scattering Power: 3.0

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 5.0
└─ Refraction Strength: 0.1

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Turquoise in shallow areas
- Deep navy blue in deep areas
- Moderate surface detail
- Natural ocean appearance
- Good for large-scale scenes

---

## Profile 3: Murky Lake

**Use Case**: Lakes, ponds, rivers with sediment or algae

```
Profile Identity
├─ Profile Name: "Lake Murky"
└─ Description: "Murky lake water with reduced visibility and organic coloring"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.4, G: 0.6, B: 0.5, A: 0.75
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.1, G: 0.3, B: 0.2, A: 0.9
│  └─ HDR: No
└─ Depth Max Distance: 8.0

Stage 1: Surface Properties
├─ Smoothness: 0.85
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 1.2

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.5, G: 0.1, B: 0.05, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.1, G: 0.4, B: 0.3, A: 1.0
└─ Scattering Power: 4.0

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 4.0
└─ Refraction Strength: 0.08

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Green-tinted shallow water
- Dark green-brown deep water
- Low clarity
- More surface distortion
- Natural, organic appearance

---

## Profile 4: Tropical Shallow

**Use Case**: Caribbean waters, tropical lagoons, coral reef areas

```
Profile Identity
├─ Profile Name: "Tropical Shallow"
└─ Description: "Clear tropical water with beautiful turquoise coloring"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.3, G: 0.9, B: 0.85, A: 0.5
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.0, G: 0.5, B: 0.7, A: 0.85
│  └─ HDR: No
└─ Depth Max Distance: 15.0

Stage 1: Surface Properties
├─ Smoothness: 0.95
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 0.5

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.4, G: 0.02, B: 0.01, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.0, G: 0.5, B: 0.6, A: 1.0
└─ Scattering Power: 2.5

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 5.5
└─ Refraction Strength: 0.12

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Bright turquoise shallow areas
- Crystal-clear appearance
- Slightly enhanced Fresnel
- Beautiful gradient to deeper blue
- Vacation/paradise aesthetic

---

## Profile 5: Stylized/Cartoon Water

**Use Case**: Stylized games, cartoon aesthetics, non-realistic rendering

```
Profile Identity
├─ Profile Name: "Stylized Water"
└─ Description: "Stylized water with bright colors and simplified detail"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.5, G: 0.9, B: 1.0, A: 0.6
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.2, G: 0.5, B: 0.9, A: 0.8
│  └─ HDR: No
└─ Depth Max Distance: 10.0

Stage 1: Surface Properties
├─ Smoothness: 0.98
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 0.2

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.3, G: 0.01, B: 0.01, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.0, G: 0.4, B: 0.5, A: 1.0
└─ Scattering Power: 1.5

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 6.0
└─ Refraction Strength: 0.15

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Very bright, saturated colors
- Minimal surface detail
- Enhanced Fresnel for "pop"
- Simplified, clean appearance
- Good for cartoon/stylized games

---

## Profile 6: Dark/Night Water

**Use Case**: Night scenes, dark/moody atmospheres, horror games

```
Profile Identity
├─ Profile Name: "Night Water"
└─ Description: "Dark water for night scenes with subtle moonlight reflections"

Stage 1: Water Color
├─ Shallow Color:
│  ├─ R: 0.05, G: 0.15, B: 0.25, A: 0.8
│  └─ HDR: No
├─ Deep Color:
│  ├─ R: 0.01, G: 0.05, B: 0.1, A: 0.95
│  └─ HDR: No
└─ Depth Max Distance: 12.0

Stage 1: Surface Properties
├─ Smoothness: 0.92
└─ Metallic: 0.0

Stage 1: Normal Mapping
├─ Normal Map: [Assign tileable water normal texture]
└─ Normal Scale: 0.7

Stage 1: Light Absorption
└─ Absorption Color:
   └─ R: 0.6, G: 0.2, B: 0.1, A: 1.0

Stage 1: Light Scattering
├─ Scattering Color:
│  └─ R: 0.0, G: 0.2, B: 0.3, A: 1.0
└─ Scattering Power: 1.0

Stage 1: Fresnel & Refraction
├─ Fresnel Power: 5.5
└─ Refraction Strength: 0.05

Stage 0: Mesh Generation
├─ Grid Resolution: 100
├─ Grid Size: 100.0
└─ Generate At Runtime: true

Stage 0: Performance
├─ LOD Levels: 3
├─ LOD Distance: 50.0
└─ Enable Instancing: true
```

**Visual Characteristics**:
- Very dark overall
- Subtle color variation
- Good for moonlight reflections
- Moody, atmospheric
- Low scattering (minimal light)

---

## Color Picker Values (Hex)

For easier color entry in Unity's color picker:

### Clear Pool
- Shallow: `#66D9F3` (alpha 0.6)
- Deep: `#1A80E6` (alpha 0.85)

### Ocean Deep
- Shallow: `#33B3CC` (alpha 0.7)
- Deep: `#0D4D99` (alpha 0.95)

### Murky Lake
- Shallow: `#669980` (alpha 0.75)
- Deep: `#1A4D33` (alpha 0.9)

### Tropical Shallow
- Shallow: `#4DE6D9` (alpha 0.5)
- Deep: `#0080B3` (alpha 0.85)

### Stylized
- Shallow: `#80E6FF` (alpha 0.6)
- Deep: `#3380E6` (alpha 0.8)

### Night Water
- Shallow: `#0D2640` (alpha 0.8)
- Deep: `#030D1A` (alpha 0.95)

---

## Recommended Normal Maps

For best results, use these types of normal maps:

1. **Calm Water**: Subtle, small-scale ripples
   - Frequency: High
   - Amplitude: Low
   - Best for pools, calm lakes

2. **Ocean Surface**: Medium ripples with directionality
   - Frequency: Medium
   - Amplitude: Medium
   - Best for seas, oceans

3. **Choppy Water**: Larger, more chaotic patterns
   - Frequency: Low
   - Amplitude: High
   - Best for rough water, storms

**Texture Requirements**:
- Format: Normal Map (RGB)
- Resolution: 1024x1024 (recommended)
- Tiling: Seamless/tileable
- Compression: Normal map compression

---

## Testing Your Profile

After creating a profile, verify:

1. **Color Gradient**
   - [ ] Smooth transition from shallow to deep
   - [ ] No harsh color bands
   - [ ] Depth distance feels appropriate

2. **Surface Quality**
   - [ ] Specular highlights visible
   - [ ] Normal map adds detail
   - [ ] Smoothness looks realistic

3. **Lighting Response**
   - [ ] Responds to directional light
   - [ ] Fresnel visible at edges
   - [ ] Not too dark or too bright

4. **Performance**
   - [ ] Renders in < 0.5ms
   - [ ] No frame drops
   - [ ] GPU instancing working

---

## Customization Tips

### Making Water Clearer
- ↓ Depth max distance (5m or less)
- ↓ Absorption color values
- ↑ Shallow color alpha
- ↓ Scattering power

### Making Water Murkier
- ↑ Depth max distance
- ↑ Absorption color (especially red channel)
- ↓ Shallow/deep color brightness
- ↑ Scattering power
- Add green/brown tint

### Increasing Realism
- ↑ Smoothness (0.9-0.95)
- ↑ Normal scale (0.8-1.2)
- Fine-tune Fresnel (4-6)
- Match colors to reference photos

### Stylizing
- ↑ Color saturation
- ↓ Normal scale (0.2-0.4)
- ↑ Fresnel power (6-8)
- Use exaggerated colors

---

## Version Control

When saving profiles to version control:

```
Assets/WaterSystem/Profiles/
├── Pool_Clear.asset
├── Ocean_Deep.asset
├── Lake_Murky.asset
├── Tropical_Shallow.asset
├── Stylized_Water.asset
└── Night_Water.asset
```

These are text-serialized assets and merge well in Git.

---

**Ready to create your profiles!**
Copy these configurations into Unity's Inspector when creating new WaterProfile assets.
