Shader "Custom/LineWithShadow_Gradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.4)
        _ShadowOffset ("Shadow Offset", Vector) = (0, -0.1, 0, 0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        // === PASS 1: Shadow ===
        Pass
        {
            Name "ShadowPass"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _ShadowColor;
            float4 _ShadowOffset;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // from LineRenderer gradient
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                v.vertex.xyz += _ShadowOffset.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Use vertex alpha for soft shadow edges
                return _ShadowColor * i.color.a;
            }
            ENDCG
        }

        // === PASS 2: Main Line ===
        Pass
        {
            Name "MainPass"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Use vertex color gradient from LineRenderer
                fixed4 texCol = tex2D(_MainTex, i.uv);
                return texCol * i.color;
            }
            ENDCG
        }
    }
}
