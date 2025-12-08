Shader "Custom/SpriteWithShadow_Instanced"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.4)
        _ShadowOffset ("Shadow Offset", Vector) = (0, -0.1, 0, 0)
        _ShadowSize ("Shadow Size", Vector) = (1.05, 1.05, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass        // ---------- SHADOW PASS ----------
        {
            Name "ShadowPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // ----------------- INSTANCING -----------------
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowOffset)
                UNITY_DEFINE_INSTANCED_PROP(float4, _ShadowSize)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 shadowOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowOffset);
                float4 shadowSize   = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowSize);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 tangent = normalize(mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
                float3 binormal = normalize(cross(worldNormal, tangent));

                float3 localOffset =
                    (v.uv.x - 0.5) * tangent * (shadowSize.x - 1.0) +
                    (v.uv.y - 0.5) * (-binormal) * (shadowSize.y - 1.0);
                    // binormal * (shadowSize.y - 1.0) * 0.5;

                worldPos += localOffset;
                worldPos += shadowOffset.xyz;

                o.vertex = UnityWorldToClipPos(worldPos);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float4 shadowColor = UNITY_ACCESS_INSTANCED_PROP(Props, _ShadowColor);
                return shadowColor * i.color.a * texColor;
            }
            ENDCG
        }

        Pass
        {
            Name "MainPass"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color); // combine vertex color & SpriteRenderer.color
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color; // final sprite color
            }
            ENDCG
        }
    }
}
