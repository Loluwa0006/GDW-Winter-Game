using UnityEngine;
using UnityEngine.UI;

public class SelectTether : MonoBehaviour
{
    public int index = 1;

  
    public void SetTether(Button button)
    {
        if (GameManager.instance == null)
        {
            Debug.Log("Couldn't find game manager");
            return;
        }
        PlayerController.TetherPresets preset = PlayerController.TetherPresets.CLASSIC;

        Debug.Log("name of button is " + button.name);
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
        Debug.Log("preset will now be " + preset.ToString());
        GameManager.instance.SetPlayerTether(index, preset);
        SetSelectedButton(button);
    }

     void SetSelectedButton(Button button)
    {
        foreach (Transform i in transform)
        {
            i.GetComponent<Image>().color = Color.white;
        }
        button.GetComponent<Image>().color = Color.green;
    }
}
