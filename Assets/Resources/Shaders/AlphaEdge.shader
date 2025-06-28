Shader "Custom/AlphaEdge"{
    Properties{
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 1
        _Threshold ("Threshold", Float) = 0.2
    }
    SubShader{
        Tags{
            "RenderType"="Transparent"
            "Queue"="Geometry"
        }
        LOD 200
        ZWrite On
        ZTest LEqual
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
//        AlphaTest Greater 0.1

        Stencil{
            Ref [_Stencil]
            Comp LEqual
        }

        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include  "Common/CommonShaderMethods.hlsl"

            struct appdata_t{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EdgeColor;
            float4 _MainTex_TexelSize;
            float _Emission;
            float _Threshold;

            v2f vert(appdata_t v){
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target{
                float4 currentPixel = tex2D(_MainTex, i.uv);
                float alpha = currentPixel.a;

                float2 offsets[8] = {
                    float2(-1, 0), float2(1, 0), float2(0, -1), float2(0, 1), float2(1, 1), float2(-1, -1), float2(1, -1), float2(-1, 1)
                };
                for (int j = 0; j < 8; j++){
                    float4 neighborPixel = tex2D(_MainTex, i.uv + offsets[j] * _MainTex_TexelSize.xy);
                    if (abs(neighborPixel.a - alpha) > _Threshold){
                        return _EdgeColor * _Emission;
                    }
                }

                if(alpha<0.2){
                    discard;
                }

                return currentPixel;
            }
            ENDHLSL
        }
    }
}