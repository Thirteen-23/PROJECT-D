using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakingTrailScript : MonoBehaviour
{
    Car_Movement carMovement; 

    [Header("Brake Trail gameObjects")]
    [SerializeField] GameObject brakeTrailLeft;
    [SerializeField]GameObject brakeTrailRight;
    [SerializeField] GameObject vehicleTarget;
    [SerializeField] TrailRenderer brakeTrailRenLeft, brakeTrailRenRight;

    [Header("Variables")]
    [SerializeField] private float changeTrailsizeBeginning;
    [SerializeField] private float changeTrailsizeEnd;
    [SerializeField] private float trailTime;
    [SerializeField] private bool checkingBrakeWork = false;



    // Start is called before the first frame update
    void Start()
    {
        carMovement = GetComponent<Car_Movement>(); 
        brakeTrailRenLeft = brakeTrailLeft.GetComponent<TrailRenderer>();
        brakeTrailRenRight = brakeTrailRight.GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        BrakesUsed();
    }


    private void BrakesUsed()
    {
        AnimationCurve curve1 = new AnimationCurve();
        AnimationCurve curve2 = new AnimationCurve();
        //sRFriction = new WheelFrictionCurve();

        float trailWidth = 1.0f;
        if (carMovement.ifHandBraking == true)
        {
            checkingBrakeWork = carMovement.ifHandBraking;
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);
            //sRFriction.asymptoteValue = 0.50f;


        }
        else if (carMovement.ifHandBraking == false)
        {
            checkingBrakeWork = carMovement.ifHandBraking;

            curve1.AddKey(1.0f, changeTrailsizeEnd);
            curve1.AddKey(0.0f, changeTrailsizeBeginning);
            curve2.AddKey(1.0f, changeTrailsizeEnd);
            curve2.AddKey(0.0f, changeTrailsizeBeginning);
            // sRFriction.asymptoteValue = 0.75f;
        }
        //rearRightWheelCollider.sidewaysFriction = sRFriction;
        brakeTrailRenLeft.time = trailTime;
        brakeTrailRenRight.time = trailTime;
        brakeTrailRenLeft.emitting = checkingBrakeWork;
        brakeTrailRenLeft.widthCurve = curve1;
        brakeTrailRenLeft.widthMultiplier = trailWidth;
        brakeTrailRenRight.emitting = checkingBrakeWork;
        brakeTrailRenRight.widthCurve = curve1;
        brakeTrailRenRight.widthMultiplier = trailWidth;
    }
}
