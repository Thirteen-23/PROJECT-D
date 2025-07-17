using UnityEditor;
using UnityEngine;

namespace AK.MapEditorTools
{
    [CustomEditor(typeof(RoadGenerator))]
    public class RoadGeneratorInspector : Editor
    {
        private SerializedProperty roadEditorProperty;
        private SerializedProperty roadWidthProperty;
        private SerializedProperty roadMaterialProperty;
        private SerializedProperty uvTilingDensityProperty;
        private SerializedProperty uvTilingWidthProperty;
        private SerializedProperty flipNormalsProperty;
        private SerializedProperty heightOffsetProperty;
        private SerializedProperty terrainMaterialProperty;
        private SerializedProperty terrainSizeProperty;
        private SerializedProperty terrainHeightOffsetProperty;

        private bool roadSettingsFoldout = true;
        private bool textureSettingsFoldout = true;
        private bool terrainSettingsFoldout = true;
        private RoadGenerator roadGenerator;

        private void OnEnable()
        {
            roadEditorProperty = serializedObject.FindProperty("roadEditor");
            roadWidthProperty = serializedObject.FindProperty("roadWidth");
            roadMaterialProperty = serializedObject.FindProperty("roadMaterial");
            uvTilingDensityProperty = serializedObject.FindProperty("uvTilingDensity");
            uvTilingWidthProperty = serializedObject.FindProperty("uvTilingWidth");
            flipNormalsProperty = serializedObject.FindProperty("flipNormals");
            heightOffsetProperty = serializedObject.FindProperty("heightOffset");
            terrainMaterialProperty = serializedObject.FindProperty("terrainMaterial");
            terrainSizeProperty = serializedObject.FindProperty("terrainSize");
            terrainHeightOffsetProperty = serializedObject.FindProperty("terrainHeightOffset");

            roadGenerator = (RoadGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Road Generator", EditorStyles.boldLabel);

            // Reference to MapEditor component
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(roadEditorProperty, new GUIContent("Map Editor"));
            if (EditorGUI.EndChangeCheck() && roadGenerator.roadEditor != null)
                roadGenerator.RegenerateMesh();

            // Road settings section
            roadSettingsFoldout = EditorGUILayout.Foldout(roadSettingsFoldout, "Road Settings", true);
            if (roadSettingsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(roadWidthProperty);
                EditorGUILayout.PropertyField(roadMaterialProperty);
                EditorGUILayout.PropertyField(heightOffsetProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    roadGenerator.RegenerateMesh();
                }
                EditorGUI.indentLevel--;
            }

            // Terrain settings section
            terrainSettingsFoldout = EditorGUILayout.Foldout(terrainSettingsFoldout, "Terrain Settings", true);
            if (terrainSettingsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(terrainMaterialProperty);
                EditorGUILayout.PropertyField(terrainSizeProperty);
                EditorGUILayout.PropertyField(terrainHeightOffsetProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    roadGenerator.RegenerateMesh();
                }
                EditorGUI.indentLevel--;
            }

            // Texture settings section
            textureSettingsFoldout = EditorGUILayout.Foldout(textureSettingsFoldout, "Texture Settings", true);
            if (textureSettingsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(uvTilingDensityProperty);
                EditorGUILayout.PropertyField(uvTilingWidthProperty);
                EditorGUILayout.PropertyField(flipNormalsProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    roadGenerator.RegenerateMesh();
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();

            if (GUILayout.Button("Regenerate Mesh"))
            {
                roadGenerator.RegenerateMesh();
                CheckMeshVisibility(roadGenerator);
            }
        }

        // Helper method to check if the mesh will be visible
        private void CheckMeshVisibility(RoadGenerator generator)
        {
            if (generator.roadEditor == null)
            {
                Debug.LogError("[RoadGeneratorInspector] MapEditor reference is missing");
                return;
            }

            int pointCount = generator.roadEditor.GetPointCount();
            if (pointCount < 2)
            {
                Debug.LogWarning($"[RoadGeneratorInspector] MapEditor has only {pointCount} points - need at least 2 points");
                return;
            }

            MeshRenderer renderer = generator.GetComponent<MeshRenderer>();
            if (renderer.sharedMaterial == null)
                Debug.LogWarning("[RoadGeneratorInspector] No material assigned to the road");

            if (generator.terrainMaterial == null)
                Debug.LogWarning("[RoadGeneratorInspector] No material assigned to the terrain");
        }
    }
}