Shader "Custom/RotateCircleWithHoles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
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
                UNITY_DEFINE_INSTANCED_PROP(float, _AnimTime)
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
            sampler2D _MaskTex;
            float4 _MainTex_ST;

            float Remap(float val, float min, float max, float newMin, float newMax)
            {
                return (val - min) / (max - min) * (newMax - newMin) + newMin;
            }

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

            float hash(float n)
            {
                return frac(sin(n * 12.9898) * 43758.5453);
            }

            float2 fade(float2 t) { return t * t * t * (t * (t * 6 - 15) + 10); }

            float grad2(float hash, float2 p)
            {
                float angle = hash * 6.2831853;
                return cos(angle) * p.x + sin(angle) * p.y;
            }
            
            float hash21(float2 p)
            {
                float h = dot(p, float2(127.1, 311.7));
                return frac(sin(h) * 43758.5453123);
            }
            
            float perlinNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
            
                float a = hash21(i + float2(0,0));
                float b = hash21(i + float2(1,0));
                float c = hash21(i + float2(0,1));
                float d = hash21(i + float2(1,1));
            
                float2 u = fade(f);
            
                return lerp(
                    lerp(grad2(a, f - float2(0,0)), grad2(b, f - float2(1,0)), u.x),
                    lerp(grad2(c, f - float2(0,1)), grad2(d, f - float2(1,1)), u.x),
                    u.y
                );
            }

            float pingPong01(float t)
            {
                return 1.0 - abs(t * 2.0 - 1.0);
            }

            // normalized coordinates (-1..1) // 0..1
            float4 drawAnimatedCircleRing(float2 uv, float t, float radius, out float ringRadius, out float ringMask)
            {
                float dist = length(uv);

                // -----------------------------
                // Circle mask
                // -----------------------------
                float circleSDF = dist - radius;
                float circleMask = smoothstep(0.01, -0.01, circleSDF);

                // -----------------------------
                // Ring animation (bounce)
                // -----------------------------
                float pp = t;
                ringRadius = lerp(0.0, radius, pp);
                float ringWidth = lerp(0, 0.2, ringRadius);

                float ringSDF = abs(dist - ringRadius);
                ringMask = smoothstep(ringWidth, 0.0, ringSDF);

                // -----------------------------
                // Bounce detection
                // -----------------------------
                float bounce = smoothstep(radius - 0.05, radius, ringRadius);

                // -----------------------------
                // Knockback wave (shockwave)
                // -----------------------------
                // float waveSpeed = 0.2;
                // float waveRadius = lerp(radius, 0.0, saturate(bounce * waveSpeed));
                float waveRadius = lerp(radius, 0.0, bounce);
                float waveWidth = 0.08;

                float wave = smoothstep(
                    waveWidth,
                    0.0,
                    abs(dist - waveRadius)
                ) * bounce;

                // -----------------------------
                // Colors
                // -----------------------------
                float circleColor = 1; // rgb(29, 43, 83)
                float ringColor   = 0.45;// * lerp(float4(0.11372549, 0.16862745, 0.3254902, 1.0), float4(1, 0, 0.30196078, 1.0), pp); // rgb(29, 43, 83), rgb(255, 0, 77)
                float glowColor   = 0.75;//8 * float4(0.33, 0.03, 0.39, 1.0);

                // -----------------------------
                // Color blending
                // -----------------------------
                float base = circleColor * circleMask;

                float ringCol = ringColor * ringMask;
                float glowCol = glowColor * (ringMask * bounce * 2.0 + wave);

                // Smooth blend ring into circle
                float blend = saturate(ringMask * 0.8);
                float blended = lerp(base, ringCol, blend);

                float3 result = blended + glowCol;
                result.r = ringCol;
                result.g = glowCol;
                result.b = circleColor;

                return float4(result, 1) * circleMask * 2;
            }

            float2 rotateUV(float2 uv, float rotation)
            {
                float angle = atan2(uv.y, uv.x) + rotation; // Add rotation
                float radius = length(uv); // Distance from center
                return float2(cos(angle), sin(angle)) * radius;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float circleCircumference = 1;
                float2 rotatedUV = rotateUV(uv, _Time * 8);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                float t = UNITY_ACCESS_INSTANCED_PROP(_Prop, _AnimTime);
                t = 1 - (sin(t * 3.1415926535897931));
                float disolveSpeed = 0.5;
                float ringMask;
                float ringRadius;
                float4 circleWithRing = drawAnimatedCircleRing(rotatedUV, Remap(t, 0, 1, 0.6, 1), circleCircumference, ringRadius, ringMask) * 0.5;

                float distance = length(rotatedUV);
                float circle = step(distance, circleCircumference);
                float smoothEdgedCircle = (1 - smoothstep(0.85, circleCircumference, distance)) * circle;
                float ringInnerMask = (1 - step(ringRadius, distance)) * ringRadius;
                
                float freq = 128;
                int octaves = 2;
                float noiseMask = distance;
                float colorDistance = length(baseColor);
                for (int i = 0; i < octaves; ++i)
                {
                    noiseMask += perlinNoise((_Time * disolveSpeed) - (rotatedUV + ((uv.yx + (colorDistance * 100)) * 0.0025) * freq * 1)) * 1 + 0.15;
                    noiseMask += perlinNoise((_Time * disolveSpeed) + (rotatedUV + ((uv.xy + (colorDistance * 100)) * 0.0005) * freq * 2)) * 1.5 + 0.15;
                }

                 // inner circle
                float noisedCircle = smoothstep((1 - noiseMask) * lerp(0, ringInnerMask, t), ringMask, t) * circle;
                noisedCircle= (((1 - noisedCircle) * ringInnerMask) * 6) + noisedCircle;
                float noisedCircleSDF = (abs(noiseMask - ringRadius)) * ringInnerMask;
                float circleNoised = smoothstep((1 - noisedCircleSDF) * noisedCircle * 10, 0, 1);
                float innerCircleNoised = (circleNoised + noisedCircleSDF) * ringInnerMask;

                float innerCircle = circleNoised;
                float outerRing = (1 - (1 - ringMask)) * (1 - ringInnerMask) * smoothEdgedCircle;
                outerRing *= smoothstep(-1, 1, noisedCircle);
                
                float leakAmount = (0.5 + distance);
                float leak = lerp(smoothEdgedCircle, noiseMask, smoothstep(0, ringRadius, 1 - t)) * leakAmount;
                leak = leak * (circle * (1 - step(0.5, ringInnerMask))) * ((1 - distance) * 16);
                
                // leak = saturate(leak);
                leak = Remap(leak, 0, 1, 0.75, 1);
                leak *= ((1 - ringInnerMask) * smoothEdgedCircle);
                // leak = 1 - leak;
                // leak *= 0.5;
                // leak = (saturate(leak) * 0.72) + (saturate(1 - leak)) * smoothEdgedCircle;
                // leak *= 2;

                // float maskIntensitiy = lerp(8, 6, ringRadius);
                // float m = ((ringInnerMask) * (maskIntensitiy * 0.075)) * (smoothstep(ringRadius + (ringMask * 0.15), 0, distance) * maskIntensitiy);
                float4 ringGlow = outerRing * (5 * t) * baseColor;

                float4 circleMaskWithRing = (innerCircleNoised) * saturate(circleWithRing * circleWithRing.g) * smoothEdgedCircle + leak;
                // circleMaskWithRing = circleMaskWithRing * (1 - distance);
                // float glow = (0.5 + 2 * t * t * t);
                float sdf = (abs(distance - innerCircle));
                float glowedSDF = sdf + ((1 - sdf) * 2) * 0.15;
                float4 output = clamp(glowedSDF * ringInnerMask * baseColor + circleMaskWithRing * baseColor + ringGlow, 0, 1);
                output.a = circle;
                return output;
            }


            ENDCG
        }
    }
}
