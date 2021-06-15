using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manually make a new timescale to apply to physics so collisions and everything is smoother
public class TimeManager : MonoBehaviour
{
    float pastTimeScale;
    public float timeScale;
    float goal;
    float linearIncrement;

    // Start is called before the first frame update
    void Start()
    {
        timeScale = 1f;
        pastTimeScale = 1;
        goal = timeScale;
        linearIncrement = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pastTimeScale = timeScale;
        if (goal != timeScale)
        {
            //increment
            timeScale += linearIncrement;

            //check becus of rounding if the timescale passed the current goal
            if ((linearIncrement / Mathf.Abs(linearIncrement) == 1 && timeScale > goal) ||
                (linearIncrement / Mathf.Abs(linearIncrement) == -1 && timeScale < goal))
            {
                timeScale = goal;
            }

            print(timeScale + " " + goal);
        }
    }

    //set the timescale gradually over the course of so many seconds
    //linear
    public void setTimeScale(float newTimeScale, float changeTime)
    {
        goal = newTimeScale;
        linearIncrement = ((newTimeScale - timeScale) / changeTime) * Time.fixedDeltaTime;
    }

    public float cvtTime(float value)
    {
        return value * (timeScale/pastTimeScale);
    }
}
