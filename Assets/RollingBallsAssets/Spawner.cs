using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private uint amount;
    // Start is called before the first frame update
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        if (gameObject)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPoint = collider.ClosestPoint(new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z)));
                GameObject instance = Instantiate(gameObject);
                instance.transform.position = spawnPoint;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
