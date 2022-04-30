using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EZCameraShake;

public class Brick : MonoBehaviour
{
    [SerializeField] float breakForce = 10f;
    [SerializeField] float breakTorque = 10f;
    [SerializeField] int width = 2;
    [SerializeField] int height = 2;
    private bool attached;
    private bool isPlayer;
    private GameObject attachedTo;
    private List<AttachmentPoint> upperAttachments;
    private List<AttachmentPoint> lowerAttachments;
    private Collider collider;
    private bool setup;
    private float epsilon = 0f;

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
          
        collider = GetComponent<Collider>();

        Player player;
        isPlayer = TryGetComponent(out player);

        upperAttachments = new List<AttachmentPoint>(width * height);
        lowerAttachments = new List<AttachmentPoint>(width * height);
        Vector3 size = collider.bounds.size;
        for (int i = 0; i < 3; i++) size[i] /= transform.localScale[i];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                upperAttachments.Add(new AttachmentPoint(j, i, width, height, false, size));
                lowerAttachments.Add(new AttachmentPoint(j, i, width, height, true, size));
            }
        }
        setup = true;
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!setup || isPlayer) return;

        if (collision.gameObject.CompareTag("Brick") && collision.impulse.magnitude > 3 && GameManager.Instance.GameState.Equals(GameStates.Running))
        {
            CameraShaker.Instance.ShakeOnce(5, 4, 1, 1);
        }

        if (attached)
        {
            Player player;
            bool attachedToPlayer = attachedTo != null && attachedTo.TryGetComponent(out player);
            if (!attachedToPlayer)
            {
                Detach();
            }
        }
        if (!attached && collision.collider.CompareTag("Brick"))
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
        if (!setup || isPlayer) return;

        if (attached && collision.collider.CompareTag("Brick"))
        {
            Brick other = collision.collider.GetComponent<Brick>();
            if (other == attachedTo || other.attachedTo == this)
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

    //private Vector3 debugPos1;
    //private Vector3 debugPos2;
    //private bool drawDebug;
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;
    //    if (drawDebug)
    //    {
    //        Gizmos.DrawSphere(debugPos1, 0.025f);
    //        Gizmos.DrawSphere(debugPos2, 0.025f);
    //    }

    //    Gizmos.color = Color.yellow;
    //    foreach (AttachmentPoint point in upperAttachments) Gizmos.DrawSphere(transform.TransformPoint(point.Position), 0.01f);
    //    Gizmos.color = Color.green;
    //    foreach (AttachmentPoint point in lowerAttachments) Gizmos.DrawSphere(transform.TransformPoint(point.Position), 0.01f);
    //}

    private void Attach(GameObject to, Vector3 at)
    {
        Brick other;
        if (to.TryGetComponent(out other) && other.attachedTo != gameObject)
        {
            AttachmentPoint ownAttachment = FindClosestAttachmentPoint(transform.InverseTransformPoint(at));
            AttachmentPoint otherAttachment = other.FindClosestAttachmentPoint(other.transform.InverseTransformPoint(at), !ownAttachment.IsSocket);

            attached = true;
            other.attached = true;
            attachedTo = to;
            transform.position = to.transform.position;
            transform.rotation = to.transform.rotation;
            Vector3 ownOffset = Vector3.Scale(ownAttachment.Position, transform.localScale);
            Vector3 otherOffset = Vector3.Scale(otherAttachment.Position, to.transform.localScale);


            transform.Translate(ownOffset * (ownAttachment.IsSocket ? -1 : 1) + otherOffset * (otherAttachment.IsSocket ? -1 : 1));




            //debugPos1 = transform.TransformPoint(ownAttachment.Position);
            //debugPos2 = other.transform.TransformPoint(otherAttachment.Position);
            //drawDebug = true;

            //transform.parent = to.transform;
            FixedJoint joint;
            if (!gameObject.TryGetComponent(out joint))
            {
                joint = gameObject.AddComponent<FixedJoint>();
            }
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
            joint.connectedAnchor = otherAttachment.Position;
            joint.connectedBody = to.GetComponent<Rigidbody>();
        }

    }

    public AttachmentPoint FindClosestAttachmentPoint(Vector3 localPosition, bool socket)
    {
        AttachmentPoint closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int index = i * width + j;
                AttachmentPoint point = socket ? lowerAttachments[index] : upperAttachments[index];
                float dist = (point.Position - localPosition).magnitude;
                if (dist < closestDist)
                {
                    closest = point;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    public AttachmentPoint FindClosestAttachmentPoint(Vector3 localPosition)
    {
        AttachmentPoint closest = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int index = i * width + j;
                AttachmentPoint point = lowerAttachments[index];
                float dist = (point.Position - localPosition).magnitude;
                if (dist < closestDist)
                {
                    closest = point;
                    closestDist = dist;
                }
                point = upperAttachments[index];
                dist = (point.Position - localPosition).magnitude;
                if (dist < closestDist)
                {
                    closest = point;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    public class AttachmentPoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector3 Position { get; private set; }
        public bool IsOccupied { get; set; } = false;
        public bool IsSocket { get; private set; } = false;

        public AttachmentPoint(int x, int y, int width, int height, bool socket, Vector3 size)
        {
            X = x;
            Y = y;
            IsSocket = socket;

            float w = size.x / width;
            float h = size.z / height;
            Position = new Vector3(w * (x + 0.5f) - size.x * 0.5f, IsSocket ? -size.y * 0.5f : size.y * 0.5f, h * (y + 0.5f) - size.z * 0.5f);
        }
    }

   
}
