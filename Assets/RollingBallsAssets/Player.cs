using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private float rollForce = 10.0f;
    [SerializeField] private float movementForce = 10.0f;
    [SerializeField] private AccelerationSensor accelerationSensor;
    private Rigidbody rb;
    private bool forwardKeyDown;
    private bool backKeyDown;
    private bool leftKeyDown;
    private bool rightKeyDown;
    private Camera mainCam;

    // Acceleration Sensor variables
    private Vector3 previousAcceleration;
    private bool isSensorConnected;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        isSensorConnected = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*        if (Input.GetKeyDown(forwardKey)) forwardKeyDown = true;
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
                }*/


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

        rb.AddForce(force * movementForce);
        rb.AddTorque(Vector3.Cross(force * rollForce, Vector3.down));
    }

    public void InitializeSensor()
    {
        isSensorConnected = true;
        previousAcceleration = accelerationSensor.Acceleration;
    }

    public void ApplyAccelerationFromSensor()
    {
        Vector3 currentAcceleration = accelerationSensor.Acceleration;
        currentAcceleration.y = 0;
        Vector3 force = currentAcceleration * Time.fixedDeltaTime * 1000.0f;
        rb.AddForce(force * movementForce);
        rb.AddTorque(Vector3.Cross(force * rollForce, Vector3.down));

        previousAcceleration = currentAcceleration;
    }
}
