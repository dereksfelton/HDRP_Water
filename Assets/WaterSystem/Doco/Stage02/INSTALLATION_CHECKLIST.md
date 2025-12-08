# Stage 02: Installation Checklist

**Use this checklist to integrate Stage 02 into your Unity project**

---

## Pre-Installation

### ✅ Prerequisites Check

- [ ] Unity 6.3 installed
- [ ] HDRP 17 package installed
- [ ] Stage 01 completed and working
- [ ] Existing water scene available for testing
- [ ] Git repository ready for commit

### ✅ Backup

- [ ] Commit current Stage 01 work
- [ ] Tag commit as `STAGE_01_COMPLETE`
- [ ] Create backup of WaterSystem folder (optional)
- [ ] Note current scene state for comparison

---

## Installation Steps

### Step 1: Add New Scripts (5 min)

Copy to: `Assets/WaterSystem/Scripts/`

- [ ] WaterSurfaceAnimator.cs
- [ ] WaterWaveData.cs

**Verify**: Unity compiles without errors

### Step 2: Replace Modified Scripts (2 min)

Replace in: `Assets/WaterSystem/Scripts/`

- [ ] WaterProfile.cs (backup old version first)

**Verify**: Existing profiles still load (may show new properties)

### Step 3: Add Editor Scripts (2 min)

Copy to: `Assets/WaterSystem/Scripts/Editor/`

Create folder if it doesn't exist!

- [ ] WaterProfileEditor.cs (replaces old version)
- [ ] WaterShaderGUI.cs

**Verify**: Profile Inspector shows new wave sections

### Step 4: Add Shader Include (2 min)

Copy to: `Assets/WaterSystem/Shaders/Include/`

Create folder if it doesn't exist!

- [ ] WaterWaves.hlsl

**Verify**: File appears in Project window

### Step 5: Replace Shader (3 min)

Replace in: `Assets/WaterSystem/Shaders/`

- [ ] WaterSurface.shader (backup old version first)

**Verify**: 
- [ ] Shader compiles successfully
- [ ] No pink materials in scene
- [ ] Material Inspector opens without errors

---

## Scene Update

### Step 6: Update Existing Water GameObjects (5 min each)

For EACH water GameObject from Stage 01:

- [ ] Select GameObject in Hierarchy
- [ ] Add Component → WaterSurfaceAnimator
- [ ] Verify component appears with default values
- [ ] Keep WaterSystem component (should already be there)

**Verify**: No console errors when adding component

### Step 7: Update Materials (2 min)

For EACH water material:

- [ ] Select material in Project window
- [ ] In Inspector, find "Shader Features" section
- [ ] Enable: **_WAVES_ENABLED**
- [ ] (Optional) Enable: **_RIPPLES_ENABLED**

**Verify**: Material preview updates in Inspector

### Step 8: Update or Create Profiles (10 min)

Option A - Update Existing Profiles:

For each existing WaterProfile:
- [ ] Open profile in Inspector
- [ ] Expand "Wave Animation (Stage 2)" section
- [ ] Click "Initialize Wave Data" button
- [ ] Configure wave layers as desired
- [ ] Save

Option B - Create New Test Profiles:

- [ ] Create new profile: "Test_Ocean_Waves"
  - [ ] Click "Reset to Ocean" button
  - [ ] Save
- [ ] Create new profile: "Test_Lake_Ripples"
  - [ ] Click "Reset to Lake" button
  - [ ] Save
- [ ] (Optional) Create River and Pool profiles

**Verify**: Profile Inspector shows wave layers and ripple settings

---

## First Test

### Step 9: Scene Verification (5 min)

- [ ] Save all changes
- [ ] Save scene
- [ ] Press Play ▶️

**Expected Results**:
- [ ] Water surface animates with waves
- [ ] No console errors
- [ ] Smooth motion at 60+ FPS
- [ ] Waves look natural

**If Issues**:
- Check TROUBLESHOOTING section below
- Verify all checkboxes above completed
- Review console for specific errors

---

## Performance Verification

### Step 10: Profiler Check (5 min)

- [ ] Window → Analysis → Profiler
- [ ] Enter Play Mode
- [ ] Select CPU Usage module
- [ ] Expand Hierarchy view
- [ ] Locate "WaterSurfaceAnimator.Update"

**Verify**:
- [ ] WaterSurfaceAnimator.Update < 0.1ms
- [ ] Total frame time acceptable (60+ FPS)
- [ ] No GC allocations in water system

---

## Quality Verification

### Step 11: Visual Quality Check (10 min)

Test each profile:

**Ocean Profile**:
- [ ] Large rolling waves visible
- [ ] Multiple wave directions
- [ ] Natural wave motion
- [ ] Ripple detail at close range

**Lake Profile**:
- [ ] Gentle ripples only
- [ ] Minimal wave height
- [ ] Calm appearance
- [ ] Subtle movement

**River Profile** (if created):
- [ ] Directional flow
- [ ] Consistent movement
- [ ] Wave alignment with flow

**Pool Profile** (if created):
- [ ] Almost still surface
- [ ] Tiny surface tension waves
- [ ] Minimal disturbance

### Step 12: LOD Verification (5 min)

- [ ] Enable Debug Info on WaterSurfaceAnimator
- [ ] Position camera close to water (<50m)
  - [ ] Check debug UI shows max waves (8)
- [ ] Move camera to medium distance (100-300m)
  - [ ] Check debug UI shows reduced waves (4-6)
- [ ] Move camera far (>500m)
  - [ ] Check debug UI shows minimum waves (4)
- [ ] Verify LOD transition is smooth (no popping)

### Step 13: Animation Control (3 min)

Test animation controls:

- [ ] Set `timeScale = 0` → water freezes
- [ ] Set `timeScale = 2` → water animates faster
- [ ] Set `animateWaves = false` → water pauses
- [ ] Set `animateWaves = true` → water resumes
- [ ] Change to different profile → water updates

---

## Final Checks

### Step 14: Stage 01 Regression (5 min)

Verify Stage 01 features still work:

- [ ] Water color gradient renders
- [ ] Depth-based coloring works
- [ ] Fresnel effect visible at grazing angles
- [ ] Normal map applies (if assigned)
- [ ] Smoothness/metallic properties work
- [ ] Profile switching updates appearance

### Step 15: Documentation Review (2 min)

- [ ] Read STAGE_02_QUICK_REFERENCE.md
- [ ] Bookmark STAGE_02_TESTING.md for detailed tests
- [ ] Review STAGE_02_DOCUMENTATION.md for deep dive

---

## Git Commit

### Step 16: Commit Changes (3 min)

- [ ] Stage all new/modified files
- [ ] Use commit message from STAGE_02_SUMMARY.md
- [ ] Create tag: `STAGE_02_COMPLETE`
- [ ] Push to repository

**Suggested Commit Message**:
```
Stage 02 Complete: Water Surface Animation

Implemented Gerstner wave system with:
- Multi-layer wave composition (8 layers)
- Noise-based ripple detail
- LOD optimization
- 4 preset profiles

Performance: <0.5ms total
All tests passed
Ready for Stage 03
```

---

## TROUBLESHOOTING

### Issue: Scripts don't compile

**Check**:
- [ ] All files in correct folders
- [ ] WaterWaves.hlsl in Shaders/Include/ (not Scripts!)
- [ ] Namespace is "WaterSystem" in all C# files
- [ ] Unity 6.3 is actually installed

**Fix**: Review folder structure, verify file locations

---

### Issue: Shader errors

**Check**:
- [ ] WaterCore.hlsl exists from Stage 01
- [ ] WaterWaves.hlsl in correct location
- [ ] Include paths correct in shader
- [ ] HDRP 17 package installed

**Fix**: 
1. Check shader compilation output
2. Verify include file paths
3. Reimport shader asset

---

### Issue: Waves don't animate

**Check**:
- [ ] WaterSurfaceAnimator component added
- [ ] `animateWaves = true`
- [ ] Shader keyword `_WAVES_ENABLED` enabled
- [ ] Profile has wave layers
- [ ] In Play Mode (not Edit Mode)

**Fix**:
1. Verify component exists on GameObject
2. Check material shader keywords
3. Verify profile has WaveData initialized

---

### Issue: Pink/magenta materials

**Check**:
- [ ] Shader compiled successfully
- [ ] Include files in correct location
- [ ] HDRP pipeline active
- [ ] Graphics API compatible (DX11/Vulkan/Metal)

**Fix**:
1. Select shader in Project window
2. Check Inspector for compilation errors
3. Fix any include path issues
4. Reimport shader

---

### Issue: Performance too slow

**Check**:
- [ ] Profiler shows >0.5ms water system
- [ ] Too many wave layers enabled
- [ ] LOD disabled or distances too large
- [ ] Multiple overlapping water instances

**Fix**:
1. Enable LOD system
2. Reduce maxWaveLayers (8 → 4)
3. Reduce rippleOctaves (4 → 2)
4. Check for other performance issues in scene

---

### Issue: Waves look wrong

**Check**:
- [ ] Steepness too high (causing breaks)
- [ ] Amplitude too large
- [ ] Wavelength too small
- [ ] Normal map not assigned

**Fix**:
1. Reduce steepness to 0.3-0.7 range
2. Check "Max Safe Steepness" in profile
3. Verify wavelength > 1.0m
4. Assign proper normal map

---

### Issue: Profile won't save

**Check**:
- [ ] Asset is in Project window (not embedded)
- [ ] Not in a prefab (create separate asset)
- [ ] Write permissions on folder

**Fix**:
1. Create new profile asset
2. Copy settings manually
3. Save project

---

## Success Criteria

Installation is successful when:

✅ **Compilation**
- No script errors
- No shader errors
- Inspector opens without issues

✅ **Runtime**
- Water animates smoothly
- Multiple profiles work
- Animation controls functional
- Performance <0.5ms

✅ **Quality**
- Waves look realistic
- Ripples visible at close range
- LOD transitions smooth
- Stage 01 features intact

✅ **Testing**
- Can create new profiles
- Can modify wave layers
- Can adjust animation speed
- Debug info displays correctly

---

## Post-Installation

### Optional Enhancements

- [ ] Create custom water profiles for your scenes
- [ ] Experiment with different wave combinations
- [ ] Set up LOD distances for your camera setup
- [ ] Create profile variants for different times of day

### Next Stage Preparation

- [ ] Review Stage 03 requirements
- [ ] Understand SSR prerequisites
- [ ] Plan reflection setup for your scenes
- [ ] Bookmark Stage 03 documentation

---

## Completion

**Installation Date**: ___________  
**Time Taken**: ___________ (Target: ~45 minutes)  
**Issues Encountered**: ___________  
**Stage 02 Status**: ___________  

**Signature**: ___________

---

**Installation Complete!** 🎉

Your water system now has realistic animated waves.

Proceed to: STAGE_02_TESTING.md for comprehensive testing  
Or jump to: STAGE_03 when ready for reflections!
