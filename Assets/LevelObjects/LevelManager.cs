using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro.EditorUtilities;
using UnityEngine.InputSystem;
public class LevelManager : MonoBehaviour
{
    List<IntelObject> intelObjects = new List<IntelObject>();
    [SerializeField] GameObject intelUI;
    [SerializeField] PlayerController _playerPrefab;

    [SerializeField] int intelRequired = 1;

    [SerializeField] GameObject exitArea;

    float levelTime = 0.0f;
    Dictionary<int, PlayerController> _playerList = new Dictionary<int, PlayerController>();

    List<PlayerController> _activePlayers = new List<PlayerController>();

    //2 different lists to keep track of players who are still in the match vs those who lost.

    [SerializeField] List<Transform> spawnLocations = new List<Transform>();




    private void Awake()
    {
        
        //exitArea.SetActive(false);

    }
    private void Start()
    {
        addPlayers(2);
    }

     
    
    public void addPlayers(int numberOfPlayers = 2) 
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            _playerList[i] = Instantiate(_playerPrefab);
            _playerList[i].EnablePlayer(i + 1);
            _playerList[i].transform.position = spawnLocations[i].transform.position;
            _playerList[i]._respawnPoint = spawnLocations[i].transform;
            _activePlayers.Add(_playerList[i]);
            
            //playerInput.currentActionMap = playerInput.action
            
        }
    }
    public void OnIntelSecured(IntelObject intel, Sprite intelIcon)
    {
        intelObjects.Add(intel);

        if (intelUI != null)
        {
            RawImage rawImage = new GameObject().AddComponent<RawImage>();

            rawImage.transform.parent = intelUI.transform;
        }
        if (intelObjects.Count >= intelRequired) 
        { 
            exitArea.SetActive (true);
        }

    }

    private void Update()
    {
        
            levelTime += Time.deltaTime;
        
    }

    public void OnLevelFinished()
    {
        Debug.Log("Finished Level in " + levelTime.ToString() + " seconds");
        Time.timeScale = 0.0f;
    }

    public void OnGameFinished()
    {
        
    }

    public void OnPlayerDefeated(int index)
    {
        _playerList.Remove(index);

    }
}
