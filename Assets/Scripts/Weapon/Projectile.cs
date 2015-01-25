using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float m_Speed = 0.4f;
    public float m_DestroySeconds = 10f;
    private float m_StartTime;

    // Use this for initialization
    void Start()
    {
        m_StartTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.position += m_Speed * this.gameObject.transform.forward;
        if(Time.time - m_StartTime >= m_DestroySeconds)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Destroy(this.gameObject);
    }
}
