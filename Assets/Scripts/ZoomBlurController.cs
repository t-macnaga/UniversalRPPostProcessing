using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ZoomBlurController : MonoBehaviour
{
    [Range(0f, 100f)]
    public float focusPower = 10f;
    [Range(0, 10)]
    public int focusDetail = 5;
    public Vector2 focusScreenPosition = Vector2.zero;

    ZoomBlur zoomBlur;

    void Start()
    {
        zoomBlur = VolumeManager.instance.stack.GetComponent<ZoomBlur>();
    }

    void Update()
    {
        zoomBlur.focusPower.value = focusPower;
        zoomBlur.focusDetail.value = focusDetail;
        zoomBlur.focusScreenPosition.value.Set(focusScreenPosition.x, focusScreenPosition.y);
    }
}
