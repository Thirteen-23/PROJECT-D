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
    float currentSteerAngle, currentBreakForce, handbreaking;
    bool quittingApplication = false;
    bool resetPosition = false;

    bool boostUp = false; 
    [SerializeField]
    private float speedFactor;
    //public float lowSpeed;
    //public float middleSpeed;
    //public float HighSpeed;

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
        handbreaking = ifHandBraking ? rearBreakForce : 0f;
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
        rearLeftWheelCollider.brakeTorque = handbreaking;
        rearRightWheelCollider.brakeTorque = handbreaking;
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

    

}
