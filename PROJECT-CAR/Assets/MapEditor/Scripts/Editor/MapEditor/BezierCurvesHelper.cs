using UnityEngine;
using UnityEditor;

namespace AK.MapEditorTools
{
    public static class BezierCurvesHelper
    {
        public static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t, uu = u * u;
            float uuu = uu * u, ttt = tt * t;

            return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        }

        public static void DrawBezierCurve(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, Color color, int segments = 20, float lineWidth = 2f)
        {
            Handles.color = color;
            Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, color, null, lineWidth);
        }

        public static Vector3[] CalculateSmoothCurveControlPoints(Vector3[] points, bool isClosed)
        {
            if (points == null || points.Length < 2)
                return new Vector3[0];

            int pointCount = points.Length;
            Vector3[] controlPoints = new Vector3[pointCount * 2];
            
            if (pointCount == 2)
            {
                Vector3 direction = (points[1] - points[0]).normalized;
                float distance = Vector3.Distance(points[0], points[1]) * 0.25f;
                
                controlPoints[0] = points[0] + direction * distance;
                controlPoints[1] = points[1] - direction * distance;
                return controlPoints;
            }
            
            Vector3[] firstDerivatives = CalculateFirstDerivatives(points, isClosed);
            
            for (int i = 0; i < pointCount; i++)
            {
                float scale = 0.3f;
                if (i < pointCount - 1)
                    scale *= Vector3.Distance(points[i], points[i+1]);
                else if (isClosed)
                    scale *= Vector3.Distance(points[i], points[0]);
                
                controlPoints[i * 2] = points[i] + firstDerivatives[i] * scale;
                controlPoints[i * 2 + 1] = points[i] - firstDerivatives[i] * scale;
            }
            
            return controlPoints;
        }
        
        private static Vector3[] CalculateFirstDerivatives(Vector3[] points, bool isClosed)
        {
            int pointCount = points.Length;
            Vector3[] derivatives = new Vector3[pointCount];
            
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 prev, next;
                
                if (i == 0)
                {
                    prev = isClosed ? points[pointCount - 1] : points[i];
                    next = points[i + 1];
                }
                else if (i == pointCount - 1)
                {
                    prev = points[i - 1];
                    next = isClosed ? points[0] : points[i];
                }
                else
                {
                    prev = points[i - 1];
                    next = points[i + 1];
                }
                
                derivatives[i] = (next - prev).normalized;
            }
            
            return derivatives;
        }
    }
}
