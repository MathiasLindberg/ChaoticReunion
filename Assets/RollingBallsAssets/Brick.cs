using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brick : MonoBehaviour
{
    [SerializeField] float breakForce = 10f;
    [SerializeField] float breakTorque = 10f;
    private bool attached;
    private GameObject attachedTo;
    // Start is called before the first frame update
    void Start()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        mat.color = new Color(Random.value, Random.value, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (attached)
        {
            Player player;
            bool attachedToPlayer = attachedTo != null && attachedTo.TryGetComponent(out player);
            if (!attachedToPlayer)
            {
                Detach();
            }
        }
        if (collision.collider.CompareTag("Brick"))
        {
            Vector3 connectionPoint = new Vector3();
            
            foreach (ContactPoint contact in collision.contacts)
            {
                connectionPoint += contact.point;
            }
            connectionPoint /= collision.contactCount;

            Attach(collision.collider.gameObject, connectionPoint);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (attached && collision.collider.CompareTag("Brick"))
        {
            Brick other = collision.collider.GetComponent<Brick>();
            if (other == attachedTo)
            {
                Detach();
            }
        }
    }

    private void Detach()
    {
        attached = false;
        attachedTo = null;
        transform.parent = null;
    }

    private void Attach(GameObject to, Vector3 at)
    {
        FixedJoint joint;
        if (!gameObject.TryGetComponent(out joint))
        {
            joint = gameObject.AddComponent<FixedJoint>();
        }
        joint.breakForce = breakForce;
        joint.breakTorque = breakTorque;
        joint.connectedAnchor = at - to.gameObject.transform.position;
        joint.connectedBody = to.GetComponent<Rigidbody>();
        attached = true;
        Brick other;
        if (TryGetComponent(out other)) other.attached = true;
        attachedTo = to;
        transform.parent = to.transform;
    }
}
