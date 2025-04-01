using System;
using UnityEngine;

public class Slime : MonoBehaviour
{

    [SerializeField] TutorialManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
            Debug.Log("Tutorial manager is now " + manager.name);
        }
    }
    private void OnDestroy()
    {
        manager.conditionTracker += 1;
        Debug.Log("Adding 1 to condition Tracker");
    }

}
