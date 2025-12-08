using UnityEngine;
using UnityEditor;

namespace WaterSystem.Editor
{
    /// <summary>
    /// Custom Material Inspector for Water Shaders
    /// Provides organized UI for wave properties and preview controls
    /// </summary>
    public class WaterShaderGUI : ShaderGUI
    {
        private bool showWaveSettings = true;
        private bool showRippleSettings = true;
        private bool showVisualSettings = true;
        private bool showLODSettings = false;
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Wave properties are managed by the WaterSurfaceAnimator component.\n" +
                "Add WaterSurfaceAnimator to your water GameObject to enable animation.",
                MessageType.Info);
            EditorGUILayout.Space();
            
            // Visual Settings
            showVisualSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showVisualSettings, "Visual Properties");
            if (showVisualSettings)
            {
                EditorGUI.indentLevel++;
                DrawVisualProperties(materialEditor, properties);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // Wave Settings (Read-only display)
            showWaveSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showWaveSettings, "Wave Animation (Read-Only)");
            if (showWaveSettings)
            {
                EditorGUI.indentLevel++;
                DrawWaveProperties(materialEditor, properties, targetMat);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // Ripple Settings (Read-only display)
            showRippleSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showRippleSettings, "Ripple Detail (Read-Only)");
            if (showRippleSettings)
            {
                EditorGUI.indentLevel++;
                DrawRippleProperties(materialEditor, properties, targetMat);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // LOD Settings
            showLODSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLODSettings, "LOD Settings");
            if (showLODSettings)
            {
                EditorGUI.indentLevel++;
                DrawLODProperties(materialEditor, properties);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // Shader keywords
            DrawShaderKeywords(targetMat);
        }
        
        private void DrawVisualProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Colors
            var shallowColor = FindProperty("_ShallowColor", properties);
            var deepColor = FindProperty("_DeepColor", properties);
            var depthMax = FindProperty("_DepthMaxDistance", properties);
            
            materialEditor.ColorProperty(shallowColor, "Shallow Color");
            materialEditor.ColorProperty(deepColor, "Deep Color");
            materialEditor.RangeProperty(depthMax, "Depth Distance");
            
            EditorGUILayout.Space();
            
            // Surface
            var smoothness = FindProperty("_Smoothness", properties);
            var metallic = FindProperty("_Metallic", properties);
            
            materialEditor.RangeProperty(smoothness, "Smoothness");
            materialEditor.RangeProperty(metallic, "Metallic");
            
            EditorGUILayout.Space();
            
            // Normal map
            var normalMap = FindProperty("_BumpMap", properties);
            var normalScale = FindProperty("_BumpScale", properties);
            var normalTiling = FindProperty("_NormalTiling", properties);
            
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), normalMap, normalScale);
            materialEditor.VectorProperty(normalTiling, "Normal Tiling");
            
            EditorGUILayout.Space();
            
            // Lighting
            var absorption = FindProperty("_AbsorptionColor", properties);
            var scattering = FindProperty("_ScatteringColor", properties);
            var scatterPower = FindProperty("_ScatteringPower", properties);
            
            materialEditor.ColorProperty(absorption, "Absorption Color");
            materialEditor.ColorProperty(scattering, "Scattering Color");
            materialEditor.RangeProperty(scatterPower, "Scattering Power");
            
            EditorGUILayout.Space();
            
            // Fresnel
            var fresnel = FindProperty("_FresnelPower", properties);
            var refraction = FindProperty("_RefractionStrength", properties);
            
            materialEditor.RangeProperty(fresnel, "Fresnel Power");
            materialEditor.RangeProperty(refraction, "Refraction Strength");
        }
        
        private void DrawWaveProperties(MaterialEditor materialEditor, MaterialProperty[] properties, Material mat)
        {
            EditorGUILayout.HelpBox(
                "Wave properties are controlled by WaterSurfaceAnimator.\n" +
                "Edit the WaterProfile to modify wave settings.",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // Show current wave count
            int waveCount = mat.GetInt("_WaveCount");
            EditorGUILayout.LabelField("Active Waves", waveCount.ToString());
            
            float waterTime = mat.GetFloat("_WaterTime");
            EditorGUILayout.LabelField("Animation Time", waterTime.ToString("F2") + "s");
            
            EditorGUILayout.Space();
            
            // Display wave layers
            for (int i = 0; i < Mathf.Min(waveCount, 4); i++)
            {
                EditorGUILayout.LabelField($"Wave Layer {i}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                Vector4 dir = mat.GetVector($"_Wave{i}_Direction");
                float amp = mat.GetFloat($"_Wave{i}_Amplitude");
                float wavelength = mat.GetFloat($"_Wave{i}_Wavelength");
                float steepness = mat.GetFloat($"_Wave{i}_Steepness");
                
                EditorGUILayout.LabelField("Direction", $"({dir.x:F2}, {dir.y:F2})");
                EditorGUILayout.LabelField("Amplitude", $"{amp:F2}m");
                EditorGUILayout.LabelField("Wavelength", $"{wavelength:F2}m");
                EditorGUILayout.LabelField("Steepness", $"{steepness:F2}");
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            
            if (waveCount > 4)
            {
                EditorGUILayout.LabelField($"... and {waveCount - 4} more wave(s)");
            }
        }
        
        private void DrawRippleProperties(MaterialEditor materialEditor, MaterialProperty[] properties, Material mat)
        {
            EditorGUILayout.HelpBox(
                "Ripple settings are controlled by WaterProfile.\n" +
                "These values are read-only in the material.",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            int octaves = mat.GetInt("_RippleOctaves");
            Vector4 windDir = mat.GetVector("_WindDirection");
            float windSpeed = mat.GetFloat("_WindSpeed");
            float rippleScale = mat.GetFloat("_RippleScale");
            float rippleStrength = mat.GetFloat("_RippleStrength");
            
            EditorGUILayout.LabelField("Ripple Octaves", octaves.ToString());
            EditorGUILayout.LabelField("Wind Direction", $"({windDir.x:F2}, {windDir.y:F2})");
            EditorGUILayout.LabelField("Wind Speed", $"{windSpeed:F2} m/s");
            EditorGUILayout.LabelField("Ripple Scale", rippleScale.ToString("F2"));
            EditorGUILayout.LabelField("Ripple Strength", rippleStrength.ToString("F2"));
        }
        
        private void DrawLODProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var lod0 = FindProperty("_LODDistance0", properties);
            var lod1 = FindProperty("_LODDistance1", properties);
            var rippleLOD = FindProperty("_RippleLODDistance", properties);
            
            materialEditor.FloatProperty(lod0, "LOD Start Distance");
            materialEditor.FloatProperty(lod1, "LOD End Distance");
            materialEditor.FloatProperty(rippleLOD, "Ripple LOD Distance");
            
            EditorGUILayout.HelpBox(
                "LOD distances control when wave detail is reduced for performance.\n" +
                "These can also be controlled via WaterSurfaceAnimator component.",
                MessageType.Info);
        }
        
        private void DrawShaderKeywords(Material mat)
        {
            EditorGUILayout.LabelField("Shader Features", EditorStyles.boldLabel);
            
            bool wavesEnabled = mat.IsKeywordEnabled("_WAVES_ENABLED");
            bool ripplesEnabled = mat.IsKeywordEnabled("_RIPPLES_ENABLED");
            
            EditorGUI.BeginChangeCheck();
            
            wavesEnabled = EditorGUILayout.Toggle("Enable Wave Animation", wavesEnabled);
            ripplesEnabled = EditorGUILayout.Toggle("Enable Ripple Detail", ripplesEnabled);
            
            if (EditorGUI.EndChangeCheck())
            {
                if (wavesEnabled)
                    mat.EnableKeyword("_WAVES_ENABLED");
                else
                    mat.DisableKeyword("_WAVES_ENABLED");
                
                if (ripplesEnabled)
                    mat.EnableKeyword("_RIPPLES_ENABLED");
                else
                    mat.DisableKeyword("_RIPPLES_ENABLED");
            }
            
            if (!wavesEnabled)
            {
                EditorGUILayout.HelpBox(
                    "Wave animation is disabled. Water will render as flat surface.",
                    MessageType.Warning);
            }
        }
    }
}
