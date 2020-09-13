using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ZoomBlurController : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    [Range(0f, 100f)]
    public float focusPower = 10f;
    [Range(0, 10)]
    public int focusDetail = 5;
    public int referenceResolutionX = 1334;
    public Vector2 focusScreenPosition = Vector2.zero;
    ZoomBlur zoomBlur;

    void Update()
    {
        if (volumeProfile == null) return;
        if (zoomBlur == null) volumeProfile.TryGet<ZoomBlur>(out zoomBlur);
        if (zoomBlur == null) return;

        zoomBlur.focusPower.value = focusPower;
        zoomBlur.focusDetail.value = focusDetail;
        zoomBlur.focusScreenPosition.value = focusScreenPosition;
        zoomBlur.referenceResolutionX.value = referenceResolutionX;
    }
}
