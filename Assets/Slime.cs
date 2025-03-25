using System;
using UnityEngine;

public class Slime : MonoBehaviour
{

    [SerializeField] TutorialManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnDestroy()
    {
        manager.conditionTracker += 1;
    }
}
