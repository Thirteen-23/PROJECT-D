using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using AK.MapEditorTools;

namespace AK.RoadEditorTools
{
    public class RoadEditorUtility
    {
        /// <summary>
        /// Handle raycast against scene colliders to find placement position.
        /// </summary>
        public static bool TryGetPlacementPosition(Vector2 mousePosition, LayerMask layerMask, out Vector3 position)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                position = hit.point;
                return true;
            }
            position = Vector3.zero;
            return false;
        }
        
        /// <summary>
        /// Export the path data to a file.
        /// </summary>
        public static void ExportPathData(RoadEditor RoadEditor, bool isClosedPath, string filename = null)
        {
            var points = RoadEditor.GetSavedPoints();
            if (points == null || points.Count == 0)
            {
                EditorUtility.DisplayDialog("Export Error", "No path points to export.", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(filename))
            {
                filename = EditorUtility.SaveFilePanel("Export Path Data", Application.dataPath, "PathData", "json");
                if (string.IsNullOrEmpty(filename)) return;
            }
            
            try
            {
                File.WriteAllText(filename, JsonUtility.ToJson(new PathData { points = points, isClosed = isClosedPath }, true));
                Debug.Log($"Path data exported to {filename}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error exporting path data: {e.Message}");
                EditorUtility.DisplayDialog("Export Error", $"Failed to export path: {e.Message}", "OK");
            }
        }
        
        /// <summary>
        /// Import path data from a file.
        /// </summary>
        public static bool ImportPathData(RoadEditor roadEditor, string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = EditorUtility.OpenFilePanel("Import Path Data", Application.dataPath, "json");
                if (string.IsNullOrEmpty(filename)) return false;
            }
            
            try
            {
                PathData pathData = JsonUtility.FromJson<PathData>(File.ReadAllText(filename));
                if (pathData?.points != null)
                {
                    Undo.RecordObject(roadEditor, "Import Path Data");
                    roadEditor.ClearPoints();
                    foreach (var point in pathData.points) roadEditor.AddPoint(point);
                    EditorUtility.SetDirty(roadEditor);
                    Debug.Log($"Path data imported from {filename}");
                    return pathData.isClosed;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error importing path data: {e.Message}");
                EditorUtility.DisplayDialog("Import Error", $"Failed to import path: {e.Message}", "OK");
            }
            return false;
        }
    }
    
    /// <summary>
    /// Data container for serializing path information.
    /// </summary>
    [System.Serializable]
    public class PathData
    {
        public List<Vector3> points;
        public bool isClosed;
    }
}
