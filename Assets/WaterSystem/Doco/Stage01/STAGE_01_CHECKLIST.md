# Stage 1 Implementation Checklist

Use this checklist to ensure complete and correct Stage 1 implementation.

---

## Pre-Installation Checks

- [ ] Stage 0 is complete and working
- [ ] Unity 6.3+ installed
- [ ] HDRP 17.x package installed
- [ ] Git repository ready for commits
- [ ] No pending errors in Console

---

## File Installation

### Shader Files
- [ ] Copy `WaterSurface.shader` to `Assets/WaterSystem/Shaders/`
- [ ] Replace `WaterCore.hlsl` in `Assets/WaterSystem/Shaders/`
- [ ] No shader compilation errors in Console
- [ ] Shader appears in Shader dropdown menu

### Script Files
- [ ] Create folder `Assets/WaterSystem/Editor/` (if doesn't exist)
- [ ] Replace `WaterProfile.cs` in `Assets/WaterSystem/Scripts/`
- [ ] Copy `WaterSurfaceShaderGUI.cs` to `Assets/WaterSystem/Editor/`
- [ ] No C# compilation errors in Console

### Material Creation
- [ ] Create folder `Assets/WaterSystem/Materials/` (if doesn't exist)
- [ ] Create new Material named `Water_Stage1`
- [ ] Set Material shader to `HDRP/Water/Surface`
- [ ] Material Inspector shows custom GUI sections

---

## HDRP Configuration

### Depth Texture Setup (CRITICAL)
- [ ] Locate HDRP Asset (usually in `Assets/Settings/`)
- [ ] Navigate to: Rendering → Camera
- [ ] **Enabled**: Depth Texture ✅
- [ ] **Configured**: Opaque Objects render depth
- [ ] Save HDRP Asset

### Render Pipeline Settings
- [ ] Default Render Pipeline set to HDRP asset
- [ ] Post-processing enabled (default in Unity 6.3)
- [ ] Color Space set to Linear (Project Settings)

---

## Sample Profile Creation

Create at minimum 3 profiles for testing:

### Profile 1: Pool_Clear
- [ ] Right-click `Assets/WaterSystem/Profiles/`
- [ ] Create → Water System → Water Profile
- [ ] Name: "Pool_Clear"
- [ ] Configure using values from SAMPLE_PROFILES.md
- [ ] Shallow: Light cyan, Deep: Medium blue
- [ ] Depth Max Distance: 5m
- [ ] Smoothness: 0.98

### Profile 2: Ocean_Deep
- [ ] Create Water Profile
- [ ] Name: "Ocean_Deep"
- [ ] Configure using values from SAMPLE_PROFILES.md
- [ ] Shallow: Turquoise, Deep: Navy blue
- [ ] Depth Max Distance: 25m
- [ ] Smoothness: 0.92

### Profile 3: Lake_Murky
- [ ] Create Water Profile
- [ ] Name: "Lake_Murky"
- [ ] Configure using values from SAMPLE_PROFILES.md
- [ ] Shallow: Green-tinted, Deep: Dark green-brown
- [ ] Depth Max Distance: 8m
- [ ] Smoothness: 0.85

---

## Normal Map Setup (Optional but Recommended)

- [ ] Import tileable water normal map texture
- [ ] Select texture in Project
- [ ] Texture Type: Normal Map
- [ ] Apply and reimport
- [ ] Assign to Water_Stage1 material
- [ ] Set Normal Scale to 0.5-1.0

---

## Scene Setup

### WaterManager Configuration
- [ ] Select WaterManager GameObject in Hierarchy
- [ ] **Material** slot → Assign `Water_Stage1`
- [ ] **Water Profile** slot → Assign `Pool_Clear`
- [ ] Component shows no errors in Inspector

### Lighting Setup
- [ ] Scene has Directional Light (represents sun)
- [ ] Light Type: Directional
- [ ] Intensity: 1.0 or higher
- [ ] Color: White or slight yellow tint
- [ ] Shadows enabled (optional)

### Camera Setup
- [ ] Main Camera configured for HDRP
- [ ] Camera positioned to view water from above
- [ ] Post-processing volume in scene (if needed)

### Test Geometry (for depth gradient)
- [ ] Create sloped plane underwater (simulates beach)
- [ ] Position at various depths (0m to 10m)
- [ ] Different depth objects for gradient testing

---

## Visual Testing

### Test 1: Basic Rendering
- [ ] Enter Play Mode
- [ ] Water is visible (not pink/magenta)
- [ ] Surface is semi-transparent
- [ ] No console errors

### Test 2: Color Gradient
- [ ] View water from above
- [ ] Shallow areas show light color
- [ ] Deep areas show dark color
- [ ] Transition is smooth (no banding)
- [ ] Gradient matches profile settings

### Test 3: Lighting Response
- [ ] Rotate directional light
- [ ] Specular highlight moves with light
- [ ] Highlight is bright and sharp
- [ ] Adjusting smoothness affects highlight size

### Test 4: Fresnel Effect
- [ ] View water from directly above (90°)
- [ ] View water at glancing angle (10-20°)
- [ ] Edges appear brighter at glancing angles
- [ ] Effect is smooth and natural

### Test 5: Normal Map Detail
- [ ] Zoom close to surface in Scene View
- [ ] Visible bump/ripple detail from normal map
- [ ] Detail moves/tiles correctly
- [ ] Adjusting Normal Scale changes intensity

### Test 6: Profile Switching
- [ ] Switch to Ocean_Deep profile
- [ ] Colors update correctly
- [ ] Switch to Lake_Murky profile
- [ ] Each profile looks distinct

### Test 7: Multiple Water Bodies
- [ ] Create second water plane
- [ ] Assign same material
- [ ] Both render correctly
- [ ] Check GPU instancing in Profiler

---

## Performance Testing

### Unity Profiler
- [ ] Open Profiler (Window → Analysis → Profiler)
- [ ] Enable CPU Usage module
- [ ] Enter Play Mode
- [ ] Navigate to: CPU Usage → Rendering
- [ ] Locate water shader rendering calls
- [ ] **Record**: Water render time: _______ ms
- [ ] **Target**: < 0.5ms ✅

### Frame Debugger
- [ ] Open Frame Debugger (Window → Analysis → Frame Debugger)
- [ ] Enable and step through frame
- [ ] Locate water draw calls
- [ ] **Record**: Draw call count: _______
- [ ] **Target**: 1-2 draw calls per water body ✅
- [ ] Verify shader used is correct

### FPS Check
- [ ] Note FPS before adding water: _______ FPS
- [ ] Note FPS after adding water: _______ FPS
- [ ] **Calculate impact**: _______ FPS drop
- [ ] **Target**: < 5 FPS impact ✅

### Memory Check
- [ ] Open Profiler → Memory module
- [ ] Note memory before water: _______ MB
- [ ] Note memory after water: _______ MB
- [ ] **Calculate increase**: _______ MB
- [ ] **Target**: < 15MB increase ✅

---

## Quality Validation

### Visual Quality Checklist
- [ ] Water looks convincing from primary viewing angle
- [ ] Colors are natural and appropriate
- [ ] Surface has visible detail (not perfectly flat)
- [ ] Lighting feels realistic
- [ ] Transparency allows seeing underwater objects
- [ ] No visual artifacts (flickering, banding, z-fighting)

### Technical Quality Checklist
- [ ] Material properties update in real-time
- [ ] Custom shader GUI is functional
- [ ] Profile validation shows warnings appropriately
- [ ] All 3 sample profiles work correctly
- [ ] Performance is within targets
- [ ] No errors or warnings in Console

---

## Camera Angle Tests

Test water appearance from multiple angles:

### Top-Down View (90°)
- [ ] Clear color gradient visible
- [ ] Specular highlight centered
- [ ] Minimal Fresnel effect
- [ ] Depth clearly visible

### 45° Angle
- [ ] Color gradient still visible
- [ ] Fresnel starting to appear
- [ ] Specular highlight clear
- [ ] Surface detail visible

### Glancing Angle (10-20°)
- [ ] Strong Fresnel edge brightness
- [ ] Elongated specular highlight
- [ ] Water appears more reflective
- [ ] Gradient compressed but visible

---

## Lighting Condition Tests

### Bright Sunlight
- [ ] Sharp, bright specular highlight
- [ ] Clear color definition
- [ ] Strong contrast
- [ ] Fresnel effect pronounced

### Overcast/Cloudy
- [ ] Softer, diffuse highlights
- [ ] More muted colors
- [ ] Less contrast
- [ ] Gentler Fresnel

### Sunset/Sunrise
- [ ] Warm color tint
- [ ] Dramatic highlight
- [ ] Enhanced Fresnel for atmosphere

---

## Documentation Review

- [ ] Read STAGE_01_GUIDE.md completely
- [ ] Understand all material properties
- [ ] Familiar with troubleshooting section
- [ ] Know how to create profiles

---

## Troubleshooting (if needed)

If you encounter issues, check:

### Shader Compilation Errors
- [ ] All #include paths correct
- [ ] HDRP package version 17.x
- [ ] Shader file not corrupted
- [ ] Reimport shader file

### No Depth Gradient
- [ ] Depth texture enabled in HDRP settings
- [ ] Objects actually underwater
- [ ] Depth Max Distance appropriate (5-15m)
- [ ] Camera depth mode correct

### No Specular Highlights
- [ ] Smoothness > 0.8
- [ ] Directional light exists and enabled
- [ ] Light intensity > 0
- [ ] Material shader is correct

### Pink/Magenta Water
- [ ] Shader set to HDRP/Water/Surface
- [ ] Shader compiled without errors
- [ ] Material not using missing shader

### Custom GUI Not Showing
- [ ] WaterSurfaceShaderGUI.cs in Editor folder
- [ ] Script compiled without errors
- [ ] Shader references correct CustomEditor
- [ ] Reimport shader file

---

## Version Control

### Commit Stage 1
- [ ] Stage all new/modified files
- [ ] Commit message: "Stage 1: Still water appearance"
- [ ] Tag commit: STAGE_01_END
- [ ] Push to remote repository

```bash
git add Assets/WaterSystem/
git commit -m "Stage 1: Still water appearance - depth gradient, PBR, Fresnel"
git tag STAGE_01_END
git push origin main --tags
```

---

## Final Validation

Before considering Stage 1 complete:

### Functionality ✅
- [ ] All files installed correctly
- [ ] No compilation errors
- [ ] Water renders in Scene and Game view
- [ ] All material properties functional
- [ ] Profiles load and apply correctly

### Performance ✅
- [ ] Water render time < 0.5ms
- [ ] FPS impact minimal (< 5 FPS)
- [ ] Memory increase acceptable (< 15MB)
- [ ] GPU instancing working

### Quality ✅
- [ ] Visual appearance meets expectations
- [ ] Looks good from multiple angles
- [ ] Responds correctly to lighting
- [ ] No artifacts or issues
- [ ] Professional quality achieved

### Documentation ✅
- [ ] All documentation files read
- [ ] Implementation process understood
- [ ] Troubleshooting knowledge acquired
- [ ] Ready for Stage 2

---

## Stage 1 Completion Checklist

Mark complete when ALL of the following are true:

- [ ] ✅ All files installed and working
- [ ] ✅ Performance within targets
- [ ] ✅ Visual quality satisfactory
- [ ] ✅ No unresolved issues
- [ ] ✅ Committed to version control
- [ ] ✅ Documentation reviewed
- [ ] ✅ Ready to proceed to Stage 2

---

## Success Criteria Summary

| Category | Criteria | Status |
|----------|----------|--------|
| **Files** | All 7 files installed | ☐ |
| **Compilation** | No errors | ☐ |
| **Rendering** | Water visible and correct | ☐ |
| **Colors** | Gradient smooth and natural | ☐ |
| **Lighting** | Specular and Fresnel working | ☐ |
| **Profiles** | 3+ profiles created | ☐ |
| **Performance** | < 0.5ms render time | ☐ |
| **Quality** | Professional appearance | ☐ |
| **Documentation** | Reviewed and understood | ☐ |
| **Version Control** | Committed and tagged | ☐ |

**All boxes checked?** → **Stage 1 Complete! 🎉**

---

## Next Steps

Once Stage 1 is complete:

1. **Document your results**
   - Take screenshots of water in your scene
   - Record performance metrics
   - Note any customizations you made

2. **Test in your target environment**
   - Different quality settings
   - Various hardware if available
   - Build and test (not just editor)

3. **Prepare for Stage 2**
   - Review Gerstner wave theory
   - Consider art direction for waves
   - Plan wave parameters for your project

4. **When ready**: Begin Stage 2 implementation
   - Surface animation
   - Time-based movement
   - Wave simulation

---

**Stage 1 Checklist Complete!**

Ready to build amazing water? Let's go! 🌊
