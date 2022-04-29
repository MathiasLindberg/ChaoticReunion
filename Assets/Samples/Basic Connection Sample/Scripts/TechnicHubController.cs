using TMPro;
using UnityEngine;

public class TechnicHubController : MonoBehaviour
{
    [SerializeField] HubBase hub;
    [SerializeField] TMP_Text output;
    [SerializeField] Transform hubModel;
    [SerializeField] Transform hubButton;
    [SerializeField] ParticleSystem hubButtonEffect;
    [SerializeField] Transform xAxisPositive;
    [SerializeField] Transform xAxisNegative;
    [SerializeField] Transform yAxisPositive;
    [SerializeField] Transform yAxisNegative;
    [SerializeField] Transform zAxisPositive;
    [SerializeField] Transform zAxisNegative;

    public void OnIsConnectedChanged(bool connected)
    {
        output.text = connected ? "Connected" : "Disconnected";
    }

    public void OnButtonChanged(bool pressed)
    {
        hubButton.localPosition = new Vector3(0f, pressed ? 1.81f: 2f, 1.6f);
        if (pressed)
        {
            hubButtonEffect.Emit(20);
        }
    }

    public void OnAccelerationChanged(Vector3 acceleration)
    {
        xAxisPositive.gameObject.SetActive(acceleration.x > 0.01f);
        xAxisNegative.gameObject.SetActive(acceleration.x < -0.01f);
        xAxisPositive.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, acceleration.x));
        xAxisNegative.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, -acceleration.x));

        yAxisPositive.gameObject.SetActive(acceleration.y > 0.01f);
        yAxisNegative.gameObject.SetActive(acceleration.y < -0.01f);
        yAxisPositive.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, acceleration.y));
        yAxisNegative.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, -acceleration.y));

        zAxisPositive.gameObject.SetActive(acceleration.z > 0.01f);
        zAxisNegative.gameObject.SetActive(acceleration.z < -0.01f);
        zAxisPositive.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, acceleration.z));
        zAxisNegative.localScale = new Vector3(100f, 100f, 1000f * Mathf.Max(0f, -acceleration.z));
    }

    public void OnOrientationChanged(Vector3 orientation)
    {
        hubModel.rotation = Quaternion.Euler(orientation);
    }

    void Update()
    {
        if (hub.IsConnected)
        {
            output.text = "Battery " + hub.BatteryLevel + "%";
        }
    }
}
