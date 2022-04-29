using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSensorController : MonoBehaviour
{
    [SerializeField] ColorSensorTechnic colorSensor;
    [SerializeField] TMP_Text output;
    [SerializeField] Image colorSwatch;
    [SerializeField] Image colorSwatchNoColor;

    public void OnIsConnectedChanged(bool connected)
    {
        output.text = connected ? "Connected" : "Not connected";
    }

    public void OnIdChanged(int id)
    {
        output.text = "Id " + id;
    }

    public void OnColorChanged(Color color)
    {
        colorSwatch.gameObject.SetActive(colorSensor.Id > -1);
        colorSwatch.color = color;

        colorSwatchNoColor.gameObject.SetActive(colorSensor.Id == -1);

    }
}
