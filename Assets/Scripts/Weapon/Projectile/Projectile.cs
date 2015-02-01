using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public GameObject m_HitFX;
    public uint m_Damage = 50;
    public float m_Speed = 0.4f;
    public float m_DestroySeconds = 10f;
    public bool m_Homing = false;
    public float m_MissleSpeedUpTime = 0.4f;
    public float m_MissleSpeedBoost = 3f;
    
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
        if(m_Homing)
        {
            transform.LookAt(m_Target.transform);
            if (Time.time - m_StartTime >= m_MissleSpeedUpTime)
            {
                transform.position += (m_Speed + m_MissleSpeedBoost) * transform.forward;
            }
            else
            {
                transform.position += m_Speed * transform.forward;
            }
        }
        else
        {
            transform.position += m_Speed * transform.forward;
        }
        if(Time.time - m_StartTime >= m_DestroySeconds)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().ProjectileCollision(m_Damage);
        }
        if(m_HitFX)
        {
            GameObject.Instantiate(m_HitFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
