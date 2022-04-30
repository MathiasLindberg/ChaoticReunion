using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTurret : MonoBehaviour
{
    [SerializeField] OrientationSensor orientationSensor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = orientationSensor.Orientation;
    }
}
