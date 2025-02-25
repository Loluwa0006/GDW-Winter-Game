using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    List<IntelObject> intelObjects = new List<IntelObject>();
    [SerializeField] GameObject intelUI;

    [SerializeField] int intelRequired = 1;

    [SerializeField] GameObject exitArea;

    float levelTime = 0.0f;

    private void Awake()
    {
        exitArea.SetActive(false);
    }
    
    public void OnIntelSecured(IntelObject intel, Sprite intelIcon)
    {
        intelObjects.Add(intel);

        if (intelUI != null)
        {
            RawImage rawImage = new GameObject().AddComponent<RawImage>();

            rawImage.transform.parent = intelUI.transform;
        }
        if (intelObjects.Count > 0) 
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
}
