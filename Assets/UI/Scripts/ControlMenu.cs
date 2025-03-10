using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using NUnit.Framework.Interfaces;
using System.Collections;
using Unity.AppUI.UI;
[System.Serializable]
public class ControlMenu : MonoBehaviour
{

    [SerializeField] List<InputActionAsset> playerControlSchemes = new List<InputActionAsset>();

    [SerializeField] GameObject ControlContent;

    string _lastKeyPressed;

    int _playerIndex;

    string _controlToChange;

    bool _listeningForKey = false;

  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeDisplayedControls(1);
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
     
        _playerIndex = index;

        InputActionAsset asset = playerControlSchemes[index - 1];

        //actionmap 0 is battle controls

        foreach (InputAction action in asset.actionMaps[0].actions)
        {
        
            
     
            Transform targetControl = ControlContent.transform.Find(action.name).transform;

            for (int i = 0; i < targetControl.childCount; i++)
            {

                Transform child = targetControl.GetChild(i);

                for (int x = 0; x < child.childCount; x++)
                {

                    Transform control = child.GetChild(x);  
                    TMP_Text text = control.GetComponent<TMP_Text>();
                    text.text = action.controls[i + x].displayName;
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
        StartCoroutine(RebindCompositeControl(), index);
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

}
