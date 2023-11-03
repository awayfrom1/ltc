Shader "AreaLight/AreaLightFace"
{
    Properties{}
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                half4 _panelLightColor;
                half _panelLightFaceInstensity;
            CBUFFER_END
            
            struct a2v
            {
                float4 positionOS : POSITION;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                return _panelLightColor * _panelLightFaceInstensity;
            }
            ENDHLSL
        }
    }
}
