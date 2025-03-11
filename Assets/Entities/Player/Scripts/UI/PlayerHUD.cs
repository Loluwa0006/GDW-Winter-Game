using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TMP_Text percentageTracker;
    [SerializeField] TMP_Text playerDisplayName;
    [SerializeField] TMP_Text stockOverflowDisplay;
    [SerializeField] GameObject StockImageHolder;
    [SerializeField] GameObject stockImage;

    HealthComponent playerHealth;

    const int maxLivesToDisplay = 6;
    public void initPlayerHUD(PlayerController player)
    {
        playerHealth = player.GetComponent<HealthComponent>();
        playerHealth.onEntityDamaged.AddListener(SetPercentageDisplay);
        playerHealth.onEntityHealed.AddListener(SetPercentageDisplay);
        player.playerEliminated.AddListener(EndHUD);
        playerDisplayName.text = player.name;

        SetLifeDisplay(player._remainingLives);
        SetPercentageDisplay(playerHealth.getHealth(), 0);

        player.playerDead.AddListener(SetLifeDisplay);
    }

    void EndHUD(int playerIndex)
    {
        percentageTracker.text = "";
        SetLifeDisplay(0);
    }

    void SetLifeDisplay(int remainingLives) 
    {
       
        if (remainingLives <= 0)
        {
            foreach(Transform child in StockImageHolder.transform)
            {
                Destroy(child.gameObject);
            }
            return;
        }

        if (remainingLives > maxLivesToDisplay)
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
            while (StockImageHolder.transform.childCount < remainingLives)
            {
                GameObject newImage = Instantiate(stockImage, StockImageHolder.transform);
            }




            for (int i = StockImageHolder.transform.childCount - 1; i >= remainingLives; i--)
            {
                Destroy(StockImageHolder.transform.GetChild(i).gameObject);
            }
        }
        }
    

    void SetPercentageDisplay(float damage, int stunTime)
    {
        percentageTracker.text = Mathf.RoundToInt(playerHealth.getHealth()).ToString();
    }


}
