using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawn : MonoBehaviour
{

    public Image m_AlertGUIBar;
    public List<GameObject> m_SpawnPoints;
    public List<int> m_Rounds;
    public List<int> m_RoundDeviation;
    public GameObject m_Enemy;
    public float m_Radius = 5;

    private int m_CurRound = 0;
    private uint m_EnemiesDestroyed = 0;

    // Use this for initialization
    void Start()
    {
        foreach(GameObject sphere in m_SpawnPoints)
        {
            sphere.renderer.enabled = false;
        }
        m_AlertGUIBar.enabled = false;
        InitiateSpawnRound(m_Rounds[m_CurRound], m_RoundDeviation[m_CurRound]);
    }

    void InitiateSpawnRound(int numOfEnemies, int deviation)
    {
        ScoreManager.Instance.SetEnemyCount((uint)numOfEnemies);
        List<int> spawns = new List<int>();
        for(int i = 0; i < deviation; ++i)
        {
            spawns.Add(Random.Range(0, m_SpawnPoints.Count-1));
        }

        int spawnGroupCount = numOfEnemies / spawns.Count;

        if(numOfEnemies % spawns.Count != 0)
        {
            SpawnEnemyInSphere(m_SpawnPoints[spawns[0]].transform.position);
        }

        for(int i = 0; i < deviation; ++i)
        {
            Vector3 spawnPosition = m_SpawnPoints[spawns[i]].transform.position;
            DisplayAlert(spawnPosition);
            for (int x = 0; x < spawnGroupCount; ++x)
            {
                SpawnEnemyInSphere(spawnPosition);
            }
        }
    }

    void SpawnEnemyInSphere(Vector3 spawnPos)
    {
        Vector3 spherePosOffset = Random.insideUnitSphere * m_Radius;
        GameObject.Instantiate(m_Enemy, spherePosOffset + spawnPos, Quaternion.identity);
    }

    void DisplayAlert(Vector3 enemyPos)
    {
        Vector3 relativePoint = Camera.main.transform.InverseTransformPoint(enemyPos);
        Debug.Log(relativePoint);
        if (relativePoint.y < 15 && relativePoint.y > -15)
        {
            // Adjacent
            if (relativePoint.x < -10 && relativePoint.z >= relativePoint.x)
            {
                Debug.Log("Left");
            }
            else if (relativePoint.x > 10 && relativePoint.z <= relativePoint.x)
            {
                // right
                Debug.Log("Right");
            }
        }
        else
        {
            // Horizontal
            if(relativePoint.y < -15)
            {
                Debug.Log("Below");
            }
            else if(relativePoint.y > 15)
            {
                Debug.Log("Above");
            }
        }
    }

    public void DestroyedEnemy()
    {
        if (++m_EnemiesDestroyed >= m_Rounds[m_CurRound])
        {
            if (++m_CurRound == m_Rounds.Count)
            {
                // End game
            }
            else
            {
                m_EnemiesDestroyed = 0;
                InitiateSpawnRound(m_Rounds[m_CurRound], m_RoundDeviation[m_CurRound]);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (GameObject sphere in m_SpawnPoints)
        {
             Gizmos.DrawWireSphere(sphere.transform.position, m_Radius);
        }
    }
}
