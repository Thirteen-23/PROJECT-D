using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Speedo : MonoBehaviour
{

     
    [SerializeField] private Rigidbody rb;
    public float maxSpeed = 0.0f; 

    [Header("UI")]
    public  TextMeshProUGUI speedLabel;
    [SerializeField] private float speed; 
    // Start is called before the first frame update
    void Start()
    {
        rb = rb.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 3.6f conversion to KM/H

        speed = rb.velocity.magnitude * 3.6f;

        if (speedLabel != null)
        {
            speedLabel.text = ((int)speed) + "km/h";

        }
    }
}
