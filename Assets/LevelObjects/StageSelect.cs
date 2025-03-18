using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField] TMP_Text selectedLevel;
     
    Dictionary<string, Image> StageThumbnails = new Dictionary<string, Image>();

    public void SetStage(GameObject level)
    { 
       // GameManager.instance.SetSelectedLevel(level);
        selectedLevel.text = level.name;


    }
}
