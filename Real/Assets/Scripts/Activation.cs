using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Activation : MonoBehaviour
{
    private Animator anim = null;
    public List<Rigidbody> rigidbodies = new List<Rigidbody>();

    public bool RagDollActive
    {
        get { return !anim.enabled; }
        set { anim.enabled = !value;
            foreach (Rigidbody r in rigidbodies)
                r.isKinematic = !value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        foreach (Rigidbody r in rigidbodies)
            r.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("hit");
          
            
        }
     
    }

}
