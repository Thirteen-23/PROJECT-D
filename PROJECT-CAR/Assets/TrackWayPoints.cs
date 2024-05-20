
using System.Collections.Generic;
using UnityEngine;

public class TrackWayPoints : MonoBehaviour
{
    public Color wayPointColour;
    [Range(0, 1)] public float sphereRadius; 
    public List<Transform> trackNodes = new List<Transform>();

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    // Start is called before the first frame update

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = wayPointColour;

        Transform[] path = GetComponentsInChildren<Transform>();

        trackNodes = new List<Transform>();
        for (int i = 1; i < path.Length; i++)
        {
            trackNodes.Add(path[i]);

        }

        for (int i = 0; i < trackNodes.Count; i++)
        {
            Vector3 currentPoint = trackNodes[i].position;
            Vector3 previousPoint = Vector3.zero;

            if (i != 0)
            { previousPoint = trackNodes[i - 1].position; }

            else if (i == 0)
            {
                previousPoint = trackNodes[trackNodes.Count - 1].position;
            }
            Debug.DrawLine(previousPoint, currentPoint);
            Gizmos.DrawLine(previousPoint, currentPoint);
            Gizmos.DrawSphere(currentPoint, sphereRadius);

        }

        int Wrap( int value, int arraySize)
        {

            return value % arraySize;

            /*return ( value % arraySize)
            return (value % arraySize + arraySize) % arraySize;
            */
        }
    }
   
}
