using UnityEngine;
using UnityEditor;

namespace WaterSystem.Editor
{
    /// <summary>
    /// Custom shader GUI for the Water Surface shader
    /// Provides organized, artist-friendly interface for editing water material properties
    /// </summary>
    public class WaterSurfaceShaderGUI : ShaderGUI
    {
        // Material properties
        private MaterialProperty shallowColor;
        private MaterialProperty deepColor;
        private MaterialProperty depthMaxDistance;
        private MaterialProperty smoothness;
        private MaterialProperty metallic;
        private MaterialProperty bumpMap;
        private MaterialProperty bumpScale;
        private MaterialProperty absorptionColor;
        private MaterialProperty scatteringColor;
        private MaterialProperty scatteringPower;
        private MaterialProperty fresnelPower;
        private MaterialProperty refractionStrength;
        
        // Foldout states (persisted via EditorPrefs)
        private bool showColorSettings = true;
        private bool showSurfaceSettings = true;
        private bool showAbsorptionSettings = true;
        private bool showAdvancedSettings = false;
        
        private const string PREFS_PREFIX = "WaterShaderGUI_";
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Find all properties
            FindProperties(properties);
            
            Material targetMat = materialEditor.target as Material;
            
            // Load foldout preferences
            LoadFoldoutStates();
            
            // Header
            EditorGUILayout.Space(5);
            DrawHeader();
            EditorGUILayout.Space(10);
            
            // Color Settings Section
            DrawColorSettings(materialEditor);
            EditorGUILayout.Space(5);
            
            // Surface Settings Section
            DrawSurfaceSettings(materialEditor);
            EditorGUILayout.Space(5);
            
            // Light Interaction Section
            DrawLightInteractionSettings(materialEditor);
            EditorGUILayout.Space(5);
            
            // Advanced Settings Section
            DrawAdvancedSettings(materialEditor);
            EditorGUILayout.Space(10);
            
            // Info box
            DrawInfoBox();
            
            // Validation warnings
            DrawValidationWarnings(targetMat);
            
            // Save foldout preferences
            SaveFoldoutStates();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Water Surface Shader", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Stage 1: Still Water Appearance", EditorStyles.miniLabel);
            
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }
        
        private void DrawColorSettings(MaterialEditor materialEditor)
        {
            showColorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showColorSettings, "Water Color");
            if (showColorSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "Controls the color gradient from shallow to deep water. " +
                    "Shallow color appears over light surfaces, deep color over dark surfaces.",
                    MessageType.None
                );
                
                materialEditor.ShaderProperty(shallowColor, "Shallow Color");
                materialEditor.ShaderProperty(deepColor, "Deep Color");
                materialEditor.ShaderProperty(depthMaxDistance, "Depth Transition Distance");
                
                // Preview gradient
                DrawColorGradientPreview();
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawSurfaceSettings(MaterialEditor materialEditor)
        {
            showSurfaceSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSurfaceSettings, "Surface Properties");
            if (showSurfaceSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "Surface smoothness controls reflection sharpness. " +
                    "Normal map adds micro-detail like small ripples.",
                    MessageType.None
                );
                
                materialEditor.ShaderProperty(smoothness, "Smoothness");
                
                if (smoothness.floatValue < 0.7f)
                {
                    EditorGUILayout.HelpBox(
                        "Low smoothness may make water look unrealistic. Consider using 0.85-0.95 for calm water.",
                        MessageType.Warning
                    );
                }
                
                materialEditor.ShaderProperty(metallic, "Metallic");
                
                if (metallic.floatValue > 0.1f)
                {
                    EditorGUILayout.HelpBox(
                        "Water is non-metallic. Metallic should typically be 0.",
                        MessageType.Warning
                    );
                }
                
                EditorGUILayout.Space(5);
                materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), bumpMap);
                
                if (bumpMap.textureValue != null)
                {
                    materialEditor.ShaderProperty(bumpScale, "Normal Strength");
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "No normal map assigned. Surface will lack micro-detail.",
                        MessageType.Info
                    );
                }
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawLightInteractionSettings(MaterialEditor materialEditor)
        {
            showAbsorptionSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAbsorptionSettings, "Light Interaction");
            if (showAbsorptionSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "Absorption affects underwater color tint. " +
                    "Scattering simulates light bouncing off particles in the water.",
                    MessageType.None
                );
                
                materialEditor.ShaderProperty(absorptionColor, "Absorption Color");
                EditorGUILayout.LabelField("Affects underwater visibility tint", EditorStyles.miniLabel);
                
                EditorGUILayout.Space(5);
                materialEditor.ShaderProperty(scatteringColor, "Scattering Color");
                materialEditor.ShaderProperty(scatteringPower, "Scattering Power");
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawAdvancedSettings(MaterialEditor materialEditor)
        {
            showAdvancedSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvancedSettings, "Advanced Settings");
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "Fresnel controls edge brightness (viewing at glancing angles). " +
                    "Refraction affects light bending at the water surface.",
                    MessageType.None
                );
                
                materialEditor.ShaderProperty(fresnelPower, "Fresnel Power");
                materialEditor.ShaderProperty(refractionStrength, "Refraction Strength");
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        private void DrawColorGradientPreview()
        {
            Rect rect = GUILayoutUtility.GetRect(100, 20);
            rect = EditorGUI.IndentedRect(rect);
            
            Color shallow = shallowColor.colorValue;
            Color deep = deepColor.colorValue;
            
            // Draw gradient
            Texture2D gradientTexture = new Texture2D(100, 1);
            for (int i = 0; i < 100; i++)
            {
                float t = i / 99f;
                Color color = Color.Lerp(shallow, deep, t);
                gradientTexture.SetPixel(i, 0, color);
            }
            gradientTexture.Apply();
            
            GUI.DrawTexture(rect, gradientTexture, ScaleMode.StretchToFill);
            
            // Labels
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 22, 50, 16), "Shallow", EditorStyles.miniLabel);
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 35, rect.y + 22, 35, 16), "Deep", EditorStyles.miniLabel);
            
            // Cleanup
            Object.DestroyImmediate(gradientTexture);
        }
        
        private void DrawInfoBox()
        {
            EditorGUILayout.HelpBox(
                "Stage 1: Still Water Appearance\n\n" +
                "This shader renders convincing still water viewed from above. " +
                "It includes depth-based coloring, PBR lighting response, and Fresnel effects. " +
                "\n\nFor best results:\n" +
                "• Keep smoothness between 0.85-0.95\n" +
                "• Use a subtle normal map for micro-detail\n" +
                "• Adjust depth max distance based on water clarity",
                MessageType.Info
            );
        }
        
        private void DrawValidationWarnings(Material material)
        {
            bool hasWarnings = false;
            
            if (smoothness.floatValue < 0.5f)
            {
                hasWarnings = true;
            }
            
            if (metallic.floatValue > 0.1f)
            {
                hasWarnings = true;
            }
            
            if (bumpMap.textureValue == null)
            {
                hasWarnings = true;
            }
            
            if (hasWarnings)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    "Review the warnings above to ensure optimal water appearance.",
                    MessageType.Warning
                );
            }
        }
        
        private void FindProperties(MaterialProperty[] props)
        {
            shallowColor = FindProperty("_ShallowColor", props);
            deepColor = FindProperty("_DeepColor", props);
            depthMaxDistance = FindProperty("_DepthMaxDistance", props);
            smoothness = FindProperty("_Smoothness", props);
            metallic = FindProperty("_Metallic", props);
            bumpMap = FindProperty("_BumpMap", props);
            bumpScale = FindProperty("_BumpScale", props);
            absorptionColor = FindProperty("_AbsorptionColor", props);
            scatteringColor = FindProperty("_ScatteringColor", props);
            scatteringPower = FindProperty("_ScatteringPower", props);
            fresnelPower = FindProperty("_FresnelPower", props);
            refractionStrength = FindProperty("_RefractionStrength", props);
        }
        
        private void LoadFoldoutStates()
        {
            showColorSettings = EditorPrefs.GetBool(PREFS_PREFIX + "ColorSettings", true);
            showSurfaceSettings = EditorPrefs.GetBool(PREFS_PREFIX + "SurfaceSettings", true);
            showAbsorptionSettings = EditorPrefs.GetBool(PREFS_PREFIX + "AbsorptionSettings", true);
            showAdvancedSettings = EditorPrefs.GetBool(PREFS_PREFIX + "AdvancedSettings", false);
        }
        
        private void SaveFoldoutStates()
        {
            EditorPrefs.SetBool(PREFS_PREFIX + "ColorSettings", showColorSettings);
            EditorPrefs.SetBool(PREFS_PREFIX + "SurfaceSettings", showSurfaceSettings);
            EditorPrefs.SetBool(PREFS_PREFIX + "AbsorptionSettings", showAbsorptionSettings);
            EditorPrefs.SetBool(PREFS_PREFIX + "AdvancedSettings", showAdvancedSettings);
        }
    }
}