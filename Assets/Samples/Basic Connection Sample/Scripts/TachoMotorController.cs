using TMPro;
using UnityEngine;

public class TachoMotorController : MonoBehaviour
{
    [SerializeField] TachoMotor tachoMotor;
    [SerializeField] HubBase hub;
    [SerializeField] TMP_Text output;
    [SerializeField] Renderer hubModel;

    public void OnIsConnectedChanged(bool connected)
    {
        output.text = connected ? "Connected" : "Not connected";
    }

    public void OnPositionChanged(int position)
    {
        output.text = "Position " + position;

        var color = Color.HSVToRGB((position + 180f) / 360f, 1f, 1f);
        hub.LedColor = color;
        hubModel.materials[2].color = color;
        hubModel.materials[2].SetColor("_EmissionColor", color);
        hubModel.materials[2].EnableKeyword("_EMISSION");
    }
}
