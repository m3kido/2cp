using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapSaveManager))]
public class SaveMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapSaveManager script = (MapSaveManager)target; // Get target script reference

        // Draw default inspector fields
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Saving", EditorStyles.boldLabel); // Header

        // Use EditorGUILayout.BeginVertical with a GUIStyle to create a boxed section 
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        base.OnInspectorGUI();

        // Add custom button for saving
        if (GUILayout.Button("Save Map"))
        {
            script.SaveMap();
        }

        EditorGUILayout.EndVertical(); // Close the boxed section
    }
}
