using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    Ray frontRay;
    Vector3 direction = Vector3.forward;
    Vector3 direction_Left = Vector3.left; 
    [SerializeField] float range; 

    Car_Movement carAI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Sensor();
    }

    private void Sensor()
    {
        frontRay = new Ray(transform.position, transform.TransformDirection(direction * range));
        Debug.DrawRay(new Vector3(transform.position.x, 1 ,transform.position.z), transform.TransformDirection(direction * range));
        if (Physics.Raycast(frontRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                Debug.Log("Hit the enivroment");


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
