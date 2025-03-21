using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MatchTimer : MonoBehaviour
{

    [SerializeField] TMP_Text label;
    [SerializeField] Color lowTimeColor;
    [SerializeField] float lowTimeMarker = 11.0f;
    [SerializeField] GameObject TimeOverDisplay;

    [SerializeField] LevelManager currentLevel;
    float elaspedTime;

    [SerializeField] UnityEvent onTimerOver = new UnityEvent();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    bool reachedZero = false;
    void Start()
    {
        if (GameManager.instance != null) 
        {
            gameObject.SetActive( (bool) GameManager.instance.GetMatchSetting("UseTimer"));
            if (gameObject.activeSelf ) 
            {
             elaspedTime =  (int) GameManager.instance.GetMatchSetting("MatchDuration");
             elaspedTime *= 60;
                //multiply by 60 to convert it to seconds
            }
        }
        else
        {
            gameObject.SetActive(false);
        }

        currentLevel.StartedSuddenDeath.AddListener(OnSuddenDeathStart);
    }

    void OnSuddenDeathStart()
    {
        TimeOverDisplay.SetActive(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (elaspedTime > 0)
        {
            elaspedTime -= Time.deltaTime;
            if (elaspedTime < 0 )
            {
                elaspedTime = 0;
            }
            SetTime(elaspedTime);
        }
        else if (!reachedZero)
        {
            TimeOverDisplay.SetActive(true);
            reachedZero = true;
            SetTime(0);
            onTimerOver.Invoke();
        }
        
        
    }

    void SetTime(float time)
    {
        int minutesElasped = Mathf.FloorToInt(elaspedTime / 60);
        int secondsElasped = Mathf.FloorToInt(elaspedTime % 60);

        label.text = string.Format("{0:0}:{1:00}", minutesElasped, secondsElasped);

        if (elaspedTime < lowTimeMarker)
        {
            label.color = lowTimeColor;
        }
    }
}
