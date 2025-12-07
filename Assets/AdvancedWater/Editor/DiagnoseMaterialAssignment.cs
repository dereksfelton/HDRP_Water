using UnityEngine;
using UnityEditor;

namespace AdvancedWater.Editor
{
    public class DiagnoseMaterialAssignment : EditorWindow
    {
        [MenuItem("Water/Diagnose Material Assignment")]
        static void Diagnose()
        {
            // Find the Ocean GameObject
            WaterSystem waterSystem = Object.FindFirstObjectByType<WaterSystem>();
            
            if (waterSystem == null)
            {
                Debug.LogError("No WaterSystem found in scene!");
                return;
            }
            
            Debug.Log("=== WATER SYSTEM DIAGNOSTIC ===");
            Debug.Log($"GameObject: {waterSystem.gameObject.name}");
            Debug.Log($"Profile: {(waterSystem.profile != null ? waterSystem.profile.profileName : "NULL")}");
            
            // Check MeshRenderer
            MeshRenderer renderer = waterSystem.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                Debug.LogError("No MeshRenderer on WaterSystem GameObject!");
                return;
            }
            
            Debug.Log($"MeshRenderer found: ✓");
            Debug.Log($"Enabled: {renderer.enabled}");
            
            // Check materials
            Debug.Log($"Material count: {renderer.sharedMaterials.Length}");
            
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                Material mat = renderer.sharedMaterials[i];
                if (mat == null)
                {
                    Debug.LogWarning($"Material slot {i}: NULL");
                    continue;
                }
                
                Debug.Log($"--- Material Slot {i} ---");
                Debug.Log($"  Name: {mat.name}");
                Debug.Log($"  Shader: {mat.shader.name}");
                Debug.Log($"  Path: {AssetDatabase.GetAssetPath(mat)}");
                
                if (mat.shader.name == "HDRP/Water/Surface")
                {
                    Debug.Log($"  ✓ Using Water Surface shader");
                    Debug.Log($"  Shallow Color: {mat.GetColor("_ShallowColor")}");
                    Debug.Log($"  Deep Color: {mat.GetColor("_DeepColor")}");
                    Debug.Log($"  Smoothness: {mat.GetFloat("_Smoothness")}");
                }
                else
                {
                    Debug.LogWarning($"  ✗ NOT using Water Surface shader!");
                }
            }
            
            // Check if it's using the correct material asset
            Material sharedMat = renderer.sharedMaterial;
            if (sharedMat != null)
            {
                string path = AssetDatabase.GetAssetPath(sharedMat);
                Debug.Log($"Primary material path: {path}");
                
                if (path.Contains("M_Water_Stage1"))
                {
                    Debug.Log("  ✓ Using M_Water_Stage1 asset");
                }
                else
                {
                    Debug.LogWarning($"  ✗ NOT using M_Water_Stage1!");
                    Debug.LogWarning($"  Currently using: {sharedMat.name}");
                }
            }
            
            Debug.Log("=== END DIAGNOSTIC ===");
        }
    }
}