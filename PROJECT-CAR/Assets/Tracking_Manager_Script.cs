using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Tracking_Manager_Script : MonoBehaviour
{

    public Tracking_Acceleration_Braking[] inputManage;
    // public List<Transform> checkpoints = new List<Transform>();
    public GameObject[] children;

    private int i;
    [SerializeField] float speeder;
    [SerializeField] int m_AccelChange;
    [SerializeField] int m_SlowDownChange;
    float geto { get; set; }
    [SerializeField] int m_SpeedUpDistanceOffsetChange;
    [SerializeField] int m_SpeedDownDistanceOffsetChange;

    // Start is called before the first frame update
    void Start()
    {
        children = new GameObject[transform.childCount];
        foreach (Transform child in transform)
        {
            children[i++] = child.gameObject;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < children.Length + 1; i++)
        {
            children[i].GetComponent<Tracking_Acceleration_Braking>();
            

        }
        //m_SlowDownChange = inputManage.distanceOffset_AccerationChange;
    }


}
