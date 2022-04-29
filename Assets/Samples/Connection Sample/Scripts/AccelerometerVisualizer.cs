using UnityEngine;

public class AccelerometerVisualizer : MonoBehaviour
{
    public GameObject hubObject;

    // x = right and left of the hub.
    // y = top and bottom of the hub. 
    // z = front and back of the hub.
    public GameObject red, green, blue;

    public void OnAccelerationChanged(Vector3 acc)
    {
        red.transform.localScale = new Vector3(acc.x, 0.1f, 0.1f);
        green.transform.localScale = new Vector3(0.1f, acc.y, 0.1f);
        blue.transform.localScale = new Vector3(0.1f, 0.1f, acc.z);

        red.transform.localPosition = new Vector3((Mathf.Sign(acc.x) * hubObject.transform.localScale.x / 2f) + (red.transform.localScale.x / 2f), 0, 0);
        green.transform.localPosition = new Vector3(0, (Mathf.Sign(acc.y) * hubObject.transform.localScale.y / 2f) + (green.transform.localScale.y / 2f), 0);
        blue.transform.localPosition = new Vector3(0, 0, (Mathf.Sign(acc.z) * hubObject.transform.localScale.z / 2f) + (blue.transform.localScale.z / 2f));
    }
}
