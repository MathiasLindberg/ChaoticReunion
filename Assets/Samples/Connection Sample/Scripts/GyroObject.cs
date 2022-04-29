using UnityEngine;

public class GyroObject : MonoBehaviour
{
    public OrientationSensor orientation;

    void Update()
    {
        if (orientation != null && orientation.IsConnected)
        {
            transform.localEulerAngles = orientation.Orientation;
        }
    }
}
