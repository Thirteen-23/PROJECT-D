using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed = 5f;
    public float dashDistance = 25f;
    public float jumpVelocty = 5f;
    public Camera cam;

    public Vector3 hitDirections; 

    [SerializeField]
    bool isGrounded;
    bool jumpInput;
    CharacterController cc;
    Vector2 moveInput;
    Vector3 velocity;
    private Animator anim;
    float pushPower = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        cam = Camera.main;
      
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        anim.SetFloat("Forwards", moveInput.y);
        anim.SetBool("Jump", isGrounded);
        

    }

    private void FixedUpdate()
    {
      
        Vector3 move;
        // ---- Player Movement ---- //


        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = cam.transform.right;


        move = (moveInput.x * camRight /*Vector3.right*/ + moveInput.y * camForward/*Vector3.forward*/) * speed * Time.fixedDeltaTime;
        // ---- jump Check ---- //
        if (jumpInput && isGrounded)
        {
            velocity.y = jumpVelocty;

        }

    
        // ---- apply gravity ----//
       
        if(!isGrounded)
        {
            hitDirections = Vector3.zero;
        }

        if(moveInput.x == 0 && moveInput.y == 0)
        {
            Vector3 horizontalHitDirection = hitDirections;
            horizontalHitDirection.y = 0;
            float displacement = horizontalHitDirection.magnitude;
            if(displacement > 0 )
            {
                velocity -= 0.02f * horizontalHitDirection / displacement; 
            }
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        velocity += Physics.gravity * Time.fixedDeltaTime;
        // --- positiontal update frames
        move += velocity * Time.deltaTime;    
        cc.Move(move * Time.deltaTime * speed);
        isGrounded = cc.isGrounded;

    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitDirections = hit.point - transform.position;

        Rigidbody body = hit.collider.attachedRigidbody;
        if(body == null || body.isKinematic)
        {
            return;

        }

        if(hit.moveDirection.y < -0.3)
        {
            return;

        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
}
