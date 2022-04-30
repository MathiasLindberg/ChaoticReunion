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
        int RandomColour = Random.Range(1,7);

        Material mat = GetComponent<MeshRenderer>().material;
      //  mat.color = new Color(Random.value, Random.value, Random.value);
        
          if (RandomColour == 1) { mat.color = new Color(217f / 255f, 217f / 255f, 214f / 255f); } //white
          if (RandomColour == 2) { mat.color = new Color(225f / 255f, 205f / 255f, 000f / 255f); } //yellow 
          if (RandomColour == 3) { mat.color = new Color(245f / 255f, 046f / 255f, 064f / 255f); } //red
          if (RandomColour == 4) { mat.color = new Color(000f / 255f, 061f / 255f, 165f / 255f); } //blue
          if (RandomColour == 5) { mat.color = new Color(039f / 255f, 037f / 255f, 031f / 255f); } //black
          if (RandomColour == 6) { mat.color = new Color(000f / 255f, 132f / 255f, 061f / 255f); } //green
          if (RandomColour == 7) { mat.color = new Color(105f / 255f, 063f / 255f, 035f / 255f); } //brown 

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
