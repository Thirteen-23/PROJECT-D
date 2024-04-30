
using UnityEngine;

public class Rocket_Leagure_Jank : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private GameObject trail;
    TrailRenderer trailing;
    public bool jumpActive = false;
    public bool boostActive = false;
    [SerializeField] private float boostPower;   
    [SerializeField] private float PushPower;
    [SerializeField] private float coolDown;
    [SerializeField] private float coolDownTimer; 
    [SerializeField] private float nextBoostTime;
    [SerializeField] Shader[] shades; 

    public float trailWidth = 1.0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailing = trail.GetComponent<TrailRenderer>();
       
        
    }

    void UpdateInputs()
    {
        jumpActive = Input.GetKey(KeyCode.Q);
        boostActive = Input.GetKey(KeyCode.E);
    }
    // Update is called once per frame
    void Update()
    {


        UpdateInputs();
        #region Jump

        if (coolDownTimer > 0 )
        {
            coolDownTimer -= Time.deltaTime;
        }

        if(coolDownTimer < 0)
        {
            rb.constraints = RigidbodyConstraints.None;
            coolDownTimer = 0;

        }

        
        if (Time.time > nextBoostTime && coolDownTimer == 0)
        {
            if (jumpActive == true)
            {
                Jump();
                rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                nextBoostTime = Time.time + coolDown;
                coolDownTimer = coolDown; 
            }
        }
        #endregion

        AnimationCurve curve = new AnimationCurve();
        bool boostTrail = false;

        // note to self fix boost trail issues
        if(boostActive == true)
        {
            Boost();
            curve.AddKey(0.0f * Time.deltaTime, 0.0f);
            curve.AddKey(1.0f, 1.0f );
            boostTrail = true;
        }
        else if(boostActive == false)
        {
            boostTrail = false;
            curve.AddKey(0.0f, 0.1f);
            curve.AddKey(0.0f, 0.0f);
        }
        trailing.emitting = boostTrail;
        trailing.widthCurve = curve;
        trailing.widthMultiplier = trailWidth; 

    }

    void Jump()
    {
        rb.AddForce(rb.transform.up * PushPower);
       
    }

    void Boost()
    {
        rb.AddForce(rb.transform.forward * boostPower);
    }

}
