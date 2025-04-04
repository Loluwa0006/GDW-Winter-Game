using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.UI;
[System.Serializable]
public class ControlMenu : MonoBehaviour
{

    [SerializeField] List<InputActionAsset> playerControlSchemes = new List<InputActionAsset>();

    [SerializeField] GameObject ControlContent;

    string _lastKeyPressed;

    int _playerIndex;

    string _controlToChange;

    bool _listeningForKey = false;

    [SerializeField] GameObject InputDevices;
    [SerializeField] GameObject GamepadDevices;

    [SerializeField] PlayerInput playerInput;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ChangeDisplayedControls(1);
        InputSystem.onDeviceChange += SetGamepadButtons;
    }

    private void Start()
    {
        SetGamepadButtons(null, InputDeviceChange.Removed);
    }
    // Update is called once per frame
    void Update()
    {
        if (_listeningForKey) { 
        foreach (var keys in Keyboard.current.allKeys)
        {
            if (keys.wasPressedThisFrame)
            {
                _lastKeyPressed = keys.path;
            }
        }
    }
    }

    public void ChangeDisplayedControls(int index)
    {

        OnPlayerIndexChanged(index);

        InputActionAsset asset = playerControlSchemes[index - 1];

        //actionmap 0 is battle controls

        
       
        foreach (InputAction action in asset.actionMaps[0].actions)
        {
            if (action.name == "DropDown")
            {
                continue;
                //drop down is just down twice
            }
            Transform targetControl = ControlContent.transform.Find(action.name).transform;
            if (targetControl)
            {
                for (int i = 0; i < targetControl.childCount; i++)
                {
                    Transform child = targetControl.GetChild(i);
                    for (int x = 0; x < child.childCount; x++)
                    {
                        Transform control = child.GetChild(x);
                        TMP_Text text = control.GetComponent<TMP_Text>();
                        Debug.Log(action.controls[i + x].path);
                        text.text = action.controls[i + x].displayName;
                    }

                }
            }
        }
    }
    public void StartRebind(string control)
    {
        _controlToChange = control;
        _listeningForKey = true;
        StartCoroutine(RebindControls());
    }

    public void StartRebindComposite(string control, int index)
    {
        _controlToChange = control;
        _listeningForKey = true;
       // StartCoroutine(RebindCompositeControl(), index);
    }
    public IEnumerator RebindControls()
    {


        yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame);

        if (_lastKeyPressed == "<Keyboard>/escape")
        {
            _listeningForKey = false;
            yield return null;
        }

        InputActionAsset control = playerControlSchemes[_playerIndex - 1];

        InputAction action = control.FindAction(_controlToChange);

        if (_lastKeyPressed != string.Empty)
        {
            action.ChangeBinding(0).WithPath(_lastKeyPressed);
        }
        ChangeDisplayedControls(_playerIndex);
        _listeningForKey = false;
    }

    public IEnumerable RebindCompositeControl(int index)
    {

        yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame);

        if (_lastKeyPressed == "<Keyboard>/escape")
        {
            _listeningForKey = false;
            yield return null;
        }

        InputActionAsset control = playerControlSchemes[_playerIndex - 1];

        InputAction action = control.FindAction(_controlToChange);

        if (_lastKeyPressed != string.Empty)
        {
            action.ChangeBinding(index).WithPath(_lastKeyPressed);
        }
        ChangeDisplayedControls(_playerIndex);
        _listeningForKey = false;
    }


    void SetGamepadButtons(InputDevice device, InputDeviceChange change)
    {

        if (Gamepad.all.Count > GameManager.MAX_PLAYERS)
        {
            return;
        }

        foreach (Transform transform in GamepadDevices.transform)
        {
            transform.gameObject.SetActive(false);
        }
       for (int i = 0; i < Gamepad.all.Count; i++)
        {
            GamepadDevices.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void SetGamepadForPlayer(Button button)
    {

        int pad = int.Parse(button.name);
        Gamepad selectedPad = Gamepad.all[pad - 1]; //forgot to start at 1 in the editor, this is way faster, oops

        GameManager.instance.SetPlayerDevice(_playerIndex, selectedPad);

        SetDeviceColors();
        button.GetComponent<Image>().color = Color.green;
    }

    public void SetKeyboardForPlayer(Button button)
    {
        SetDeviceColors();
        button.GetComponent<Image>().color = Color.green;

        GameManager.instance.SetPlayerDevice(_playerIndex, Keyboard.current);
    }

    public void SetDeviceColors()
    {
        for (int i = 0; i < GamepadDevices.transform.childCount; i++)
        {
            GamepadDevices.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    public void SetDeviceStatus(Button keyboardButton)
    {
        SetDeviceColors();
        var device = GameManager.instance.GetPlayerDevice(_playerIndex);

        if (device == Keyboard.current)
        {
            keyboardButton.GetComponent<Image>().color = Color.white;
        }
        else
        {
            int index = 0;
            foreach (Gamepad pad in Gamepad.all)
            {
                if (pad == device)
                {
                    break;
                }
                index++;
            }
            GamepadDevices.transform.GetChild(index).GetComponent<Image>().color = Color.green;
        }
    }
    public void OnPlayerIndexChanged(int index)
    {
         _playerIndex = index;
       // UseKeyboard.gameObject.SetActive(index <= 2);
    }

   
}
