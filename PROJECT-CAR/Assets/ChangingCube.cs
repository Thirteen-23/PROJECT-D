using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    CurrentClickedGameObject(raycastHit.transform.gameObject);
                }
            }
            Debug.Log("have been pressed");
            rb.AddForce(rb.transform.up * 200.0f);
        }

    }
    public void CurrentClickedGameObject(GameObject gameObject)
    {
        if (gameObject.tag == "something")
        {

        }
    }
}
    
