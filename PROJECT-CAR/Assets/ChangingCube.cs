using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ChangingCube : MonoBehaviour
{

    Rigidbody rb;
    Renderer mats;

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
                    body.AddForce(ray.direction *500f);
            }
           
        }

    }
   
}
    
