using UnityEngine;
using UnityEditor;

namespace AdvancedWater
{
    /// <summary>
    /// Custom Inspector editor for WaterProfile with Create Variant button
    /// Place this file in an "Editor" folder: Assets/AdvancedWater/Editor/
    /// </summary>
    [CustomEditor(typeof(WaterProfile))]
    public class WaterProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector
            DrawDefaultInspector();

            // Add some space
            EditorGUILayout.Space(10);

            // Draw a separator line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(5);

            WaterProfile profile = (WaterProfile)target;

            // Create Variant button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Create Variant", GUILayout.Width(150), GUILayout.Height(30)))
            {
                CreateVariant(profile);
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "Click 'Create Variant' to create a copy of this profile that you can customize independently.",
                MessageType.Info
            );
        }

        private void CreateVariant(WaterProfile sourceProfile)
        {
            // Prompt for variant name
            string defaultName = sourceProfile.profileName + " Variant";
            string variantName = EditorInputDialog.Show("Create Water Profile Variant", 
                "Enter name for the new variant:", defaultName);

            if (string.IsNullOrEmpty(variantName))
                return; // User cancelled

            // Create the variant
            WaterProfile variant = Instantiate(sourceProfile);
            variant.profileName = variantName;

            // Save to same folder as source
            string sourcePath = AssetDatabase.GetAssetPath(sourceProfile);
            string sourceFolder = System.IO.Path.GetDirectoryName(sourcePath);
            string variantFileName = variantName.Replace(" ", "_") + ".asset";
            string variantPath = System.IO.Path.Combine(sourceFolder, variantFileName);

            // Ensure unique filename
            variantPath = AssetDatabase.GenerateUniqueAssetPath(variantPath);

            // Create asset
            AssetDatabase.CreateAsset(variant, variantPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select and ping the new variant
            EditorGUIUtility.PingObject(variant);
            Selection.activeObject = variant;

            Debug.Log($"Created water profile variant: {variantName} at {variantPath}");
        }
    }

    /// <summary>
    /// Simple input dialog utility
    /// </summary>
    public class EditorInputDialog : EditorWindow
    {
        private string inputText = "";
        private string description = "";
        private System.Action<string> onComplete;

        public static string Show(string title, string description, string defaultValue = "")
        {
            EditorInputDialog window = GetWindow<EditorInputDialog>(true, title, true);
            window.description = description;
            window.inputText = defaultValue;
            window.minSize = new Vector2(400, 120);
            window.maxSize = new Vector2(400, 120);
            window.ShowModal();

            return window.inputText;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(10);

            GUI.SetNextControlName("InputField");
            inputText = EditorGUILayout.TextField("Name:", inputText);

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                inputText = ""; // Clear to signal cancellation
                Close();
            }

            if (GUILayout.Button("Create", GUILayout.Width(80)))
            {
                Close();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Focus input field on first frame
            if (Event.current.type == EventType.Layout)
            {
                EditorGUI.FocusTextInControl("InputField");
            }

            // Handle Enter key
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                Close();
            }
        }
    }
}