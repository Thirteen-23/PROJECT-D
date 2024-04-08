using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blow_Back1 : MonoBehaviour
{
    [SerializeField] private float pushPower;
    [SerializeField]  GameObject boxes;



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

            rb.AddForce((-rb.transform.forward + -rb.transform.right + rb.transform.up) * pushPower);



        }
    }

    private void OnCollisionExit(UnityEngine.Collision collision)
    {
        //Instantiate(boxes);
        

    }
}
