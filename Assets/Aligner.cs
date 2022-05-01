using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aligner : MonoBehaviour
{
    float lastAlignmentTime;
    public Transform parent;
    public Vector3 Direction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Align();
    }
    private void Align()
    {
        transform.position = parent.transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward * 10));
    }
}
