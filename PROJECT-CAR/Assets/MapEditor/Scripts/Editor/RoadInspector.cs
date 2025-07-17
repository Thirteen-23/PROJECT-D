using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AK.MapEditorTools
{
    [CustomEditor(typeof(RoadEditor))]
    public class RoadInspector : Editor
    {
        private RoadEditor roadEditor;
        private bool editingEnabled;

        private void OnEnable()
        {
            roadEditor = (RoadEditor)target;
            Tools.hidden = editingEnabled;
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Road Editing", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            RoadEditor.EditMode newEditMode = (RoadEditor.EditMode)EditorGUILayout.EnumPopup("Edit Mode", roadEditor.currentMode);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(roadEditor, "Change Road Edit Mode");
                roadEditor.currentMode = newEditMode;
                editingEnabled = newEditMode != RoadEditor.EditMode.Disabled;
                Tools.hidden = editingEnabled;
            }

            EditorGUI.BeginChangeCheck();
            bool showControlPoints = EditorGUILayout.Toggle("Show Control Points", roadEditor.showControlPoints);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(roadEditor, "Toggle Control Points");
                roadEditor.showControlPoints = showControlPoints;
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            int resolution = EditorGUILayout.IntSlider("Curve Resolution", roadEditor.curveResolution, 1, 20);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(roadEditor, "Change Curve Resolution");
                roadEditor.curveResolution = resolution;
                roadEditor.UpdateRoadGenerator();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Clear All Points"))
            {
                Undo.RecordObject(roadEditor, "Clear All Points");
                roadEditor.ClearPoints();
                SceneView.RepaintAll();
            }

            // Add more custom inspector UI as needed
        }

        private void OnSceneGUI()
        {
            if (roadEditor == null) return;

            // Drawing and editing logic for roads in the scene view
            DrawRoad();

            if (roadEditor.currentMode != RoadEditor.EditMode.Disabled)
            {
                HandlePointEditing();
            }
        }

        private void DrawRoad()
        {
            // Draw road path visualization
            List<Vector3> points = roadEditor.GetBezierCurvePoints();
            if (points.Count > 1)
            {
                Handles.color = Color.green;
                Handles.DrawPolyLine(points.ToArray());
            }

            // Draw control points if enabled
            if (roadEditor.showControlPoints)
            {
                var bezierPoints = roadEditor.GetBezierPoints();
                for (int i = 0; i < bezierPoints.Count; i++)
                {
                    var point = bezierPoints[i];
                    bool isSelected = roadEditor.selectedPointIndex == i;
                    
                    Handles.color = isSelected ? Color.yellow : Color.white;
                    float size = HandleUtility.GetHandleSize(point.position) * 0.1f;
                    
                    if (Handles.Button(point.position, Quaternion.identity, size, size, Handles.SphereHandleCap))
                    {
                        roadEditor.SelectPoint(i);
                        Repaint();
                    }

                    // Draw handles
                    if (roadEditor.showControlPoints)
                    {
                        Handles.color = Color.blue;
                        Handles.DrawLine(point.position, point.handleIn);
                        Handles.DrawLine(point.position, point.handleOut);

                        Handles.color = Color.red;
                        float handleSize = HandleUtility.GetHandleSize(point.handleIn) * 0.075f;
                        
                        if (Handles.Button(point.handleIn, Quaternion.identity, handleSize, handleSize, Handles.SphereHandleCap))
                        {
                            roadEditor.SelectHandle(i, RoadEditor.HandleType.InHandle);
                            Repaint();
                        }
                        
                        if (Handles.Button(point.handleOut, Quaternion.identity, handleSize, handleSize, Handles.SphereHandleCap))
                        {
                            roadEditor.SelectHandle(i, RoadEditor.HandleType.OutHandle);
                            Repaint();
                        }
                    }
                }
            }
        }

        private void HandlePointEditing()
        {
            Event e = Event.current;

            if (roadEditor.currentMode == RoadEditor.EditMode.AddPoints)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Undo.RecordObject(roadEditor, "Add Road Point");
                        roadEditor.AddPoint(hit.point);
                        e.Use();
                    }
                    else
                    {
                        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                        float distance;

                        if (groundPlane.Raycast(ray, out distance))
                        {
                            Vector3 point = ray.GetPoint(distance);
                            Undo.RecordObject(roadEditor, "Add Road Point");
                            roadEditor.AddPoint(point);
                            e.Use();
                        }
                    }
                }
            }
            else if (roadEditor.currentMode == RoadEditor.EditMode.EditPoints)
            {
                if (roadEditor.selectedPointIndex >= 0)
                {
                    if (roadEditor.selectedHandleType == RoadEditor.HandleType.None)
                    {
                        // Move the entire point
                        var point = roadEditor.GetPointAt(roadEditor.selectedPointIndex);
                        EditorGUI.BeginChangeCheck();
                        Vector3 newPos = Handles.PositionHandle(point, Quaternion.identity);

                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(roadEditor, "Move Road Point");
                            roadEditor.UpdatePointPosition(roadEditor.selectedPointIndex, newPos);
                            SceneView.RepaintAll();
                        }
                    }
                    else
                    {
                        // Move a handle
                        var bezierPoint = roadEditor.GetBezierPointAt(roadEditor.selectedPointIndex);
                        if (bezierPoint != null)
                        {
                            Vector3 handlePos = roadEditor.selectedHandleType == RoadEditor.HandleType.InHandle 
                                ? bezierPoint.handleIn 
                                : bezierPoint.handleOut;
                            
                            EditorGUI.BeginChangeCheck();
                            Vector3 newPos = Handles.PositionHandle(handlePos, Quaternion.identity);
                            
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(roadEditor, "Move Handle");
                                roadEditor.UpdateControlPoint(roadEditor.selectedPointIndex, roadEditor.selectedHandleType, newPos);
                                SceneView.RepaintAll();
                            }
                        }
                    }
                }
            }
        }
    }
}
