
using UnityEngine;

public class WheelColldierViewer : MonoBehaviour
{
    //[SerializeField] WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField] WheelCollider m_LBWheel, m_RBWheel;
    [SerializeField] WheelCollider[] wheels = new WheelCollider[2];
    WheelHit hit = new WheelHit();

    WheelFrictionCurve l_wheelSettings;
    WheelFrictionCurve r_wheelSettings;


    [Header("values")]
    [SerializeField] float m_SlipValues;
    [SerializeField] float m_SideFStiffness_Value;
    [SerializeField] float l_SidewaysFrictionStiffness;
    [SerializeField] float r_SidewaysFrictionStiffness;
    // Start is called before the first frame update
    void Start()
    {
        l_wheelSettings = m_LBWheel.sidewaysFriction;
        r_wheelSettings = m_RBWheel.sidewaysFriction;
    }


    // Update is called once per frame
    void Update()
    {
        l_SidewaysFrictionStiffness = l_wheelSettings.extremumValue;
        r_SidewaysFrictionStiffness = r_wheelSettings.extremumValue;

       

        checkcheck();
        //print(hit.sidewaysSlip);
        CheckforWheelsSlip();
    }
    private void CheckforWheelsSlip()
    {
        //if (m_LBWheel.GetGroundHit(out hit))
        //{
        //    Debug.Log("wheel slipping?");
        //    m_SlipValues = hit.sidewaysSlip;
        //}
        //if (m_RBWheel.GetGroundHit(out hit))
        //{
        //    Debug.Log("wheel slipping?");
        //    m_SlipValues = hit.sidewaysSlip;
        //}
    }

    private float changingthefriction(int i )
    {
        return 0;
    }
    private void checkcheck()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].GetGroundHit(out hit))
            {
               
                m_SlipValues = hit.sidewaysSlip;
                if(m_SlipValues >0.15)
                {
                    Debug.Log("wheel slipping?");

                }
            }
        }
    }
}
