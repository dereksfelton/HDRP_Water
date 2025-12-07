using UnityEngine;
using UnityEditor;

namespace AdvancedWater.Editor
{
    /// <summary>
    /// Custom inspector for WaterProfile ScriptableObject
    /// Provides validation, warnings, and profile management tools
    /// </summary>
    [CustomEditor(typeof(WaterProfile))]
    public class WaterProfileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space(10);
            
            // Validation section
            DrawValidationSection();
            
            EditorGUILayout.Space(10);
            
            // Profile tools
            DrawProfileTools();
        }
        
        private void DrawValidationSection()
        {
            WaterProfile profile = (WaterProfile)target;
            
            EditorGUILayout.LabelField("Profile Validation", EditorStyles.boldLabel);
            
            string[] warnings = profile.Validate();
            
            if (warnings.Length == 0)
            {
                EditorGUILayout.HelpBox("✓ Profile is valid with no warnings", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Profile has warnings:\n\n" + string.Join("\n", warnings),
                    MessageType.Warning
                );
            }
        }
        
        private void DrawProfileTools()
        {
            EditorGUILayout.LabelField("Profile Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Create Variant", GUILayout.Height(30)))
            {
                CreateProfileVariant();
            }
            
            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "Reset Profile",
                    "Reset this profile to default values? This cannot be undone.",
                    "Reset",
                    "Cancel"))
                {
                    ResetToDefaults();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void CreateProfileVariant()
        {
            WaterProfile sourceProfile = (WaterProfile)target;
            WaterProfile variant = Object.Instantiate(sourceProfile);
            variant.profileName = sourceProfile.profileName + " Variant";
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Profile Variant",
                variant.profileName,
                "asset",
                "Save the new profile variant"
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(variant, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = variant;
            }
        }
        
        private void ResetToDefaults()
        {
            WaterProfile profile = (WaterProfile)target;
            
            // Stage 1: Reset to defaults
            profile.shallowColor = new Color(0.325f, 0.807f, 0.971f, 0.725f);
            profile.deepColor = new Color(0.086f, 0.407f, 1.0f, 0.749f);
            profile.depthMaxDistance = 10f;
            profile.smoothness = 0.95f;
            profile.metallic = 0.0f;
            profile.normalScale = 1.0f;
            profile.absorptionColor = new Color(0.45f, 0.029f, 0.018f, 1.0f);
            profile.scatteringColor = new Color(0.0f, 0.46f, 0.54f, 1.0f);
            profile.scatteringPower = 3.0f;
            profile.fresnelPower = 5.0f;
            profile.refractionStrength = 0.1f;
            
            // Stage 0: Reset to defaults
            profile.meshSettings = new WaterMeshSettings
            {
                gridResolution = 100,
                gridSize = 100f,
                generateAtRuntime = true
            };
            
            profile.performanceSettings = new PerformanceSettings
            {
                lodLevels = 3,
                lodDistance = 50f,
                enableInstancing = true
            };
            
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
        }
    }
}