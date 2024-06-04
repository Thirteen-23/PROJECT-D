
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Movement : MonoBehaviour
{
    //keeping track of how many laps in the race. 
    public int numberOfLaps; 
    CarNewInputSystem input;

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

    float currentBreakForce, handbraking;
    bool resetPosition = false;

    [Header("Speed and Power of the Car")]
    [SerializeField] float horsePower;
    [SerializeField] public AnimationCurve enginePower;
    [SerializeField] float maxSpeed;
    [SerializeField] float totalPowerInCar;
    [SerializeField] float currentSpeed;
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

    [Header("Manual Shift")]
    [SerializeField] bool shiftUp = false;
    [SerializeField] bool shiftDown = false; 
    [SerializeField] float shift_Value;
    [SerializeField] float currentShift_Value;

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] bool isBreaking;
    public bool ifHandBraking;



    [Header("Handling & Brakes")]
    [SerializeField] float allBrakeForce;
    [SerializeField] float frontBrakeForce;
    [SerializeField] float rearBrakeForce;
    [SerializeField] float steering_Value;
    // make the steering smoother when useing a  keyboard 
    [SerializeField] float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    [SerializeField] float brakes_value;
    [SerializeField] float brakeDampening;

    private float turnSpeed;
    [SerializeField] AnimationCurve steeringCurve;

    //handling Waypoints
    [Header("WayPoints Setup for Position")]
    public TrackWayPoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    public Transform currentWaypoint;
    public int currentWaypointIndex;
    [SerializeField] float waypointApproachThreshold;

    // Start is called before the first frame update 
    private void Awake()
    {
        
        input = new CarNewInputSystem();
       
    }

    private void OnEnable()
    {
        input.Enable();
        input.Movement.Acceration.performed += ApplyingThrottleInput;
        input.Movement.Acceration.canceled += ReleaseThrottleInput;
        input.Movement.Steering.performed += ApplySteeringInput;
        input.Movement.Steering.canceled += ReleaseSteeringInput;
        input.Movement.braking.performed += BrakingInput;
        input.Movement.braking.canceled += ReleaseBrakingInput;

    }
    private void OnDisable()
    {
        input.Disable();

    }
    void Start()
    {
        nodes = waypoints.trackNodes;
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        bodyOfCar.centerOfMass = centerMass.localPosition;
        //carNewInputSystem = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        GettingInput();
        HandlingMotor();
        HandlingSteering();
        AnimatedWheels();
        DampeningSystem();
        calculatingEnginePower();
        quitApplication();
        ResettingCar();
        Shifting();
        SetEngineRPMAndTorque();
        CheckingDistanceOfWaypoints();
    }

    private void GettingInput()
    {
        isBreaking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        resetPosition = Input.GetKey(KeyCode.R);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitApplication();
        }

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
        if (brakes_value >0.7f)
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
        horsePower = (totalPowerInCar * engineRPM) / 5252;
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

    public void ApplySteeringInput(InputAction.CallbackContext context)
    {

        steering_Value = context.ReadValue<float>();
        //print(steering_Value);
    }

    public void ReleaseSteeringInput(InputAction.CallbackContext context)
    {

        steering_Value = 0;
        //print(steering_Value);
    }

    public void ApplyingThrottleInput(InputAction.CallbackContext context)
    {
        acceration_Value = context.ReadValue<float>();

        //print(acceration_Value + "accerating");
    }

    public void ReleaseThrottleInput(InputAction.CallbackContext context)
    {
        acceration_Value = 0;
        //print(acceration_Value + " not accerating");
    }

    public void BrakingInput(InputAction.CallbackContext context)
    {
        brakes_value = context.ReadValue<float>();
    }
    public void ReleaseBrakingInput(InputAction.CallbackContext context)
    {
        brakes_value = 0;
    }
    public void ShiftingUp(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            //Debug.Log("started");
            shiftUp = true;
            shift_Value++;
        }
      else if(context.performed)
        {
           // Debug.Log("performed");
            shiftUp = false; 
        }
        else if( context.canceled)
        {
           // Debug.Log("cancelled");
        }
    }
  
    public void ShiftingDown(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            shiftDown = true;
            shift_Value--; 
        }
        else if (context.performed)
        {
           // Debug.Log("performed");
            shiftDown = false;
        }
        else if (context.canceled)
        {
           // Debug.Log("cancelled");
        }
    }


    private void Shifting()
    {
        if (transmission == TransmissionTypes.Manual)
        {
            if ((shiftUp == true && shift_Value > currentShift_Value) && (gearNum < gearSpeedBox.Length - 1))
            {
                //Debug.Log(gearNum);
                //Debug.Log(gearSpeedBox[gearNum]);
               
                    gearNum++;
                    currentShift_Value = shift_Value; 

            }
            else if ((shiftDown == true && shift_Value < currentShift_Value) && (gearNum > 0))
            {
               
                    gearNum--;
                    shift_Value = currentShift_Value; 

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

    private void CheckingDistanceOfWaypoints()
    {
        Vector3 difference = nodes[currentWaypointIndex].transform.position - bodyOfCar.transform.position;
        if (difference.magnitude < waypointApproachThreshold)
        {
            currentWaypointIndex++;
            currentWaypointIndex %= nodes.Count;
        }
    }
    #region Old Code not used
    /*public void ReadingHandlingInput()
    {
        steering_Value = m_Movement.ReadValue<float>();
        print(steering_Value);
    }

    private void ReadingAccerationInput()
    {
        acceration_Value += m_Acceration.ReadValue<float>() * 0.10f;
        acceration_Value = Mathf.Clamp(acceration_Value, -1, 1);
        print(acceration_Value);
    }
    */
    #endregion
}


