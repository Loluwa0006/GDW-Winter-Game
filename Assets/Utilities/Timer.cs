using System.Timers;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Timer : MonoBehaviour
{
    [SerializeField] string timerID = string.Empty;

    [SerializeField] float defaultWaitTime = 1.0f;
    [SerializeField] bool repeat = false;
    [SerializeField] bool startOnWake = false;

    public UnityEvent timerOver = new UnityEvent();

    float waitTime = 0.0f;

    bool timerActive = false;
    bool destroyOnFinish = false;

    float remainingTime = 0.0f;




    private void Awake()
    {
        timerOver = new UnityEvent();
        timerOver.AddListener(OnTimerOver);
        if (startOnWake)
        {
            StartTimer(repeat);
        }

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
        Debug.Log("Timer already named " + timerID);
    }
    public void StartTimer(float waitTime, bool repeat = false, bool destroyOnFinish = false)
    {
        this.waitTime = Mathf.Max(waitTime, 0);


        this.repeat = repeat;

        remainingTime = this.waitTime;

        this.destroyOnFinish = destroyOnFinish;

        timerActive = true;
        //  Debug.Log("Internal wait time is " + this.waitTime.ToString() + ", parameter is " + waitTime.ToString());

    }

    public void StartTimer(bool repeat = false, bool destroyOnFinish = false)
    {
        waitTime = defaultWaitTime;


        this.repeat = repeat;

        remainingTime = waitTime;

        this.destroyOnFinish = destroyOnFinish;

        timerActive = true;
        Debug.Log("Internal wait time is " + waitTime.ToString() + ", default wait time is " + defaultWaitTime.ToString());

    }

    public void StopTimer()
    {
        timerActive = false;
        remainingTime = 0.0f;
    }

    void Update()
    {


        if (timerActive)
        {
            remainingTime -= Time.deltaTime;
            //wremainingTime = Mathf.Max(remainingTime, 0);
            if (remainingTime <= 0)
            {
                Debug.Log("calling signal timerOver");
                timerOver.Invoke();
            }
        }
    }
    public float TimeRemaining()
    {
        return remainingTime;
    }

    public bool IsStopped()
    {
        return remainingTime <= 0 && timerActive == true;
    }
    void OnTimerOver()
    {

        if (destroyOnFinish)
        {
            Destroy(gameObject);
        }
        else if (repeat)
        {
            Debug.Log("restarting");
            StartTimer(repeat);
        }
        else
        {
            timerActive = false;
        }

    }
}