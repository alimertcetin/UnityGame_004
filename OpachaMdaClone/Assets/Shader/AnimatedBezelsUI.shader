Shader "Custom/AnimatedBezelsUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColor("Background Color", Color) = (0,0,0,1)

        _BezelFrequency("Bezel Frequency", Range(0,1)) = 0.5
        _BezelMinSize("Bezel Min Size", Float) = 0.05
        _BezelMaxSize("Bezel Max Size", Float) = 0.15

        _BezelColor0("Bezel Color 0", Color) = (1,0,0,1)
        _BezelColor1("Bezel Color 1", Color) = (0,1,0,1)
        _BezelColor2("Bezel Color 2", Color) = (0,0,1,1)
        _BezelColor3("Bezel Color 3", Color) = (1,1,0,1)

        _BezelMinSpeed("Bezel Min Speed", Float) = 0.5
        _BezelMaxSpeed("Bezel Max Speed", Float) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="Always" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BackgroundColor;
            float _BezelFrequency;
            float _BezelMinSize;
            float _BezelMaxSize;
            float4 _BezelColor0;
            float4 _BezelColor1;
            float4 _BezelColor2;
            float4 _BezelColor3;
            float _BezelMinSpeed;
            float _BezelMaxSpeed;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Sample main texture
                float4 col = tex2D(_MainTex, uv) * _BackgroundColor;

                // --- 4 bezels ---
                float4 bezelColors[4];
                bezelColors[0] = _BezelColor0;
                bezelColors[1] = _BezelColor1;
                bezelColors[2] = _BezelColor2;
                bezelColors[3] = _BezelColor3;

                [unroll]
                for(int b=0; b<4; b++)
                {
                    float2 seedUV = uv + float2(b, b*1.23);

                    // Bezel spawn probability
                    if(random(seedUV) > _BezelFrequency) continue;

                    // Random bezel position
                    float2 center = float2(random(seedUV), random(seedUV + 1.0));

                    // Random speed
                    float speed = lerp(_BezelMinSpeed, _BezelMaxSpeed, random(seedUV + 2.0));

                    // Animate bezel radius
                    float t = frac(_Time * speed + random(seedUV + 3.0));
                    float radius = lerp(_BezelMinSize, _BezelMaxSize, sin(t * 3.1415));

                    // Distance from pixel to bezel center
                    float dist = distance(uv, center);

                    // Circular mask with soft edge
                    float mask = smoothstep(radius, radius*0.95, dist);

                    // Blend bezel color
                    col = lerp(bezelColors[b], col, mask);
                }

                return col;
            }

            ENDCG
        }
    }
}
