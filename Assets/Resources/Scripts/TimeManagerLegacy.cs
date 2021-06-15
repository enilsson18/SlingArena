using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagerLegacy : MonoBehaviour
{
    float goal;
    float linearIncrement;
    //float exponentialIncrement;

    // Start is called before the first frame update
    void Start()
    {
        goal = Time.timeScale;
        linearIncrement = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (goal != Time.timeScale)
        {
            //increment
            Time.timeScale += linearIncrement * Time.deltaTime;

            //check becus of rounding if the timescale passed the current goal
            if ((linearIncrement / Mathf.Abs(linearIncrement) == 1 && Time.timeScale > goal) ||
                (linearIncrement / Mathf.Abs(linearIncrement) == -1 && Time.timeScale < goal))
            {
                Time.timeScale = goal;
            }
        }
    }

    //set the timescale gradually over the course of so many seconds
    //linear
    public void setTimeScale(float timeScale, float changeTime)
    {
        goal = timeScale;
        linearIncrement = (timeScale - Time.timeScale) / changeTime;
        //exponentialIncrement = 1;
    }

    //exponential
    public void setTimeScale(float timeScale, float changeTime, float exponentialIncrement)
    {

    }
}
