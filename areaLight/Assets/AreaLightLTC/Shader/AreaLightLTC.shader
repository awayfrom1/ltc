Shader "AreaLightLTC/AreaLightLTC"
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
			//Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			ZTest Always

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#define UNITY_PI 3.1415926

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		CBUFFER_START(UnityPerMaterial)
			half4 _panelLightPos;
			half4 _panelLightColor;

			half _panelLightInstensity;
			half _panelLightRoughness;
		CBUFFER_END
			float4x4 _LightVerts;

		TEXTURE2D(_transformInvMatrixSpecularTexture);   SAMPLER(sampler_transformInvMatrixSpecularTexture);
		TEXTURE2D(_transformInvMatrxDiffuseTexture);  SAMPLER(sampler_transformInvMatrxDiffuseTexture);
		TEXTURE2D(_ampDiffAmpSpecFresnelTexture);  SAMPLER(sampler_ampDiffAmpSpecFresnelTexture);
		//延迟渲染图
		TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
		TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);
		//TEXTURE2D(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture);

		//获取到世界空间
		inline float3 GetFragmentWorldPos(float2 screenPos)
		{
			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, screenPos);
			float4 ndc = float4(screenPos * 2 - 1, depth, 1);
#if UNITY_UV_STARTS_AT_TOP
			ndc.y *= -1;
#endif
			float4 worldPos = mul(UNITY_MATRIX_I_VP, ndc);
			worldPos /= worldPos.w;

			return worldPos.xyz;
		}

		//将灯光方向转移到view_normal空间之后，在根据采样到的图将灯光方向变换到brdf为cos的空间中
		inline half IntegrateEdge(half3 v1, half3 v2)
		{
			half d = dot(v1, v2);
			half theta = acos(max(-0.9999, dot(v1, v2)));
			half theta_sintheta = theta / sin(theta);
			return theta_sintheta * (v1.x * v2.y - v1.y * v2.x);
		}

		inline half PolygonRadiance(half4x3 L)
		{
			uint config = 0;
			if (L[0].z > 0) config += 1;
			if (L[1].z > 0) config += 2;
			if (L[2].z > 0) config += 4;
			if (L[3].z > 0) config += 8;
			half3 L4 = L[3];

			uint n = 0;
			switch (config)
			{
			case 0: // clip all
				break;

			case 1: // V1 clip V2 V3 V4
				n = 3;
				L[1] = -L[1].z * L[0] + L[0].z * L[1];
				L[2] = -L[3].z * L[0] + L[0].z * L[3];
				break;

			case 2: // V2 clip V1 V3 V4
				n = 3;
				L[0] = -L[0].z * L[1] + L[1].z * L[0];
				L[2] = -L[2].z * L[1] + L[1].z * L[2];
				break;

			case 3: // V1 V2 clip V3 V4
				n = 4;
				L[2] = -L[2].z * L[1] + L[1].z * L[2];
				L[3] = -L[3].z * L[0] + L[0].z * L[3];
				break;

			case 4: // V3 clip V1 V2 V4
				n = 3;
				L[0] = -L[3].z * L[2] + L[2].z * L[3];
				L[1] = -L[1].z * L[2] + L[2].z * L[1];
				break;

			case 5: // V1 V3 clip V2 V4: impossible
				break;

			case 6: // V2 V3 clip V1 V4
				n = 4;
				L[0] = -L[0].z * L[1] + L[1].z * L[0];
				L[3] = -L[3].z * L[2] + L[2].z * L[3];
				break;

			case 7: // V1 V2 V3 clip V4
				n = 5;
				L4 = -L[3].z * L[0] + L[0].z * L[3];
				L[3] = -L[3].z * L[2] + L[2].z * L[3];
				break;

			case 8: // V4 clip V1 V2 V3
				n = 3;
				L[0] = -L[0].z * L[3] + L[3].z * L[0];
				L[1] = -L[2].z * L[3] + L[3].z * L[2];
				L[2] = L[3];
				break;

			case 9: // V1 V4 clip V2 V3
				n = 4;
				L[1] = -L[1].z * L[0] + L[0].z * L[1];
				L[2] = -L[2].z * L[3] + L[3].z * L[2];
				break;

			case 10: // V2 V4 clip V1 V3: impossible
				break;

			case 11: // V1 V2 V4 clip V3
				n = 5;
				L[3] = -L[2].z * L[3] + L[3].z * L[2];
				L[2] = -L[2].z * L[1] + L[1].z * L[2];
				break;

			case 12: // V3 V4 clip V1 V2
				n = 4;
				L[1] = -L[1].z * L[2] + L[2].z * L[1];
				L[0] = -L[0].z * L[3] + L[3].z * L[0];
				break;

			case 13: // V1 V3 V4 clip V2
				n = 5;
				L[3] = L[2];
				L[2] = -L[1].z * L[2] + L[2].z * L[1];
				L[1] = -L[1].z * L[0] + L[0].z * L[1];
				break;

			case 14: // V2 V3 V4 clip V1
				n = 5;
				L4 = -L[0].z * L[3] + L[3].z * L[0];
				L[0] = -L[0].z * L[1] + L[1].z * L[0];
				break;

			case 15: // V1 V2 V3 V4
				n = 4;
				break;
			}

			if (n == 0)
				return 0;

			// normalize
			L[0] = normalize(L[0]);
			L[1] = normalize(L[1]);
			L[2] = normalize(L[2]);
			if (n == 3)
				L[3] = L[0];
			else
			{
				L[3] = normalize(L[3]);
				if (n == 4)
					L4 = L[0];
				else
					L4 = normalize(L4);
			}

			// integrate
			half sum = 0;
			sum += IntegrateEdge(L[0], L[1]);
			sum += IntegrateEdge(L[1], L[2]);
			sum += IntegrateEdge(L[2], L[3]);
			if (n >= 4)
				sum += IntegrateEdge(L[3], L4);
			if (n == 5)
				sum += IntegrateEdge(L4, L[0]);

			sum *= 0.15915; // 1/2pi

			return max(0, sum);
		}

		inline half TransformPolygonRadiance(half4x3 lightDir, half2 uv, half3 normal,
			TEXTURE2D_PARAM(transformInv, sample_transformInv), half amplitude)
		{
			float3x3 matrixLight = 0;
			matrixLight._m22 = 1;
			matrixLight._m00_m02_m11_m20 = SAMPLE_TEXTURE2D(transformInv, sample_transformInv, uv);
			float4x3 LTransformed = mul(lightDir, matrixLight);
			return PolygonRadiance(LTransformed) * amplitude;
		}

		inline half3 CalculateLight(half3 position, half3 diffColor, half3 specColor, half roughness, half3 normal,
			half3 lightColor)
		{
			roughness = clamp(roughness, 0.01, 0.93);
			half oneMinusRoughness = 1 - roughness;
			half3 viewDir = normalize(_WorldSpaceCameraPos - position);

			half3x3 basis;
			basis[0] = normalize(viewDir - normal * dot(viewDir, normal));
			basis[1] = normalize(cross(normal, basis[0]));
			basis[2] = normal;

			//获得四个光照
			half4x3 lightDir;
			lightDir = _LightVerts - half4x3(position, position, position, position);
			lightDir = mul(lightDir, transpose(basis));

			half theta = acos(dot(viewDir, normal));
			half2 uv = half2(oneMinusRoughness, theta / 1.57);

			half3 AmpDiffAmpSpecFresnel = SAMPLE_TEXTURE2D(_ampDiffAmpSpecFresnelTexture, sampler_ampDiffAmpSpecFresnelTexture, uv).rgb;

			half3 result = 0;
			half diffuseTerm = TransformPolygonRadiance(lightDir, uv, normal, 
				TEXTURE2D_ARGS(_transformInvMatrxDiffuseTexture, sampler_transformInvMatrxDiffuseTexture), AmpDiffAmpSpecFresnel.x);
			result = diffuseTerm * diffColor;

			half specularTerm = TransformPolygonRadiance(lightDir, uv, normal, 
				TEXTURE2D_ARGS(_transformInvMatrixSpecularTexture, sampler_transformInvMatrixSpecularTexture), AmpDiffAmpSpecFresnel.y);
			half fresnelTerm = specColor + (1.0 - specColor) * AmpDiffAmpSpecFresnel.z;
			result += specularTerm * fresnelTerm * UNITY_PI;
			return result * lightColor;
		}

		struct a2v
		{
			float4 positionOS : POSITION;
			float3 normal : Normal;
		};

		struct v2f
		{
			float4 uv           : TEXCOORD2;
			float4 positionCS   : SV_POSITION;
		};


		v2f vert(a2v v)
		{
			v2f o;
			o.positionCS = TransformObjectToHClip(v.positionOS);
			o.uv = ComputeScreenPos(o.positionCS);
			return o;
		}

		half4 frag(v2f i) : SV_Target
		{
			i.uv.xy /= i.uv.w;
			half3 diffuse = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraDepthTexture, i.uv.xy).xyz;
			half3 specular = 0.0f;
			half3 worldNormal = mul(unity_CameraToWorld, half4(SampleSceneNormals(i.uv.xy), 0));
			half3 worldPos = GetFragmentWorldPos(i.uv.xy);

			half3 col = CalculateLight(worldPos, diffuse, specular, _panelLightRoughness, worldNormal, _panelLightColor * _panelLightInstensity);
			return half4(col, 1);
		}
		ENDHLSL
	}
}
}
