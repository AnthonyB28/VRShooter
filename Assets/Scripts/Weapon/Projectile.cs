using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float m_Speed = 0.4f;
    public float m_DestroySeconds = 10f;
    public bool m_Homing = false;
    private float m_StartTime;
    private GameObject m_Target;

    // Use this for initialization
    void Start()
    {
        m_StartTime = Time.time;
    }

    public void SetTarget(GameObject target)
    {
        m_Homing = true;
        m_Target = target;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.position += m_Speed * this.gameObject.transform.forward;
        if(m_Homing)
        {
            this.gameObject.transform.LookAt(m_Target.transform);
        }
        if(Time.time - m_StartTime >= m_DestroySeconds)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
