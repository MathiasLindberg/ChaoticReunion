using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private ForceSensor forceSensor;
    [SerializeField] private float jumpForce = 200.0f;

    public void AddJumpingForce()
    {
        if (!forceSensor.Touch) return;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, 1, 0) * jumpForce);
    }
}
