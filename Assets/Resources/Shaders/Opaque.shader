Shader
"Custom/OpaqueSprite"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 1
        [HideInInspector]
        _Stencil ("Stencil", Int) = 1
    }



    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Sprite"
        }
        LOD 100

        ZWrite On
        Cull off
        //        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Less

        Stencil
        {
            Ref [_Stencil]
            Comp LEqual
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Emission;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                clip(col.a - 0.2);
                float4 color = _Color * col * (_Emission + 1);
                return color * i.color;
            }
            ENDCG
        }
    }
}