using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro.EditorUtilities;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
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

    [SerializeField] CinemachineTargetGroup cinemachineFramer;



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
            _activePlayers.Add(_playerList[i]);


            SetPlayerLocation(_playerList[i], i);
            SetPlayerID(_playerList[i], i);
            InitPlayerEvents(_playerList[i]);

            cinemachineFramer.AddMember(_playerList[i].transform, 1, 0.5f);

            //playerInput.currentActionMap = playerInput.action

        }
    }

    void SetPlayerID(PlayerController player, int index)
    {
        player.gameObject.name = "Player " + (index + 1).ToString();
        player.playerIndex = (index + 1);
    }
    void SetPlayerLocation(PlayerController player, int index)
    {
        player.transform.position = spawnLocations[index].transform.position;
        player._respawnPoint = spawnLocations[index].transform;
    }
    void InitPlayerEvents(PlayerController player)
    {
        player.playerEliminated.AddListener(OnPlayerEliminated);
        player.playerDead.AddListener(OnPlayerDefeated);
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
            exitArea.SetActive(true);
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
        Debug.Log("----------------------------------------------------");
        foreach (PlayerController p in _activePlayers)
        {
            Debug.Log(p.transform.name);
        }
        Debug.Log("----------------------------------------------------");

        Debug.Log(_activePlayers.ToArray()[0].transform.name + " wins!");
        Time.timeScale = 0.0f;
    }

    public void OnPlayerEliminated(int index)
    {
        index -= 1;
        //need to shift index backwards to account for data structures starting at 0
        cinemachineFramer.RemoveMember(_activePlayers[index].transform);



        if (_activePlayers.Count == 1)
        {
            OnGameFinished();
        }
        Debug.Log(_activePlayers[index].gameObject.name + " defeated");
        _activePlayers.RemoveAt(index);
    }
    public void OnPlayerDefeated(int index)
    {

    }
}