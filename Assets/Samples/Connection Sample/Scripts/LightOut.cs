using UnityEngine;

public class LightOut : MonoBehaviour
{
    new public Renderer renderer;
    public Color color;
    public HubBase LEGOHub;

    void Update()
    {
        if (LEGOHub != null && LEGOHub.IsConnected)
        {
            color = LEGOHub.LedColor;
            renderer.material.color = color;
            renderer.material.SetColor("_EmissionColor", color);
        }
    }
}
