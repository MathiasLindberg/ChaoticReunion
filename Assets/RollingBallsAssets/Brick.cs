using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Brick : MonoBehaviour
{
    public bool IsPlayer { get; private set; }
    public bool IsChainedToPlayer { get; private set; }
    [SerializeField] float breakForce = 10f;
    [SerializeField] float breakTorque = 10f;
    [SerializeField] int width = 2;
    [SerializeField] int height = 2;
    [SerializeField] LayerMask shootableLayer;
    [SerializeField] bool bottomOnly;
    private bool attached;
    private Brick parentBrick;
    public List<Brick> ChildBricks { get; private set; }
    private List<AttachmentPoint> upperAttachments;
    private List<AttachmentPoint> lowerAttachments;
    private List<AttachmentPoint> parentAttachments;
    private Collider collider;
    private bool setup;
    private float epsilon = 0f;
    private bool aligned = false;
    private bool emittingForce = false;
    private float emitStart;
    private float emitTime;
    private float emitDelay;
    private float emitForce;
    private float emitLastPulseTime;
    private float jointAttachmentTime;
    private bool ignoreShake;

    void Start()
    {
        int RandomColour = Random.Range(1,7);

        ChildBricks = new List<Brick>();
        Player player;
        IsPlayer = TryGetComponent(out player);
        IsChainedToPlayer = IsPlayer;

        if (!IsPlayer)
        {
            Material mat = GetComponent<MeshRenderer>().material;
        
            if (RandomColour == 1) { mat.color = new Color(217f / 255f, 217f / 255f, 214f / 255f); } //white
            if (RandomColour == 2) { mat.color = new Color(225f / 255f, 205f / 255f, 000f / 255f); } //yellow 
            if (RandomColour == 3) { mat.color = new Color(245f / 255f, 046f / 255f, 064f / 255f); } //red
            if (RandomColour == 4) { mat.color = new Color(000f / 255f, 061f / 255f, 165f / 255f); } //blue
            if (RandomColour == 5) { mat.color = new Color(039f / 255f, 037f / 255f, 031f / 255f); } //black
            if (RandomColour == 6) { mat.color = new Color(000f / 255f, 132f / 255f, 061f / 255f); } //green
            if (RandomColour == 7) { mat.color = new Color(105f / 255f, 063f / 255f, 035f / 255f); } //brown 
        }
          
        collider = GetComponent<Collider>();

        

        upperAttachments = new List<AttachmentPoint>(width * height);
        lowerAttachments = new List<AttachmentPoint>(width * height);
        parentAttachments = new List<AttachmentPoint>((width * height) / 2);
        Vector3 size = collider.bounds.size;
        size.y *= 0.5f;
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

    private void OnDrawGizmosSelected()
    {
        if (!setup) return;
        Vector3 size = collider.bounds.size;
        float w = size.x / width;
        float h = size.z / height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int index = i * width + j;
                AttachmentPoint up = upperAttachments[index];
                Gizmos.color = up.Connection == null ? Color.red : Color.green;
                Gizmos.DrawCube(transform.TransformPoint(up.Position), new Vector3(w, size.y, h) * 0.5f);
                AttachmentPoint down = lowerAttachments[index];
                Gizmos.color = down.Connection == null ? Color.red : Color.green;
                Gizmos.DrawCube(transform.TransformPoint(down.Position), new Vector3(w, size.y, h) * 0.5f);
                
            }
        }
    }

    void Update()
    {
        if (emittingForce) Emit();

        if (TryGetComponent(out Joint joint) && Time.time - jointAttachmentTime > 3.0f)
        {
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;
            ignoreShake = false;
        }
    }

    public bool TryGetPlayerFromChain(out Player player)
    {
        player = null;
        for (Brick brick = this; brick.parentBrick != null; brick = brick.parentBrick)
        {
            if (brick.IsPlayer)
            {
                player = brick.GetComponent<Player>();
                return true;
            }
        }
        return false;
    }

    public Brick GetOutermostBrick(Vector3 direction)
    {
        Brick next = null;
        float smallestDiff = float.MaxValue;

        foreach (Brick child in ChildBricks)
        {
            float diff = Vector3.Angle(child.transform.position - transform.position, direction);
            if (diff < smallestDiff)
            {
                smallestDiff = diff;
                next = child;
            }
        }
        if (next == null) return this;
        return next.GetOutermostBrick(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!setup || IsPlayer) return;

        var brickComponent = collision.gameObject.GetComponent<Brick>();
        if (collision.gameObject.CompareTag("Brick") && brickComponent.IsChainedToPlayer && GameManager.Instance.GameState.Equals(GameStates.Running))
        {
            CameraShaker.Instance.ShakeOnce(5, 4, 1, 1);
            AkSoundEngine.PostEvent("Play_Camerashake", this.gameObject);
        }

        if (attached && !IsChainedToPlayer && collision.collider.TryGetComponent(out Brick brick) && brick.IsChainedToPlayer)
        {
            Detach();
        }
        if (!attached && collision.collider.CompareTag("Brick") && !IsChainedToPlayer)
        {
            Vector3 connectionPoint = new Vector3();

            foreach (ContactPoint contact in collision.contacts)
            {
                connectionPoint += contact.point;
            }
            connectionPoint /= collision.contactCount;

            Attach(collision.collider.GetComponent<Brick>(), connectionPoint);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!setup || IsPlayer) return;

        if (attached && collision.collider.CompareTag("Brick"))
        {
            Brick other = collision.collider.GetComponent<Brick>();

            if (other == parentBrick && (!TryGetComponent(out FixedJoint joint) || joint.connectedBody != collision.collider.GetComponent<Rigidbody>()))
            {
                Detach();
            }
        }
    }

    public void Detach()
    {
        attached = false;
        parentBrick.ChildBricks.Remove(this);
        foreach (AttachmentPoint attachment in parentAttachments)
        {
            if (attachment.Connection != null)
            {
                attachment.Connection.Connection = null;
            }
            attachment.Connection = null;
        }
        parentAttachments.Clear();
        IsChainedToPlayer = false;
        parentBrick = null;
        transform.parent = null;
        aligned = false;

        if (TryGetComponent(out FixedJoint joint)) Destroy(joint);
    }

    public void EmitForce(float time, float force, float delay)
    {
        emitTime = time;
        emitForce = force;
        emitDelay = delay;
        emittingForce = true;
    }

    private void Emit()
    {
        float t = Time.time - emitTime;
        if (t > emitTime) emittingForce = false;
        else if (t > emitDelay && t - emitLastPulseTime > 0.01f)
        {
            float radius = this.collider.bounds.size.magnitude;
            foreach (Collider collider in Physics.OverlapSphere(transform.position, radius, shootableLayer.value))
            {
                if (collider == this.collider) continue;
                collider.GetComponent<Rigidbody>().AddForce((collider.transform.position - transform.position).normalized * emitForce, ForceMode.Impulse);
            }
            emitLastPulseTime = t;
        }
    }

    private void ForceUpdateAttachment()
    {
        if (!TryGetComponent(out FixedJoint joint))
        {
            foreach (AttachmentPoint attachment in upperAttachments)
            {
                if (attachment.Connection != null)
                {
                    attachment.Connection.Connection = null;
                }
                attachment.Connection = null;
            }
            foreach (AttachmentPoint attachment in lowerAttachments)
            {
                if (attachment.Connection != null)
                {
                    attachment.Connection.Connection = null;
                }
                attachment.Connection = null;
            }
        }
    }

    private void Attach(Brick to, Vector3 at)
    {
        if (to.parentBrick != gameObject)
        {

            AttachmentPoint ownAttachment = FindClosestAttachmentPoint(transform.InverseTransformPoint(at));
            if (ownAttachment == null) return;

            AttachmentPoint otherAttachment = to.FindClosestAttachmentPoint(to.transform.InverseTransformPoint(at), !ownAttachment.IsSocket);
            if (otherAttachment == null) return;

            to.transform.position += Vector3.up;

            transform.position = to.transform.position;
            transform.rotation = to.transform.rotation;
            Vector3 ownOffset = Vector3.Scale(ownAttachment.Position, transform.localScale);
            Vector3 otherOffset = Vector3.Scale(otherAttachment.Position, to.transform.localScale);
            Vector3 diff = otherOffset - ownOffset;
            diff.y = (Mathf.Abs(ownOffset.y) + Mathf.Abs(otherOffset.y)) * (ownAttachment.IsSocket ? 1 : -1);

            transform.Translate(diff);

            if (!aligned)
            {
                aligned = true;
                return;
            }

            FixedJoint joint;
            if (!gameObject.TryGetComponent(out joint))
            {
                joint = gameObject.AddComponent<FixedJoint>();
            }
            joint.connectedAnchor = otherAttachment.Position;
            joint.connectedBody = to.GetComponent<Rigidbody>();
            jointAttachmentTime = Time.time;
            ignoreShake = true;

            to.ChildBricks.Add(this);
            if (to.IsChainedToPlayer)
            {
                IsChainedToPlayer = true;
            }
            attached = true;
            parentBrick = to;
            if (ownAttachment.IsSocket)
            {
                float threshold = new Vector3(collider.bounds.size.x / width, collider.bounds.size.y / 4, collider.bounds.size.z / height).magnitude * 0.1f;
                foreach (AttachmentPoint thisAttachment in lowerAttachments)
                {
                    if (thisAttachment.Connection == null)
                    {
                        Vector3 pos = to.transform.TransformPoint(thisAttachment.Position);
                        AttachmentPoint other = to.FindClosestAttachmentPoint(pos, !thisAttachment.IsSocket);

                        if (other != null && (to.transform.TransformPoint(other.Position) - transform.TransformPoint(thisAttachment.Position)).magnitude < threshold)
                        {
                            thisAttachment.Connection = other;
                            other.Connection = thisAttachment;
                            parentAttachments.Add(other);
                        }
                    }

                }
            }
            else
            {
                float threshold = new Vector3(collider.bounds.size.x / width, collider.bounds.size.y / 4, collider.bounds.size.z / height).magnitude * 0.1f;
                foreach (AttachmentPoint thisAttachment in upperAttachments)
                {
                    if (thisAttachment.Connection == null)
                    {
                        Vector3 pos = to.transform.TransformPoint(thisAttachment.Position);
                        AttachmentPoint other = to.FindClosestAttachmentPoint(pos, !thisAttachment.IsSocket);

                        if (other != null && (to.transform.TransformPoint(other.Position) - transform.TransformPoint(thisAttachment.Position)).magnitude < threshold)
                        {
                            thisAttachment.Connection = other;
                            other.Connection = thisAttachment;
                            parentAttachments.Add(other);
                        }
                    }

                }
            }
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
                if (point.Connection == null && dist < closestDist)
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
                if (point.Connection == null && dist < closestDist)
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
        public bool IsSocket { get; private set; }
        public AttachmentPoint Connection { get; set; }

        public AttachmentPoint(int x, int y, int width, int height, bool socket, Vector3 size)
        {
            X = x;
            Y = y;
            IsSocket = socket;

            float w = size.x / width;
            float h = size.z / height;
            Position = new Vector3(w * (x + 0.5f) - size.x * 0.5f, socket ? -size.y * 0.5f : size.y * 0.5f, h * (y + 0.5f) - size.z * 0.5f);
        }
    }

   
}
