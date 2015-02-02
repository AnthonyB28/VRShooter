using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawn : MonoBehaviour
{

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

        InitiateSpawnRound(m_Rounds[m_CurRound], m_RoundDeviation[m_CurRound]);
    }

    void InitiateSpawnRound(int numOfEnemies, int deviation)
    {
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
            // Spawn a bunch of enemies at our first spawn
            for (int x = 0; x < spawnGroupCount; ++x)
            {
                SpawnEnemyInSphere(m_SpawnPoints[spawns[i]].transform.position);
            }
        }
    }

    void SpawnEnemyInSphere(Vector3 spawnPos)
    {
        Vector3 spherePosOffset = Random.insideUnitSphere * m_Radius;
        GameObject.Instantiate(m_Enemy, spherePosOffset + spawnPos, Quaternion.identity);
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
