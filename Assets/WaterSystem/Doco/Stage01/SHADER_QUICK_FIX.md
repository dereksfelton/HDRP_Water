# QUICK FIX - Shader Won't Compile

**Problem**: Shader shows "Failed to compile" in dropdown  
**Cause**: Unity 6.3 HDRP shader API changes  
**Solution**: Download updated shader below ⬇️

---

## Fix (1 Step)

### Download & Replace

**[WaterSurface.shader](computer:///mnt/user-data/outputs/WaterSurface.shader)** ← Unity 6.3 compatible version

Copy to:
```
Assets/WaterSystem/Shaders/WaterSurface.shader
```

Replace the existing file.

---

## Verify

After Unity recompiles:

1. Open M_Water_Stage1 material
2. Shader dropdown → `HDRP → Water → Surface`
3. Should now appear in normal list (not "Failed to compile") ✅
4. Custom GUI should appear with organized sections ✅

---

## What Was Fixed

- ✅ LightMode tag: "Forward" → "ForwardOnly" (Unity 6.3 requirement)
- ✅ Updated HDRP includes for version 17
- ✅ Fixed function names to match Unity 6.3 API
- ✅ Corrected depth texture sampling

---

## Next Steps

Once shader compiles:
1. Create water profiles (Pool, Ocean, Lake)
2. Assign material to WaterManager
3. Test in scene!

---

**Details**: [SHADER_COMPILATION_FIX.md](computer:///mnt/user-data/outputs/SHADER_COMPILATION_FIX.md)
