
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
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
    enum TransmissionTypes
    {
        Automatic,
        Manual
    }
    [SerializeField] private TransmissionTypes transmission;
    [SerializeField] private DifferentialTypes drive;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    
    //[SerializeField] AnimationCurve gearRatio;

    float currentBreakForce, handbraking;
    bool quittingApplication = false;
    bool resetPosition = false;

    [Header("Speed of the Car")]
    [SerializeField] public AnimationCurve enginePower;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Rigidbody bodyOfCar;
    [SerializeField] private float totalPowerInCar;

    [Header("GearBox System")]
    [SerializeField] int gearChangingNum;
    [SerializeField] public int idleRPM;
    [SerializeField] private float maxRPM;
    [SerializeField] private float minRPM;
    [SerializeField] private int maxRPMForCar;
    [SerializeField] public float engineRPM;
    [SerializeField] private float finalDriveRatio;
    [SerializeField] private float[] gearSpeedBox = new float[0];
    [SerializeField] public int gearNum;
    private float m_RPMOfWheels;
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2[] keyRPMSet = new Vector2[0]; 

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] private bool isBreaking;
    public bool ifHandBraking;


    [Header("Wheel Modifiers")]
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
        Shifting();
        SetEngineRPMAndTorque();

    }

    private void GettingInput()
    {
        isBreaking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        //quittingApplication = Input.GetKeyDown(KeyCode.Escape);
        resetPosition = Input.GetKey(KeyCode.R);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitApplication();
        }

    }

    private void quitApplication()
    {
        Application.Quit();
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

    private void calculatingEnginePower()
    {
        EngineRPMSystem();

        totalPowerInCar = enginePower.Evaluate(engineRPM) * gearSpeedBox[gearNum] * m_PlayerMovement.y;
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

    public void Charactermove()
    {
        m_PlayerMovement = m_Movement.ReadValue<Vector2>();
        //print(m_PlayerMovement);

    }

    private void Shifting()
    {
        if (transmission == TransmissionTypes.Manual)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log(gearNum);
                Debug.Log(gearSpeedBox[gearNum]);
                if (gearNum < gearSpeedBox.Length-1)
                {
                    
                    gearNum++;
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
        }
    }

    private void SetEngineRPMAndTorque()
    {
        for(int i  = 0; i<keyRPMSet.Length; i++)
        {
            enginePower.AddKey(keyRPMSet[i].x, keyRPMSet[i].y);
            
        }
       
    }
}



