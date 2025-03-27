using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Cinemachine;
using System.Collections;
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
    public UnityEvent StartedSuddenDeath = new ();

    Dictionary<PlayerController, bool> playerRespawned = new();
   
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
        _cinemachineGrouper.enabled = true;
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
        player.playerEliminated.AddListener(OnPlayerEliminated);
        player.GetComponent<HealthComponent>().livesChanged.AddListener(OnPlayerDefeated);
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

    public void OnPlayerEliminated(PlayerController player)
    {
        int index = _activePlayers.IndexOf(player);
        //need to shift index backwards to account for data structures starting at 0
        cinemachineFramer.RemoveMember(_activePlayers[index].transform);
        Debug.Log(_activePlayers[index].gameObject.name + " defeated");
        _activePlayers.RemoveAt(index);


        if (_activePlayers.Count == 1)
        {
            OnGameFinished();
        }

    }
    public void OnPlayerDefeated(PlayerController player, int lives)
    {
        cinemachineFramer.RemoveMember(player.transform);
        if (player.activeGrapple)
        {
            Destroy(player.activeGrapple);
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

    public void EndGame()
    {
        for (int i = _playerList.Count - 1; i > 0; i++)
        {
           DestroyPlayer(_playerList[i]);
        }
        _playerList.Clear();
    }

    public void OnTimerOver()
    {
        StartCoroutine(TimerOver());
    }

    IEnumerator TimerOver ()
    {
        int mostLives = -9999;
        foreach (var player in _activePlayers)
        {
            HealthComponent healthComponent = player.GetComponent<HealthComponent>();
            int lives = healthComponent.GetRemainingLives();
            if (lives >= mostLives)
            {
                mostLives = lives;
                Debug.Log(player.name + " has " + lives.ToString() + " lives remaining");
            }
            else
            {
                _activePlayers.Remove(player);
            }

        }

        //if there's more then 1, there's a tie, time for sudden death
        if (_activePlayers.Count > 1)
        {
            Time.timeScale = 0.15f;
            yield return new WaitForSecondsRealtime (2);
            Time.timeScale = 1.0f;
            StartedSuddenDeath.Invoke();
            StartSuddenDeath();
        }
    }

    void StartSuddenDeath()
    {
        foreach (var player in _playerList.Values)
        {
            if (!_activePlayers.Contains(player))
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
        foreach (var player in _activePlayers)
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
