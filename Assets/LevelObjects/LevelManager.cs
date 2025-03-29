using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{

    [SerializeField] PlayerController _playerPrefab;

    float levelTime = 0.0f;
    Dictionary<int, PlayerController> _playerList = new Dictionary<int, PlayerController>();

    List<PlayerController> _activePlayers = new List<PlayerController>();

    //2 different lists to keep track of players who are still in the match vs those who lost.

    [SerializeField] List<Transform> spawnLocations = new List<Transform>();

    [SerializeField] CinemachineTargetGroup cinemachineFramer;
    [SerializeField] CinemachineGroupFraming _cinemachineGrouper;

    [SerializeField] PlayerHUD hudPrefab;

    [SerializeField] GameObject hudHolder;


    [HideInInspector]

    Dictionary<PlayerController, bool> playerRespawned = new();

    AnnouncementSystem announcementSystem;
    private void Start()
    {
        if (GameManager.instance != null)
        {
            addPlayers(GameManager.instance.GetPlayerCount());

        }
        else
        {
            addPlayers(2);
        }

    }

    void InitRespawnEvents()
    {
        for (int i = 0; i < _playerList.Count; i++)
        {
            PlayerController p = _playerList[i];
            p.playerRespawned.AddListener(SetRespawnEvent);
        }
    }

    void SetRespawnEvent(PlayerController player)
    {
        playerRespawned[player] = true;
    }


    public void addPlayers(int numberOfPlayers = 2)
    {
        Time.timeScale = 0.0f;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            _playerList[i] = Instantiate(_playerPrefab);
            playerRespawned.Add(_playerList[i], false);
            _playerList[i].EnablePlayer(i + 1);
            _activePlayers.Add(_playerList[i]);


            SetPlayerLocation(_playerList[i], i);
            SetPlayerID(_playerList[i], i);
            InitPlayerEvents(_playerList[i]);
            AddNewHud(_playerList[i]);

            cinemachineFramer.AddMember(_playerList[i].transform, 1, 0.5f);
            //playerInput.currentActionMap = playerInput.action
        }
        InitRespawnEvents();
       InitAnnoucementSystem();
        _cinemachineGrouper.enabled = true;

      StartCoroutine(  StartGame());
    }

    void DisableAllPlayers()
    {
        foreach (PlayerController player in _activePlayers)
        {
            player._playerInput.actions.Disable();
        }
    }

    void EnableAllPlayers()
    {
        foreach (PlayerController player in _activePlayers)
        {
            player._playerInput.actions.Enable();
        }
    }
    void InitAnnoucementSystem()
    {
        announcementSystem = GameObject.FindGameObjectWithTag("AnnouncementSystem").GetComponent<AnnouncementSystem>();
    }

    IEnumerator StartGame()
    {
        DisableAllPlayers();
        Time.timeScale = 0.0f;
        yield return StartCoroutine( announcementSystem.StartAnnouncement("3", 1.0f, false));
        yield return StartCoroutine( announcementSystem.StartAnnouncement("2", 1.0f, false));
        yield return StartCoroutine(announcementSystem.StartAnnouncement("1", 1.0f, false));
        Time.timeScale = 1.0f;
        EnableAllPlayers();
        StartCoroutine(  announcementSystem.StartAnnouncement("E N G A G E", 1.5f, false) );
    }
    void AddNewHud(PlayerController player)
    {
        PlayerHUD newHud = Instantiate(hudPrefab, hudHolder.transform);
        newHud.InitPlayerHUD(player);
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
        player.GetComponent<HealthComponent>().livesChanged.AddListener(OnPlayerDefeated);
        player.playerEliminated.AddListener(OnPlayerEliminated);
    }

    private void Update()
    {
        levelTime += Time.deltaTime;
    }

    public void OnLevelFinished()
    {
    }

    public IEnumerator OnGameFinished()
    {
        yield return StartCoroutine( announcementSystem.StartAnnouncement("PLAYER " + _activePlayers[0].playerIndex + " WINS", 2.5f, true));
        StartCoroutine (EndGame());
    }
    public void OnPlayerEliminated(PlayerController player)
    {
        int index = _activePlayers.IndexOf(player);
        cinemachineFramer.RemoveMember(_activePlayers[index].transform);
        _activePlayers.RemoveAt(index);
        player.gameObject.SetActive(false);

        if (_activePlayers.Count == 1)
        {
            StartCoroutine(OnGameFinished());
        }
        

    }
    public void OnPlayerDefeated(PlayerController player, int lives)
    {
        cinemachineFramer.RemoveMember(player.transform);
        if (player.activeGrapple)
        {
            Destroy(player.activeGrapple);
        }
        if (_activePlayers.Count == 2)
        {
            int playerOneLives = _playerList[0].GetComponent<HealthComponent>().GetRemainingLives();
            int playerTwoLives = _playerList[1].GetComponent<HealthComponent>().GetRemainingLives();
            StartCoroutine(announcementSystem.StartAnnouncement(playerOneLives + " | " + playerTwoLives, PlayerController.GetRespawnDelay(), false));
        }
        player._playerInput.actions.Disable();
        StartCoroutine(AddPlayerBackAfterKO(player));
    }

    IEnumerator AddPlayerBackAfterKO(PlayerController player)
    {
        yield return new WaitUntil(() => playerRespawned[player] == true);
        cinemachineFramer.AddMember(player.transform, 1, 0.5f);
        playerRespawned[player] = false;
        player._playerInput.actions.Enable();
    }

    public void DestroyPlayer(PlayerController player)
    {
        player._playerInput.DeactivateInput();
        player.playerEliminated.RemoveAllListeners();
        player.GetComponent<HealthComponent>().onEntityDamaged.RemoveAllListeners();
        _activePlayers.Remove(player);
        Destroy(player.gameObject);
    }

    public IEnumerator EndGame()
    {
        for (int i = _playerList.Count - 1; i > 0; i++)
        {
           DestroyPlayer(_playerList[i]);
        }
        _playerList.Clear();
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnTimerOver()
    {
        StartCoroutine(TimerOver());
    }

    IEnumerator TimerOver ()
    {
        int mostLives = int.MinValue;
        List<PlayerController> winningPlayers = new List<PlayerController>();
        foreach (PlayerController player in _activePlayers)
        {
            HealthComponent healthComponent = player.GetComponent<HealthComponent>();
            int lives = healthComponent.GetRemainingLives();

            Debug.Log("Looking at " + player.name + " which has " + lives + " remaining");  
            if (lives >= mostLives)
            {
                winningPlayers.Add(player);
                mostLives = lives;
            }
        }
        foreach (PlayerController player in winningPlayers)
        {
            player.GetComponent<HealthComponent>().livesChanged.RemoveListener(OnPlayerDefeated);
            //remove listener so that the 1v1 text doesn't pop up
        }
        //if there's more then 1, there's a tie, time for sudden death
        if (winningPlayers.Count > 1)
        {
            yield return announcementSystem.StartAnnouncement("TIME", 2.0f, true);
            StartSuddenDeath(winningPlayers);
        }
        else
        {
          StartCoroutine(OnGameFinished());
        }
    }

    void StartSuddenDeath(List<PlayerController> winningPlayers)
    {
        foreach (PlayerController player in _playerList.Values)
        {
            if (!winningPlayers.Contains(player))
            {
                DestroyPlayer(player);
            }
        }

        foreach (GameObject moveableObject in GameObject.FindGameObjectsWithTag("MoveableObject"))
        {
            Destroy(moveableObject);
        }

        foreach (GameObject itemDropper in GameObject.FindGameObjectsWithTag("ItemDropper"))
        {
            ObjectSpawner positionalSpawner = itemDropper.GetComponent<ObjectSpawner>();
            if (positionalSpawner != null)
            {
                positionalSpawner.StopAllCoroutines();
                positionalSpawner.StartCoroutine(positionalSpawner.SpawnObject());
            }
            else
            {
                ItemHolder dyanmicSpawner = itemDropper.GetComponent<ItemHolder>();
                if (dyanmicSpawner != null)
                {
                    dyanmicSpawner.ResetDropper();
                }
            }
        }
        int index = 0;
        foreach (var player in winningPlayers)
            {
                HealthComponent hp = player.GetComponent<HealthComponent>();
                hp.Damage(Vector2.zero, 999);
                //die in 1 hit
                hp.SetRemainingLives(1);
                //no more respawning
                player.transform.position = spawnLocations[index].transform.position;
            index++;
            }
        }
    }
