using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    public float speed;
    public float distance;
    public Transform cam;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 angles = transform.eulerAngles;
            float dx = Input.GetAxis("Mouse Y");
            float dy = Input.GetAxis("Mouse X");

            angles.x = Mathf.Clamp(angles.x + dx * speed * Time.deltaTime, 0, 70);
            angles.y += dy * speed * Time.deltaTime;
            transform.eulerAngles = angles;

        }

        transform.position = target.position - distance * transform.forward;

        Vector3 camForward = cam.forward;
        camForward.y = 0;
        camForward.Normalize();
    }

    private void FixedUpdate()
    {
        
    }
}
