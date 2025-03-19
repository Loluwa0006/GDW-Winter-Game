using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField] TMP_Text selectedLevel;
    [SerializeField] SceneHandler SceneHandler;

    Dictionary<string, Image> StageThumbnails = new Dictionary<string, Image>();
    public void SetStage(GameObject level)
    { 
       // GameManager.instance.SetSelectedLevel(level);
        selectedLevel.text = level.name;
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
       if (int.TryParse(stockTracker.text, out int result ))
        {
            GameManager.instance.SetMatchSetting("StockCount", result);
        }
    }

    public void IncrementStockCount(Button button)
    {
        int count = button.name.Contains("Increase") ? 1 : -1;
        Debug.Log("Count is " + count.ToString());
        int stocks = (int) GameManager.instance.GetMatchSetting("StockCount");
        stocks += count;
        GameManager.instance.SetMatchSetting("StockCount", stocks);

        TMP_InputField stockTracker = button.transform.parent.GetComponent<TMP_InputField>();
        stockTracker.text = stocks.ToString();
    }
}
