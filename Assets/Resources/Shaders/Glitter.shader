Shader "Custom/Glitter"{
    Properties{
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 1
        [HideInInspector]
        _Stencil ("Stencil", Int) = 0
        _Speed ("Speed", Float) = 0.5
        _Scale("Scale", Float) = 1
        _UseWorldSpace("UseWorldSpace (1 for true)", Int) = 0
    }

    SubShader{
        Tags{
            "Queue" = "Transparent"
            "RenderType" = "Sprite"
        }
        LOD 100

        ZWrite off
        Cull off
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil{
            Ref [_StencilRef]
            Comp LEqual
        }

        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include  "Common/CommonShaderMethods.hlsl"

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            }; 

            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Emission;
            float _Speed;
            float4 _Color;
            int _UseWorldSpace;
            float _Scale;

            v2f vert(appdata v){
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target{
                float a = tex2D(_MainTex, i.uv).a;
                float glit = 0;
                glit += _UseWorldSpace * clamp(GradientNoise(TransformObjectToHClip(float3(i.vertex.xyz)).xy + _Time * _Speed, _Scale / 1000), 0.1, 1);
                glit += (1 - _UseWorldSpace) * clamp(GradientNoise(i.uv + _Time * _Speed / -15, 1.5), 0.1, 1);
                float4 color = _Color * (_Emission + 1);
                return float4(color.rgb * glit, pow(clamp(a * (length(_Color.rgb) - 0.5), 0, 1.5), 5)) * i.color;
            }
            ENDHLSL
        }
    }
}