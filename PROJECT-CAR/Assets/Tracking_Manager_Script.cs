using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tracking_Manager_Script : MonoBehaviour
{
    public List<Transform> checkpointNodes = new List<Transform>();
    [SerializeField] float checkpointIndex;
    [SerializeField] GameObject[] carsInGame;
    public int carPosition; 

    void Start()
    {
        //children = new Transform[transform.childCount];
        //foreach (Transform child in transform)
        //{
        //    children[i++] = child.transform;

        //}
        Transform[] paths = GetComponentsInChildren<Transform>();
        checkpointNodes = new List<Transform>(); 
        for(int i = 0; i < paths.Length; i++)
        {
            checkpointNodes.Add(paths[i]); 
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void CheckpointPass()
    {
        
    }

}
