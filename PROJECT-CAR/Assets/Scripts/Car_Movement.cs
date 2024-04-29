
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Movement : MonoBehaviour
{
    private Vector2 m_PlayerMovement = Vector2.zero;
    private InputAction m_Movement;
    private PlayerInput carNewInputSystem;


    enum DifferentialTypes
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive

    }
    [SerializeField] private DifferentialTypes drive;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    [SerializeField] AnimationCurve enginePower;
    [SerializeField] AnimationCurve gearRatio;

    float currentSteerAngle, currentBreakForce, handbraking;
    bool quittingApplication = false;
    bool resetPosition = false;

    [SerializeField]
    private float speedFactor;
    [Header("Speed of the Car")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Rigidbody bodyOfCar;
    [SerializeField] private float totalPowerInCar;

    [Header("GearBox System")]
    //[SerializeField] private float minEngineRPM;
    //[SerializeField] private float maxEngineRPM;
    [SerializeField] private float engineRPM;
    [SerializeField] private float finalDriveRatio;
    [SerializeField] private float[] gearSpeedBox = new float[0];
    [SerializeField] private int gearNum;
    [SerializeField] private float m_RPMOfWheels;
    [SerializeField] private float smoothTime;

    [Header("Brake Trail Settings")]
    [SerializeField] GameObject brakeTrailLeft;
    [SerializeField] GameObject brakeTrailRight;
    TrailRenderer brakeTrailRenLeft, brakeTrailRenRight;
    [SerializeField] private float changeTrailsizeBeginning;
    [SerializeField] private float changeTrailsizeEnd;
    [SerializeField] private float trailTime;

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] private bool isBreaking;
    // [SerializeField] private bool isMotorOn;
    public bool ifHandBraking;


    [Header("Wheel Modifiers")]
    [SerializeField] private float engineTorque;
    //[SerializeField] private float frontEngineTorque;
    // [SerializeField] private float rearEngineTorque;
    [SerializeField] private float breakForce;
    [SerializeField] private float frontBreakForce;
    [SerializeField] private float rearBreakForce;
    [SerializeField] private float maxSteerAngle;

    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        brakeTrailRenLeft = brakeTrailLeft.GetComponent<TrailRenderer>();
        brakeTrailRenRight = brakeTrailRight.GetComponent<TrailRenderer>();
        carNewInputSystem = GetComponent<PlayerInput>();
        m_Movement = carNewInputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        GettingInput();
        HandlingMotor();
        HandlingSteering();
        AnimatedWheels();
        calculatingEnginePower();
        Charactermove();
        quitApplication();
        ResettingCar();
        BrakesUsed();
        Shifting();


    }

    private void GettingInput()
    {
        isBreaking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        quittingApplication = Input.GetKeyDown(KeyCode.Escape);
        resetPosition = Input.GetKey(KeyCode.R);

    }

    private void quitApplication()
    {
        if (quittingApplication == true)
        {
            Application.Quit();
        }
    }

    private void ResettingCar()
    {
        if (resetPosition == true)
        {
            gameObject.transform.position = originalPos;
            gameObject.transform.rotation = rotations;

        }
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
                    wheels4[i].motorTorque =  /*m_PlayerMovement.y*/ totalPowerInCar / 4;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque =  m_PlayerMovement.y * (totalPowerInCar / 2);
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = m_PlayerMovement.y * (totalPowerInCar / 2);
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
                    wheels4[i].motorTorque = m_PlayerMovement.y * 0;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = m_PlayerMovement.y * 0;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = m_PlayerMovement.y * 0;
                }
            }

        }

        currentBreakForce = isBreaking ? breakForce : 0f;
        handbraking = ifHandBraking ? rearBreakForce : 0f;
        ApplyBreaking();
        ApplyHandBraking();
    }

    private void ApplyBreaking()
    {
        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce;
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
            wheels4[i].steerAngle = m_PlayerMovement.x * maxSteerAngle;
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


    private void BrakesUsed()
    {
        AnimationCurve curve1 = new AnimationCurve();
        AnimationCurve curve2 = new AnimationCurve();
        //sRFriction = new WheelFrictionCurve();

        float trailWidth = 1.0f;
        if (ifHandBraking == true)
        {
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);
            //sRFriction.asymptoteValue = 0.50f;


        }
        else if (ifHandBraking == false)
        {


            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);
            // sRFriction.asymptoteValue = 0.75f;
        }
        //rearRightWheelCollider.sidewaysFriction = sRFriction;
        brakeTrailRenLeft.time = trailTime;
        brakeTrailRenRight.time = trailTime;
        brakeTrailRenLeft.emitting = ifHandBraking;
        brakeTrailRenLeft.widthCurve = curve1;
        brakeTrailRenLeft.widthMultiplier = trailWidth;
        brakeTrailRenRight.emitting = ifHandBraking;
        brakeTrailRenRight.widthCurve = curve1;
        brakeTrailRenRight.widthMultiplier = trailWidth;
    }

    private void calculatingEnginePower()
    {
        EngineRPMSystem();
        // WheelsRPM();

        totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * m_PlayerMovement.y;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 800 + (Mathf.Abs(m_RPMOfWheels)* finalDriveRatio * (gearSpeedBox[gearNum])), ref velocity, smoothTime);

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

    // another way to calculate RPM. 
    private void WheelsRPM()
    {
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels4[i].rpm;
            R++;

        }
        m_RPMOfWheels = (R != 0) ? sum / R : 0;
    }

    public void Charactermove()
    {
        m_PlayerMovement = m_Movement.ReadValue<Vector2>();
        print(m_PlayerMovement);

    }

    private void Shifting()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            if (gearNum < 4)
            {
                gearNum++;
            }
            else if (gearNum == 4)
            {
                gearNum = 4;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (gearNum > 0)
            {
                gearNum--;
            }
            else if(gearNum == 0)
            {
                gearNum = 0;
            }
        }


    }


}
