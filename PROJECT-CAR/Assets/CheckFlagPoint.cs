using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFlagPoint : MonoBehaviour
{
    public List<GameObject> listOfCars = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "Player")
        {
            Debug.Log("has passed");
            listOfCars.Add(other.gameObject);
        }
        if (other.tag == "AI")
        {
            Debug.Log(" AI has passed");
            listOfCars.Add(other.gameObject);
        }
    }
}
