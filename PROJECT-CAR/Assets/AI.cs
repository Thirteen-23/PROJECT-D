using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float editingRotation;

    Ray frontRay;
    Ray leftRay;
    Ray rightRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float range;
    AI_Controls carAI;
    [SerializeField] GameObject m_AICarBody;
    [SerializeField] GameObject m_AICarBodyDetection;
    [SerializeField] bool leftLocked = false;
    [SerializeField] bool rightLocked = false;
    [SerializeField] float steer_Value;
    [SerializeField] float steering_Angle;
    [SerializeField] float steering_valueLeft, steering_valueRight;
    [SerializeField] float returningToOriginalTurnValue = 0;
    [SerializeField] float adjustRayLeft;
    [SerializeField] float adjustRayRight;
    [SerializeField] float acceration_Value;
    
    //checking waypoints
    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 1)] public float steeringForce;
    public Transform currentWaypoint;



    // Start is called before the first frame update
    void Start()
    {
        carAI = m_AICarBody.GetComponent<AI_Controls>();
        //rb = m_AICarBody.GetComponent<Rigidbody>();
        rb = m_AICarBody.GetComponentInChildren<Rigidbody>();
    }

    void Awake()
    {
        //waypoints 
        waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponent<TrackWayPoints>();
        nodes = waypoints.trackNodes;

    }
    // Update is called once per frame
    void Update()
    {
        carAI.acceration_Value = acceration_Value;
        Sensor();
        CalculateDistanceOfWaypoints();
     

    }

    private void FixedUpdate()
    {
        AISteer();
    }
    private void definingRays()
    {
        frontRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(direction * range));
        leftRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayLeft, 0, 1) * range));
        rightRay = new Ray(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayRight, 0, 1) * range));

    }
    private void Sensor()
    {
        definingRays();
        Debug.DrawRay(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(direction * range));
        Debug.DrawRay(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayLeft, 0, 1) * range));
        Debug.DrawRay(m_AICarBodyDetection.transform.position, m_AICarBodyDetection.transform.TransformDirection(new Vector3(adjustRayRight, 0, 1) * range));

        FrontRaySensor();
        LeftRaySensor();
        RightRaySensor();



    }

    private void LeftRaySensor()
    {
        // raycast Left if comes in contact
        if (Physics.Raycast(leftRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                leftLocked = true;
                Debug.Log("Hit the enivroment in left");
                carAI.steering_Value = steering_valueLeft;

            }

        }
        else
        {
            carAI.steering_Value = 0;
        }
    }
    private void RightRaySensor()
    {
        // raycast Right if comes in contact
        if (Physics.Raycast(rightRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                rightLocked = true;
                Debug.Log("Hit the enivroment in Right");
                carAI.steering_Value = steering_valueRight;

            }

        }
        //else
        //{

        //    steering_valueRight = returningToOriginalTurnValue;
        //    carAI.steering_Value = returningToOriginalTurnValue;
        //    steering_valueRight = -1;

        //}
    }
    private void FrontRaySensor()
    {
        // raycast front if comes in contact
        if (Physics.Raycast(frontRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Platform"))
            {

                Debug.Log("Hit the enivroment in front");


            }
            else
            {

                Debug.Log("left the enivroment front");


            }
        }
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = rb.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                currentWaypoint = nodes[i + distanceOffset];
                distance = currentDistance;

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currentWaypoint.position, 3);
    }

    //private void AIAccerate()
    //{
        
       
    //}
    private void AISteer()
    {
        Vector3 relative = rb.transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;
        carAI.steering_Value = steer_Value; 
        steer_Value = (relative.x / relative.magnitude) * steeringForce; 
    }
}
