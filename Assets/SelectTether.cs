using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SelectTether : MonoBehaviour
{
    public int index = 1;

   [SerializeField] Button selectedButton;

    private void Awake()
    {
        SetTether(selectedButton);
    }

    public void SetTether(Button button)
    {
        if (GameManager.instance == null)
        {
            Debug.Log("Couldn't find game manager");
            return;
        }
        PlayerController.TetherPresets preset = PlayerController.TetherPresets.CLASSIC;

        switch (button.name)
        {
            case "Slingshot":
                preset = PlayerController.TetherPresets.SLINGSHOT;
                break;
            case "Surge":
                preset = PlayerController.TetherPresets.CHARGE;
                break;
            case "Charge":
                preset = PlayerController.TetherPresets.CHARGE;
                break;
            default:
                preset = PlayerController.TetherPresets.CLASSIC;
                break;
        }

        GameManager.instance.SetPlayerTether(index, preset);
        SetSelectedButton(button);
    }

     void SetSelectedButton(Button button)
     {
        selectedButton.GetComponentInChildren<Image>().color = Color.white;
        Debug.Log("Looking at button " + button.name);
        button.GetComponent<Image>().color = Color.green;
        selectedButton = button;
     }
}
