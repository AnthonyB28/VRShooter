using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public enum ScoreMode
    {
        Disabled,
        Score,
        Enemies
    }
    public Text m_GUIScore;
    public ScoreMode m_Mode;
    private uint m_Score = 0;
    private uint m_EnemyCount = 0;

    //Here is a private reference only this class can access
    private static ScoreManager _instance;

    //This is the public reference that other classes will use
    public static ScoreManager Instance
    {
        get
        {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ScoreManager>();
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }

        UpdateGUI();
    }

    private void UpdateGUI()
    {
        switch (m_Mode)
        {
            case ScoreMode.Disabled: m_GUIScore.text = ""; break;
            case ScoreMode.Score: m_GUIScore.text = "Score: " + m_Score.ToString(); break;
            case ScoreMode.Enemies: m_GUIScore.text = "Targets: " + m_Score.ToString() + " / " + m_EnemyCount.ToString(); break;
        }
    }

    public void SetEnemyCount(uint enemies)
    {
        m_EnemyCount += enemies;
        UpdateGUI();
    }

    public void DestroyedEnemy(uint score)
    {
        if (m_Mode == ScoreMode.Enemies)
        {
            ++m_Score;
        }
        else
        {
            m_Score += score;
        }

        UpdateGUI();
        gameObject.GetComponentInChildren<EnemySpawn>().DestroyedEnemy();
    }
}