using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{

    bool drawCrosshair = true;

    Color crosshairColor = Color.white;   //The crosshair color

    float width = 1;      //Crosshair width
    float height = 5;     //Crosshair height

    class Spread
    {
        public float spread = 1.0f;          //Adjust this for a bigger or smaller crosshair
        public float maxSpread = 10.0f;
        public float minSpread = 3.0f;
        public float spreadPerSecond = 30.0f;
        public float decreasePerSecond = 25.0f;
    }

    Spread spread = new Spread();

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
            spread.spread += spread.spreadPerSecond * Time.deltaTime;       //Incremente the spread
            Fire();
        }
        else
        {
            spread.spread -= spread.decreasePerSecond * Time.deltaTime;      //Decrement the spread        
        }

        spread.spread = Mathf.Clamp(spread.spread, spread.minSpread, spread.maxSpread);
    }

    void OnGUI()
    {
        var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        if (drawCrosshair)
        {
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y - (height + spread.spread), width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y + spread.spread, width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x + spread.spread, (centerPoint.y - width / 2), height, width), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - (height + spread.spread), (centerPoint.y - width / 2), height, width), "", lineStyle);
        }
    }

    void Fire()
    {
        //Carry out your normal shooting and stuff
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
