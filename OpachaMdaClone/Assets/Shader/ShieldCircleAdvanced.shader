Shader "Custom/ShieldCircleAdvanced"
{
    Properties
    {
        _ColorActive("Active Color", Color) = (0.1,0.8,1,1)
        _ColorInactive("Inactive Color", Color) = (0.02,0.1,0.12,1)
        _GlowColor("Glow Color", Color) = (0.08,0.9,1,1)

        _TotalShield("Total Shield", Float) = 12.0
        _CurrentShield("Current Shield", Float) = 8.0

        _Radius("Radius", Float) = 0.45
        _Thickness("Thickness", Float) = 0.08
        _GapDeg("Gap Degrees", Float) = 6.0

        _EdgeSoftness("Edge Softness", Float) = 0.008
        _GlowIntensity("Glow Intensity", Float) = 1.5

        _PulseAmp("Pulse Amp", Float) = 0.06
        _PulseFreq("Pulse Freq", Float) = 1.6
        _PulseSeed("Pulse Seed", Float) = 0.0

        _DamageProgress("Damage Progress", Float) = 0.0 // 0 = no damage anim, 1 = fully applied
        _DamageFlash("Damage Flash Strength", Float) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            float4 _GlowColor;

            float _TotalShield;
            float _CurrentShield;

            float _Radius;
            float _Thickness;
            float _GapDeg;
            float _EdgeSoftness;
            float _GlowIntensity;

            float _PulseAmp;
            float _PulseFreq;
            float _PulseSeed;

            float _DamageProgress;
            float _DamageFlash;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Assume quad uv 0..1 -> remap to -1..1
                o.uv = v.uv * 2.0 - 1.0;
                return o;
            }

            // convert angle to [0,360)
            float AngleDeg(float2 uv)
            {
                float a = degrees(atan2(uv.y, uv.x));
                return a < 0 ? a + 360.0 : a;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // distance from center (0..inf)
                float dist = length(uv);

                // pulse offset changes radius slightly per object (gives breathing)
                float globalTime = _Time.y; // seconds
                float pulse = sin(globalTime * _PulseFreq + _PulseSeed) * _PulseAmp;
                float r = _Radius + pulse;

                // inner/outer radii
                float innerR = r - _Thickness;
                float outerR = r;

                // quick reject for performance
                if (dist > outerR + _EdgeSoftness || dist < innerR - _EdgeSoftness)
                    discard;

                // radial falloff for smooth edge
                float edge;
                if (dist > outerR - _EdgeSoftness) edge = saturate((outerR - dist) / _EdgeSoftness);
                else if (dist < innerR + _EdgeSoftness) edge = saturate((dist - innerR) / _EdgeSoftness);
                else edge = 1.0;

                // compute angle and segment
                float angle = AngleDeg(uv);
                float segSize = 360.0 / max(1.0, _TotalShield);
                float segIndexFloat = floor(angle / segSize);
                float segLocalAngle = angle - segIndexFloat * segSize; // 0..segSize

                // gap handling (centered gap)
                float halfGap = _GapDeg * 0.5;
                float gapMask = step(halfGap, segLocalAngle) * step(segLocalAngle, segSize - halfGap);

                // fractional current shield: current can be float (like 3.4)
                // segment active if segIndex < floor(_CurrentShield) => fully active
                // if segIndex == floor(_CurrentShield) then partially active by fractional part.
                float curr = _CurrentShield;
                float fullCount = floor(curr);
                float frac = curr - fullCount;

                float isActive = 0.0;
                float idx = segIndexFloat;

                if (idx < fullCount)
                {
                    isActive = 1.0;
                }
                else if (idx == fullCount)
                {
                    // for the partial segment, we allow portion of the segment from start angle.
                    // compute fraction of segment that should be filled (0..1)
                    float fillPortion = frac;
                    // compare segLocalAngle / segSize to fillPortion
                    float localNorm = segLocalAngle / segSize;
                    // smooth edge for partial
                    isActive = smoothstep(0.0, 1.0, (fillPortion - localNorm) * 100.0); // sharp step
                }
                else
                {
                    isActive = 0.0;
                }

                // apply gap mask (0 if within gap)
                isActive *= gapMask;

                // base color depending on active
                float4 baseColor = lerp(_ColorInactive, _ColorActive, isActive);

                // apply damage flash: _DamageProgress goes 0->1 during damage, we add a flash on active segments
                // Damage flash intensity modulated by whether segment is being removed: if segment index >= curr then it was just lost.
                float beingRemoved = step(curr, idx); // 1 if idx >= curr (i.e., inactive segments at/after current)
                // but we want flash on the transition segment: when damage is happening, user script will animate curr and _DamageProgress
                float dmg = _DamageProgress; // 0..1
                float flash = smoothstep(0.0, 1.0, (1.0 - beingRemoved) * dmg); // flash only on active ones during hit
                // Add flash color
                baseColor.rgb = lerp(baseColor.rgb, baseColor.rgb + _DamageFlash * float3(1,0.6,0.2), flash);

                // normalized distance from outer edge (0 at outerR, 1 beyond)
                float outerDistNorm = saturate((dist - (outerR - _EdgeSoftness)) / (_Thickness + _EdgeSoftness));
                // glow: create a radial glow outward from the outer edge, stronger on active segments
                float glowFactor = (1.0 - outerDistNorm) * isActive;
                float glow = pow(glowFactor, 1.2) * _GlowIntensity;
                float3 glowCol = _GlowColor.rgb * glow;

                // alpha based on edge * segment presence
                float alpha = edge * (isActive * 1.0 + (1.0 - isActive) * 0.6); // inactive dimmer but visible

                // final color
                float3 col = baseColor.rgb + glowCol;

                // optional soft fade near gaps: reduce alpha near gap edges
                float gapEdgeSoft = 0.02;
                float gapEdgeFactor = 1.0;
                if (gapMask < 0.5)
                {
                    // inside gap -> discard to create hard gap
                    discard;
                }
                else
                {
                    // compute distance to gap edges to slightly soften
                    float leftEdge = halfGap;
                    float rightEdge = segSize - halfGap;
                    float edgeDist = min(segLocalAngle - leftEdge, rightEdge - segLocalAngle);
                    float soft = saturate(edgeDist / gapEdgeSoft);
                    alpha *= smoothstep(0.0, 1.0, soft);
                }

                return float4(col, alpha);
            }

            ENDHLSL
        }
    }
}
