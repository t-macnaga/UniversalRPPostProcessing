using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlurRenderFeature : ScriptableRendererFeature
{
    ZoomBlurPass zoomBlurPass;

    public override void Create()
    {
        zoomBlurPass = new ZoomBlurPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        zoomBlurPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(zoomBlurPass);
    }
}

public class ZoomBlurPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Render ZoomBlur Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetZoomBlur");
    static readonly int FocusPowerId = Shader.PropertyToID("_FocusPower");
    static readonly int FocusDetailId = Shader.PropertyToID("_FocusDetail");
    static readonly int FocusScreenPositionId = Shader.PropertyToID("_FocusScreenPosition");
    static readonly int ReferenceResolutionXId = Shader.PropertyToID("_ReferenceResolutionX");
    ZoomBlur zoomBlur;
    Material zoomBlurMaterial;
    RenderTargetIdentifier currentTarget;

    public ZoomBlurPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        var shader = Shader.Find("PostEffect/ZoomBlur");
        if (shader == null)
        {
            Debug.LogError("Shader not found.");
            return;
        }
        zoomBlurMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (zoomBlurMaterial == null)
        {
            Debug.LogError("Material not created.");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled) return;

        var stack = VolumeManager.instance.stack;
        zoomBlur = stack.GetComponent<ZoomBlur>();
        if (zoomBlur == null) { return; }
        if (!zoomBlur.IsActive()) { return; }

        var cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        var source = currentTarget;
        int destination = TempTargetId;

        var w = cameraData.camera.scaledPixelWidth;
        var h = cameraData.camera.scaledPixelHeight;
        zoomBlurMaterial.SetFloat(FocusPowerId, zoomBlur.focusPower.value);
        zoomBlurMaterial.SetInt(FocusDetailId, zoomBlur.focusDetail.value);
        zoomBlurMaterial.SetVector(FocusScreenPositionId, zoomBlur.focusScreenPosition.value);
        zoomBlurMaterial.SetInt(ReferenceResolutionXId, zoomBlur.referenceResolutionX.value);

        int shaderPass = 0;
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, zoomBlurMaterial, shaderPass);
    }
}