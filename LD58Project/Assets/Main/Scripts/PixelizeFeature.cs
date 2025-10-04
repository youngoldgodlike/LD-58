using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class PixelizeFeature : ScriptableRendererFeature
{
[System.Serializable]
public class Settings
{
public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
[Range(32, 512)]
public int screenHeight = 144;
}

public Settings settings = new Settings();
private PixelizePass pixelizePass;

public override void Create()
{
    pixelizePass = new PixelizePass(settings);
}

public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
{
    renderer.EnqueuePass(pixelizePass);
}

protected override void Dispose(bool disposing)
{
    pixelizePass?.Dispose();
}

class PixelizePass : ScriptableRenderPass
{
    private Settings settings;
    private Material blitMaterial;

    private class PassData
    {
        internal TextureHandle source;
        internal TextureHandle tempTexture;
        internal Material material;
    }

    public PixelizePass(Settings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        
        Shader shader = Shader.Find("Hidden/Universal Render Pipeline/Blit");
        if (shader != null)
        {
            blitMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        if (resourceData.isActiveTargetBackBuffer)
            return;

        TextureHandle source = resourceData.activeColorTexture;
        if (!source.IsValid() || blitMaterial == null)
            return;

        RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
        int pixelWidth = Mathf.RoundToInt(desc.width * settings.screenHeight / (float)desc.height);
        int pixelHeight = settings.screenHeight;

        RenderTextureDescriptor lowResDesc = desc;
        lowResDesc.width = pixelWidth;
        lowResDesc.height = pixelHeight;
        lowResDesc.depthBufferBits = 0;
        lowResDesc.msaaSamples = 1;

        TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph,
            lowResDesc,
            "_TempPixelizeTexture",
            false,
            FilterMode.Point
        );

        // Первый проход: source -> tempTexture (downscale)
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Pixelize Downscale", out var passData))
        {
            passData.source = source;
            passData.tempTexture = tempTexture;
            passData.material = blitMaterial;

            builder.UseTexture(passData.source, AccessFlags.Read);
            builder.SetRenderAttachment(passData.tempTexture, 0, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }

        // Второй проход: tempTexture -> source (upscale)
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Pixelize Upscale", out var passData))
        {
            passData.source = tempTexture;
            passData.tempTexture = source;
            passData.material = blitMaterial;

            builder.UseTexture(passData.source, AccessFlags.Read);
            builder.SetRenderAttachment(passData.tempTexture, 0, AccessFlags.Write);
            builder.AllowPassCulling(false);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }
    }

    public void Dispose()
    {
        CoreUtils.Destroy(blitMaterial);
    }
}

}