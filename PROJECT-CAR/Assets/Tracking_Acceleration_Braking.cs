using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tracking_Acceleration_Braking : MonoBehaviour
{
    AI m_AIControl; 
    public enum types
    {
        braking,
        accerating
    }
    public types postsForAI;

    [Header("Exiting Corner values")]
    public float m_AIAccerationValueChange;
    public int distanceOffset_AccerationChange;

    [Header("Entering Corner values")]
    public float m_AISlowDownValueChange;
    public int distanceOffset_BrakeChange;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
    

    }

    private void OnTriggerEnter(Collider other)
    {
        m_AIControl = other.gameObject.GetComponentInParent<AI>();
        if (postsForAI == types.braking)
        {
            if (other.gameObject.CompareTag("AI"))
            {
                Debug.Log("tag");

              
                //gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                m_AIControl.acceration_Value = m_AISlowDownValueChange;
                m_AIControl.distanceOffset = distanceOffset_BrakeChange;
               // other.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }

        if (postsForAI == types.accerating)
        {
            if (other.gameObject.CompareTag("AI"))
            {
                Debug.Log("tag");

               // gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                m_AIControl.acceration_Value = m_AIAccerationValueChange;
                m_AIControl.distanceOffset = distanceOffset_AccerationChange;
                // other.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("AI"))
        {
            Debug.Log(" not tagged");

            //gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            //other.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("AI"))
        {
            Debug.Log(" tag stayed");

        }
    }
}
