using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMP_Text percentageTracker;
    [SerializeField] TMP_Text playerDisplayName;
    [SerializeField] TMP_Text stockOverflowDisplay;
    [SerializeField] GameObject StockImageHolder;
    [SerializeField] GameObject stockImage;
    [SerializeField] Image HUDBackground;
    [SerializeField] RawImage TetherIcon;

   [SerializeField] List<Color32> HUDColors;

    HealthComponent playerHealth;

    const int  MAX_LIVES_TO_DISPLAY = 6;

    const int MIN_FONT_SIZE = 36;
    const int MAX_FONT_SIZE = 50;

    [SerializeField] List<Texture> tetherIcons = new();

    
    public void InitPlayerHUD(PlayerController player)
    {
        playerHealth = player.GetComponent<HealthComponent>();
        playerDisplayName.text = player.name;

        SetLifeDisplay(player, playerHealth.GetRemainingLives());


        HUDBackground.color = player.playerSprite.GetComponent<SpriteRenderer>().color;

        AddHUDListeners(playerHealth, player);

        SetTetherDisplay(player.selectedTether);
    }

    void SetTetherDisplay(PlayerController.TetherPresets preset)
    {
        Debug.Log("There are " + tetherIcons.Count + " icons");
        int presetIndex = (int)preset;
        Debug.Log("Looking at preset " + presetIndex );


        TetherIcon.texture = tetherIcons[presetIndex];
        
    }

    void AddHUDListeners(HealthComponent health, PlayerController player)
    {
        health.healthInitalized.AddListener(SetLifeDisplay);
        health.onPlayerInjured.AddListener((info) => SetPercentageDisplay());
        health.onEntityHealed.AddListener( (somefloat, someint) /* 5 am coding gang rise up */ => SetPercentageDisplay());
        player.playerEliminated.AddListener(EndHUD);

        playerHealth.livesChanged.AddListener((victim, lives, killer) => SetLifeDisplay(victim, lives));
        playerHealth.livesChanged.AddListener((victim, lives, killer) => SetPercentageDisplay());
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
            if (currentStocks < remainingLives)
            {
                for (int i = currentStocks; i < remainingLives; i++)
                {
                    Instantiate(stockImage, StockImageHolder.transform);
                }
            }
            else
            {
                for (int i = currentStocks; i > remainingLives; i--)
                {
                    Destroy(StockImageHolder.transform.GetChild(StockImageHolder.transform.childCount - 1).gameObject);
                }
            }
            //deepseek code ends

        }
       
        }

    void SetPercentageDisplay()
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
