using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Windows;

public class StageSelect : MonoBehaviour
{
    [SerializeField] TMP_Text selectedLevel;
    [SerializeField] SceneHandler SceneHandler;

    [SerializeField] Color NoTimerColor = Color.red;
    [SerializeField] Color UseTimerColor = Color.green;

    [SerializeField] GameObject TimerDuration;

    Dictionary<string, Image> StageThumbnails = new Dictionary<string, Image>();

    [SerializeField] GameObject TetherSelectors;
    [SerializeField] RawImage selectedStageImage;


    private void Awake()
    {

        if (GameManager.instance != null)
        {
            if (TetherSelectors.transform.childCount > GameManager.instance.GetPlayerCount())
            {
                for (int i = GameManager.instance.GetPlayerCount(); i < TetherSelectors.transform.childCount; i++)
                {
                    Destroy(TetherSelectors.transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            for (int i = 2; i < TetherSelectors.transform.childCount; i++)
            {
                Destroy(TetherSelectors.transform.GetChild(i).gameObject);
            }
            //Assume 2 players
        }
    }

    public void SetStage(GameObject level)
    { 
       // GameManager.instance.SetSelectedLevel(level);
        selectedLevel.text = level.name;
        selectedStageImage.texture = level.GetComponent<Image>().mainTexture;
    }

    public void StartGame()
    {
        SceneHandler.SwapScene(selectedLevel.text);
    }

    public void ReturnToMainMenu()
    {
        SceneHandler.SwapScene("MainMenu");
    }

    void SetTimer()
    {

    }

   public void SetStockCount(TMP_InputField stockTracker)
    {
        stockTracker.text = stockTracker.text.Trim();
        if (int.TryParse(stockTracker.text, out int result))
        {
            Debug.Log("Result is " + result.ToString());
            GameManager.instance.SetMatchSetting("StockCount", result);
        }
        else
        {
            Debug.LogWarning("Could not parse stock tracker text (" + stockTracker.text + ") as int");
        }
    }

    public void IncrementStockCount(Button button)
    {
        int count = button.name.Contains("Increase") ? 1 : -1;
        int stocks = (int) GameManager.instance.GetMatchSetting("StockCount");
        stocks += count;
        GameManager.instance.SetMatchSetting("StockCount", stocks);

        TMP_InputField stockTracker = button.transform.parent.GetComponent<TMP_InputField>();
         
        stockTracker.text = stocks.ToString().Trim();
    }

    public void ToggleTimer(Toggle toggle)
    {
        bool isOn = toggle.isOn;
        GameManager.instance.SetMatchSetting("UseTimer", isOn);
        TimerDuration.SetActive(isOn);

    }

    public void SetTimerDuration(TMP_InputField timerField)
    {
        timerField.text = timerField.text.Trim();
        if (int.TryParse(timerField.text, out int result))
        {
            Debug.Log("Result is " + result.ToString());
            GameManager.instance.SetMatchSetting("MatchDuration", result );
        }
        else
        {
            Debug.LogWarning("Could not parse timer field text (" + timerField.text + ") as int");
        }
    }

    public void IncrementTimerDuration(Button button)
    {
        int count = button.name.Contains("Increase") ? 1 : -1;
        int time = (int) GameManager.instance.GetMatchSetting("MatchDuration");
        time += count;
        GameManager.instance.SetMatchSetting("MatchDuration", time);

        TMP_InputField timerField = button.transform.parent.GetComponent<TMP_InputField>();

        timerField.text = time.ToString().Trim();
    }
}
