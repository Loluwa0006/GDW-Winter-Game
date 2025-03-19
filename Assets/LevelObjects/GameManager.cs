using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    const int MAX_PLAYERS = 4;

    int numberOfPlayers = 2;

    string currentLevel = "PlatformStage1";


    Dictionary<string, object> gameSettings = new Dictionary<string, object>();
    Dictionary<string, object> matchSettings = new Dictionary<string, object>();




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
            DontDestroyOnLoad(gameObject);
        }

        InitSettings();
    }
    public static GameManager getManager()
    {
        return instance;
    }
    void InitSettings()
    {
        gameSettings.Add("Brightness", 1.0f);
        gameSettings.Add("Volume", 1.0f);
        gameSettings.Add("TimerDuration", 60f * 7/*i don't feel like doing math */ );


        matchSettings.Add("CameraShake", false);
        matchSettings.Add("StockCount", 3);
        matchSettings.Add("UseTimer", false);
        matchSettings.Add("TeamBattle", false);

     
        //including team battles because there might be a use case in the future
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

