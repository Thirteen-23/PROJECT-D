using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    [SerializeField] 
    Collider myCollision;
    Rigidbody[] rigidbodies;
    bool isRagdoll = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        ToggleRagDoll(true);
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if(!isRagdoll && collision.gameObject.tag == "Player")
        {
            ToggleRagDoll(false); 
        }
    }

    private void ToggleRagDoll(bool isAnimating)
    {
        isRagdoll = !isAnimating;
        myCollision.enabled = isAnimating;
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            ragdollBone.isKinematic = isAnimating;

        }
    }
    private void Awake()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
