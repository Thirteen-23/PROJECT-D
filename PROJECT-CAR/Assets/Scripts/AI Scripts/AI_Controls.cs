
using UnityEngine;
using UnityEngine.InputSystem;

public class AI_Controls : MonoBehaviour
{
    enum DifferentialTypes
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive

    }
    enum TransmissionTypes
    {
        Automatic,
        Manual
    }
    [SerializeField] TransmissionTypes transmission;
    [SerializeField] DifferentialTypes drive;
    [SerializeField] Rigidbody bodyOfCar;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    [SerializeField] Transform centerMass;
    //[SerializeField] AnimationCurve gearRatio;
    [SerializeField] float downForceValue;
    float currentBreakForce, handbraking;

    [Header("Speed and Power of the Car")]
    [SerializeField] public AnimationCurve enginePower;
    public float maxSpeed;
    [SerializeField] float totalPowerInCar;
    public float currentSpeed;
    // dampening for smoother acceration input for keyboard 
    public float acceration_Value;
    [SerializeField] float AccerationDamping;

    [Header("GearBox System")]
    [SerializeField] public int idleRPM;
    [SerializeField] float maxRPM;
    [SerializeField] float minRPM;
    [SerializeField] int maxRPMForCar;
    [SerializeField] public float engineRPM;
    [SerializeField] float finalDriveRatio;
    [SerializeField] float[] gearSpeedBox = new float[0];
    [SerializeField] public int gearNum;
    private float m_RPMOfWheels;
    [SerializeField] float smoothTime;
    [SerializeField] Vector2[] keyRPMSet = new Vector2[0];

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] bool isBreaking;
    public bool ifHandBraking;



    [Header("Handling & Brakes")]
    [SerializeField] float allBrakeForce;
    [SerializeField] float frontBrakeForce;
    [SerializeField] float rearBrakeForce;
    public float steering_Value;
    // make the steering smoother when useing a  keyboard 
    [SerializeField] float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    [SerializeField] float brakes_value;
    [SerializeField] float brakeDampening;
    private float turnSpeed;
    public AnimationCurve steeringCurve;
    // Start is called before the first frame update 
    private void Awake()
    {

    }

    void Start()
    {
        bodyOfCar.centerOfMass = centerMass.localPosition;
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        //carNewInputSystem = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        HandlingMotor();
        HandlingSteering();
        AnimatedWheels();
        DampeningSystem();
        calculatingEnginePower();
        Shifting();
        SetEngineRPMAndTorque();
        ApplyingDownForce();
    }

    private float SmoothTransition(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionSpeed);
    }

    private void DampeningSystem()
    {
        AccerationDamping = SmoothTransition(acceration_Value, AccerationDamping);
        steeringDamping = SmoothTransition(steering_Value, steeringDamping);
        brakeDampening = SmoothTransition(brakes_value, brakeDampening);
    }

    private void HandlingMotor()
    {
        // calculation of kilometers / hour
        currentSpeed = bodyOfCar.velocity.magnitude * 3.6f;
        EngineRPMSystem();
        // code for restricting the car to max speed set. 
        if (currentSpeed < maxSpeed)
        {
            #region New Driving system 
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    // wheels torque equal to engine Rpm * gearbox * final drive ratio and input from player
                    wheels4[i].motorTorque = totalPowerInCar / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar / 2;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar / 2;
                }
            }

            #endregion


        }

        else
        {
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = acceration_Value * 0;
                }
            }

        }
        if (brakes_value > 0.7f)
        {
            isBreaking = true;
        }
        currentBreakForce = isBreaking ? (allBrakeForce * brakeDampening) : 0f;
        //currentBreakForce =  allBrakeForce * brakeDampening;
        handbraking = ifHandBraking ? rearBrakeForce : 0f;
        ApplyBreaking();
        ApplyHandBraking();
    }

    private void ApplyBreaking()
    {
        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * Time.deltaTime;
        }




    }

    private void ApplyHandBraking()
    {
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = handbraking;
        }
    }


    private void HandlingSteering()
    {
        for (int i = 0; i < wheels4.Length - 2; i++)
        {
            turnSpeed = steeringDamping * steeringCurve.Evaluate(currentSpeed);
            wheels4[i].steerAngle = turnSpeed;
        }

    }

    void AnimatedWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotations = Quaternion.identity;

        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].GetWorldPose(out wheelPosition, out wheelRotations);
            wheelmeshes[i].transform.position = wheelPosition;
            wheelmeshes[i].transform.rotation = wheelRotations;
        }

    }

    private void calculatingEnginePower()
    {
        EngineRPMSystem();

        //totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * m_PlayerAcceration;
        totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * AccerationDamping;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, idleRPM + (Mathf.Abs(m_RPMOfWheels) * finalDriveRatio * (gearSpeedBox[gearNum])), ref velocity, smoothTime);
    }


    private void EngineRPMSystem()
    {
        float sum = 0;
        int rR = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels4[i].rpm;
            rR++;
        }
        m_RPMOfWheels = (rR != 0) ? sum / rR : 0;

    }
    private void Shifting()
    {
       
        if (transmission == TransmissionTypes.Automatic)
        {
            if (engineRPM > maxRPM)
            {
                if (gearNum < gearSpeedBox.Length - 1)
                {
                    gearNum++;
                }
            }
            if (engineRPM < minRPM)
            {
                if (gearNum > 0)
                {
                    gearNum--;
                }
                else
                {
                    gearNum = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (gearNum > 0)
                {
                    gearNum--;
                }

            }
        }
    }

    private void SetEngineRPMAndTorque()
    {
        for (int i = 0; i < keyRPMSet.Length; i++)
        {
            enginePower.AddKey(keyRPMSet[i].x, keyRPMSet[i].y);

        }

    }
    private void ApplyingDownForce()
    {
        bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);
    }

}