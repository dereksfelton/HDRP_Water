# Profile Change Detection Fix

**Issue**: Changing water profile in Inspector doesn't update material or appearance  
**Status**: ✅ Solution provided

---

## Problem

When you change the Water Profile in the WaterSystem Inspector:
- Material properties don't update ❌
- Water appearance doesn't change ❌

This is because `WaterSystem` needs to detect the profile change and call `ApplyToMaterial()`.

---

## Solution: Add OnValidate Method

Your `WaterSystem.cs` needs an `OnValidate()` method to detect Inspector changes.

### Add This to WaterSystem.cs

```csharp
// Add this method to your WaterSystem class
private WaterProfile lastProfile = null;

private void OnValidate()
{
    // Detect profile changes in Editor
    if (currentProfile != lastProfile)
    {
        lastProfile = currentProfile;
        ApplyCurrentProfile();
    }
}

// Also ensure this method exists and applies to material
private void ApplyCurrentProfile()
{
    if (currentProfile == null)
        return;
    
    // Apply to WaterSystem (which internally calls ApplyToMaterial)
    currentProfile.ApplyToWaterSystem(this);
    
    // Force material update if in Editor
    #if UNITY_EDITOR
    if (!Application.isPlaying)
    {
        UnityEditor.EditorUtility.SetDirty(this);
        
        // Also update the material directly
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            currentProfile.ApplyToMaterial(renderer.sharedMaterial);
            UnityEditor.EditorUtility.SetDirty(renderer.sharedMaterial);
        }
    }
    #endif
}
```

---

## Where to Add It

Open `Assets/WaterSystem/Scripts/WaterSystem.cs` and add:

1. **Field** at the top of the class (with other fields):
   ```csharp
   private WaterProfile lastProfile = null;
   ```

2. **OnValidate method** anywhere in the class:
   ```csharp
   private void OnValidate()
   {
       if (currentProfile != lastProfile)
       {
           lastProfile = currentProfile;
           ApplyCurrentProfile();
       }
   }
   ```

3. **Update ApplyCurrentProfile** if it exists, or add it:
   ```csharp
   private void ApplyCurrentProfile()
   {
       if (currentProfile == null)
           return;
       
       currentProfile.ApplyToWaterSystem(this);
       
       #if UNITY_EDITOR
       if (!Application.isPlaying)
       {
           UnityEditor.EditorUtility.SetDirty(this);
           
           MeshRenderer renderer = GetComponent<MeshRenderer>();
           if (renderer != null && renderer.sharedMaterial != null)
           {
               currentProfile.ApplyToMaterial(renderer.sharedMaterial);
               UnityEditor.EditorUtility.SetDirty(renderer.sharedMaterial);
           }
       }
       #endif
   }
   ```

---

## Alternative: Manual Button Approach

If you prefer not to modify WaterSystem.cs right now, you can **manually apply profiles**:

### Option 1: Use WaterProfileEditor Button
1. Select your profile (e.g., Pool_Still)
2. In Inspector, you should see profile validation
3. The profile is now "active" - just need to trigger application

### Option 2: Script Workaround
Create a simple Editor script to force application:

**Create**: `Assets/WaterSystem/Editor/WaterSystemEditor.cs`

```csharp
using UnityEngine;
using UnityEditor;

namespace WaterSystem.Editor
{
    [CustomEditor(typeof(WaterSystem))]
    public class WaterSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            WaterSystem waterSystem = (WaterSystem)target;
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Apply Current Profile", GUILayout.Height(30)))
            {
                if (waterSystem.currentProfile != null)
                {
                    waterSystem.currentProfile.ApplyToWaterSystem(waterSystem);
                    
                    MeshRenderer renderer = waterSystem.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.sharedMaterial != null)
                    {
                        waterSystem.currentProfile.ApplyToMaterial(renderer.sharedMaterial);
                        EditorUtility.SetDirty(renderer.sharedMaterial);
                    }
                    
                    Debug.Log($"Applied profile: {waterSystem.currentProfile.profileName}");
                }
                else
                {
                    Debug.LogWarning("No profile assigned!");
                }
            }
        }
    }
}
```

This adds an **"Apply Current Profile"** button to WaterSystem Inspector!

---

## Quick Test After Fix

1. Select "Ocean" GameObject
2. Change Water Profile dropdown (e.g., Pool_Still → Ocean_Default)
3. **Should automatically update!** ✅
   - Or click "Apply Current Profile" button if using Option 2

---

## Verification

After applying the fix, test:
- [ ] Change profile in Inspector
- [ ] M_Water_Stage1 material properties update
- [ ] Water color changes in Scene view
- [ ] Smoothness warning disappears (if using high smoothness profile)

---

## Why This Happens

Unity's Inspector doesn't automatically call methods when you change fields. You need:
- `OnValidate()` - Called when Inspector values change
- `EditorUtility.SetDirty()` - Marks objects as modified
- Manual application - Or provide a button

Stage 0 likely had this issue too, and we solved it similarly.

---

**Choose your approach:**
1. ✅ **Recommended**: Add OnValidate to WaterSystem.cs (permanent fix)
2. ✅ **Quick**: Create WaterSystemEditor.cs with button (easy workaround)
3. ❌ **Manual**: Call ApplyToMaterial from code/console (tedious)

---

Let me know which approach you'd like, and I can provide the complete updated file!
