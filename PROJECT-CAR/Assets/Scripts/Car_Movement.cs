
using UnityEngine;
using UnityEngine.InputSystem;

public class Car_Movement : MonoBehaviour
{
    //keeping track of how many laps in the race. 
    public int numberOfLaps;
    CarNewInputSystem input;
    enum Terrain
    {
        Tarmac,
        Gravel,
        Dirt
    }
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
    [SerializeField] Terrain terrain;
    [SerializeField] TransmissionTypes transmission;
    [SerializeField] DifferentialTypes drive;
    [SerializeField] Rigidbody bodyOfCar;
    [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];
    [SerializeField] Transform centerMass;

    [SerializeField] float downForceValue;
    float currentBreakForce, handbraking;
    bool resetPosition = false;

    [Header("Speed and Power of the Car")]
    [SerializeField] float horsePower;
    public AnimationCurve enginePower;
    public float maxSpeed;
    private float totalPowerInCar;
    [SerializeField] float currentSpeed;
    // dampening for smoother acceration input for keyboard 
    public float acceration_Value;
    [SerializeField] float AccerationDamping;

    [Header("GearBox System")]
    [SerializeField] public int idleRPM;
    [SerializeField] float maxRPM;
    [SerializeField] float minRPM;
    [SerializeField] int maxRPMForCar;
    public float engineRPM;
    [SerializeField] float finalDriveRatio;
    [SerializeField] float[] gearSpeedBox = new float[0];
    public int gearNum;
    private float m_RPMOfWheels;
    [SerializeField] float smoothTime;
    [SerializeField] Vector2[] keyRPMSet = new Vector2[0];
    [SerializeField] ParticleSystem exhaust_Shift;
    [SerializeField] float[] slip = new float[4];
    public float amountOfSlipToShift;

    [Header("Manual Shift")]
    private bool shiftUp = false;
    private bool shiftDown = false;
    private float shift_Value;
    private float currentShift_Value;

    private Vector3 originalPos;
    private Quaternion rotations;

    [Header("Handbraking")]
    [SerializeField] bool isBraking;
    public bool ifHandBraking;
    WheelFrictionCurve sidewaysFriction, forwardFriction;
    public float handBrakefrictionMulitplier = 2f;
    float handbrakefriction;
    float tempo;
    [SerializeField] float whenNotDrifting;
    [SerializeField] float whenDrifting;
    private float driftFactor;

    [Header("Abilities Value")]
    public bool turnOnAllTerrain = false;
    public float frictionPlusValueForAbility;

    [Header("Handling & Brakes")]
    [SerializeField] float allBrakeForce;
    [SerializeField] float frontBrakeForce;
    [SerializeField] float rearBrakeForce;
    private float steering_Value;
    // make the steering smoother when useing a  keyboard 
    [SerializeField] private float steeringDamping;
    [SerializeField] float smoothTransitionSpeed;
    public float smoothTransitionReleasSpeed; 
    private float brakes_value;
    private float brakeDampening;
    public float releaseSteeringDampening;

    private float turnSpeed;
    [SerializeField] AnimationCurve steeringCurve;

    //drafting values
    Ray draftingRay;
    Vector3 direction = Vector3.forward;
    [SerializeField] float m_RayRange;
    [SerializeField] float draftingMultiplierValue;


   

    // Start is called before the first frame update 
    private void Awake()
    {

        input = new CarNewInputSystem();

    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
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
        CarBraking();
            ResettingCar();
        Shifting();
        SetEngineRPMAndTorque();
        Drafting();
        AdjustTractionForDrifting();
        CheckingforSlip();
        //CheckingDistanceOfWaypoints();
    }

    private void GettingInput()
    {
        isBraking = Input.GetKey(KeyCode.B);
        ifHandBraking = Input.GetKey(KeyCode.Space);
        resetPosition = Input.GetKey(KeyCode.R);
        if (resetPosition == true)
        {
            bodyOfCar.linearVelocity = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitApplication();
        }

    }
    // For Handling, acceration and brake
    private float SmoothTransition(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionSpeed);
    }

    private float SmoothTransitionForRelease(float input, float output)
    {
        return Mathf.Lerp(output, input, Time.deltaTime * smoothTransitionReleasSpeed);
    }


    private void DampeningSystem()
    {
        AccerationDamping = SmoothTransition(acceration_Value, AccerationDamping);
        steeringDamping = SmoothTransition(steering_Value, steeringDamping);
        brakeDampening = SmoothTransition(brakes_value, brakeDampening);
        releaseSteeringDampening = SmoothTransitionForRelease(steering_Value, releaseSteeringDampening); 
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
        currentSpeed = bodyOfCar.linearVelocity.magnitude * 3.6f;
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
                        wheels4[i].motorTorque = totalPowerInCar;
                   
                    //Debug.Log(wheels4[i].motorTorque);
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar * 2F;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = totalPowerInCar * 2F;
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
    }
    private void CarBraking()
    {
        if (brakes_value > 0.7f)
        {
            isBraking = true;
        }

        currentBreakForce = isBraking ? (allBrakeForce * brakeDampening) : Mathf.Lerp(currentSpeed, 0f, 1);
        //handbraking = ifHandBraking ? rearBrakeForce : 0f;

        ApplyBraking();


    }
    private void ApplyBraking()
    {
        for (int i = 0; i < wheels4.Length - 2; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * brakeDampening;
        }
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = currentBreakForce * brakeDampening / 2;
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
        if (context.started)
        {

        }
        else if (context.performed)
        {
            steering_Value = context.ReadValue<Vector2>().x;
        }
        else if (context.canceled)
        {
            steering_Value = 0;
        }
    }

    public void ReleaseSteeringInput(InputAction.CallbackContext context)
    {

        steering_Value = 0;
        //print(steering_Value);
    }

    public void ApplyingThrottleInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
        else if (context.performed)
        {

            acceration_Value = context.ReadValue<float>();

        }
        else if (context.canceled)
        {
            acceration_Value = 0;
        }
    }
    public void ReleaseThrottleInput(InputAction.CallbackContext context)
    {
        acceration_Value = 0;
        //print(acceration_Value + " not accerating");
    }

    public void BrakingInput(InputAction.CallbackContext context)
    {
        if (context.started)
            brakes_value = context.ReadValue<float>();

        else if (context.canceled)
        {
            brakes_value = 0;
        }
    }
    public void ReleaseBrakingInput(InputAction.CallbackContext context)
    {
        
        brakes_value = 0;
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
        handbraking = context.ReadValue<float>();
        if (context.started)
        {
            if (handbraking == 1)
            {
                ifHandBraking = true;
            }
        }
        if (context.performed)
        {
            ifHandBraking = true;
        }
        else if (context.canceled)
        {
            ifHandBraking = false;
            handbraking = 0;
        }
    }
    [Header("Shifting Time Values")]
    public float maxTimeToShiftGear;
    public float timerToShift;
    public float rpmLimiter = 5000;
    private void Shifting()
    {
        float temp = acceration_Value;

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
            WheelHit wheelHit;
            switch (drive)
            {
                case DifferentialTypes.AllWheelDrive:
                    for (int i = 2; i < wheels4.Length; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;
                        if (engineRPM >= maxRPM)
                        {
                            if (slip[i] > amountOfSlipToShift)
                            {
                                /*if(engineRPM > rpmLimiter)
                                {
                                    engineRPM = Mathf.Clamp(engineRPM, 0, rpmLimiter); 
                                }*/
                                return;
                            }
                            else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                            {
                                // changes to shifting 
                                if (timerToShift > 0)
                                {
                                    timerToShift -= Time.deltaTime;

                                }
                                else
                                {

                                    gearNum++;
                                    exhaust_Shift.Play();
                                    timerToShift = maxTimeToShiftGear;
                                }
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
                    break;
                case DifferentialTypes.RearWheelDrive:
                    for (int i = 2; i < wheels4.Length; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;

                        if (engineRPM >= maxRPM)
                        {
                            if (slip[i] > amountOfSlipToShift)
                            {
                                return;
                            }
                            else if (gearNum < gearSpeedBox.Length - 1 && slip[i] < amountOfSlipToShift)
                            {
                                if (timerToShift > 0)
                                {
                                    timerToShift -= Time.deltaTime;
                                    engineRPM = Mathf.Clamp(engineRPM, 0, maxRPM - 500);
                                }
                                else
                                {
                                    gearNum++;
                                    exhaust_Shift.Play();
                                    timerToShift = maxTimeToShiftGear;
                                }
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
                    break;
                case DifferentialTypes.FrontWheelDrive:
                    for (int i = 0; i < wheels4.Length - 2; i++)
                    {
                        wheels4[i].GetGroundHit(out wheelHit);
                        slip[i] = wheelHit.forwardSlip;

                        if (engineRPM >= maxRPM && slip[i] < amountOfSlipToShift)
                        {
                            if (gearNum < gearSpeedBox.Length - 1)
                            {
                                gearNum++;
                                // exhaust_Shift.Play();
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
                    break;
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
        bodyOfCar.AddForce(-transform.up * downForceValue * bodyOfCar.linearVelocity.magnitude);
    }

    private void Drafting()
    {
        draftingRay = new Ray(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(direction * m_RayRange));
        Debug.DrawRay(bodyOfCar.transform.position, bodyOfCar.transform.TransformDirection(direction * m_RayRange));

        if (Physics.Raycast(draftingRay, out RaycastHit hit, m_RayRange))
        {
            if (hit.collider.CompareTag("AI") || hit.collider.CompareTag("Player"))
            {
                Debug.Log("Im behind");
                bodyOfCar.AddForce(bodyOfCar.transform.forward * (1000f * draftingMultiplierValue));

            }

        }
    }

    [Header(" boost values when drifting ")]
    [SerializeField] float boostWhenExitingDrift = 20000f;
    public float findme;
   
    public float minDrag = 0;
    public float maxDrag = 4;
    public float boostWhileDrifting = 25000f;
    [SerializeField] float tt = 1;
    [SerializeField] float f_MaxAmountOfGrip;
    [SerializeField] float f_MinAmountOfGripAtStart;
    [SerializeField] float s_MaxAmountOfGrip;
    [SerializeField] float s_MinAmountOfGripAtStart;
    float driftEndingGrip;
    public bool meBoosting = false;
    public float boostValue = 3000f;
    [Header("drift release values ")]
    public float m_returnToNormalValues;
    public WheelFrictionCurve testSidefriction;

    private void AdjustTractionForDrifting()
    {
        if (ifHandBraking)
        {
            for (int i = 0; i < wheels4.Length; i++)
            {
                wheels4[i].sidewaysFriction = testSidefriction;
                wheels4[i].forwardFriction = forwardFriction;
            }
            testSidefriction.extremumSlip = 5f;
            testSidefriction.asymptoteSlip = 10f;
            testSidefriction.extremumValue = testSidefriction.asymptoteValue = testSidefriction.stiffness = 1;
            tt = 0f;
        }

        #region taking out drifting
        ///// time it takes to go from drive to drift
        //float driftSmoothFactor = 0.7f * Time.deltaTime;
        //if (ifHandBraking && currentSpeed > 40 || currentSpeed > 40 && handbraking > 0)
        //{
        //    bodyOfCar.angularDamping = whenDrifting;
        //    //bodyOfCar.angularDrag = Mathf.Lerp(minDrag, maxDrag, tt * 2f );
        //    //bodyOfCar.angularDrag = Mathf.Clamp(bodyOfCar.angularDrag,minDrag ,maxDrag);
        //    sidewaysFriction = wheels4[0].sidewaysFriction;
        //    forwardFriction = wheels4[0].forwardFriction;

        //    float velocity = 0;

        //    driftEndingGrip = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
        //    Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakefrictionMulitplier, ref velocity, driftSmoothFactor);


        //    for (int i = 0; i < 4; i++)
        //    {

        //        wheels4[i].sidewaysFriction = sidewaysFriction;
        //        wheels4[i].forwardFriction = forwardFriction;
        //    }
        //    sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.2f;

        //    // extra grip for front wheels
        //    for (int i = 0; i < 2; i++)
        //    {
        //        wheels4[i].sidewaysFriction = sidewaysFriction;
        //        wheels4[i].forwardFriction = forwardFriction;

        //    }
        //    // bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * boostInDrifting);

        //    if (wheels4[0].steerAngle > 20 || wheels4[0].steerAngle < -20)
        //    {
        //        bodyOfCar.AddForce(bodyOfCar.transform.forward * boostWhileDrifting);
        //    }
        //    // bodyOfCar.AddRelativeForce(bodyOfCar.transform.forward * steeringCurve.Evaluate(180f));
        //    WheelHit wheelHit;

        //    for (int i = 2; i < wheels4.Length; i++)
        //    {
        //        wheels4[i].GetGroundHit(out wheelHit);
        //        slip[i] = wheelHit.sidewaysSlip /*/ wheels4[i].sidewaysFriction.extremumSlip*/;
        //        if (slip[i] > 0.4f || slip[i] < -0.4f)
        //        {

        //        }
        //    }
        //    tt = 0;
        //}
        //// executed when handbrake is not held
        else
        {
            #endregion

            #region option for drifting
            forwardFriction = wheels4[0].forwardFriction;
            sidewaysFriction = wheels4[0].sidewaysFriction;


            if (tt > 1f)
            {

                Debug.Log("normal friction");
                for (int i = 0; i < 4; i++)
                {
                    wheels4[i].forwardFriction = forwardFriction;
                    wheels4[i].sidewaysFriction = sidewaysFriction;
                }
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, f_MinAmountOfGripAtStart, f_MaxAmountOfGrip);
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, s_MinAmountOfGripAtStart, s_MaxAmountOfGrip);
            }
            else
            {
                tt += Time.deltaTime;

                forwardFriction.extremumValue = forwardFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, f_MinAmountOfGripAtStart, f_MaxAmountOfGrip);
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, s_MinAmountOfGripAtStart, s_MaxAmountOfGrip);



            }
            for (int i = 0; i < 4; i++)
            {
                wheels4[i].forwardFriction = forwardFriction;
                wheels4[i].sidewaysFriction = sidewaysFriction;
            }

            WheelHit wheelHit;
            
            #region SEE IF I CAN THIS OUT
            /*
            for (int i = 2; i < wheels4.Length; i++)
            {
                wheels4[i].GetGroundHit(out wheelHit);
                slip[i] = wheelHit.sidewaysSlip;
                if (slip[i] > 0.4f || slip[i] < -0.4f)
                {
                    tt = 1f;
                    forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
               Mathf.Lerp(driftEndingGrip, Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 2f, 0, 4), tt * 1f);


                    bodyOfCar.AddForce(bodyOfCar.transform.forward * (currentSpeed / 400) * boostWhenExitingDrift);
                    //leftTrail.emitting = true;
                    //rightTrail.emitting = true;


                    for (int j = 0; j < 4; j++)
                    {
                        wheels4[j].forwardFriction = forwardFriction;
                        wheels4[j].sidewaysFriction = sidewaysFriction;
                    }
                }
                else
                {
                    // leftTrail.emitting = false;
                    // rightTrail.emitting = false;

                }
                if (forwardFriction.extremumValue >= Mathf.Clamp((currentSpeed * handBrakefrictionMulitplier / 300) + 1f, minAmountOfGripAtStart, maxAmountOfGrip))
                {
                    // float bodyDrag = bodyOfCar.angularDrag;
                    //bodyDrag = Mathf.Lerp(bodyDrag, whenNotDrifting,  tt);
                    // bodyOfCar.angularDrag = bodyDrag;
                    tt = 1.0f;
                    return;
                }
                
            }
            */
            #endregion
            bodyOfCar.angularDamping = whenNotDrifting;

            #endregion

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


