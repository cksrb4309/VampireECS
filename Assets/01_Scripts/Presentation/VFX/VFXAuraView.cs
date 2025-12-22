using UnityEngine;
using UnityEngine.VFX;

public class VFXAuraView : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;

    float radius = 0;

    public void SetRadius(float r)
    {
        if (radius != r)
        {
            radius = r;

            vfx.SetFloat("Radius", r);
        }
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public void Play() => vfx.Play();
    public void Stop() => vfx.Stop();
}
