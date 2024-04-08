using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class moving_platform : MonoBehaviour
{
    [SerializeField] public bool OnPlatform = false;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed;
    private int _currentWaypoint;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {

        MovingThePlatform();




    }
    private void MovingThePlatform()
    {
        rb.MovePosition(Vector3.MoveTowards(transform.position, waypoints[_currentWaypoint].transform.position, (speed * Time.deltaTime)));
        if (transform.position == waypoints[_currentWaypoint].transform.position)
        {
            _currentWaypoint++;
            _currentWaypoint %= waypoints.Count;

            //_currentWaypoint = (_currentWaypoint + 1) % waypoints.Count;
        }


    }

   

}

