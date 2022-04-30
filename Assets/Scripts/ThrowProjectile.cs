using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] ForceSensor forceSensor;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform turretTip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnForceChanged()
    {
        GameObject projectile = Instantiate(projectilePrefab, turretTip.position, Quaternion.identity);
        Vector3 direction = turretTip.position;
        float speed = forceSensor.Force / 10.0f;
        projectile.GetComponent<Rigidbody>().velocity = speed * direction;
    }
}
