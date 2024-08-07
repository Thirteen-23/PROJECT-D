
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Movement : MonoBehaviour
{
    CarNewInputSystem input;
    public float numberOfLaps;
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
    float currentBreakForce;
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
    [SerializeField] ParticleSystem exhaust_Shift;

    [Header("Manual Shift")]
    [SerializeField] bool shiftUp = false;
    [SerializeField] bool shiftDown = false;
    [SerializeField] float shift_Value;
    [SerializeField] float currentShift_Value;

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking / Drfting")]
    [SerializeField] bool isBraking;
    public bool ifHandBraking;
    WheelFrictionCurve sidewaysFriction, forwardFriction;
    public float handBrakefrictionMulitplier = 2f;
    float handbrakefriction;
    float tempo;
    [SerializeField] float[] slip = new float[4];
    [SerializeField] float whenNotDrifting;
    [SerializeField] float whenDrifting;

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
    [SerializeField] float handbraking;
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

    }
    private void OnDisable()
    {
        input.Disable();

    }
    void Start()
    {
        // nodes = waypoints.trackNodes;
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        bodyOfCar.centerOfMass = centerMass.localPosition;
        exhaust_Shift = GetComponentInChildren<ParticleSystem>();
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
        ApplyingDownForce();
        ResettingCar();
        Shifting();
        SetEngineRPMAndTorque();
        AdjustTractionForDrifting();
        CheckingforSlip();
        //CheckingDistanceOfWaypoints();
    }

    private void GettingInput()
    {
        //isBreaking = Input.GetKey(KeyCode.B);
       // ifHandBraking = Input.GetKey(KeyCode.Space);
        //resetPosition = Input.GetKey(KeyCode.R);

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
        if (brakes_value > 0.7f)
        {
            isBraking = true;
        }
        currentBreakForce = isBraking ? (allBrakeForce * brakeDampening) : 0f;
        //handbraking = ifHandBraking ? rearBrakeForce : 0f;
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
        //for (int i = 2; i < wheels4.Length; i++)
        //{
        //    wheels4[i].brakeTorque = handbraking;

        //}
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
        if(context.started)
        {
            brakes_value = context.ReadValue<float>();
        }

        if(context.canceled)
        {
            brakes_value = 0;
            isBraking = false;
        }
       
    }
    public void ShiftingUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (shift_Value <= gearSpeedBox.Length - 1 && shift_Value < gearSpeedBox.Length - 1)
            {    //Debug.Log("started");
                shiftUp = true;
                shift_Value++;
            }
            else if (shift_Value == gearSpeedBox.Length - 1)
            {
                return;
            }
        }
        else if (context.performed)
        {
            // Debug.Log("performed");
            shiftUp = false;
        }
        else if (context.canceled)
        {
            // Debug.Log("cancelled");
        }
    }

    public void ShiftingDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (shift_Value <= gearSpeedBox.Length - 1 && shift_Value > 0)
            {
                shiftDown = true;
                shift_Value--;
            }
            else if (shift_Value == 0)
            {
                shift_Value = 0;
            }
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
    public void Handbraking(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            handbraking = context.ReadValue<float>();
            if(handbraking ==1)
            {
                ifHandBraking = true; 
            }
        }
       
       else if (context.canceled)
        {
            handbraking = 0;
            ifHandBraking = false;
        }
    }

    private void Shifting()
    {
        if (transmission == TransmissionTypes.Manual)
        {
            Mathf.Clamp(shift_Value, 0, gearSpeedBox.Length - 1);
            if ((shiftUp == true && shift_Value > currentShift_Value) && (gearNum < gearSpeedBox.Length - 1))
            {
                //Debug.Log(gearNum);
                //Debug.Log(gearSpeedBox[gearNum]);

                gearNum++;
                currentShift_Value = shift_Value;
                exhaust_Shift.Play();
                if (shift_Value == gearSpeedBox.Length - 1)
                {
                    shift_Value = gearSpeedBox.Length - 1;
                }
            }
            else if ((shiftDown == true && shift_Value < currentShift_Value) && (gearNum > 0))
            {

                gearNum--;

                currentShift_Value = shift_Value;

            }
        }
        if (transmission == TransmissionTypes.Automatic)
        {
            if (engineRPM >= maxRPM)
            {
                if (gearNum < gearSpeedBox.Length - 1)
                {
                    gearNum++;
                }


            }
            if (engineRPM <= minRPM)
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
        for (int i = 0; i < keyRPMSet.Length; i++)
        {
            enginePower.AddKey(keyRPMSet[i].x, keyRPMSet[i].y);

        }

    }
    private void ApplyingDownForce()
    {
        bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.velocity.magnitude);
    }

    private void AdjustingTraction()
    {
        if (!ifHandBraking)
        {
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = (currentSpeed * handBrakefrictionMulitplier / 300) + 1;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = (currentSpeed * handBrakefrictionMulitplier / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels4[i].forwardFriction = forwardFriction;
                wheels4[i].sidewaysFriction = sidewaysFriction;
            }

        }

        else if (ifHandBraking)
        {
            sidewaysFriction = wheels4[0].sidewaysFriction;
            forwardFriction = wheels4[0].forwardFriction;

            float velocity = 0;

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.SmoothDamp(sidewaysFriction.asymptoteValue, handbrakefriction, ref velocity, 0.05f * Time.deltaTime);
            forwardFriction.extremumValue = forwardFriction.asymptoteValue = Mathf.SmoothDamp(forwardFriction.asymptoteValue, handbrakefriction, ref velocity, 0.05f * Time.deltaTime);

            for (int i = 2; i < 4; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = 1.5f;
            forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.5f;

            for (int i = 0; i < 2; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;

            }



        }

    }
    private void checkWheelSpin()
    {
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels4[i].GetGroundHit(out wheelHit);
            if (wheelHit.sidewaysSlip < 0)
            {

                tempo = (1 + -steering_Value) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakefrictionMulitplier);
                if (tempo < 0.5)
                {
                    tempo = 0.5f;
                }
            }
            else if (wheelHit.sidewaysSlip > 0)
            {
                tempo = (1 + steering_Value) * Mathf.Abs(wheelHit.sidewaysSlip * handBrakefrictionMulitplier);
                if (tempo < 0.5f)
                {
                    tempo = 0.5f;
                }
            }

            else if (wheelHit.sidewaysSlip > 0.99f || wheelHit.sidewaysSlip < -0.99f)
            {
                float velocity = 0;
                handbrakefriction = Mathf.SmoothDamp(handbrakefriction, tempo * 3, ref velocity, 0.1f * Time.deltaTime);
            }
            else
            {
                handbrakefriction = tempo;
            }
        }
    }

    private float driftFactor;

    private void AdjustTractionForDrifting()
    {
        // time it takes to go from drive to drift

        float driftSmoothFactor = 0.7f * Time.deltaTime;
        if (ifHandBraking || handbraking == 1)
        {
            bodyOfCar.angularDrag = whenDrifting;
           
            sidewaysFriction = wheels4[0].sidewaysFriction;
            forwardFriction = wheels4[0].forwardFriction;

            float velocity = 0;

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
            Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakefrictionMulitplier, ref velocity, driftSmoothFactor);


            for (int i = 0; i < 4; i++)
            {

                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;
            }
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1f;

            // extra grip for front wheels
            for (int i = 0; i < 2; i++)
            {
                wheels4[i].sidewaysFriction = sidewaysFriction;
                wheels4[i].forwardFriction = forwardFriction;

            }

            bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * 10000);
        }
        // executed when handbrake is held
        else
        {
            bodyOfCar.angularDrag =whenNotDrifting;
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.Lerp((forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue), (currentSpeed * handBrakefrictionMulitplier / 300) + 1, Time.deltaTime /2 );

            for (int i = 0; i < 4; i++)
            {
                wheels4[i].forwardFriction = forwardFriction;
                wheels4[i].sidewaysFriction = sidewaysFriction;
            }

        }

        // check the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels4[i].GetGroundHit(out wheelHit);

            if (wheelHit.sidewaysSlip < 0)
            {
                driftFactor = (1 + -steering_Value) * Mathf.Abs(wheelHit.sidewaysSlip);

            }
            if (wheelHit.sidewaysSlip > 0)
            {
                driftFactor = (1 + steering_Value) * Mathf.Abs(wheelHit.sidewaysSlip);

            }
        }
    }


    private void CheckingforSlip()
    {
        WheelHit wheelHit;

        for (int i = 0; i < wheels4.Length; i++)
        {
            wheels4[i].GetGroundHit(out wheelHit);
            slip[i] = wheelHit.sidewaysSlip;
        }
    }

}


