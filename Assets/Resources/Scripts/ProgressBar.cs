using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    //0-1 progress
    public float progress;
    public float scale;

    GameObject bar;

    // Start is called before the first frame update
    void Start()
    {
        //find the bar to display progress
        bar = GameObject.FindGameObjectWithTag("bar").gameObject;

        //default bar values
        bar.transform.localPosition = new Vector3(-0.5f, 0.1f, 0);
        bar.transform.localScale = new Vector3(0, 1, 1);

        progress = 0;
    }

    //scale is how many increments it takes to reach 1
    public void increment()
    {
        progress += 1 / scale;

        if (progress > 1)
        {
            progress = 1;
        }

        //adjust scale and position locally so the progress bar appears to grow
        GameObject.FindGameObjectWithTag("bar").transform.localScale = new Vector3(progress, 1, 1);
        GameObject.FindGameObjectWithTag("bar").transform.localPosition = new Vector3(-((1-progress) / 2), 0.1f, 0);
    }
}
