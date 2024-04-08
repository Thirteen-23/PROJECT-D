using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPLOSION : MonoBehaviour
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

            rb = GetComponent<Rigidbody>();
            //Debug.Log(victim + " Hit");


            rb.AddForce(-gameObject.transform.forward * pushPower);



        }
    }
}
