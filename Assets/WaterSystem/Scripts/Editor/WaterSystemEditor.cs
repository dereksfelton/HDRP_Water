using UnityEngine;
using UnityEditor;

namespace WaterSystem.Editor
{
    /// <summary>
    /// Custom Inspector for WaterSystem component
    /// Adds "Apply Current Profile" button to force profile application
    /// </summary>
    [CustomEditor(typeof(WaterSystem))]
    public class WaterSystemEditor : UnityEditor.Editor
    {
        private WaterProfile lastProfile = null;
        
        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();
            
            WaterSystem waterSystem = (WaterSystem)target;
            
            // Check if profile changed
            if (waterSystem.currentProfile != lastProfile)
            {
                lastProfile = waterSystem.currentProfile;
                // Auto-apply when profile changes
                ApplyProfile(waterSystem);
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Profile Management", EditorStyles.boldLabel);
            
            // Manual apply button
            if (GUILayout.Button("Apply Current Profile", GUILayout.Height(30)))
            {
                ApplyProfile(waterSystem);
            }
            
            // Show current profile info
            if (waterSystem.currentProfile != null)
            {
                EditorGUILayout.HelpBox(
                    $"Current Profile: {waterSystem.currentProfile.profileName}\n" +
                    $"Click button above to force re-application.",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No profile assigned. Assign a Water Profile to configure water appearance.",
                    MessageType.Warning
                );
            }
        }
        
        private void ApplyProfile(WaterSystem waterSystem)
        {
            if (waterSystem.currentProfile == null)
            {
                Debug.LogWarning("[WaterSystem] No profile assigned!");
                return;
            }
            
            Debug.Log($"[WaterSystem] Starting to apply profile: {waterSystem.currentProfile.profileName}");
            
            // Apply to WaterSystem
            waterSystem.currentProfile.ApplyToWaterSystem(waterSystem);
            Debug.Log("[WaterSystem] Called ApplyToWaterSystem");
            
            // Apply to material
            MeshRenderer renderer = waterSystem.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Debug.Log($"[WaterSystem] Found MeshRenderer");
                
                if (renderer.sharedMaterial != null)
                {
                    Material mat = renderer.sharedMaterial;
                    Debug.Log($"[WaterSystem] Found material: {mat.name}");
                    Debug.Log($"[WaterSystem] Material shader: {mat.shader.name}");
                    
                    // Log BEFORE values
                    Color shallowBefore = mat.GetColor("_ShallowColor");
                    Debug.Log($"[WaterSystem] BEFORE - Shallow Color: {shallowBefore}");
                    
                    // Apply profile
                    waterSystem.currentProfile.ApplyToMaterial(mat);
                    
                    // Log AFTER values
                    Color shallowAfter = mat.GetColor("_ShallowColor");
                    Color deepAfter = mat.GetColor("_DeepColor");
                    float smoothnessAfter = mat.GetFloat("_Smoothness");
                    Debug.Log($"[WaterSystem] AFTER - Shallow Color: {shallowAfter}");
                    Debug.Log($"[WaterSystem] AFTER - Deep Color: {deepAfter}");
                    Debug.Log($"[WaterSystem] AFTER - Smoothness: {smoothnessAfter}");
                    
                    EditorUtility.SetDirty(mat);
                    Debug.Log($"[WaterSystem] Marked material as dirty");
                    
                    Debug.Log($"[WaterSystem] ✓ Applied profile: {waterSystem.currentProfile.profileName}");
                }
                else
                {
                    Debug.LogWarning("[WaterSystem] MeshRenderer has no sharedMaterial!");
                }
            }
            else
            {
                Debug.LogWarning("[WaterSystem] No MeshRenderer found!");
            }
            
            // Mark as dirty
            EditorUtility.SetDirty(waterSystem);
            
            // Repaint scene view to show changes
            SceneView.RepaintAll();
        }
    }
}