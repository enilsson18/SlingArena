using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    TextMeshProUGUI tmp;

    //stage 0,1,2,3 (off ,start, pause, fade out)
    public int stage;

    public float fadeSpeed = 0.02f;
    public bool fadeAfterFade = true;

    // Start is called before the first frame update
    void Start()
    {
        //adjust speeds to match the change in fixed update
        fadeSpeed /= Time.fixedDeltaTime / 0.015f;

        stage = 0;
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stage == 0)
        {

        }
        else if (stage == 1)
        {
            if (tmp.color.a + fadeSpeed >= 1)
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1);
                stage = 2;
                if (fadeAfterFade)
                {
                    stage = 3;
                }
            }
            else
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a + fadeSpeed);
            }
        }
        else if (stage == 2)
        {

        }
        else if (stage == 3)
        {
            if (tmp.color.a - fadeSpeed <= 0)
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
                stage = 0;
            }
            else
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, tmp.color.a - fadeSpeed);
            }
        }
    }

    public void SetText(string str)
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.SetText(str);
    }

    public void FadeIn()
    {
        if (stage == 0)
        {
            stage = 1;
        }
    }

    public void FadeOut()
    {
        stage = 3;
    }
}
