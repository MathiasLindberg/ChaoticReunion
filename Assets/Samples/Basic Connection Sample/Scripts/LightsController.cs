using TMPro;
using UnityEngine;

public class LightsController : MonoBehaviour
{
    [SerializeField] WhiteLight whiteLight;
    [SerializeField] TMP_Text output;

    public void OnIsConnectedChanged(bool connected)
    {
        output.text = connected ? "Connected" : "Not connected";
    }
}
