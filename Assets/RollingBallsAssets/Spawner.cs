using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private uint amount;
    // Start is called before the first frame update

    public void Spawn()
    {
        var collider = GetComponent<Collider>();
        var min = collider.bounds.min;
        var max = collider.bounds.max;

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
}
