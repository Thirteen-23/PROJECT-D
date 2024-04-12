using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{

   enum DifferentialTypes
        {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive

        }
    [SerializeField] private DifferentialTypes drive; 
   [SerializeField] WheelCollider[] wheels4 = new WheelCollider[4];
    [SerializeField] GameObject[] wheelmeshes = new GameObject[4];

    [SerializeField] private WheelFrictionCurve sRFriction;



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
    [SerializeField] private Rigidbody rb;

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
    [SerializeField]
    public float engineTorque;
    public float frontEngineTorque;
    public float rearEngineTorque;
    public float breakForce;
    public float frontBreakForce;
    public float rearBreakForce;
    public float maxSteerAngle;
    #region old wheel collder and transform code
    [Header("Wheel Collider and Transform")]

    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;

    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        originalPos = gameObject.transform.position;
        rotations = gameObject.transform.rotation;
        brakeTrailRenLeft = brakeTrailLeft.GetComponent<TrailRenderer>();
        brakeTrailRenRight = brakeTrailRight.GetComponent<TrailRenderer>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GettingInput();
        HandlingMotor();
        HandlingSteering();

        AnimatedWheels();

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
        currentSpeed = rb.velocity.magnitude * 3.36f;
        EngineRPMSystem();
        // code for restricting the car to max speed set. 
        if (currentSpeed < maxSpeed)
        {
            #region New Driving system
            if (drive == DifferentialTypes.AllWheelDrive)
            {
                for(int i = 0; i <wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = verticalInput * (engineTorque / 4);
                }
            }
            else if( drive == DifferentialTypes.RearWheelDrive)
            {
                for(int i= 2; i< wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = verticalInput * (engineTorque / 2);
                }
            }
            else if(drive == DifferentialTypes.FrontWheelDrive)
           
            {
                for(int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = verticalInput * (engineTorque / 2); 
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
                    wheels4[i].motorTorque = verticalInput * 0;
                }
            }
            else if (drive == DifferentialTypes.RearWheelDrive)
            {
                for (int i = 2; i < wheels4.Length; i++)
                {
                    wheels4[i].motorTorque = verticalInput * 0;
                }
            }
            else if (drive == DifferentialTypes.FrontWheelDrive)

            {
                for (int i = 0; i < wheels4.Length - 2; i++)
                {
                    wheels4[i].motorTorque = verticalInput * 0;
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
        for(int i = 0; i< wheels4.Length; i++)
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
        for(int i = 2; i < wheels4.Length; i++)
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
        for(int i = 0; i < wheels4.Length-2; i++)
        {
            wheels4[i].steerAngle = horizontalInput * maxSteerAngle;
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

        for(int i = 0; i< wheels4.Length; i++)
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

    private void CarSetUp()
    {
        foreach( WheelCollider Wheel in wheels4 )
        {
           
        }

    }

    private void EngineRPMSystem()
    {
        if(drive == DifferentialTypes.AllWheelDrive)
        {
            for(int i = 0; i< wheels4.Length; i++)
            {
                engineRPM = wheels4[i].rpm / 4; 
            }
        }
        else if (drive == DifferentialTypes.FrontWheelDrive)
        {
            for (int i = 0; i < wheels4.Length -2; i++)
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
        engineRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2 * gearRatio[currentGear];
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
}
