using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float editingRotation; 

    Ray frontRay;
    Vector3 leftRotate;
    Quaternion left_rotate; 
    Ray leftRay;
    Ray rightRay;
    Vector3 direction = Vector3.forward;
    Vector3 direction_Left; 
    [SerializeField] float range;
    LineRenderer line; 
    AI_Controls carAI;
    [SerializeField] GameObject m_AICarBody;
    [SerializeField] bool leftLocked = false;
    [SerializeField] bool rightLocked = false;
    [SerializeField] float steering_valueLeft , steering_valueRight; 
    

    // Start is called before the first frame update
    void Start()
    {
        carAI = m_AICarBody.GetComponentInParent<AI_Controls>();
        rb = m_AICarBody.GetComponent<Rigidbody>();
    }

    void Awake()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        Sensor();
       
    }

    private void definingRays()
    {
        frontRay = new Ray(transform.position, transform.TransformDirection(direction * range));
        leftRay = new Ray(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1) * range));
        rightRay = new Ray(transform.position, transform.TransformDirection(new Vector3(1, 0, 1) * range));

    }
    private void Sensor()
    {
        definingRays();
        Debug.DrawRay(transform.position, transform.TransformDirection(direction * range));
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1.5f, 0, 1) * range));
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1.5f, 0, 1) * range));

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
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            //Rigidbody _rb;
            //_rb = gameObject.GetComponent<Rigidbody>();

            //_rb.AddForce(0, 100, 0);

        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        if (collision.transform.CompareTag("Platform"))
        {
            //    Rigidbody _rb;
            //    _rb = gameObject.GetComponent<Rigidbody>();

            //    _rb.AddForce(0, 0, 0);
        }
    }
}
