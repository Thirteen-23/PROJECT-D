using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AK.MapEditorTools
{
    [ExecuteInEditMode]
    public class RoadEditor : MonoBehaviour
    {
        public enum EditMode { Disabled, AddPoints, EditPoints }
        public enum HandleType { None, Anchor, InHandle, OutHandle }

        [HideInInspector] public EditMode currentMode = EditMode.Disabled;
        [HideInInspector] public int selectedPointIndex = -1;
        [HideInInspector] public bool showControlPoints = true;
        [HideInInspector] public int curveResolution = 10;
        [HideInInspector] public HandleType selectedHandleType = HandleType.None;

        [HideInInspector] public RoadGenerator roadGenerator;

        [SerializeField, HideInInspector] private List<Vector3> savedPoints = new List<Vector3>();
        [SerializeField, HideInInspector] private List<BezierPoint> bezierPoints = new List<BezierPoint>();

        [System.Serializable]
        public class BezierPoint
        {
            public Vector3 position;
            public Vector3 handleIn;
            public Vector3 handleOut;

            public BezierPoint(Vector3 pos)
            {
                position = pos;
                handleIn = pos + new Vector3(-1, 0, 0);
                handleOut = pos + new Vector3(1, 0, 0);
            }

            public BezierPoint(Vector3 pos, Vector3 inHandle, Vector3 outHandle)
            {
                position = pos;
                handleIn = inHandle;
                handleOut = outHandle;
            }
        }

        public void UpdateRoadGenerator()
        {
            if (roadGenerator != null)
                roadGenerator.RegenerateMesh();
        }

        public void AddPoint(Vector3 point)
        {
            if (savedPoints == null)
            {
                savedPoints = new List<Vector3>();
                bezierPoints = new List<BezierPoint>();
            }

            savedPoints.Add(point);
            BezierPoint newPoint;

            if (bezierPoints.Count > 0)
            {
                int lastIndex = bezierPoints.Count - 1;
                BezierPoint prevPoint = bezierPoints[lastIndex];
                Vector3 direction = (point - prevPoint.position).normalized;
                float segmentLength = Vector3.Distance(point, prevPoint.position);
                float handleLength = Mathf.Min(segmentLength * 0.4f, 2.0f);

                if (bezierPoints.Count > 1)
                {
                    BezierPoint prevPrevPoint = bezierPoints[lastIndex - 1];
                    Vector3 prevDirection = (prevPoint.position - prevPrevPoint.position).normalized;
                    Vector3 avgDirection = ((prevDirection + direction) * 0.5f).normalized;

                    prevPoint.handleOut = prevPoint.position + avgDirection * handleLength;
                    Vector3 inDirection = -avgDirection;
                    
                    newPoint = new BezierPoint(
                        point,
                        point + inDirection * handleLength,
                        point + direction * handleLength
                    );

                    bezierPoints[lastIndex] = prevPoint;

                    if (lastIndex > 0)
                    {
                        prevPrevPoint.handleOut = prevPrevPoint.position + prevDirection * handleLength;
                        bezierPoints[lastIndex - 1] = prevPrevPoint;
                    }
                }
                else
                {
                    prevPoint.handleOut = prevPoint.position + direction * handleLength;
                    bezierPoints[lastIndex] = prevPoint;

                    newPoint = new BezierPoint(
                        point,
                        point - direction * handleLength,
                        point + direction * handleLength
                    );
                }
            }
            else
            {
                newPoint = new BezierPoint(point);
            }

            bezierPoints.Add(newPoint);
            if (currentMode == EditMode.EditPoints) SelectPoint(GetPointCount() - 1);
            UpdateRoadGenerator();
        }

        public void UpdatePointPosition(int index, Vector3 newPosition)
        {
            if (index >= 0 && index < savedPoints.Count)
            {
                Vector3 offset = newPosition - savedPoints[index];
                savedPoints[index] = newPosition;

                BezierPoint bezierPoint = bezierPoints[index];
                bezierPoint.position = newPosition;
                bezierPoint.handleIn += offset;
                bezierPoint.handleOut += offset;
            }
            UpdateRoadGenerator();
        }

        public void UpdateControlPoint(int pointIndex, HandleType handleType, Vector3 newPosition)
        {
            if (pointIndex < 0 || pointIndex >= bezierPoints.Count) return;

            BezierPoint point = bezierPoints[pointIndex];
            switch (handleType)
            {
                case HandleType.Anchor:
                    Vector3 offset = newPosition - point.position;
                    savedPoints[pointIndex] = newPosition;
                    point.position = newPosition;
                    point.handleIn += offset;
                    point.handleOut += offset;
                    break;
                case HandleType.InHandle: point.handleIn = newPosition; break;
                case HandleType.OutHandle: point.handleOut = newPosition; break;
            }
            bezierPoints[pointIndex] = point;
            UpdateRoadGenerator();
        }

        public void SelectPoint(int index) => selectedPointIndex = (index >= -1 && index < savedPoints.Count) ? index : -1;

        public void SelectHandle(int pointIndex, HandleType handleType)
        {
            selectedPointIndex = pointIndex;
            selectedHandleType = handleType;
        }

        public List<Vector3> GetSavedPoints() => new List<Vector3>(savedPoints);
        public List<BezierPoint> GetBezierPoints() => bezierPoints;
        public int GetPointCount() => savedPoints.Count;
        public Vector3 GetPointAt(int index) => (index >= 0 && index < savedPoints.Count) ? savedPoints[index] : Vector3.zero;
        public BezierPoint GetBezierPointAt(int index) => (index >= 0 && index < bezierPoints.Count) ? bezierPoints[index] : null;

        public void ClearPoints()
        {
            savedPoints.Clear();
            bezierPoints.Clear();
            selectedPointIndex = -1;
            selectedHandleType = HandleType.None;
            UpdateRoadGenerator();
        }

        public List<Vector3> GetBezierCurvePoints()
        {
            List<Vector3> curvePoints = new List<Vector3>();
            
            if (bezierPoints.Count < 2)
            {
                foreach (var point in bezierPoints)
                    curvePoints.Add(point.position);
                return curvePoints;
            }

            curvePoints.Add(bezierPoints[0].position);
            for (int i = 0; i < bezierPoints.Count - 1; i++)
            {
                BezierPoint p0 = bezierPoints[i];
                BezierPoint p1 = bezierPoints[i + 1];

                for (int j = 1; j <= curveResolution; j++)
                {
                    float t = j / (float)curveResolution;
                    curvePoints.Add(CalculateBezierPoint(p0.position, p0.handleOut, p1.handleIn, p1.position, t));
                }
            }
            return curvePoints;
        }

        private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1 - t;
            float uu = u * u, tt = t * t;
            float uuu = uu * u, ttt = tt * t;

            Vector3 point = uuu * p0;
            point += 3 * uu * t * p1;
            point += 3 * u * tt * p2;
            point += ttt * p3;

            return point;
        }

        public void SyncPointsWithBezierPoints()
        {
            if (savedPoints != null && savedPoints.Count > 0 &&
                (bezierPoints == null || bezierPoints.Count != savedPoints.Count))
            {
                bezierPoints = new List<BezierPoint>(savedPoints.Count);
                foreach (var point in savedPoints)
                    bezierPoints.Add(new BezierPoint(point));
            }
            else if (bezierPoints != null && bezierPoints.Count > 0 &&
                    (savedPoints == null || savedPoints.Count != bezierPoints.Count))
            {
                savedPoints = new List<Vector3>(bezierPoints.Count);
                foreach (var bezierPoint in bezierPoints)
                    savedPoints.Add(bezierPoint.position);
            }
        }
    }
}
