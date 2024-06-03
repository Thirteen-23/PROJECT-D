using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Tracking_Manager_Script : MonoBehaviour
{
    CheckFlagPoint m_Sensors; 
    public List<Transform> checkpointNodes = new List<Transform>();
    public List<GameObject> listOfCars = new List<GameObject>();
    public List<GameObject> tester = new List<GameObject>();
    [SerializeField] int[] checkpointIndex = new int[0];
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
        for(int i = 1; i < paths.Length; i++)
        {
            checkpointNodes.Add(paths[i]);
            
        }
        foreach(Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            tester.Add(child.gameObject);
        }
        CheckpointPass();
        m_Sensors = GetComponentInChildren<CheckFlagPoint>(); 
    }

    // Update is called once per frame
    void Update()
    {
        listOfCars = m_Sensors.listOfCars; 
    }

    private void CheckpointPass()
    {
       //for(int i = 1; i < tester.Capacity ; i++)
       // {
       //     tester[i].AddComponent<CheckFlagPoint>();
       // }

       //foreach(Transform child in gameObject.transform)
       // {
       //     child.AddComponent<CheckFlagPoint>();
       // }
        int childCount = gameObject.transform.childCount;
        for(int x = 0; x < childCount; x++)
        {
            gameObject.transform.GetChild(x).AddComponent<CheckFlagPoint>();
        }

    }
  

}
