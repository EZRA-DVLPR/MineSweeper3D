using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stopwatch : MonoBehaviour
{
    //events to handle timer start/stop
    public delegate void StartTimer();
    public static event StartTimer OnStartTimer;

    public delegate void StopTimer();
    public static event StopTimer OnStopTimer;

    bool stopwatchActive = false;
    float currTime;

    // Start is called before the first frame update
    void Start()
    {
        currTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //start stopwatch
        if (Input.GetKeyDown(KeyCode.P))
        {
            stopwatchActive = true;
        }

        //stop stopwatch
        if (Input.GetKeyDown(KeyCode.O))
        {
            stopwatchActive = false;
        }

        //update and display the time
        updateTime();
        displayTime();
    }

    //updates currTime
    private void updateTime()
    {
        if (stopwatchActive)
        {
            currTime = currTime + Time.deltaTime;
        }
    }

    //only display the time if it is changing, otherwise just display it once
    private void displayTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(currTime);
        if (stopwatchActive)
        {
            Debug.Log($"{time.ToString(@"hh\:mm\:ss\:ff")}");
        }
    }
}
