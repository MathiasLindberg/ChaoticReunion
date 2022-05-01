using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects = new();
    [SerializeField] private uint amount;
    // Start is called before the first frame update

    [Header("Periodic Spawn Variables")] 
    [SerializeField] private float spawnFrequency;
    [SerializeField] private float spawnAmount;

    private float _frequencyTimer;
    
    public void Spawn()
    {
        var collider = GetComponent<Collider>();
        var min = collider.bounds.min;
        var max = collider.bounds.max;

        if (gameObjects.Count > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                var gameObj = gameObjects[Random.Range(0, gameObjects.Count - 1)];
                Vector3 spawnPoint = collider.ClosestPoint(new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z)));
                GameObject instance = Instantiate(gameObj);
                instance.transform.position = spawnPoint;
            }
        }
        
        StartCoroutine(KeepSpawning());
    }

    private IEnumerator KeepSpawning()
    {
        var collider = GetComponent<Collider>();
        var min = collider.bounds.min;
        var max = collider.bounds.max;
        
        _frequencyTimer = spawnFrequency;
        
        while (true)
        {
            _frequencyTimer -= Time.deltaTime;
            
            if (_frequencyTimer <= 0)
            {
                if (gameObjects.Count > 0)
                {
                    for (var i = 0; i < spawnAmount; i++)
                    {
                        var gameObj = gameObjects[Random.Range(0, gameObjects.Count - 1)];
                        Vector3 spawnPoint = collider.ClosestPoint(new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z)));
                        GameObject instance = Instantiate(gameObj);
                        instance.transform.position = spawnPoint;
                    }
                }
                
                _frequencyTimer = spawnFrequency;
            }
            
            yield return null;
        }

        yield return null;
    }
}
