using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{

    bool drawCrosshair = true;

    Color crosshairColor = Color.white;   //The crosshair color

    float width = 1;      //Crosshair width
    float height = 5;     //Crosshair height

    public float spread = 3.0f;          //Adjust this for a bigger or smaller crosshair
    public float maxSpread = 13.0f;
    public float minSpread = 3.0f;
    public float spreadPerSecond = 30.0f;
    public float decreasePerSecond = 25.0f;

    Texture2D tex;

    GUIStyle lineStyle;

    void Awake()
    {
        tex = new Texture2D(1, 1);

        SetColor(tex, crosshairColor); //Set color

        lineStyle = new GUIStyle();
        lineStyle.normal.background = tex;
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            spread += spreadPerSecond * Time.deltaTime;       //Incremente the spread
        }
        else
        {
            spread -= decreasePerSecond * Time.deltaTime;      //Decrement the spread        
        }

        spread = Mathf.Clamp(spread, minSpread, maxSpread);
    }

    void OnGUI()
    {
        var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        if (drawCrosshair)
        {
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y - (height + spread), width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y + spread, width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x + spread, (centerPoint.y - width / 2), height, width), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - (height + spread), (centerPoint.y - width / 2), height, width), "", lineStyle);
        }
    }

    public Vector3 GetRandomSpread()
    {
//         float spreadMod = spread / 4;
//         float randomNumberX = Random.Range(-spreadMod, spreadMod);
//         float randomNumberY = Random.Range(-spreadMod, spreadMod);
//         float randomNumberZ = Random.Range(-spreadMod, spreadMod);
//         return new Vector3(randomNumberX, randomNumberY, randomNumberZ);

       Vector2 center = new Vector3(Screen.width / 2, Screen.height / 2);
       return center + Random.insideUnitCircle * spread / 2.0f;
    }

    //Applies color to the crosshair
    void SetColor(Texture2D myTexture, Color myColor)
    {
        for (int y = 0; y < myTexture.height; ++y)
        {
            for (int x = 0; x < myTexture.width; ++x)
            {
                myTexture.SetPixel(x, y, myColor);
            }
        }

        myTexture.Apply();
    }
}
