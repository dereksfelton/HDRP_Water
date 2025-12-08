using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace WaterSystem.Editor
{
    /// <summary>
    /// Custom Inspector for WaterProfile ScriptableObjects
    /// Extended in Stage 02 to include wave layer editing and preview
    /// </summary>
    [CustomEditor(typeof(WaterProfile))]
    public class WaterProfileEditor : UnityEditor.Editor
    {
        private SerializedProperty profileNameProp;
        private SerializedProperty descriptionProp;
        
        // Stage 1 properties
        private SerializedProperty shallowColorProp;
        private SerializedProperty deepColorProp;
        private SerializedProperty depthMaxDistanceProp;
        private SerializedProperty smoothnessProp;
        private SerializedProperty metallicProp;
        private SerializedProperty absorptionColorProp;
        private SerializedProperty scatteringColorProp;
        private SerializedProperty scatteringPowerProp;
        private SerializedProperty fresnelPowerProp;
        private SerializedProperty refractionStrengthProp;
        private SerializedProperty normalMapProp;
        private SerializedProperty normalStrengthProp;
        private SerializedProperty normalTilingProp;
        
        // Stage 2 properties
        private SerializedProperty waveDataProp;
        private SerializedProperty enableAnimationProp;
        private SerializedProperty animationSpeedProp;
        
        // Foldout states
        private bool showProfileInfo = true;
        private bool showVisualProperties = true;
        private bool showWaveAnimation = true;
        private bool showWaveLayers = false;
        private bool showRippleSettings = false;
        
        // Wave preview
        private bool showWavePreview = false;
        private float previewTime = 0f;
        private const int PREVIEW_RESOLUTION = 200;
        
        private void OnEnable()
        {
            // Cache serialized properties
            profileNameProp = serializedObject.FindProperty("profileName");
            descriptionProp = serializedObject.FindProperty("description");
            
            shallowColorProp = serializedObject.FindProperty("shallowColor");
            deepColorProp = serializedObject.FindProperty("deepColor");
            depthMaxDistanceProp = serializedObject.FindProperty("depthMaxDistance");
            smoothnessProp = serializedObject.FindProperty("smoothness");
            metallicProp = serializedObject.FindProperty("metallic");
            absorptionColorProp = serializedObject.FindProperty("absorptionColor");
            scatteringColorProp = serializedObject.FindProperty("scatteringColor");
            scatteringPowerProp = serializedObject.FindProperty("scatteringPower");
            fresnelPowerProp = serializedObject.FindProperty("fresnelPower");
            refractionStrengthProp = serializedObject.FindProperty("refractionStrength");
            normalMapProp = serializedObject.FindProperty("normalMap");
            normalStrengthProp = serializedObject.FindProperty("normalStrength");
            normalTilingProp = serializedObject.FindProperty("normalTiling");
            
            waveDataProp = serializedObject.FindProperty("waveData");
            enableAnimationProp = serializedObject.FindProperty("enableAnimation");
            animationSpeedProp = serializedObject.FindProperty("animationSpeed");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            WaterProfile profile = (WaterProfile)target;
            
            // ================================================================
            // PROFILE INFO
            // ================================================================
            
            showProfileInfo = EditorGUILayout.BeginFoldoutHeaderGroup(showProfileInfo, "Profile Information");
            if (showProfileInfo)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(profileNameProp, new GUIContent("Profile Name"));
                EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // ================================================================
            // VISUAL PROPERTIES (STAGE 1)
            // ================================================================
            
            showVisualProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showVisualProperties, "Visual Properties");
            if (showVisualProperties)
            {
                EditorGUI.indentLevel++;
                DrawVisualProperties();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // ================================================================
            // WAVE ANIMATION (STAGE 2)
            // ================================================================
            
            showWaveAnimation = EditorGUILayout.BeginFoldoutHeaderGroup(showWaveAnimation, "Wave Animation (Stage 2)");
            if (showWaveAnimation)
            {
                EditorGUI.indentLevel++;
                DrawWaveAnimationProperties(profile);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            EditorGUILayout.Space();
            
            // ================================================================
            // QUICK ACTIONS
            // ================================================================
            
            DrawQuickActions(profile);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawVisualProperties()
        {
            EditorGUILayout.LabelField("Water Colors", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(shallowColorProp, new GUIContent("Shallow Color"));
            EditorGUILayout.PropertyField(deepColorProp, new GUIContent("Deep Color"));
            EditorGUILayout.PropertyField(depthMaxDistanceProp, new GUIContent("Depth Max Distance"));
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Surface Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(smoothnessProp, new GUIContent("Smoothness"));
            EditorGUILayout.PropertyField(metallicProp, new GUIContent("Metallic"));
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Light Interaction", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(absorptionColorProp, new GUIContent("Absorption Color"));
            EditorGUILayout.PropertyField(scatteringColorProp, new GUIContent("Scattering Color"));
            EditorGUILayout.PropertyField(scatteringPowerProp, new GUIContent("Scattering Power"));
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Fresnel & Refraction", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(fresnelPowerProp, new GUIContent("Fresnel Power"));
            EditorGUILayout.PropertyField(refractionStrengthProp, new GUIContent("Refraction Strength"));
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Normal Mapping", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(normalMapProp, new GUIContent("Normal Map"));
            EditorGUILayout.PropertyField(normalStrengthProp, new GUIContent("Normal Strength"));
            EditorGUILayout.PropertyField(normalTilingProp, new GUIContent("Normal Tiling"));
        }
        
        private void DrawWaveAnimationProperties(WaterProfile profile)
        {
            EditorGUILayout.PropertyField(enableAnimationProp, new GUIContent("Enable Animation"));
            EditorGUILayout.PropertyField(animationSpeedProp, new GUIContent("Animation Speed"));
            
            EditorGUILayout.Space();
            
            if (profile.waveData == null)
            {
                EditorGUILayout.HelpBox("Wave data is null. Initialize with default settings.", MessageType.Warning);
                
                if (GUILayout.Button("Initialize Wave Data"))
                {
                    Undo.RecordObject(profile, "Initialize Wave Data");
                    profile.waveData = new WaterWaveData();
                    profile.waveData.AddLayer(WaveLayer.CreateDefault());
                    EditorUtility.SetDirty(profile);
                }
                return;
            }
            
            // Wave Layers
            showWaveLayers = EditorGUILayout.Foldout(showWaveLayers, $"Wave Layers ({profile.waveData.layers.Count}/8)", true);
            if (showWaveLayers)
            {
                EditorGUI.indentLevel++;
                DrawWaveLayers(profile);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Ripple Settings
            showRippleSettings = EditorGUILayout.Foldout(showRippleSettings, "Ripple Detail Settings", true);
            if (showRippleSettings)
            {
                EditorGUI.indentLevel++;
                DrawRippleSettings(profile);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Wave Statistics
            DrawWaveStatistics(profile);
            
            EditorGUILayout.Space();
            
            // Wave Preview
            DrawWavePreview(profile);
        }
        
        private void DrawWaveLayers(WaterProfile profile)
        {
            var waveData = profile.waveData;
            
            for (int i = 0; i < waveData.layers.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.LabelField($"Layer {i}", EditorStyles.boldLabel);
                
                var layer = waveData.layers[i];
                
                EditorGUI.BeginChangeCheck();
                
                layer.direction = EditorGUILayout.Vector2Field("Direction", layer.direction);
                layer.amplitude = EditorGUILayout.Slider("Amplitude (m)", layer.amplitude, 0f, 10f);
                layer.wavelength = EditorGUILayout.Slider("Wavelength (m)", layer.wavelength, 0.1f, 100f);
                layer.steepness = EditorGUILayout.Slider("Steepness", layer.steepness, 0f, 1f);
                layer.speed = EditorGUILayout.Slider("Speed (m/s, 0=auto)", layer.speed, 0f, 20f);
                layer.phase = EditorGUILayout.Slider("Phase Offset", layer.phase, 0f, 6.28318f);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(profile, "Modify Wave Layer");
                    layer.NormalizeDirection();
                    EditorUtility.SetDirty(profile);
                }
                
                // Show calculated values
                EditorGUILayout.LabelField("Effective Speed", $"{layer.GetEffectiveSpeed():F2} m/s");
                EditorGUILayout.LabelField("Max Safe Steepness", $"{layer.CalculateMaxSteepness(waveData.layers.Count):F2}");
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Remove Layer"))
                {
                    Undo.RecordObject(profile, "Remove Wave Layer");
                    waveData.RemoveLayer(i);
                    EditorUtility.SetDirty(profile);
                    break;
                }
                
                GUI.enabled = i > 0;
                if (GUILayout.Button("↑"))
                {
                    Undo.RecordObject(profile, "Move Wave Layer Up");
                    var temp = waveData.layers[i];
                    waveData.layers[i] = waveData.layers[i - 1];
                    waveData.layers[i - 1] = temp;
                    EditorUtility.SetDirty(profile);
                }
                GUI.enabled = i < waveData.layers.Count - 1;
                if (GUILayout.Button("↓"))
                {
                    Undo.RecordObject(profile, "Move Wave Layer Down");
                    var temp = waveData.layers[i];
                    waveData.layers[i] = waveData.layers[i + 1];
                    waveData.layers[i + 1] = temp;
                    EditorUtility.SetDirty(profile);
                }
                GUI.enabled = true;
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            // Add new layer button
            GUI.enabled = waveData.layers.Count < 8;
            if (GUILayout.Button("Add Wave Layer"))
            {
                Undo.RecordObject(profile, "Add Wave Layer");
                waveData.AddLayer(WaveLayer.CreateDefault());
                EditorUtility.SetDirty(profile);
            }
            GUI.enabled = true;
            
            if (waveData.layers.Count >= 8)
            {
                EditorGUILayout.HelpBox("Maximum 8 wave layers for performance.", MessageType.Info);
            }
        }
        
        private void DrawRippleSettings(WaterProfile profile)
        {
            var waveData = profile.waveData;
            
            EditorGUI.BeginChangeCheck();
            
            waveData.enableRipples = EditorGUILayout.Toggle("Enable Ripples", waveData.enableRipples);
            
            if (waveData.enableRipples)
            {
                waveData.windDirection = EditorGUILayout.Vector2Field("Wind Direction", waveData.windDirection);
                waveData.windSpeed = EditorGUILayout.Slider("Wind Speed (m/s)", waveData.windSpeed, 0f, 20f);
                waveData.rippleScale = EditorGUILayout.Slider("Ripple Scale", waveData.rippleScale, 0.1f, 10f);
                waveData.rippleStrength = EditorGUILayout.Slider("Ripple Strength", waveData.rippleStrength, 0f, 1f);
                waveData.rippleOctaves = EditorGUILayout.IntSlider("Noise Octaves", waveData.rippleOctaves, 1, 6);
                waveData.rippleNormalSampleOffset = EditorGUILayout.Slider("Normal Sample Offset", waveData.rippleNormalSampleOffset, 0.01f, 1f);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(profile, "Modify Ripple Settings");
                EditorUtility.SetDirty(profile);
            }
        }
        
        private void DrawWaveStatistics(WaterProfile profile)
        {
            EditorGUILayout.LabelField("Wave Statistics", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("Total Wave Height", $"{profile.GetMaxWaveHeight():F2} m");
            
            Vector2 direction = profile.GetWaveDirection();
            EditorGUILayout.LabelField("Dominant Direction", $"({direction.x:F2}, {direction.y:F2})");
            
            EditorGUILayout.LabelField("Active Layers", $"{profile.waveData.layers.Count}");
        }
        
        private void DrawWavePreview(WaterProfile profile)
        {
            showWavePreview = EditorGUILayout.Foldout(showWavePreview, "Wave Preview (Simplified)", true);
            
            if (!showWavePreview)
                return;
            
            EditorGUILayout.HelpBox(
                "This is a simplified 2D wave height preview. " +
                "Actual 3D waves in-game will be more complex with Gerstner displacement.",
                MessageType.Info);
            
            // Time control
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview Time", GUILayout.Width(100));
            previewTime = EditorGUILayout.Slider(previewTime, 0f, 10f);
            if (GUILayout.Button("Reset", GUILayout.Width(60)))
            {
                previewTime = 0f;
            }
            EditorGUILayout.EndHorizontal();
            
            // Simple wave visualization
            Rect rect = GUILayoutUtility.GetRect(PREVIEW_RESOLUTION, 60);
            DrawWaveGraph(rect, profile, previewTime);
        }
        
        private void DrawWaveGraph(Rect rect, WaterProfile profile, float time)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            
            // Background
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1f));
            
            // Center line
            Rect centerLine = new Rect(rect.x, rect.y + rect.height * 0.5f - 0.5f, rect.width, 1f);
            EditorGUI.DrawRect(centerLine, new Color(0.5f, 0.5f, 0.5f, 1f));
            
            // Calculate wave heights
            float[] heights = new float[PREVIEW_RESOLUTION];
            float maxHeight = 0.01f;
            
            var waveData = profile.waveData;
            if (waveData != null && waveData.layers.Count > 0)
            {
                for (int x = 0; x < PREVIEW_RESOLUTION; x++)
                {
                    float worldX = (x / (float)PREVIEW_RESOLUTION) * 50f; // 50m width
                    float height = 0f;
                    
                    // Sum all waves (simplified - just vertical displacement)
                    foreach (var layer in waveData.layers)
                    {
                        float omega = 6.28318f / layer.wavelength;
                        float speed = layer.GetEffectiveSpeed();
                        float phase = worldX * omega + time * speed + layer.phase;
                        height += layer.amplitude * Mathf.Sin(phase);
                    }
                    
                    heights[x] = height;
                    maxHeight = Mathf.Max(maxHeight, Mathf.Abs(height));
                }
                
                // Draw wave line
                Vector3[] points = new Vector3[PREVIEW_RESOLUTION];
                for (int x = 0; x < PREVIEW_RESOLUTION; x++)
                {
                    float normalizedHeight = heights[x] / maxHeight;
                    float y = rect.y + rect.height * (0.5f - normalizedHeight * 0.4f);
                    points[x] = new Vector3(rect.x + x, y, 0);
                }
                
                Handles.color = new Color(0.3f, 0.7f, 1f, 1f);
                Handles.DrawAAPolyLine(2f, points);
            }
            
            // Draw scale
            GUI.Label(new Rect(rect.x + 5, rect.y + 5, 100, 20), $"±{maxHeight:F2}m", EditorStyles.miniLabel);
        }
        
        private void DrawQuickActions(WaterProfile profile)
        {
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Create Variant"))
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Save Water Profile Variant",
                    profile.profileName + " (Variant)",
                    "asset",
                    "Save variant as...");
                
                if (!string.IsNullOrEmpty(path))
                {
                    WaterProfile variant = profile.CreateVariant();
                    AssetDatabase.CreateAsset(variant, path);
                    AssetDatabase.SaveAssets();
                    EditorGUIUtility.PingObject(variant);
                }
            }
            
            if (GUILayout.Button("Reset to Ocean"))
            {
                if (EditorUtility.DisplayDialog("Reset Profile",
                    "Reset this profile to ocean defaults? This cannot be undone.",
                    "Reset", "Cancel"))
                {
                    Undo.RecordObject(profile, "Reset to Ocean");
                    var ocean = WaterProfile.CreateOceanProfile();
                    EditorUtility.CopySerialized(ocean, profile);
                    DestroyImmediate(ocean);
                    EditorUtility.SetDirty(profile);
                }
            }
            
            if (GUILayout.Button("Reset to Lake"))
            {
                if (EditorUtility.DisplayDialog("Reset Profile",
                    "Reset this profile to lake defaults? This cannot be undone.",
                    "Reset", "Cancel"))
                {
                    Undo.RecordObject(profile, "Reset to Lake");
                    var lake = WaterProfile.CreateLakeProfile();
                    EditorUtility.CopySerialized(lake, profile);
                    DestroyImmediate(lake);
                    EditorUtility.SetDirty(profile);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
