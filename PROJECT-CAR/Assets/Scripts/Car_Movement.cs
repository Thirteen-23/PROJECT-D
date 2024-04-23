
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
    // [SerializeField] private WheelFrictionCurve sRFriction;

    float m_steering; 
  
    float horizontalInput;
    float verticalInput;
    float currentSteerAngle, currentBreakForce, handbraking;
    bool quittingApplication = false;
    bool resetPosition = false;

    [SerializeField]
    private float speedFactor;
    [Header("Speed of the Car")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private Rigidbody bodyOfCar;

    [Header("GearBox System")]
    [SerializeField] private float minEngineRPM;
    [SerializeField] private float maxEngineRPM;
    [SerializeField] private float engineRPM;
    [SerializeField] private int currentGear;
    private int[] gearRatio = new int[6];
    private int theCorrectGear;

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
    [SerializeField] private bool ifHandBraking;


    [Header("Wheel Modifiers")]
    [SerializeField] private float engineTorque;
    //[SerializeField] private float frontEngineTorque;
    // [SerializeField] private float rearEngineTorque;
    [SerializeField] private float breakForce;
    [SerializeField] private float frontBreakForce;
    [SerializeField] private float rearBreakForce;
    [SerializeField] private float maxSteerAngle;
    #region old wheel collder and transform code
    [Header("Wheel Collider and Transform")]

    /* [SerializeField]*/
    private WheelCollider frontRightWheelCollider;
    /* [SerializeField]*/
    private WheelCollider frontLeftWheelCollider;
    /* [SerializeField]*/
    private WheelCollider rearRightWheelCollider;
    /* [SerializeField]*/
    private WheelCollider rearLeftWheelCollider;

    /* [SerializeField]*/
    private Transform frontRightWheelTransform;
    /* [SerializeField]*/
    private Transform frontLeftWheelTransform;
    /* [SerializeField]*/
    private Transform rearRightWheelTransform;
    /* [SerializeField]*/
    private Transform rearLeftWheelTransform;
    #endregion

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
        Charactermove();
        quitApplication();
        ResettingCar();
        BrakesUsed();



        #region old code

        //WheelsUpdating();
        #endregion
    }

    private void GettingInput()
    {

        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = Input.GetAxis("Vertical");

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
                    wheels4[i].motorTorque = verticalInput + m_PlayerMovement.y * (engineTorque / 4);
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = verticalInput + m_PlayerMovement.y * (engineTorque / 2);
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = verticalInput + m_PlayerMovement.y * (engineTorque / 2);
                }
            }

            #endregion

            #region old Driving Force code
            /*frontLeftWheelCollider.motorTorque = verticalInput * (engineTorque + frontEngineTorque) * speedFactor * Time.deltaTime;
              frontRightWheelCollider.motorTorque = verticalInput * (engineTorque + frontEngineTorque) * speedFactor * Time.deltaTime;
              rearLeftWheelCollider.motorTorque = verticalInput * (engineTorque + rearEngineTorque) * speedFactor * Time.deltaTime;
              rearRightWheelCollider.motorTorque = verticalInput * (engineTorque + rearEngineTorque) * speedFactor * Time.deltaTime;*/

            #endregion
        }

        else
        {
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for (int i = 0; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = horizontalInput + m_PlayerMovement.y * 0;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = horizontalInput + m_PlayerMovement.y * 0;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = horizontalInput + m_PlayerMovement.y * 0;
                }
            }

            #region old Driving Force code
            /*engineRPM = maxEngineRPM;
            frontLeftWheelCollider.motorTorque = verticalInput * 0 * Time.deltaTime;
            frontRightWheelCollider.motorTorque = verticalInput * 0 * Time.deltaTime;
            rearLeftWheelCollider.motorTorque = verticalInput * 0 * Time.deltaTime;
            rearRightWheelCollider.motorTorque = verticalInput * 0 * Time.deltaTime;*/
            #endregion
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

        #region old brakng system code
        /*frontLeftWheelCollider.brakeTorque = currentBreakForce;
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        rearLeftWheelCollider.brakeTorque = currentBreakForce;
        rearRightWheelCollider.brakeTorque = currentBreakForce; */
        #endregion


    }

    private void ApplyHandBraking()
    {
        for (int i = 2; i < wheels4.Length; i++)
        {
            wheels4[i].brakeTorque = handbraking;
        }

        #region old rear brake code
        /*
        rearLeftWheelCollider.brakeTorque = handbraking;
        rearRightWheelCollider.brakeTorque = handbraking;
       */
        #endregion
    }
    private void HandlingSteering()
    {
        for (int i = 0; i < wheels4.Length - 2; i++)
        {
            wheels4[i].steerAngle = horizontalInput /*+ m_PlayerMovement.x*/ * maxSteerAngle;
        }

        #region old Steering system
        /*  currentSteerAngle = maxSteerAngle * horizontalInput;
          frontLeftWheelCollider.steerAngle = currentSteerAngle;
          frontRightWheelCollider.steerAngle = currentSteerAngle; */
        #endregion
    }

    #region Old Updating code for updating animation on wheels 
    /* private void WheelsUpdating()
     {
         UpdateOneWheel(frontLeftWheelCollider, frontLeftWheelTransform);
         UpdateOneWheel(frontRightWheelCollider, frontRightWheelTransform);
         UpdateOneWheel(rearLeftWheelCollider, rearLeftWheelTransform);
         UpdateOneWheel(rearRightWheelCollider, rearRightWheelTransform);
     }

     private void UpdateOneWheel(WheelCollider collider, Transform wheelTransform)
     {
         Vector3 pos;
         Quaternion rot;
         collider.GetWorldPose(out pos, out rot);
         wheelTransform.rotation = rot;
         wheelTransform.position = pos;


     }
    */
    #endregion

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

   

    private void EngineRPMSystem()
    {
        if (drive == DifferentialTypes.AllWheelDrive)
        {
            for (int i = 0; i < wheels4.Length; i++)
            {
                engineRPM = wheels4[i].rpm / 4 * gearRatio[currentGear];
            }
        }
        else if (drive == DifferentialTypes.FrontWheelDrive)
        {
            for (int i = 0; i < wheels4.Length - 2; i++)
            {
                engineRPM = wheels4[i].rpm / 2;
            }
        }

        else if (drive == DifferentialTypes.RearWheelDrive)
        {
            for (int i = 2; i < wheels4.Length; i++)
            {
                engineRPM = wheels4[i].rpm / 2;
            }
        }
        //engineRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2 * gearRatio[currentGear];
        //ShiftGears();
    }

    //private void ShiftGears()
    //{

    //    if (engineRPM >= maxEngineRPM)
    //    {
    //        for(int i = 0; i<gearRatio.Length; i++)
    //        {
    //            if(rearLeftWheelCollider.rpm * gearRatio[i] < maxEngineRPM)
    //            {
    //                theCorrectGear = i;
    //            }
    //            break;
    //        }
    //        currentGear = theCorrectGear; 
    //    }

    //    if(engineRPM <= minEngineRPM)
    //    {
    //        for(int j = gearRatio.Length; j >=0; j--) 
    //        {
    //            if(rearLeftWheelCollider.rpm * gearRatio[j] > minEngineRPM)
    //            {
    //                theCorrectGear = j;
    //                break;
    //            }
    //        }

    //    }
    //    currentGear = theCorrectGear;
    //}

    public void Charactermove()
    {
        // context.ReadValue<Vector2>();
        // print(context);
        m_PlayerMovement = m_Movement.ReadValue<Vector2>();
        print(m_PlayerMovement); 

    }
    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    public void Steering(InputAction.CallbackContext context)
    {
        m_steering = context.ReadValue<float>();
    }
    
}
