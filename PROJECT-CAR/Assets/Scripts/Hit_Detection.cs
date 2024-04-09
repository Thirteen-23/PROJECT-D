using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Hit_Detection : MonoBehaviour
{
    public Vector3 hitDirection;
    public float pushPower;  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("hit");
        hitDirection = hit.point - transform.position;
        
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;

        }

        if (hit.moveDirection.y < -0.3)
        {
            return;

        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;

    }
}
