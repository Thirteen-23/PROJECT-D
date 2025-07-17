using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AK.MapEditorTools;

namespace AK.RoadEditorTools
{
    public class PointManagementHelper
    {
        public static void AddPoint(RoadEditor roadEditor, Vector3 position, int index = -1)
        {
            Undo.RecordObject(roadEditor, "Add Map Point");

            if (index < 0 || index >= roadEditor.GetPointCount())
                roadEditor.AddPoint(position);
            else
            {
                // Insert at specific index - not implemented yet
                roadEditor.AddPoint(position);
                Debug.LogWarning("Insert at specific index not implemented yet");
            }

            EditorUtility.SetDirty(roadEditor);
        }

        public static void RemovePoint(RoadEditor roadEditor, int index)
        {
            if (index < 0 || index >= roadEditor.GetPointCount())
                return;

            Undo.RecordObject(roadEditor, "Remove Map Point");
            var points = roadEditor.GetSavedPoints();
            roadEditor.ClearPoints();

            for (int i = 0; i < points.Count; i++)
                if (i != index)
                    roadEditor.AddPoint(points[i]);

            EditorUtility.SetDirty(roadEditor);
        }

        public static void ClearAllPoints(RoadEditor RoadEditor)
        {
            Undo.RecordObject(RoadEditor, "Clear All Map Points");
            RoadEditor.ClearPoints();
            EditorUtility.SetDirty(RoadEditor);
        }

        public static bool FindClosestPoint(RoadEditor roadEditor, Vector3 position, float maxDistance, out int index)
        {
            float closestDistance = float.MaxValue;
            index = -1;

            var points = roadEditor.GetSavedPoints();
            for (int i = 0; i < points.Count; i++)
            {
                float distance = Vector3.Distance(position, points[i]);
                if (distance < closestDistance && distance <= maxDistance)
                {
                    closestDistance = distance;
                    index = i;
                }
            }

            return index != -1;
        }

        public static void MovePoint(RoadEditor roadEditor, int index, Vector3 newPosition)
        {
            if (index < 0 || index >= roadEditor.GetPointCount())
                return;

            Undo.RecordObject(roadEditor, "Move Map Point");
            roadEditor.UpdatePointPosition(index, newPosition);
            EditorUtility.SetDirty(roadEditor);
        }
    }
}
