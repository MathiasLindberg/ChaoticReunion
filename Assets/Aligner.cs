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
        float yRot = Mathf.Atan2(Direction.z, Direction.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(parent.transform.position, Quaternion.AngleAxis(yRot, Vector3.down));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward * 10));
    }
}
