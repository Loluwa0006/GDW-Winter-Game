using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public const int MAX_PLAYERS = 4;

    int numberOfPlayers = 2;

    string currentLevel = "PlatformStage1";

    [SerializeField] AudioSource BGMPlayer;


    Dictionary<string, object> gameSettings = new Dictionary<string, object>();
    Dictionary<string, object> matchSettings = new Dictionary<string, object>();


    
    public class PlayerData
    {
        public InputActionAsset controls;
        public PlayerController.TetherPresets selectedTether = PlayerController.TetherPresets.CLASSIC;
        public int index = 0;
        public string pName = "Player 1";
    }
    List<PlayerData> playerData = new List<PlayerData>();   


  
    private void Awake()
    {
       
        if( (instance != this) && (instance != null) )
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        DontDestroyOnLoad(BGMPlayer.gameObject);
        SceneManager.sceneLoaded += EndBGM;
        InitSettings();

        for (int i = 0; i < MAX_PLAYERS; i++)
        {
            PlayerData data = new PlayerData();
            playerData.Add(data);
        }
    }

    void EndBGM(Scene scene, LoadSceneMode sceneMode)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "StageSelect")
        {
            BGMPlayer.Stop();
        }
    }
    public static GameManager getManager()
    {
        return instance;
    }
    void InitSettings()
    {
        gameSettings.Add("Brightness", 1.0f);
        gameSettings.Add("Volume", 0.0f);

        matchSettings.Add("MatchDuration", 7);
        //save match duration as minutes
        matchSettings.Add("CameraShake", false);
        matchSettings.Add("StockCount", 3);
        matchSettings.Add("UseTimer", false);
        matchSettings.Add("TeamBattle", false);

     
        //including team battles because there might be a use case in the future
    }

    public void SetPlayers(int num)
    {
        numberOfPlayers = Mathf.Clamp(num, 2, MAX_PLAYERS);
    }

    public void SetPlayerTether(int index, PlayerController.TetherPresets preset)
    {
        playerData[index].selectedTether = preset;
    }

    public PlayerController.TetherPresets GetPlayerTether(int index)
    {
        return playerData[index].selectedTether;
    }

    public int GetPlayerCount()
    {
        return numberOfPlayers;
    }

    public string GetSelectedLevel()
    {
        return currentLevel;
    }

    public void SetSelectedLevel(string newLevel)
    {
        currentLevel = newLevel;
    }

    public object GetGameSetting(string settingName)
    {
        if (gameSettings.ContainsKey(settingName))
        {
            return gameSettings[settingName];
        }
        Debug.LogWarning("Could not find setting " +  settingName + " in game settings.");
        return null;

    }


    public object GetMatchSetting(string settingName)
    {
        if (matchSettings.ContainsKey(settingName))
        {
            return matchSettings[settingName];
        }
        Debug.LogWarning("Could not find setting " + settingName + " in match settings.") ;
        return null;

    }

    public bool SetMatchSetting(string settingName, object value)
    {
        
        if (matchSettings.ContainsKey(settingName))
        {
            matchSettings[settingName] = value;
            return true;
        }
        Debug.LogWarning("Could not set setting (" + settingName + ") in match settings"); 
        return false;
    }

    public bool SetGameSetting(string settingName, object value)
    {
       

        if (gameSettings.ContainsKey(settingName))
        {
            gameSettings[settingName] = value;
            return true;
        }
        Debug.LogWarning("Could not set setting " + settingName + " in game settings");
        return false;
    }
}

