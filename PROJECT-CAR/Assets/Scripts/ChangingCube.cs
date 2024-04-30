using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ChangingCube : MonoBehaviour
{

    Rigidbody rb;
   [SerializeField] GameObject clone;
    Renderer mats;
   [SerializeField] GameObject woo;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        mats = gameObject.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        onClick();
    }

    void onClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                Rigidbody body = raycastHit.collider.GetComponent<Rigidbody>();
                if (body)
                {
                    body.AddForce(ray.direction * 500f);

                    mats.material.SetColor("_RimColor", Color.blue);
                   
                   Instantiate(clone, woo.transform.position, woo.transform.rotation); 
                    
                }
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                Rigidbody body = raycastHit.collider.GetComponent<Rigidbody>();
                if (body)
                {
                    body.AddForce(ray.direction * -500f);

                    mats.material.SetColor("_RimColor", Color.red);

                }

            }
        }
    }
}
    
