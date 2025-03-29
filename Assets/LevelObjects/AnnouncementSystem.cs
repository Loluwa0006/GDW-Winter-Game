using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AnnouncementSystem : MonoBehaviour
{

    const float GAME_SLOW_AMOUNT = 0.15f;

  
    [SerializeField] GameObject AnnoucementObject;
    [SerializeField] TMP_Text AnnoucementText;


    private void Awake()
    {
        AnnoucementObject.SetActive(false);
    }
    

    public IEnumerator StartAnnouncement( string text, float duration, bool slowGame = false)
    {
        Debug.Log("Starting annoucement");
        if (slowGame)
        {
            Time.timeScale = GAME_SLOW_AMOUNT;
        }
        AnnoucementObject.SetActive (true);
        AnnoucementText.text = text;

        yield return new WaitForSecondsRealtime (duration);
        AnnoucementObject.SetActive(false);

        if (slowGame)
        {
            Time.timeScale = 1.0f;
        }

    } 

    public void CancelAnnoucement()
    {
        AnnoucementObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
