using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuFunctions : MonoBehaviour
{
    public GameObject _pauseMenu;
    public Slider _brightness;
    public Slider _volume;
    public SpriteRenderer _overLay;
    public AudioSource _bgmPlayer;

   [SerializeField] bool isLocked = false;
    //reusing the scene for the main menu, don't want the player to be able to turn it off
    //if its locked its always active

    private void Start()
    {
        if (!isLocked)
        {
            _pauseMenu.SetActive(false);
        }
        if (GameManager.instance)
        {
            _brightness.value = (float)GameManager.instance.GetGameSetting("Brightness");
            _volume.value = (float)GameManager.instance.GetGameSetting("Volume");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isLocked)
        {
            SwitchState();
        }

        DarkOverlay();
        VolumeControl();
    }

    void SwitchState()
    {
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
    }

    void DisableMenu()
    {
        _pauseMenu.SetActive(false);
    }

    void VolumeControl()
    {
        if (_bgmPlayer)
        {
            _bgmPlayer.volume = _volume.value;
            if (GameManager.instance)
            {
                GameManager.instance.SetGameSetting("Volume", _volume.value);
            }
        }
    }

    void DarkOverlay()
    {
        if (_overLay)
        {
            var tempColor = _overLay.color;
            tempColor.a = 1 - _brightness.value;
            _overLay.color = tempColor;

            if (GameManager.instance)
            {
                GameManager.instance.SetGameSetting("Brightness", _brightness.value);
            }
        }
    }

    public void ExitToMainMenu()
    {
        Debug.Log("Pause menu is trying to leave");
        GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
        bool foundManager = false;
        if (levelManager != null)
        {
           if (levelManager.TryGetComponent<LevelManager>(out LevelManager manager))
            {
                foundManager = true;
                StartCoroutine(manager.EndGame());

            }
        }
        if (!foundManager)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
