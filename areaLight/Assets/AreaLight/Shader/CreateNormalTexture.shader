Shader "Unlit/CreateNormalTexture"
{
	Properties
	{}

		SubShader
	{
		Tags {"LightMode" = "UniversalGBuffer" "RenderType" = "Opaque" }

		Pass
		{
			Fog { Mode Off }
			ZWrite Off
			Blend One One
			Cull Front
			ZTest Always

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#define UNITY_PI 3.1415926

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

			struct a2v
			{
				float2 uv           : TEXCOORD0;
				float4 positionOS   : POSITION;
			};

			struct v2f
			{
				float2 uv           : TEXCOORD0;
				float4 positionCS   : SV_POSITION;
			};


			v2f vert(a2v v)
			{
				v2f o;
				o.positionCS = TransformObjectToHClip(v.positionOS);
				o.uv = v.uv;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half3 normal = SampleSceneNormals(i.uv);
				//normal = SampleSceneNormals(uv);
				return half4(normal, 1);
			}
			ENDHLSL
		}
	}
}
