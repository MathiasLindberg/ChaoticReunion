using TMPro;
using UnityEngine;

public class ForceSensorController : MonoBehaviour
{
    [SerializeField] ForceSensor forceSensor;
    [SerializeField] TachoMotor tachoMotor;
    [SerializeField] WhiteLight whiteLight;
    [SerializeField] TMP_Text output;

    public void OnIsConnectedChanged(bool connected)
    {
        output.text = connected ? "Connected" : "Not connected";
    }

    public void OnForceChanged(int force)
    {
        output.text = "Force " + force;

        var normalisedForce = Mathf.RoundToInt(Mathf.Max(0f, (force - 25f) / 75f) * 100f);

        if (tachoMotor.IsConnected)
        {
            tachoMotor.SetPower(normalisedForce);
        }

        if (whiteLight.IsConnected)
        {
            whiteLight.SetIntensity(normalisedForce);
        }
    }
}
