Shader "Custom/CircleTangentTriangle"
{
    Properties
    {
        _Color("Color", Color) = (1,1,0,1)

        _Center("Circle Center (UV)", Vector) = (0.5,0.5,0,0)
        _Radius("Circle Radius (UV)", Float) = 0.25

        _Angle("Attach Angle (deg)", Range(0,360)) = 0
        _Length("Triangle Length (UV)", Float) = 0.6
    }

    SubShader
    {
        Tags{"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
            };

            float4 _Color;

            float4 _Center;
            float _Radius;
            float _Angle;
            float _Length;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // signed distance to line (point-to-line)
            float sdLine(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                return length(pa - ba * h);
            }

            // signed side of infinite line
            float side(float2 p, float2 a, float2 b)
            {
                float2 d = b - a;
                float2 n = float2(-d.y, d.x);
                return dot(p - a, n);
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 C  = _Center.xy;

                float angleRad = radians(_Angle);
                float2 apex = C + float2(cos(angleRad), sin(angleRad)) * _Radius;

                // tangent angle offset
                float phi = acos(_Radius / (_Radius + _Length));

                float2 dir = normalize(apex - C);

                float2 t1 = float2(
                    dir.x * cos(phi) - dir.y * sin(phi),
                    dir.x * sin(phi) + dir.y * cos(phi)
                );

                float2 t2 = float2(
                    dir.x * cos(-phi) - dir.y * sin(-phi),
                    dir.x * sin(-phi) + dir.y * cos(-phi)
                );

                float2 p1 = apex;
                float2 p2 = apex + t1 * _Length;
                float2 p3 = apex + t2 * _Length;

                float s1 = side(uv, p1, p2);
                float s2 = side(uv, p2, p3);
                float s3 = side(uv, p3, p1);

                float inside = (s1 >= 0 && s2 >= 0 && s3 >= 0) ? 1.0 : 0.0;

                float4 col = _Color;
                col.a *= inside;

                return col;
            }

            ENDCG
        }
    }
}
