Shader "Custom/URP2D/SpriteCastShadow_Water"
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
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTestMode ("ZTest Mode", Float) = 4
        _DepthBias ("Depth Bias", Range(-1, 1)) = 0
        _NormalBias ("Normal Bias", Range(-1, 1)) = 0
        _ShakeStrength ("Shake Strength", Range(0, 0.5)) = 0.1
        _ShakeSpeed ("Shake Speed", Float) = 2.0
        [Header(Water Refract)]
        _RefractSpeed ("Speed", Vector) = (1,1,1,1)

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
        ZTest [_ZTestMode]

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

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            #include "Assets/Resources/Shaders/Common/CommonShaderMethods.hlsl"


            #pragma vertex CustomShadowVertex
            #pragma fragment CustomShadowFragment

            float _DepthBias, _NormalBias;
            bool _OverrideBias;
            float _ShakeStrength;
            float _ShakeSpeed;

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct CustomVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CustomVaryings CustomShadowVertex(Attributes v)
            {

                float yFactor = saturate(v.texcoord.y);
                float timeNoise = GradientNoise(v.texcoord * 3.0 + _Time * _ShakeSpeed, 0.11);
                float angle = timeNoise * 6.28318; // [0,1] → [0,2π]
                float2 offset = float2(cos(angle), sin(angle)) * _ShakeStrength * yFactor;

                v.positionOS.xy += offset;

                #if _CAST_SHADOW
                #if _OVERRIDE_BIAS
                    _ShadowBias.xy = float2(_DepthBias / -10, _NormalBias / -10);
                #endif
				return ShadowPassVertex(v);
                #else
                Varyings varyings = ShadowPassVertex(v);
                CustomVaryings vary;
                vary.positionCS = float4(-1, -1, -1, -100);
                vary.uv = v.texcoord;
                return vary;
                #endif
            }

            half4 CustomShadowFragment(CustomVaryings IN) : SV_Target
            {
                half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                clip(a - _Cutoff);
                Varyings v;
                v.positionCS = IN.positionCS;
                return ShadowPassFragment(v);
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
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragBase
            #pragma multi_compile _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #define ADDITIONAL_LIGHT_CALCULATE_SHADOWS


            #pragma multi_compile _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/Resources/Shaders/Common/CommonShaderMethods.hlsl"


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // float4x4 _AdditionalLightsWorldToShadow[MAX_VISIBLE_LIGHTS];

            float4 _Color, _RefractSpeed;
            float _Cutoff, _NoiseScale;
            float _ShakeStrength;
            float _ShakeSpeed;


            struct Attributes
            {
                float3 positionOS: POSITION;
                float4 color : COLOR;
                float2 uv: TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct Varyings
            {
                float2 uv: TEXCOORD0;
                float4 color: COLOR;
                float4 positionCS: SV_POSITION;
                float3 positionWS: TEXCOORD1;
                float3 normalWS: NORMAL;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 原始位置
                float3 pos = IN.positionOS;

                // 计算扰动：以 uv.y 为权重，叠加噪声和正弦移动
                float yFactor = saturate(IN.uv.y);
                float timeNoise = GradientNoise(IN.uv * 3.0 + _Time * _ShakeSpeed, 0.11);
                float angle = timeNoise * 6.28318; // [0,1] → [0,2π]
                float2 offset = float2(cos(angle), sin(angle)) * _ShakeStrength * yFactor;

                pos.xy += offset;

                OUT.positionCS = TransformObjectToHClip(pos);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normal);
                OUT.positionWS = TransformObjectToWorld(pos);
                return OUT;
            }

            float3 AdjustSaturation(float3 color, float saturation)
            {
                float gray = dot(color, float3(0.299, 0.587, 0.114));

                return lerp(float3(gray, gray, gray), color, saturation);
            }


            half4 fragBase(Varyings IN) : SV_Target
            {
                float noise1 = GradientNoise(IN.uv + _Time * _RefractSpeed, 15);
                float noise2 = GradientNoise(IN.uv + _Time * _RefractSpeed * float4(1.1, -1.5, 1, 1), 20);

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, clamp(IN.uv + noise1*noise2 * 0.1, 0.1, 0.8));
                clip(tex.a - _Cutoff);
                tex.rgb = AdjustSaturation(tex.rgb, IN.color.b);
                half noise = SimpleNoise(IN.uv, _NoiseScale);
                noise = 1 - lerp(0, 0.3, pow(noise, 2));

                float3 worldPos = IN.positionWS.xyz;

                Light light = GetAdditionalLight(0, worldPos);
                float3 lightDir = normalize(light.direction);
                float NdotL = saturate(5 * abs(dot(IN.normalWS, lightDir)));
                float3 finalLight = light.color * NdotL * light.distanceAttenuation * light.shadowAttenuation;

                //Additional Light Shadow
                half4 shadowParams = GetAdditionalLightShadowParams(0);
                ShadowSamplingData shadowSamplingData = GetAdditionalLightShadowSamplingData(0);

                int shadowSliceIndex = shadowParams.w;
                float4 shadowCoord = mul(_AdditionalLightsWorldToShadow[shadowSliceIndex], float4(IN.positionWS, 1.0));
                float shadow = SampleShadowmap(
                    TEXTURE2D_ARGS(_AdditionalLightsShadowmapTexture, sampler_LinearClampCompare), shadowCoord, shadowSamplingData, shadowParams, true);
                return half4(
                    tex.rgb * _Color * noise * IN.color.r * finalLight * 2 * saturate(shadow + 0.4), pow(IN.color.a * tex.a, 0.7));
            }
            ENDHLSL
        }
    }
}