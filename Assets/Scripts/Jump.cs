using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private ForceSensor forceSensor;
    [SerializeField] private float jumpForce = 200.0f;
    [SerializeField] private float shootingForce = 100.0f;
    [SerializeField] private float impactForce = 5.0f;
    [SerializeField] private ParticleSystem particleSystem;

    public void AddJumpingForce(bool ignoreForceSensor = false)
    {
        if (!forceSensor.Touch) return;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, jumpForce, 0));

        if (TryGetComponent(out Player player) && TryGetComponent(out Brick brick))
        {
            Brick shotBrick = brick.GetOutermostBrick(player.MovementDirection);
            if (shotBrick != brick)
            {
                shotBrick.Detach();
                shotBrick.EmitForce(1.5f, impactForce, 0.1f);
                shotBrick.transform.position += player.MovementDirection * 0.1f;
                shotBrick.GetComponent<Rigidbody>().AddForce((player.MovementDirection + Vector3.up * 0.1f) * shootingForce, ForceMode.Impulse);
                particleSystem.GetComponent<Aligner>().Direction = player.MovementDirection;
                particleSystem.Play();
            }
        }
    }
}
