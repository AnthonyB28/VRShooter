using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int m_Health = 100;
    public uint m_Score = 25;
    
    // Use this for initialization
    void Start()
    {

    }

    public void ProjectileCollision(uint damage)
    {
        m_Health -= (int)damage;
        if (m_Health <= 0)
        {
            if (ScoreManager.Instance)
            {
                ScoreManager.Instance.DestroyedEnemy(m_Score);
            }
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
