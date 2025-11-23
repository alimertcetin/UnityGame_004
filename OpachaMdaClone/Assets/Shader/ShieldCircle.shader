Shader "Custom/ShieldCircle"
{
    Properties
    {
        _ColorActive("Active Color", Color) = (0,1,1,1)
        _ColorInactive("Inactive Color", Color) = (0,0.2,0.2,1)

        _TotalShield("Total Shield", Float) = 12
        _CurrentShield("Current Shield", Float) = 6

        _Radius("Radius", Float) = 0.45
        _Thickness("Thickness", Float) = 0.08

        _Gap("Segment Gap (degrees)", Float) = 6.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float4 _ColorActive;
            float4 _ColorInactive;

            float _TotalShield;
            float _CurrentShield;

            float _Radius;
            float _Thickness;
            float _Gap;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // -1 .. 1 screen-space quad
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;

                float dist = length(uv);
                if (dist > _Radius || dist < _Radius - _Thickness)
                    discard;

                // Angle in degrees 0–360
                float angle = degrees(atan2(uv.y, uv.x));
                angle = angle < 0 ? angle + 360 : angle;

                float segmentSize = 360.0 / _TotalShield;
                float segIndex = floor(angle / segmentSize);

                // Compute local angle inside segment
                float localAngle = angle - segIndex * segmentSize;
                if (localAngle < _Gap * 0.5 || localAngle > segmentSize - _Gap * 0.5)
                    discard; // create gaps

                // Active or inactive?
                bool active = segIndex < _CurrentShield;

                return active ? _ColorActive : _ColorInactive;
            }

            ENDHLSL
        }
    }
}
