using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float pushPower;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            Rigidbody rb;

            Debug.Log("Box has being hit!");

            GameObject victim = collision.gameObject;
            //Debug.Log(victim + " Hit");
            rb = victim.GetComponent<Rigidbody>();

            rb.AddForce(rb.transform.forward * pushPower);



        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody rb;

            Debug.Log("Speed Boost!");

            GameObject victim = other.gameObject;
            //Debug.Log(victim + " Hit");
            rb = victim.GetComponentInParent<Rigidbody>();

            rb.AddForce(rb.transform.forward * pushPower);



        }


    }

}
