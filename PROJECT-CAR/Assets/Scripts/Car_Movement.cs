using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{
    float horizontalInput;
    float verticalInput;
    float currentSteerAngle, currentBreakForce, handbraking;
    bool quittingApplication = false;
    bool resetPosition = false;

    [SerializeField]
    private float speedFactor;

    [SerializeField] GameObject brakeTrailLeft, brakeTrailRight;
    TrailRenderer brakeTrailRenLeft, brakeTrailRenRight;
    [SerializeField] private float changeTrailsizeBeginning;
    [SerializeField] private float changeTrailsizeEnd;
    [SerializeField] private float trailTime;
    private Vector3 originalPos;
    private Quaternion rotations;    
    [SerializeField] private bool isBreaking;
   // [SerializeField] private bool isMotorOn;
    [SerializeField] private bool ifHandBraking;

    [Header("Wheel Modifiers")]
    [SerializeField]
    public float motorForce;
    public float frontMotorForce;
    public float rearMotorForce;
    public float breakForce;
    public float frontBreakForce;
    public float rearBreakForce;
    public float maxSteerAngle;

    [Header("Wheel Collider and Transform")]
    
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;


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
        WheelsUpdating();
        quitApplication();
        ResettingCar();
        BrakesUsed();


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
        if(quittingApplication == true)
        {
            Application.Quit();
        }
    }

    private void ResettingCar()
    {
        if(resetPosition == true)
        {
            gameObject.transform.position = originalPos;
            gameObject.transform.rotation = rotations;
        }
    }
    private void HandlingMotor()
    {

        frontLeftWheelCollider.motorTorque = verticalInput * (motorForce + frontMotorForce) * speedFactor * Time.deltaTime;
        frontRightWheelCollider.motorTorque = verticalInput * (motorForce + frontMotorForce) * speedFactor * Time.deltaTime;
        rearLeftWheelCollider.motorTorque = verticalInput * (motorForce + rearMotorForce) * speedFactor * Time.deltaTime;
        rearRightWheelCollider.motorTorque = verticalInput * (motorForce + rearMotorForce) * speedFactor * Time.deltaTime;
        currentBreakForce = isBreaking ? breakForce : 0f;
        handbraking = ifHandBraking ? rearBreakForce : 0f;
            ApplyBreaking();
            ApplyHandBraking();
    }

    private void ApplyBreaking()
    {
            frontLeftWheelCollider.brakeTorque = currentBreakForce;
            frontRightWheelCollider.brakeTorque = currentBreakForce;
            rearLeftWheelCollider.brakeTorque = currentBreakForce;
            rearRightWheelCollider.brakeTorque = currentBreakForce;

            
        
    }

    private void ApplyHandBraking()
    {
        rearLeftWheelCollider.brakeTorque = handbraking;
        rearRightWheelCollider.brakeTorque = handbraking;
        //brakeTrailRen.emitting = Handbraking;
    }
    private void HandlingSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;

    }

    private void WheelsUpdating()
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

    void BrakesUsed()
    {
        AnimationCurve curve1 = new AnimationCurve();
        AnimationCurve curve2 = new AnimationCurve();
        float trailWidth = 1.0f;
        if( ifHandBraking == true)
        {
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);



        }
        else if(ifHandBraking == false)
        {
            
            
            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);

        }
        brakeTrailRenLeft.time = trailTime;
        brakeTrailRenRight.time = trailTime;
        brakeTrailRenLeft.emitting = ifHandBraking;
        brakeTrailRenLeft.widthCurve = curve1;
        brakeTrailRenLeft.widthMultiplier = trailWidth;
        brakeTrailRenRight.emitting = ifHandBraking;
        brakeTrailRenRight.widthCurve = curve1;
        brakeTrailRenRight.widthMultiplier = trailWidth;
    }
    

}
