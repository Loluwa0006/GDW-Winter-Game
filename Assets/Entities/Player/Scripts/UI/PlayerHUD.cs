using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMP_Text percentageTracker;
    [SerializeField] TMP_Text playerDisplayName;
    [SerializeField] TMP_Text stockOverflowDisplay;
    [SerializeField] GameObject StockImageHolder;
    [SerializeField] GameObject stockImage;
    [SerializeField] Image HUDBackground;

   [SerializeField] List<Color32> HUDColors;

    HealthComponent playerHealth;

    const int  MAX_LIVES_TO_DISPLAY = 6;
    public void InitPlayerHUD(PlayerController player)
    {
        playerHealth = player.GetComponent<HealthComponent>();
        playerHealth.healthInitalized.AddListener(SetLifeDisplay);
        playerHealth.onEntityDamaged.AddListener(SetPercentageDisplay);
        playerHealth.onEntityHealed.AddListener(SetPercentageDisplay);
        player.playerEliminated.AddListener(EndHUD);
        playerDisplayName.text = player.name;

        
        SetLifeDisplay(player, playerHealth.GetRemainingLives());
        
        SetPercentageDisplay(playerHealth.GetHealth(), 0);

        playerHealth.livesChanged.AddListener(SetLifeDisplay);
        playerHealth.livesChanged.AddListener( (player,lives) => SetPercentageDisplay(0,0) );

        Debug.Log("Setting up HUD for player " + player.playerIndex.ToString());
        HUDBackground.color = HUDColors[player.playerIndex - 1];
        

    }

    void EndHUD(PlayerController player)
    {
        percentageTracker.text = "";
        SetLifeDisplay(player, 0);
    }

    void SetLifeDisplay(PlayerController player, int remainingLives) 
    {
        Debug.Log("Remaining Lives is now " + remainingLives.ToString());
       
        if (remainingLives <= 0)
        {
            foreach(Transform child in StockImageHolder.transform)
            {
                Destroy(child.gameObject);
            }
            return;
        }

        if (remainingLives > MAX_LIVES_TO_DISPLAY)
        {
            for (int i = 1; i < StockImageHolder.transform.childCount; i++)
            {
                Destroy(StockImageHolder.transform.GetChild(i).gameObject);
            }
            stockOverflowDisplay.text = "x" + remainingLives.ToString();
            stockOverflowDisplay.gameObject.SetActive(true);

        }
        else
        {
            stockOverflowDisplay.gameObject.SetActive(false);
            int currentStocks = StockImageHolder.transform.childCount;
            //deepseek code starts
            for (int i = currentStocks; i < remainingLives; i++)
            {
                Instantiate(stockImage, StockImageHolder.transform);
            }

            for (int i = currentStocks; i > remainingLives; i--)
            {
                Destroy(StockImageHolder.transform.GetChild(StockImageHolder.transform.childCount - 1).gameObject);
            }
            //deepseek code ends

        }
       
        }
    

    void SetPercentageDisplay(float damage, int stunTime)
    {
        percentageTracker.text = Mathf.RoundToInt(playerHealth.GetHealth()).ToString();
    }

  

}
