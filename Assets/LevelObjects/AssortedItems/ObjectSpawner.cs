using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;

public class ObjectSpawner : MonoBehaviour
{

    public float cooldown = 6.0f;
    public UnityEvent spawnedObject = new UnityEvent();
    [SerializeField] List<MoveableObject> objectPrefabs = new List<MoveableObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] bool spawnOnMatchStart = false;


    bool stopSpawning = false;

    private void Start()
    {
        if (!spawnOnMatchStart)
        {
            StartCoroutine(StartSpawning());
        }
        else
        {
            StartCoroutine(SpawnObject());
        }
    }
    
    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(cooldown);
        StartCoroutine(SpawnObject());
    }



    private void FixedUpdate()
    {

    }

    public IEnumerator SpawnObject()
    {
        Debug.Log("Spawning object");
        int randomIndex = Random.Range(0, objectPrefabs.Count);

        MoveableObject newObject = Instantiate(objectPrefabs[randomIndex]);
        newObject.transform.position = transform.position;

        spawnedObject.Invoke();
        yield return new WaitForSeconds(cooldown);
        if (stopSpawning )
        {
            yield return null;
        }
        StartCoroutine(SpawnObject());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
