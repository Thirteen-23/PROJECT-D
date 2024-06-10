using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fastest_Man_Alive : MonoBehaviour
{
    CharacterController controller;
    private bool playerIsGrounded;
    [SerializeField] float speed;
    [SerializeField] float gravity; 
    Animator animate;
    Vector3 playerVelocity;


    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    public Transform currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        animate = GetComponent<Animator>();
        
        animate.GetBool("Move");
        
    }

    // Update is called once per frame
    void Update()
    {
        playerIsGrounded = controller.isGrounded; 
        if(playerIsGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f; 
        }

        playerVelocity.y += gravity * Time.deltaTime;

        CalculateDistanceOfWaypoints();
        AISteer();
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = controller.transform.position;
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

    private void AISteer()
    {
        Vector3 relative = controller.transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;
       // controller.transform.forward
        //carAI.steering_Value = steer_Value;
        //steer_Value = (relative.x / relative.magnitude) * steeringForce;
    }
}
