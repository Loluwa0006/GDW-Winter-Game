using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using NUnit.Framework.Interfaces;
[System.Serializable]
public class ControlMenu : MonoBehaviour
{

    [SerializeField] List<InputActionAsset> playerControlSchemes = new List<InputActionAsset>();

    [SerializeField] GameObject ControlContent;
    int _currentPlayer = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeDisplayedControls(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDisplayedControls(int index)
    {
        _currentPlayer = index;

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
}
