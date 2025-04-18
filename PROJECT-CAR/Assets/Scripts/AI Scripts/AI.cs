using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class AI : MonoBehaviour
{
   public enum aI_Difficulty
    {
        easy,
        normal,
        hard
    }
    public aI_Difficulty difficultness;
    [SerializeField] Rigidbody rb;
    [Header("Adjust the sensor for AI distance")]
    Ray frontRay;
    Ray leftRay;
    Ray rightRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float range;
    AI_Controls carAI;
    [SerializeField] GameObject m_AICarBody;
    [SerializeField] GameObject m_AICarBodyDetection;
    [SerializeField] float steer_Value;
    [SerializeField] float adjustRayLeft;
    [SerializeField] float adjustRayRight;
    private float forceTurn = 25000f;
    [HideInInspector] public float acceration_Value;
    public float speed_Reader;
    public float speed_Limiter = 200f; 

    //checking waypoints
    [Header("Waypoints system")]
    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float steeringForce;
    public Transform currentWaypoint;
    public int currentWaypointIndex;
    [SerializeField] float waypointApproachThreshold;
    public int numberOfLaps;

    Tracking_Manager_Script valueBeingRead;
    [SerializeField] GameObject bridge;

    // Start is called before the first frame update
    void Start()
    {
        carAI = m_AICarBody.GetComponent<AI_Controls>();
        rb = m_AICarBody.GetComponentInChildren<Rigidbody>();
        bridge = GameObject.Find("Checkpoints");
        valueBeingRead = bridge.GetComponent<Tracking_Manager_Script>();
    }

    void Awake()
    {
        //waypoints 
        //waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponent<TrackWayPoints>();
        nodes = waypoints.trackNodes;

    }
    // Update is called once per frame
    void Update()
    {
        carAI.acceration_Value = acceration_Value;
        speed_Reader = carAI.currentSpeed;
        Sensor();
        //CalculateDistanceOfWaypoints();
        changingDistanceOffset();
        CheckForUpdatedWaypoints();
        AISteer();
      
    }

    private void FixedUpdate()
    {

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
            if (hit.collider.CompareTag("AI"))
            {
                // Debug.Log("Hit the enivroment in left");
                //carAI.acceration_Value = -2f;
                rb.AddForce(rb.transform.right * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(rb.transform.right * forceTurn);
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
            if (hit.collider.CompareTag("AI"))
            {
                //   Debug.Log("Hit the enivroment in Right");
                // carAI.acceration_Value = -2f;
                rb.AddForce(-rb.transform.right * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(-rb.transform.right * forceTurn);
            }
        }
    }
    private void FrontRaySensor()
    {
        // raycast front if comes in contact
        if (Physics.Raycast(frontRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("AI"))
            {

                //Debug.Log("Hit the enivroment in front");
                //carAI.acceration_Value = -2f;
                rb.AddForce(-rb.transform.forward * forceTurn);
            }
            else if (hit.collider.CompareTag("walls"))
            {
                rb.AddForce(-rb.transform.forward * forceTurn);
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

    private void CheckForUpdatedWaypoints() //call this every update
    {
        //These should be defined in the class not as local variables
        // int currentWaypointIndex;
        //float waypointApproachThreshold;
        carAI.maxSpeed = speed_Limiter;
        Vector3 difference = nodes[currentWaypointIndex].transform.position - rb.transform.position;
        if (difference.magnitude < waypointApproachThreshold)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
            ChangeMaxSpeed();
        }

    }
    private void OnDrawGizmos()
    {
      //  Gizmos.DrawWireSphere(currentWaypoint.position, 3);
    }

    //private void AIAccerate()
    //{


    //}
    private void AISteer()
    {
        Vector3 targetPosition = nodes[(currentWaypointIndex + distanceOffset) % nodes.Count].transform.position;
        Vector3 relative = rb.transform.InverseTransformPoint(/*currentWaypoint.transform.position*/targetPosition);
        relative /= relative.magnitude;
        carAI.steering_Value = steer_Value;
        steer_Value = (relative.x / relative.magnitude) * steeringForce;
    }


    private void changingDistanceOffset()
    {
        switch (difficultness)
        {
            case aI_Difficulty.easy:

                acceration_Value = 1f;


                break;
            case aI_Difficulty.normal:

                acceration_Value = 1.2f;

                break;
            case aI_Difficulty.hard:

                acceration_Value = 1.5f;


                break;



        }


        if (speed_Reader > 150 && speed_Reader < 190)
        {
            if (difficultness == aI_Difficulty.easy)
            {
                distanceOffset = 2;
                acceration_Value = 1f;
                waypointApproachThreshold = 17f;
            }
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                acceration_Value = 1.2f;
                waypointApproachThreshold = 17f;
            }
            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 3;
                acceration_Value = 1.5f;
                
            }
        }

        if (speed_Reader > 50 && speed_Reader <150)
        {

            if (difficultness == aI_Difficulty.easy)
            {
                distanceOffset = 2;
                acceration_Value = 1f;
                waypointApproachThreshold = 17f;
            }
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                acceration_Value = 1.2f;
                waypointApproachThreshold = 17f;
            }

            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 2;
                acceration_Value = 1.5f;
                waypointApproachThreshold = 17f; 
            }
            /*distanceOffset = 2;
            acceration_Value = 1;*/
        }
        if (speed_Reader < 200 && speed_Reader > 190)
        {
            if (difficultness == aI_Difficulty.normal)
            {
                distanceOffset = 2;
                waypointApproachThreshold = 17f;
            }
            if (difficultness == aI_Difficulty.hard)
            {
                distanceOffset = 3;
                steeringForce = 0.9f;
                waypointApproachThreshold = 20f;
            }
           
        }

        if (speed_Reader < 50)
        {
            distanceOffset = 1;
            steeringForce = 1f;
            waypointApproachThreshold = 20f;
            if (Physics.Raycast(frontRay, out RaycastHit hit, range))
            {
                waypointApproachThreshold = 50f;
                if (hit.collider.CompareTag("walls"))
                {
                    steeringForce = 1.5f;
                    Debug.Log("Hitting wall");
                    carAI.acceration_Value = -2f;
                    // carAI.steering_Value = -carAI.steering_Value;
                }
                else if (!hit.collider.CompareTag("walls"))
                {

                    //  Debug.Log("left the enivroment front");
                    carAI.acceration_Value = 1.0f;
                    waypointApproachThreshold = 20f;
                    steeringForce = 1f;
                }
            }
        }

    }

    private void ChangeMaxSpeed()
    {
        carAI.maxSpeed = speed_Limiter;
        if (nodes[currentWaypointIndex].gameObject.CompareTag("AccerateNode"))
        {
            speed_Limiter = valueBeingRead.changingSpeedToAccerate; 
        }
        else if(nodes[currentWaypointIndex].gameObject.CompareTag("SlowNode"))
        {
            speed_Limiter = valueBeingRead.changingSpeedToSlowDown;
        }
        else if (nodes[currentWaypointIndex].gameObject.CompareTag("SlowNoder"))
        {
            speed_Limiter = valueBeingRead.changingSpeedToSlowDownMore;
        }
        else
            return;
       
    }
}
