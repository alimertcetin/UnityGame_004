Shader "Custom/TriangleAttachedToCircle"
{
    Properties
    {
        _Color("Color", Color) = (1,1,0,1)

        _Center("Circle Center (UV)", Vector) = (0.5,0.5,0,0)
        _CircleRadius("Circle Radius (UV)", Float) = 0.25

        _AttachAngle("Attach Angle (deg)", Range(0,360)) = 0
        _AngularWidth("Angular Width (deg)", Range(1,180)) = 25

        _TriangleLength("Triangle Length (UV)", Float) = 0.25
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float4 _Color;

            float4 _Center;
            float _CircleRadius;

            float _AttachAngle;
            float _AngularWidth;
            float _TriangleLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // returns degrees in range -180..+180
            float AngleDiff(float a, float b)
            {
                float d = a - b;
                d = fmod(d + 180.0, 360.0);
                if (d < 0) d += 360.0;
                return d - 180.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 dir = (uv - _Center.xy) * 2;
                float r = length(dir);

                // angle of pixel
                float ang = atan2(dir.y, dir.x) * 57.29578;

                // how far angularly from the triangle center
                float halfW = _AngularWidth * 0.5;
                float dAng = abs(AngleDiff(ang, _AttachAngle));

                // completely outside angular wedge → discard
                if (dAng > halfW)
                    return float4(0,0,0,0);

                // compute linear triangle outer radius at this angle
                // At angle 0 → full length
                // At angle +/-halfW → 0 length
                float t = 1.0 - dAng / halfW;
                float outerRadius = _CircleRadius + t * (_TriangleLength * 0.1);

                // inside triangle (between circle radius and triangle tip)
                if (r < _CircleRadius || r > outerRadius)
                    return float4(0,0,0,0);

                return _Color;
            }
            ENDCG
        }
    }
}
