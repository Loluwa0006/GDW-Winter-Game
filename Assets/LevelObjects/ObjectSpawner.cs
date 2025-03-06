using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Timer))]
public class ObjectSpawner : MonoBehaviour
{
    public UnityEvent spawnedObject = new UnityEvent();
    [SerializeField] List<MoveableObject> objectPrefabs = new List<MoveableObject>();
    Timer spawnTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        spawnTimer = GetComponent<Timer>();
    }

    private void Start()
    {
        spawnTimer.timerOver.AddListener(SpawnObject);
    }

    private void FixedUpdate()
    {
        Debug.Log("Spawn timer wait time = " + spawnTimer.timeRemaining().ToString());
    }

    public void SpawnObject()
    {
        Debug.Log("Spawning object");
        int randomIndex = Random.Range(0, objectPrefabs.Count);

        MoveableObject newObject = Instantiate(objectPrefabs[randomIndex]);
        newObject.transform.position = transform.position;

        spawnedObject.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
