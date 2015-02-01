using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public Text m_GUIScore;
    private uint m_Score = 0;

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
        m_GUIScore.text = "Score: " + m_Score.ToString();
    }

    public void DestroyedEnemy(uint score)
    {
        m_Score += score;
        m_GUIScore.text = "Score: " + m_Score.ToString();
    }
}