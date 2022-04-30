using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private ForceSensor forceSensor;
    [SerializeField] private float throwForce = 2.0f;

    public void AddJumpingForce()
    {
        if (!forceSensor.Touch) return;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, 200, 0));
    }
}
