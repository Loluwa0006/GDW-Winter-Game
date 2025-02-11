using System.Timers;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Timer : MonoBehaviour
{
    [SerializeField] string timerID = string.Empty;

    [SerializeField] float defaultWaitTime = 1.0f;
    [SerializeField] bool repeat = false;

    UnityEvent timerOver;

    float waitTime = 0.0f;

    bool timerActive = false;
    bool destroyOnFinish = false;

     float remainingTime = 0.0f;




    private void Start()
    {
        timerOver = new UnityEvent();
    }
    public string GetID()
    {
        return timerID;
    }

    public void SetID(string newID)
    {
        if (timerID == string.Empty)
        {
            timerID = newID;
        }
        Debug.Log("Timer already named");
    }
    public void StartTimer(float waitTime, bool repeat = false, bool destroyOnFinish = false)
    {
        this.waitTime = Mathf.Max(waitTime, 0);
        
       
        this.repeat = repeat;
        
        remainingTime = this.waitTime;

        this.destroyOnFinish = destroyOnFinish;

        timerActive = true;
        Debug.Log("Internal wait time is " + this.waitTime.ToString() + ", parameter is " + waitTime.ToString());

    }

    public void StartTimer(bool repeat = false, bool destroyOnFinish = false)
    {
        waitTime = defaultWaitTime;
        
        
       this.repeat = repeat;
        
        remainingTime = this.waitTime;

        this.destroyOnFinish = destroyOnFinish;

        timerActive = true;
        Debug.Log("Internal wait time is " + waitTime.ToString() + ", default wait time is " + defaultWaitTime.ToString());

    }

    void Update()
    {


        if (timerActive)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(remainingTime, 0);
            if (remainingTime <= 0)
            {
                onTimerOver();
            }
        }
    }
    public float timeRemaining()
    {
        //Debug.Log(remainingTime.ToString());
        return remainingTime;
    }

    public bool isStopped()
    {
        return remainingTime <= 0 && timerActive == true;
    }
    void onTimerOver()
    {
        timerOver.Invoke();
        if (destroyOnFinish)
        {
            Destroy(gameObject);
        }
       else if (repeat)
        {
            remainingTime = waitTime;
        }
        else
        {
            timerActive = false;
        }
       
    }
}
