using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class PauseMenuFunctions : MonoBehaviour
{
    public GameObject _pauseMenu;
    public Slider _brightness;
    public Slider _volume;
    public SpriteRenderer _overLay;
    public AudioSource _bgmPlayer;

    bool _isPauseMenuActive = false;

    private void Start()
    {
        _pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SwichToPause();
        }

        DarkOverlay();
        VolumeControl();
    }

    void SwichToPause()
    {
        if (!_isPauseMenuActive)
        {
            _pauseMenu.SetActive(true);
            _isPauseMenuActive = true;
        }
        else if (_isPauseMenuActive)
        {
            _pauseMenu.SetActive(false);
            _isPauseMenuActive = false;
        }
    }

    void DisableMenu()
    {
        _pauseMenu.SetActive(false);
    }

    void VolumeControl()
    {
        _bgmPlayer.volume = _volume.value;
    }

    void DarkOverlay()
    {
        var tempColor = _overLay.color;
        tempColor.a = (1 - _brightness.value);
        _overLay.color = tempColor;
    }
}
