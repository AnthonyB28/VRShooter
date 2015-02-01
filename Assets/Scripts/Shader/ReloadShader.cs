using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReloadShader : MonoBehaviour
{
    public Color start;
    public Color end;
    public Material CircleMaterial;
    public float m_LastValue = 0f;

    public void Awake()
    {
        CircleMaterial.SetFloat("_Angle", Mathf.Lerp(-3.14f, 3.14f, m_LastValue));
    }

    // Update is called once per frame
    public void ChangeCircle(float value)
    {
        Debug.Log(value);
        if (value > m_LastValue)
        {
            m_LastValue = value;
        }
        else
        {
            CircleMaterial.SetFloat("_Angle", Mathf.Lerp(-3.14f, 3.14f, value));
            CircleMaterial.SetColor("_Color", Color.Lerp(start, end, value));
        }
        m_LastValue = value;
    }
}
