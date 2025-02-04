using System.Timers;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{

    float waitTime;
    float remainingTime;
    bool repeat;

    UnityEvent timerOver;

    bool timerStarted = false;

   const int MAXINTERATIONSWITHOUTSTARTING = 1000;
    int interationCount = 0;
    public void StartTimer(float waitTime, bool repeat = false)
    {
        this.waitTime = waitTime;
        remainingTime = waitTime;
        this.repeat = repeat;

        timerStarted = true;
    }

    void FixedUpdate()
    {
        if (!timerStarted)
        {
            interationCount++;
            if (interationCount == MAXINTERATIONSWITHOUTSTARTING)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                onTimerOver();
            }
        }
    }
     public float timeRemaining()
    {
        return remainingTime;
    }
    void onTimerOver()
    {
        timerOver.Invoke();
        if (repeat)
        {
            remainingTime = waitTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
