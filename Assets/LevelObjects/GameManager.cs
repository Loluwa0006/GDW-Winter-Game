using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const int MAX_PLAYERS = 4;

    int numberOfPlayers = 2;

    PlayerController.GrapplePresets[] playerSelectedTether = new PlayerController.GrapplePresets[MAX_PLAYERS];
    private void Awake()
    {
        if( (instance != this) && (instance != null) )
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void SetPlayers(int num)
    {
        numberOfPlayers = num;
    }

    public void SetPlayerTether(int index, PlayerController.GrapplePresets grappleType)
    {
        playerSelectedTether[index - 1] = grappleType;
    }

    public int GetPlayerCount()
    {
        return numberOfPlayers;
    }
}
