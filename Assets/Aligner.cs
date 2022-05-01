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
        float yRot = Mathf.Atan(Direction.x / Direction.z) * Mathf.Rad2Deg;
        Vector3 fwd = new Vector3(transform.forward.x, 0, transform.forward.z);
        transform.position = parent.transform.position;
        transform.forward = fwd;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward * 10));
    }
}
