using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AK.MapEditorTools
{
    public class VisualizationHelper
    {
        public static void DrawPathInScene(
            RoadEditor roadEditor, 
            int selectedPointIndex, 
            Color normalPointColor,
            Color selectedPointColor,
            Color curveColor,
            float pointSize,
            bool isClosedPath,
            float curveWidth = 2f)
        {
            var points = roadEditor.GetSavedPoints();
            if (points == null || points.Count == 0) return;
                
            // Draw points
            for (int i = 0; i < points.Count; i++)
            {
                Color pointColor = (i == selectedPointIndex) ? selectedPointColor : normalPointColor;
                float currentPointSize = (i == selectedPointIndex) ? pointSize * 1.5f : pointSize;
                DrawPoint(points[i], pointColor, currentPointSize);
            }
            
            // Draw path
            if (points.Count >= 2)
                DrawBezierPath(roadEditor, curveColor, isClosedPath, curveWidth);
            else if (points.Count == 2)
            {
                Handles.color = curveColor;
                Handles.DrawAAPolyLine(curveWidth, points[0], points[1]);
            }
        }
        
        public static void DrawPoint(Vector3 position, Color color, float size)
        {
            Handles.color = color;
            Handles.SphereHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
        }

        public static void DrawBezierHandles(
            RoadEditor.BezierPoint bezierPoint, 
            Color handleLineColor,
            Color handlePointColor,
            float handleLineWidth = 1f,
            float handlePointSize = 0.2f)
        {
            Handles.color = handleLineColor;
            Handles.DrawAAPolyLine(handleLineWidth, bezierPoint.position, bezierPoint.handleIn);
            Handles.DrawAAPolyLine(handleLineWidth, bezierPoint.position, bezierPoint.handleOut);
            
            Handles.color = handlePointColor;
            Handles.SphereHandleCap(0, bezierPoint.handleIn, Quaternion.identity, handlePointSize, EventType.Repaint);
            Handles.SphereHandleCap(0, bezierPoint.handleOut, Quaternion.identity, handlePointSize, EventType.Repaint);
        }

        public static void DrawBezierPath(RoadEditor roadEditor, Color curveColor, bool isClosedPath, float curveWidth = 2f)
        {
            var bezierPoints = roadEditor.GetBezierPoints();
            if (bezierPoints.Count < 2) return;
            
            Handles.color = curveColor;
            int segments = roadEditor.curveResolution;
            
            // Draw segments
            for (int i = 0; i < bezierPoints.Count - 1; i++)
                DrawBezierSegment(bezierPoints[i], bezierPoints[i + 1], curveColor, segments, curveWidth);
            
            // Close path if needed
            if (isClosedPath && bezierPoints.Count > 2)
                DrawBezierSegment(bezierPoints[bezierPoints.Count - 1], bezierPoints[0], curveColor, segments, curveWidth);
        }
        
        private static void DrawBezierSegment(RoadEditor.BezierPoint p0, RoadEditor.BezierPoint p1, Color curveColor, int segments, float curveWidth)
        {
            BezierCurvesHelper.DrawBezierCurve(
                p0.position, 
                p0.handleOut, 
                p1.handleIn, 
                p1.position, 
                curveColor, 
                segments,
                curveWidth);
        }
    }
}
