using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Speedo : MonoBehaviour
{

     
    [SerializeField] private Rigidbody rb;
    public float maxSpeed = 0.0f;
    private float currentGear = 0f;
    [SerializeField]Car_Movement car;
    private float m_rpmIndicator;
    [SerializeField] Animation curve; 

    [Header("UI")]
    public  TextMeshProUGUI speedLabel;
    public TextMeshProUGUI currentGearLabel;
    public Slider m_RPM;
    public TextMeshProUGUI RpmNum;
    [SerializeField] private int gear;
    [SerializeField] private float speed; 
    // Start is called before the first frame update
    void Start()
    {
        rb = rb.GetComponent<Rigidbody>();
        car = car.GetComponent<Car_Movement>();

    }

    // Update is called once per frame
    void Update()
    {
        speedo();
        GearChangeFunction();
        RPMBar();
        ExactNumOfRPM();
    }

    private void speedo()
    {
        // 3.6f conversion to KM/H

        speed = rb.velocity.magnitude * 3.6f;

        if (speedLabel != null)
        {
            speedLabel.text = ((int)speed) + "km/h";

        }
    }

    private void GearChangeFunction()
    {
        currentGear = car.gearNum + 1;
     if(currentGearLabel != null)
        {
            currentGearLabel.text = ((int)currentGear + "");
        }

        
    }

    private void RPMBar()
    {
        float timeTime;
        m_rpmIndicator = car.engineRPM;
        Keyframe lastkey = car.enginePower[car.enginePower.length - 1];
        timeTime = lastkey.time;
        m_RPM.maxValue = timeTime;
        m_RPM.minValue = car.idleRPM;
        m_RPM.value = m_rpmIndicator;

    }

    private void ExactNumOfRPM()
    {
        m_RPM.value = m_rpmIndicator;

        if (currentGearLabel != null)
        {
            RpmNum.text = ((int)m_rpmIndicator/100 + "00RPM");
        }


    }
}
