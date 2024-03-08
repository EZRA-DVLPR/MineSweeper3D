using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stopwatch : MonoBehaviour
{
    //events to handle timer start/stop
    public delegate void StartTimerEvent();
    public event StartTimerEvent OnStartTimer;

    public delegate void StopTimerEvent();
    public event StopTimerEvent OnStopTimer;

    //is stopwatch active and currentTime in float
    bool stopwatchActive = false;
    float currTime;

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive)
        {
            //update time
            currTime = currTime + Time.deltaTime;
        }
    }

    public void startTimer()
    {
        currTime = 0f;
        stopwatchActive = true;
        OnStartTimer?.Invoke();
    }

    public string stopTimer()
    {
        stopwatchActive = false;
        TimeSpan time = TimeSpan.FromSeconds(currTime);
        string timeString = time.ToString(@"hh\:mm\:ss\:ff");
        OnStopTimer?.Invoke();
        return timeString;
    }
}
