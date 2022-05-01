using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode shootKey = KeyCode.LeftControl;
    [SerializeField] private float rollForce = 10.0f;
    [SerializeField] private float movementForce = 10.0f;
    [SerializeField] private float sensorRollForce = 2.0f;
    [SerializeField] private float sensorMovementForce = 2.0f;
    [SerializeField] private AccelerationSensor accelerationSensor;
    [SerializeField] GameObject arrow;
    [SerializeField] private LayerMask playfieldLayerMask;
    GameObject arrowInstance;
    private Rigidbody rb;
    private bool forwardKeyDown;
    private bool backKeyDown;
    private bool leftKeyDown;
    private bool rightKeyDown;
    private Camera mainCam;
    private float movingAvgRatio = 0.85f;
    
    // Acceleration Sensor variables
    private Vector3 previousAcceleration;
    public bool isSensorConnected { get; set; }
    public Vector3 MovementDirection { get; private set; }
    
    public bool IsAlive { get; set; } = true;
    public bool CanMove { get; set; }
    
    public bool HasShaken { get; set; }

    private float shakeThreshold = 1.25f;
    private Vector3 oldAcceleration;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        isSensorConnected = false;
        arrowInstance = Instantiate(arrow);
    }

    // Update is called once per frame
    void Update()
    {        
        if (!CanMove) return;
        
        
        if (Input.GetKeyDown(forwardKey)) forwardKeyDown = true;
        else if (Input.GetKeyUp(forwardKey)) forwardKeyDown = false;
        if (Input.GetKeyDown(backKey)) backKeyDown = true;
        else if (Input.GetKeyUp(backKey)) backKeyDown = false;
        if (Input.GetKeyDown(leftKey)) leftKeyDown = true;
        else if (Input.GetKeyUp(leftKey)) leftKeyDown = false;
        if (Input.GetKeyDown(rightKey)) rightKeyDown = true;
        else if (Input.GetKeyUp(rightKey)) rightKeyDown = false;

        if (forwardKeyDown || backKeyDown || leftKeyDown || rightKeyDown)
        {
            ApplyMotion();
            if (Input.GetKeyDown(shootKey)) GetComponent<Jump>().AddJumpingForce(true);
        }
        else
        {
            arrowInstance.transform.position = Vector3.zero;
        }
        UpdateArrow();
    }

    private void ApplyMotion()
    {
        Vector3 forward = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z).normalized;
        Vector3 right = new Vector3(mainCam.transform.right.x, 0, mainCam.transform.right.z).normalized;

        Vector3 force = new Vector3();
        if (forwardKeyDown) force += forward;
        else if (backKeyDown) force -= forward;
        if (leftKeyDown) force -= right;
        else if (rightKeyDown) force += right;

        force *= Time.fixedDeltaTime * 1000.0f;
        MovementDirection = force.normalized * movingAvgRatio + MovementDirection * (1.0f - movingAvgRatio);


        rb.AddForce(force * movementForce);
        rb.AddTorque(Vector3.Cross(force * rollForce, Vector3.down));
    }

    private void UpdateArrow()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 100, playfieldLayerMask.value, QueryTriggerInteraction.Collide))
        {
            Vector3 fwd = new Vector3(MovementDirection.x, 0, MovementDirection.z);
            if (fwd.magnitude == 0)
            {
                arrowInstance.transform.position = Vector3.zero;
            }
            else
            {
                arrowInstance.transform.position = hit.point + fwd * 2.0f;
                arrowInstance.transform.forward = fwd;
            }
        }
    }
    
    public void InitializeSensor()
    {
        isSensorConnected = true;
        previousAcceleration = accelerationSensor.Acceleration;
        oldAcceleration = accelerationSensor.Acceleration;
    }

    public void ApplyAccelerationFromSensor()
    {
        Vector3 currentAcceleration = accelerationSensor.Acceleration;
        
        if ((oldAcceleration - currentAcceleration).magnitude > shakeThreshold)
        {
            HasShaken = true;
        }
        else
        {
            if (CanMove)
            {
                currentAcceleration.y = 0;
                Vector3 force = currentAcceleration * Time.fixedDeltaTime * 1000.0f;
                Brick brick = GetComponent<Brick>();
                foreach (Brick child in brick.ChildBricks)
                {
                    child.GetComponent<Rigidbody>().AddForce(force * (sensorMovementForce / 100.0f));
                }
                rb.AddForce(force * (sensorMovementForce / 100.0f));
                rb.AddTorque(Vector3.Cross(force * sensorRollForce, Vector3.down));
                MovementDirection = force.normalized * movingAvgRatio + MovementDirection * (1.0f - movingAvgRatio);
                previousAcceleration = currentAcceleration;
            }
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsAlive = false;
            CanMove = false;
        }
    }
}
