Shader "Unlit/AreaLightEmission"
{
    Properties
    {

    }
        SubShader
        {
            LOD 100
            Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "Queue" = "Geometry"  "IgnoreProjector" = "Ture"}

            Pass
            {
                Tags{ "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag

                CBUFFER_START(UnityPerMaterial)
                    float4 _AreaLightEmission;
                CBUFFER_END

                struct a2v
                {
                    float4 postionOS    : POSITION;
                };

                struct v2f
                {
                    float4 positionCS   : SV_POSITION;
                };


                v2f vert(a2v i)
                {
                    v2f o;
                    o.positionCS = TransformObjectToHClip(i.postionOS);
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    return _AreaLightEmission;
                }
                ENDHLSL
        }
        }
}
