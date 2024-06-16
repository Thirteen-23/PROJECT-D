using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fastest_Man_Alive : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] private bool playerIsGrounded = false;
    [SerializeField] float speed =0;
    [SerializeField] float gravity; 
    Animator animate;
    Vector3 playerVelocity;
    Vector3 moveForward;
    public Button run;
    public Button idle; 

    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    public Transform currentWaypoint;
    public int currentWaypointIndex;
    [SerializeField] float waypointApproachThreshold;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        animate = gameObject.GetComponent<Animator>();
        
        animate.GetBool("Towards");
        
        
        
    }

    private void Awake()
    {
        nodes = waypoints.trackNodes;
    }
    // Update is called once per frame
    void Update()
    {
        playerIsGrounded = controller.isGrounded; 
        if(playerIsGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            Debug.Log(controller.isGrounded);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        CalculateDistanceOfWaypoints();
        
        //if(speed > 0)
        //{
        //    animate.SetBool("Towards",true);
        //}
        //else
        //{
        //    animate.SetBool("Towards", false);
        //}
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 difference = nodes[currentWaypointIndex].transform.position - controller.transform.position;
        MoveTowards();
        if (difference.magnitude < waypointApproachThreshold)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
           
        }
    }

    private void MoveTowards()
    {
        Vector3 destination = nodes[currentWaypointIndex].transform.position;
        Vector3 newPos = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        transform.position = newPos;

        transform.forward = (destination - transform.position) * Time.deltaTime;


        float distance = Vector3.Distance(transform.position, destination);
        if(distance <= 1)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
        }
       
    }
    public void RunOnclick()
    {
        speed = 20; 
        animate.SetBool("Towards", true);
    }

    public void IdleOnclick()
    {
        speed = 0;
        animate.SetBool("Towards", false);
    }
}
