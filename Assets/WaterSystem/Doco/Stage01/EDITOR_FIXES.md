# Editor Script Namespace Fixes

**Issue**: Missing namespace imports in Editor scripts  
**Date**: December 6, 2025  
**Status**: ✅ FIXED

---

## Errors Encountered

### WaterSurfaceShaderGUI.cs - Line 232
```
"The name 'DestroyImmediate' does not exist in the current context"
```

**Cause**: In Editor context, need to use `Object.DestroyImmediate()` instead of just `DestroyImmediate()`

**Fix**: Changed to `Object.DestroyImmediate(gradientTexture);`

---

### WaterProfileEditor.cs - Multiple Errors

#### Line 11: "Editor is a namespace but is used like a type"
```csharp
public class WaterProfileEditor : Editor  // ❌ Wrong
```

**Cause**: Missing `using UnityEditor;` directive

**Fix**: 
```csharp
using UnityEditor;
...
public class WaterProfileEditor : UnityEditor.Editor  // ✅ Correct
```

#### Line 16: "DrawDefaultInspector does not exist"
#### Line 25: "target does not exist"  
#### Line 57: "Instantiate does not exist"

**Cause**: All inherited from `UnityEditor.Editor` base class, but namespace import was missing

**Fix**: Added proper using directives at top of file

---

## Download Fixed Files

### 1. WaterSurfaceShaderGUI.cs (UPDATED)
[Download WaterSurfaceShaderGUI.cs](computer:///mnt/user-data/outputs/WaterSurfaceShaderGUI.cs)

**What changed**: 
- Line 232: `DestroyImmediate(gradientTexture)` → `Object.DestroyImmediate(gradientTexture)`

---

### 2. WaterProfileEditor.cs (NEW/FIXED)
[Download WaterProfileEditor.cs](computer:///mnt/user-data/outputs/WaterProfileEditor.cs)

**What changed**:
- Added `using UnityEditor;`
- Changed `Editor` to `UnityEditor.Editor` for clarity
- Fixed all method calls to use proper base class references

---

## Installation

### Step 1: WaterSurfaceShaderGUI.cs
Replace the existing file:
```
WaterSurfaceShaderGUI.cs (UPDATED) → Assets/WaterSystem/Editor/
```

### Step 2: WaterProfileEditor.cs
Replace or add this file:
```
WaterProfileEditor.cs (FIXED) → Assets/WaterSystem/Editor/
```

### Step 3: Verify
After Unity recompiles:
- ✅ No Editor errors
- ✅ Console is clean
- ✅ Can edit WaterProfile in Inspector
- ✅ Can edit Water materials with custom GUI

---

## What These Files Do

### WaterSurfaceShaderGUI.cs
- Custom Inspector GUI for water materials
- Shows organized property sections
- Displays color gradient preview
- Provides validation warnings
- Used when editing materials with `HDRP/Water/Surface` shader

### WaterProfileEditor.cs
- Custom Inspector for WaterProfile assets
- Shows validation warnings
- Provides "Create Variant" button
- Provides "Reset to Defaults" button
- Makes creating water profiles easier

---

## Complete Editor File Structure

After installation, you should have:

```
Assets/WaterSystem/Editor/
├── WaterSurfaceShaderGUI.cs    ← For material editing
└── WaterProfileEditor.cs        ← For profile editing
```

Both files must be in the `Editor/` folder for Unity to recognize them as Editor scripts.

---

## Common Unity Editor Script Requirements

For future reference, Editor scripts need:

### Required Using Directives
```csharp
using UnityEngine;
using UnityEditor;
```

### Correct Base Classes
```csharp
// For custom inspectors
public class MyEditor : UnityEditor.Editor

// For shader GUIs  
public class MyShaderGUI : ShaderGUI

// For custom windows
public class MyWindow : EditorWindow
```

### Editor-Only Methods
These come from `UnityEditor.Editor`:
- `DrawDefaultInspector()`
- `target` (current object being inspected)
- `serializedObject`

These come from `UnityEngine.Object`:
- `Instantiate()`
- `Destroy()` / `DestroyImmediate()`

---

## Verification Checklist

After installing fixed files:

- [ ] Console shows no errors
- [ ] Open a WaterProfile asset in Inspector
- [ ] See validation section and tools buttons
- [ ] Select water material in Inspector
- [ ] See custom shader GUI with organized sections
- [ ] Can edit all properties without errors

---

## Next Steps

Once these errors are cleared:

1. Continue with Stage 1 implementation
2. Create your water profiles
3. Create water material
4. Test in scene

Follow [STAGE_01_CHECKLIST.md](computer:///mnt/user-data/outputs/STAGE_01_CHECKLIST.md) for complete setup.

---

**Updated**: December 6, 2025  
**Files Ready**: Download links above ⬆️  
**Status**: ✅ All Editor errors fixed
