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

    const int MIN_FONT_SIZE = 36;
    const int MAX_FONT_SIZE = 50;

    [SerializeField] List<Sprite> tetherIcons = new List<Sprite>();
    public void InitPlayerHUD(PlayerController player)
    {
        playerHealth = player.GetComponent<HealthComponent>();
        playerDisplayName.text = player.name;

        SetLifeDisplay(player, playerHealth.GetRemainingLives());

        SetPercentageDisplay(playerHealth.GetHealth(), 0, 0, Vector2.zero);

        HUDBackground.color = player.playerSprite.GetComponent<SpriteRenderer>().color;

        AddHUDListeners(playerHealth, player);
    }

    void AddHUDListeners(HealthComponent health, PlayerController player)
    {
        health.healthInitalized.AddListener(SetLifeDisplay);
        health.onEntityDamaged.AddListener(SetPercentageDisplay);
        health.onEntityHealed.AddListener(SetPercentageDisplay);
        player.playerEliminated.AddListener(EndHUD);


        playerHealth.livesChanged.AddListener(SetLifeDisplay);
        playerHealth.livesChanged.AddListener((player, lives) => SetPercentageDisplay(0, 0, 0, Vector2.zero));

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
    

    void SetPercentageDisplay(float damage, int stunTime, float shakeAmount, Vector2 knockback)
    {
        float hp = playerHealth.GetHealth();
        percentageTracker.text = Mathf.RoundToInt(hp).ToString();
        SetPercentageText(hp);
    }

    void SetPercentageDisplay(float damage, int stunTime)
    {
        float hp = playerHealth.GetHealth();
        percentageTracker.text = Mathf.RoundToInt(hp).ToString();
        SetPercentageText(hp);
    }
    
    void SetPercentageText(float dmg)
    {
        float transition = Mathf.Clamp01(dmg / 150.0f);
        Debug.Log("Transition is now " + transition.ToString());
        percentageTracker.color = Color.Lerp(Color.white, Color.red, transition);
        
        
        percentageTracker.fontSize = Mathf.Lerp(MIN_FONT_SIZE, MAX_FONT_SIZE, transition);
    }



}
