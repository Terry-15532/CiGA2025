Shader "Custom/URP2D/SpriteCastShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.1
        _NoiseScale ("Noise Scale", Float) = 1.0
        [Header(Casting Shadow)]
        [Space(10)]
        [Toggle(_CAST_SHADOW)] _CastShadow ("Cast Sahdow", Integer) = 0
        [Toggle(_OVERRIDE_BIAS)] _OverrideBias ("Override Shadow Bias", Integer) = 0
        _DepthBias ("Depth Bias", Range(-1, 1)) = 0
        _NormalBias ("Normal Bias", Range(-1, 1)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
            "IgnoreProjector"="True"
        }
        Cull Off
        ZWrite On
        ZTest LEqual

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            HLSLPROGRAM
            
            
            
            
            
			#pragma shader_feature _CAST_SHADOW
			#pragma shader_feature _OVERRIDE_BIAS

			#include "Packages/com.unity.render-pipelines.unive	rsal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
			#pragma vertex CustomShadowVertex
			#pragma fragment ShadowPassFragment

			float _DepthBias, _NormalBias;
			bool _OverrideBias;

			Varyings CustomShadowVertex(Attributes v){
				#if _CAST_SHADOW
				#if _OVERRIDE_BIAS
                    _ShadowBias.xy = float2(_DepthBias / -10, _NormalBias / -10);
				#endif
				return ShadowPassVertex(v);
				#else
				Varyings varyings = ShadowPassVertex(v);
				varyings.positionCS = float4(-1, -1, -1, -100);
				return varyings;
				#endif
			}
			
            
            
            
            
            ENDHLSL
        }

        //———— 正常透明渲染 Pass ————
        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode"="UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBase
            #pragma multi_compile _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Resources/Shaders/Common/CommonShaderMethods.hlsl"

            TEXTURE2D (_MainTex);
            SAMPLER (sampler_MainTex);
            float4 _Color;
            float _Cutoff, _NoiseScale;

            struct Attributes
            {
                float3 positionOS: POSITION;
                float4 color : COLOR;
                float2 uv: TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv: TEXCOORD0;
                float4 color: COLOR;
                float4 positionCS: SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            float3 AdjustSaturation(float3 color, float saturation)
            {
                // 使用 luminance 系数计算灰度值（人眼对不同颜色敏感度不同）
                float gray = dot(color, float3(0.299, 0.587, 0.114));

                // 从灰度（无饱和度）插值回原色（完整饱和度）
                return lerp(float3(gray, gray, gray), color, saturation);
            }


            half4 fragBase(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                tex.rgb = AdjustSaturation(tex.rgb, IN.color.b);
                half noise = SimpleNoise(IN.uv, _NoiseScale);
                noise = 1 - lerp(0, 0.3, pow(noise, 2));
                clip(tex.a - _Cutoff);
                return half4(tex.rgb * _Color * noise * IN.color.r, 1 * tex.a * IN.color.a);
            }
            ENDHLSL
        }
    }
}