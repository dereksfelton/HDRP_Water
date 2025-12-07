# Advanced Water System - Architecture Diagram

## System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         Unity Scene                              │
│                                                                   │
│  ┌────────────────┐         ┌─────────────────┐                │
│  │  Water Plane   │────────▶│  WaterSystem    │                │
│  │  (GameObject)  │         │  (Component)    │                │
│  └────────────────┘         └────────┬────────┘                │
│                                       │                          │
│                          ┌────────────┼────────────┐            │
│                          │            │            │            │
│                          ▼            ▼            ▼            │
│              ┌───────────────┐  ┌──────────┐  ┌───────────┐   │
│              │ WaveSimulator │  │Interaction│  │Underwater │   │
│              │               │  │ Manager   │  │ Renderer  │   │
│              └───────────────┘  └──────────┘  └───────────┘   │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Component Hierarchy

```
WaterSystem (Main Controller)
├── Configuration
│   ├── WaterProfile (ScriptableObject)
│   │   ├── Visual Settings (colors, opacity)
│   │   ├── Wave Parameters (amplitude, frequency)
│   │   ├── Foam Settings
│   │   └── Underwater Settings
│   │
│   └── Inspector Properties
│       ├── Water Body Type
│       ├── Surface Scale
│       ├── Smoothness
│       └── Enable Flags
│
├── Subsystems (Runtime Components)
│   ├── WaterWaveSimulator
│   │   ├── Gerstner Wave Engine
│   │   ├── Height Queries
│   │   └── Normal Calculations
│   │
│   ├── WaterInteractionManager
│   │   ├── Splash System
│   │   ├── Wake Generation
│   │   ├── Foam Particles
│   │   └── Displacement Maps
│   │
│   └── WaterUnderwaterRenderer
│       ├── Volume Management
│       ├── Volumetric Fog
│       ├── Transition Detection
│       └── Post-Processing
│
└── Rendering Pipeline
    ├── WaterCustomPass (HDRP)
    │   ├── Planar Reflections
    │   ├── Refraction Capture
    │   └── RT Management
    │
    └── Material System
        ├── Water Shader Graph (Stage 1+)
        ├── Material Property Block
        └── WaterShaderIncludes.hlsl
```

---

## Data Flow

### Initialization Flow
```
Scene Load
    │
    ├──▶ WaterSystem.OnEnable()
    │       │
    │       ├──▶ Initialize WaveSimulator
    │       │       └──▶ Setup Gerstner waves
    │       │
    │       ├──▶ Initialize InteractionManager
    │       │       └──▶ Create RenderTextures
    │       │
    │       ├──▶ Initialize UnderwaterRenderer
    │       │       └──▶ Setup HDRP Volume
    │       │
    │       └──▶ UpdateMaterialProperties()
    │               └──▶ Apply to MaterialPropertyBlock
    │
    └──▶ WaterCustomPass.Setup()
            ├──▶ Allocate RTHandles
            └──▶ Find Water Material
```

### Runtime Update Flow
```
Every Frame
    │
    ├──▶ WaterSystem.Update()
    │       │
    │       ├──▶ UpdateMaterialProperties()
    │       │       └──▶ Set shader parameters
    │       │
    │       └──▶ UpdateSubsystems()
    │               │
    │               ├──▶ WaveSimulator.Update()
    │               │       └──▶ Advance wave time
    │               │
    │               ├──▶ InteractionManager.Update()
    │               │       ├──▶ Update active interactions
    │               │       ├──▶ Update foam particles
    │               │       └──▶ Update displacement map
    │               │
    │               └──▶ UnderwaterRenderer.Update()
    │                       ├──▶ Check camera position
    │                       ├──▶ Update transition progress
    │                       └──▶ Adjust volume weights
    │
    └──▶ HDRP Render Pipeline
            │
            ├──▶ Custom Pass Injection Point
            │       │
            │       └──▶ WaterCustomPass.Execute()
            │               ├──▶ Render planar reflections
            │               ├──▶ Capture refraction
            │               └──▶ Bind textures to shader
            │
            └──▶ Water Material Render
                    └──▶ Use WaterShaderIncludes.hlsl functions
```

### Gameplay Query Flow
```
Gameplay Code
    │
    ├──▶ GetWaterHeightAtPosition(worldPos)
    │       └──▶ WaveSimulator.GetHeightAtPosition()
    │               └──▶ Evaluate all Gerstner waves
    │                       └──▶ Return summed height
    │
    ├──▶ IsUnderwater(worldPos)
    │       └──▶ Compare Y with wave height
    │               └──▶ Return bool
    │
    └──▶ GetSurfaceNormal(worldPos)
            └──▶ WaveSimulator.GetNormalAtPosition()
                    └──▶ Calculate from wave derivatives
                            └──▶ Return normalized normal
```

---

## Memory Layout

```
┌─────────────────────────────────────────────────────────────┐
│                    CPU Memory (~2MB)                         │
├─────────────────────────────────────────────────────────────┤
│ WaterSystem Instance                           ~500 bytes    │
│   ├── Component fields                                       │
│   └── Cached property IDs                                    │
│                                                               │
│ WaveSimulator                                  ~2 KB         │
│   ├── GerstnerWave[8]                         ~512 bytes    │
│   └── Cached calculations                     ~1.5 KB       │
│                                                               │
│ InteractionManager                             ~100 KB       │
│   ├── List<WaterInteraction> (64)             ~4 KB         │
│   └── List<FoamParticle> (1000)               ~96 KB        │
│                                                               │
│ UnderwaterRenderer                             ~1 KB         │
│   └── Volume references                                      │
│                                                               │
│ WaterProfile (Asset)                           ~500 bytes    │
│   └── Configuration data                                     │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                   GPU Memory (~8MB)                          │
├─────────────────────────────────────────────────────────────┤
│ Displacement Map (512×512, RGBAFloat)         ~4 MB         │
│ Previous Displacement (512×512, RGBAFloat)    ~4 MB         │
│ Planar Reflection RT (varies by resolution)   ~variable     │
│ Refraction RT (full screen, RGBA16F)          ~variable     │
│ Material textures (normal, foam, etc.)        Stage 1+      │
└─────────────────────────────────────────────────────────────┘
```

---

## Execution Timeline (Single Frame)

```
Frame Start (0.00ms)
│
├─ Update Phase (0.00 - 0.10ms)
│  ├─ WaterSystem.Update()                      0.05ms
│  ├─ WaveSimulator.Update()                    0.02ms
│  ├─ InteractionManager.Update()               0.02ms
│  └─ UnderwaterRenderer.Update()               0.01ms
│
├─ Render Phase (varies - Stage 1+)
│  ├─ Culling
│  ├─ Shadow Pass
│  │
│  ├─ Custom Pass: Before Transparent
│  │  └─ WaterCustomPass.Execute()              TBD
│  │     ├─ Planar reflection render            TBD
│  │     └─ Refraction capture                  TBD
│  │
│  ├─ Transparent Objects
│  │  └─ Water Material Render                  TBD
│  │     └─ Vertex displacement                 GPU
│  │     └─ Fragment shading                    GPU
│  │
│  └─ Post-Processing
│     └─ Underwater volume effects              TBD
│
└─ Frame End
```

---

## Thread Distribution

```
Main Thread (Game Loop)
├─ WaterSystem.Update()              [CPU]
├─ Component management              [CPU]
└─ MaterialPropertyBlock updates     [CPU]

Render Thread (Graphics)
├─ Custom Pass execution             [GPU Setup]
├─ Draw calls                        [GPU Setup]
└─ Shader execution                  [GPU]

GPU
├─ Vertex Processing                 [Parallel]
│  └─ Wave displacement
├─ Fragment Processing               [Parallel]
│  ├─ Color calculation
│  ├─ Reflection/refraction
│  └─ Lighting
└─ Post-Processing                   [Parallel]
   └─ Underwater effects

(Future: Job System for wave calculations)
Worker Threads
└─ WaveSimulator.UpdateJob()         [Multi-core]
```

---

## Shader Pipeline

```
Water Material (Shader Graph - Stage 1+)
│
├─ Vertex Shader
│  ├─ Input: Mesh vertices
│  ├─ Include: WaterShaderIncludes.hlsl
│  ├─ Process: EvaluateAllWaves()
│  └─ Output: Displaced position + normals
│
├─ Fragment Shader
│  ├─ Input: Interpolated vertex data
│  ├─ Include: WaterShaderIncludes.hlsl
│  │
│  ├─ Calculate depth
│  │  └─ CalculateWaterDepth()
│  │
│  ├─ Blend colors
│  │  └─ CalculateWaterColor()
│  │
│  ├─ Sample textures
│  │  ├─ Normal maps
│  │  ├─ Foam
│  │  └─ Caustics
│  │
│  ├─ Apply lighting
│  │  ├─ Fresnel
│  │  ├─ Specular
│  │  └─ Reflection/refraction
│  │
│  └─ Output: Final color + alpha
│
└─ HDRP Lighting
   ├─ Screen Space Reflections
   ├─ Volumetric Fog
   └─ Post-Processing
```

---

## Extension Points

### Adding New Features

```
New Wave Type
    └─ Extend WaveSimulator
        └─ Add wave evaluation method
            └─ Update GetHeightAtPosition()

New Interaction Type
    └─ Add to InteractionType enum
        └─ Extend InteractionManager
            └─ Add creation method

New Visual Effect
    └─ Add to WaterProfile
        └─ Add shader parameters
            └─ Update WaterShaderIncludes.hlsl

New Physics Behavior
    └─ Create new component
        └─ Register with WaterSystem
            └─ Call from UpdateSubsystems()
```

---

## Performance Budgets

### CPU Budget (per frame @ 60 FPS = 16.67ms total)
```
WaterSystem:              0.10ms  (0.6%)
├─ WaveSimulator:         0.05ms  (0.3%)
├─ InteractionManager:    0.03ms  (0.2%)
└─ UnderwaterRenderer:    0.02ms  (0.1%)

Target: < 0.5ms total CPU (3% of frame)
```

### GPU Budget (per frame @ 60 FPS)
```
Custom Pass:              TBD     (Stage 1+)
Water Material:           TBD     (Stage 1+)
Post-Processing:          TBD     (Stage 1+)

Target: < 2ms total GPU (12% of frame)
```

### Memory Budget
```
CPU Memory:               < 5 MB
GPU Memory:               < 20 MB
Texture Streaming:        On-demand

Target: Minimal impact on memory pool
```

---

## Dependencies Graph

```
WaterSystem.cs
├── Requires: Unity.Mathematics
├── Requires: UnityEngine.Rendering.HighDefinition
├── Creates: WaterWaveSimulator
├── Creates: WaterInteractionManager
└── Creates: WaterUnderwaterRenderer

WaterProfile.cs
└── Requires: UnityEngine

WaterWaveSimulator.cs
├── Requires: Unity.Mathematics
└── Requires: Unity.Collections

WaterInteractionManager.cs
├── Requires: UnityEngine
└── Uses: RenderTexture

WaterUnderwaterRenderer.cs
├── Requires: UnityEngine.Rendering
├── Requires: UnityEngine.Rendering.HighDefinition
└── Uses: Volume, VolumetricFog

WaterCustomPass.cs
├── Requires: UnityEngine.Rendering.HighDefinition
└── Uses: CustomPass, RTHandles

WaterShaderIncludes.hlsl
├── Requires: HDRP Shader Library
└── Used by: Shader Graph (Stage 1+)
```

---

## State Machine: Underwater Transitions

```
                    ┌─────────────┐
                    │   Above     │
                    │   Water     │
                    └──────┬──────┘
                           │
                    Y > WaterHeight + 0.5m
                           │
                           ▼
                    ┌─────────────┐
            ┌───────│  Transition │───────┐
            │       │    Zone     │       │
            │       └─────────────┘       │
            │                             │
    Y < WaterHeight        Y > WaterHeight - 0.5m
    - 0.5m                                │
            │                             │
            ▼                             ▼
     ┌─────────────┐             ┌─────────────┐
     │   Under     │             │   Surface   │
     │   Water     │◀────────────│   Breaking  │
     └─────────────┘  Y < WaterHeight  └─────────────┘
                      - 0.5m

States:
- Above Water: Volume weight = 0.0
- Transition Zone: Volume weight = smooth interpolation
- Surface Breaking: Special effects (Stage 5+)
- Under Water: Volume weight = 1.0
```

---

This architecture is designed for:
✓ Modularity
✓ Performance
✓ Extensibility
✓ Maintainability

Each component has a single responsibility and clear interfaces.
