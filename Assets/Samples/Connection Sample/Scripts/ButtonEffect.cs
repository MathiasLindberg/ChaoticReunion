using UnityEngine;

public class ButtonEffect : MonoBehaviour
{
    public ParticleSystem particles;

    public void OnButtonPressChange(bool pressed)
    {
        if (pressed)
        {
            EmitParticles();
        }
    }

    public void EmitParticles()
    {
        particles.Emit(30);
    }
}
