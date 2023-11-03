using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class depthAndNormal : ScriptableRendererFeature
{
    public LayerMask layerMask;
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingPrePasses;

    private Material material;

    public class CustomPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetIdentifier identifier;
        private RenderTextureDescriptor desc;
        private int depthNormalTexID;

        private string profilterTag = "DepthNormals";
        private ShaderTagId tag = new ShaderTagId("DepthOnly");
        private FilteringSettings filteringSettings;
        private DrawingSettings drawingSettings;
        private CullingResults cullingResults;
        private LayerMask layerMask;
        private SortingCriteria sortFlag;

        public CustomPass()
        { 

        }

        public void Setup(Material material, RenderPassEvent Event, LayerMask layerMask) 
        {
            this.material = material;
            this.renderPassEvent = Event;
            this.layerMask = layerMask;
            this.depthNormalTexID = Shader.PropertyToID("_CameraNormalsTexture");
            this.filteringSettings = new FilteringSettings(RenderQueueRange.all, this.layerMask);
            this.identifier = new RenderTargetIdentifier(depthNormalTexID);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            desc = cameraTextureDescriptor;
            cmd.GetTemporaryRT(depthNormalTexID, desc);

            ConfigureTarget(identifier);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilterTag);

            using (new ProfilingScope(cmd, new ProfilingSampler(profilterTag)))
            {
                //先提交一次，以便于在profilter里面显示出来
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                sortFlag = renderingData.cameraData.defaultOpaqueSortFlags;
                drawingSettings = CreateDrawingSettings(tag, ref renderingData, sortFlag);
                drawingSettings.overrideMaterial = this.material;
                cullingResults = renderingData.cullResults;
                context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

                context.Submit();
                cmd.SetGlobalTexture(depthNormalTexID, identifier);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(depthNormalTexID);
        }
    }

    CustomPass customPass;

    public override void Create()
    {
        //使用引擎自带的生成深度发现纹理
        material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        customPass = new CustomPass();
        customPass.Setup(material, Event, layerMask);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(customPass);
    }
}
