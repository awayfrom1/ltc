Shader "AreaLight/AreaLight"
{
    Properties
	{
	}
    
    SubShader
    {
        Tags {"LightMode" = "UniversalGBuffer" "RenderType"="Opaque" }

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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _panelLightPos;
				float4 _panelLightWorldPNormal;
                half4 _panelLightColor;
                half4 _panelLightRange;
				half4 _panellightAttenution;
				half _panelLightifTranverse;
            
                half _panelLightInstensity;
                half _panelLocalScale;
				half _panelLightAsQuad;
            CBUFFER_END
                float4x4 _panelLocalMatrix;

                // TEXTURE2D(_GBuffer0); SAMPLER(sampler_GBuffer0);
                // TEXTURE2D(_GBuffer1); SAMPLER(sampler_GBuffer1);
                // TEXTURE2D(_GBuffer2); SAMPLER(sampler_GBuffer2);
				TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
                TEXTURE2D(_CameraOpaqueTexture); SAMPLER(sampler_CameraOpaqueTexture);
                TEXTURE2D(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture);
            
	        struct a2v
	        {
	            float4 positionOS : POSITION;
	            float3 normal : Normal;
	        };

	        struct v2f
	        {
	            float3 worldNormal  : TEXCOORD0;
	            float3 positionWS   : TEXCOORD1;
	        	float4 uv           : TEXCOORD2;
	            float4 positionCS   : SV_POSITION;
	        };

		    float3 GetFragmentWorldPos(float2 screenPos)
		    {
		        float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, screenPos);
		        float4 ndc = float4(screenPos.x * 2 - 1, screenPos.y * 2 - 1, sceneRawDepth, 1);
		        #if UNITY_UV_STARTS_AT_TOP
		            ndc.y *= -1;
		        #endif
		        float4 worldPos = mul(UNITY_MATRIX_I_VP, ndc);
		        worldPos /= worldPos.w;
		    
		        return worldPos.xyz;
		    }
            
			float3 DeferredCalculateLightParams (v2f i)
			{
				float2 uv = i.uv.xy / i.uv.w;
		        float3 worldPos = GetFragmentWorldPos(uv);
				return worldPos;
			}
            
            v2f vert (a2v v)
            {
                v2f o;
				o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.positionWS = TransformObjectToWorld(v.positionOS);
                o.positionCS = TransformObjectToHClip(v.positionOS);
				o.uv = ComputeScreenPos(o.positionCS);
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                float3 worldPos = DeferredCalculateLightParams(i);
            	float4 positionPannel = mul(_panelLocalMatrix, float4(worldPos, 1));
            	positionPannel.z = _panelLightifTranverse ? -positionPannel.z: positionPannel.z;
		
            	half lightRangeAttenution = (1 - _panelLightWorldPNormal.y);
				_panelLightRange.z *= lightRangeAttenution;
            	_panelLightRange.y = lerp(5, _panelLightRange.y, lightRangeAttenution);
            	_panellightAttenution.x *= lightRangeAttenution;
            	_panellightAttenution.z = max(0, _panellightAttenution.z);

            	half LightTransformAttenution = saturate(pow(lightRangeAttenution, 0.1));
            	half verticalAttenution = (5 - positionPannel.z) * _panelLightRange.z;
            	
            	half attenution = saturate(lerp(0, 1, (positionPannel.z + _panelLightRange.y) *
            		 lerp(max(0, 2 - _panellightAttenution.z * 0.5), max(0, _panellightAttenution.y), LightTransformAttenution)));
            	attenution *= saturate(lerp(0, 1, (-positionPannel.z + 5)));
            	attenution *= step(positionPannel.y, 0);
            	
            	attenution *= saturate(lerp(1, 0, (positionPannel.x - verticalAttenution - _panelLightRange.x + _panellightAttenution.z)
            		/ ((5 - positionPannel.z) * max(0, _panellightAttenution.x) + _panellightAttenution.z)));
            	attenution *= saturate(lerp(1, 0, (-positionPannel.x - verticalAttenution - _panelLightRange.x + _panellightAttenution.z)
            		/ ((5 - positionPannel.z) * max(0, _panellightAttenution.x) + _panellightAttenution.z)));

                float4 albedo = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, i.uv.xy / i.uv.w);
            	half4 panelLightColor = albedo * _panelLightColor * _panelLightInstensity;
                return panelLightColor * attenution;
            }
            ENDHLSL
        }
    }
}
