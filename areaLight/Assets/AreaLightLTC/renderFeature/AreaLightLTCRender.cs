using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AreaLightLTCRender : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Mesh mesh;
        public Material material;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    
    [SerializeField] 
    private Settings _settings;

    public class AreaLightPass : ScriptableRenderPass
    {
        private Mesh mesh;
        private Material material;
        
        public AreaLightPass(Settings settings)
        {
            this.mesh = settings.mesh;
            this.material = settings.material;
            this.renderPassEvent = settings.passEvent;
        }

        public void Setup()
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer buffer = CommandBufferPool.Get("EreaLightRenderLTC");
            for (int i = 0; i < AreaLightLTCManager.Instance.areaLightList.Count; i++)
            {
                AreaLightLTC areaLight = AreaLightLTCManager.Instance.areaLightList[i];
                AreaLightLTCManager.Instance.SetDatas(areaLight, buffer);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 200);
                buffer.DrawMesh(this.mesh, areaLight.transform.localToWorldMatrix * matrix, this.material);
            }
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    private AreaLightPass pass;
    
    public override void Create()
    {
        pass = new AreaLightPass(_settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.Setup();
        renderer.EnqueuePass(pass);
    }
}