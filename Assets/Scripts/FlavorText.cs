using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlavorText : MonoBehaviour
{
    public static FlavorText flavorText;
    private static TextMeshProUGUI textObj;

    public static bool displaying;
    // Start is called before the first frame update
    void Awake()
    {
        flavorText = this;
        textObj = this.GetComponent<TextMeshProUGUI>();
        displaying = false;
    }

    public void showText(string text)
    {
        if(displaying == false)
        {
            textObj.text = text;
            displaying = true;
            StartCoroutine(FadeInText(0.5f));
        }
    }

    private IEnumerator FadeInText(float timeSpeed)
    {
        Debug.Log("fading in");
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0);
        while (textObj.color.a < 1.0f)
        {
            textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, textObj.color.a + (Time.deltaTime * timeSpeed));

            yield return null;
        }
        StartCoroutine(FadeOutText(0.5f));
    }
    private IEnumerator FadeOutText(float timeSpeed)
    {
        Debug.Log("fading out");
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
        while (textObj.color.a > 0.0f)
        {
            textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, textObj.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
        displaying = false;
    }
}
