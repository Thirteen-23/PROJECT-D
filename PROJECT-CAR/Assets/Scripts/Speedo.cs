
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Speedo : MonoBehaviour
{

     
    [SerializeField] Rigidbody rb;
    [SerializeField] Car_Movement car;

    [Header("RPM UI")]
    [SerializeField] Image RPMNeedle;
    [SerializeField] float minRPMAngle;
    [SerializeField] float maxRPMAngle;
    public Slider m_RPM;
   // public TextMeshProUGUI RpmNum;
    private float m_rpmIndicator;
    private float final_RPMIndicator;

    [Header("Speedo UI")]
    public TextMeshProUGUI speedLabel;
    public float maxSpeed = 0.0f;
    [SerializeField] float speed;
    private float finalSpeed;
    const float speedoSnap = 2f; 

    [Header("Gear UI")]
    public TextMeshProUGUI currentGearLabel;
    private float currentGear = 0f;
    [SerializeField] int gear;

    
   
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
        finalSpeed = Mathf.Lerp(finalSpeed, speed, speedoSnap * Time.deltaTime); 
        if (speedLabel != null)
        {
            speedLabel.text = ((int)finalSpeed) + "km/h";
            //speedoNeedle.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedAngle, maxSpeedAngle, speed / maxSpeed));
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
        final_RPMIndicator = Mathf.Lerp(final_RPMIndicator, m_rpmIndicator, speedoSnap * Time.deltaTime);
        RPMNeedle.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minRPMAngle, maxRPMAngle, final_RPMIndicator / timeTime));
    }

    private void ExactNumOfRPM()
    {
        m_RPM.value = m_rpmIndicator;
        final_RPMIndicator = Mathf.Lerp(final_RPMIndicator, m_RPM.value, speedoSnap * Time.deltaTime);
        


    }
}
