using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    public float m_HeightBeforeFire = 2f;
    public float m_Speed = 0.4f;
    public float m_DestroySeconds = 10f;
    public bool m_Homing = false;
    public float m_MissleSpeedUpTime = 0.4f;
    public float m_MissleSpeedBoost = 3f;

    private GameObject m_Target;
    private float m_StartTime;
    private float m_SpawnHeight;
    private bool m_Climbing;

    // Use this for initialization
    void Start() 
    {
        m_StartTime = Time.time;
        m_Climbing = true;
        m_SpawnHeight = transform.position.y;
    }

    public void SetTarget(GameObject target)
    {
        m_Homing = true;
        m_Target = target;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_Climbing)
        {
            if (m_Homing)
            {
                this.gameObject.transform.LookAt(m_Target.transform);
                if (Time.time - m_StartTime >= m_MissleSpeedUpTime)
                {
                    transform.position += (m_Speed + m_MissleSpeedBoost) * transform.forward;
                }
                else
                {
                    Travel();
                }
            }
            else
            {
                Travel();
            }
            if (Time.time - m_StartTime >= m_DestroySeconds)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position += m_Speed * transform.up;
            if(transform.position.y >= m_SpawnHeight + m_HeightBeforeFire)
            {
                m_Climbing = false;
            }

        }
    }

    void Travel()
    {
        transform.position += m_Speed * transform.forward;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().ProjectileCollision();
        }
        Destroy(gameObject);
    }
}
