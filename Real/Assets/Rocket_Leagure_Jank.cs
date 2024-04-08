using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Rocket_Leagure_Jank : MonoBehaviour
{
    Rigidbody rb;
    public bool booostActi = false;
    [SerializeField] private float PushPower;
    [SerializeField] private float coolDown;
    [SerializeField] private float coolDownTimer; 
    [SerializeField] private float nextBoostTime; 
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
       

    }

    void UpdateInputs()
    {
        booostActi = Input.GetKey(KeyCode.Q);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateInputs();


        if(coolDownTimer > 0 )
        {
            coolDownTimer -= Time.deltaTime;
        }

        if(coolDownTimer < 0)
        {
            rb.constraints = RigidbodyConstraints.None;
            coolDownTimer = 0;

        }

        
        if (Time.time > nextBoostTime && coolDownTimer == 0)
        {
            if (booostActi == true)
            {
                Boost();
                rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                nextBoostTime = Time.time + coolDown;
                coolDownTimer = coolDown; 
            }
        }
    }

    void Boost()
    {
        rb.AddForce(rb.transform.up * PushPower);
       
    }
}
