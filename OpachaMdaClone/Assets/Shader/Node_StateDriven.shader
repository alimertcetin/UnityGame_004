Shader "Custom/Node_StateDriven"
{
    Properties
    {
        _Color ("Node Color (Ownership)", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0.2,1)) = 0.9
        _RingWidth ("Ring Width", Range(0.01,0.2)) = 0.08
        _RotSpeed ("Rotation Speed", Range(0,10)) = 1.0
        _CycleSpeed ("Cycle Speed", Range(0.1,5)) = 1.0
        _AnimTime ("Animation Time", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 _Color;
            float _Radius;
            float _RingWidth;
            float _RotSpeed;
            float _CycleSpeed;
            float _AnimTime;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            float2 rotate(float2 p, float a)
            {
                float s = sin(a);
                float c = cos(a);
                return float2(c*p.x - s*p.y, s*p.x + c*p.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float2 uv = i.uv;
                float dist = length(uv);

                // -----------------------------
                // Time & State
                // -----------------------------
                float t = frac(_Time.y * _CycleSpeed);
                // t = _AnimTime;

                float prep      = smoothstep(0.0, 0.7, t);
                float generate  = smoothstep(0.7, 0.95, t);
                float done      = smoothstep(0.95, 1.0, t);

                // -----------------------------
                // Motion
                // -----------------------------
                float rot =
                    _Time.y * _RotSpeed *
                    lerp(0.5, 1.8, prep) *
                    (1.0 - done);

                float2 rUV = rotate(uv, rot);
                float d = length(rUV);

                // -----------------------------
                // Ring behavior
                // -----------------------------
                float ringRadius = lerp(_Radius * 1.1, _Radius * 0.85, prep);
                ringRadius = lerp(ringRadius, _Radius, generate);

                float ring = smoothstep(
                    _RingWidth,
                    0.0,
                    abs(d - ringRadius)
                );

                // -----------------------------
                // Inner energy (holes / noise-lite)
                // -----------------------------
                float pulse = sin(_Time.y * 6 + d * 12) * 0.5 + 0.5;
                pulse *= _AnimTime;
                float inner = smoothstep(ringRadius - 0.2, ringRadius - 0.05, d);
                inner *= lerp(pulse, 1.0, generate);

                // -----------------------------
                // Generated impact
                // -----------------------------
                float shockRadius = lerp(0.0, 1.3, done);
                float shock = smoothstep(
                    0.05,
                    0.0,
                    abs(dist - shockRadius)
                ) * done;

                // -----------------------------
                // Color composition
                // -----------------------------
                float3 baseCol = _Color.rgb;

                float intensity =
                    lerp(0.85, 1.2, prep) *
                    lerp(1.0, 1.1, generate);

                float3 col =
                    baseCol * intensity * (ring + inner * 0.6);

                // White accent ONLY for generation moment
                col += float3(1,1,1) * shock * 1.5;

                float alpha =
                    // saturate(ring + inner) *
                    step(dist, _Radius + 0.05);

                return float4(col + (baseCol * (0.8 - (dist))), alpha);
            }
            ENDCG
        }
    }
}
