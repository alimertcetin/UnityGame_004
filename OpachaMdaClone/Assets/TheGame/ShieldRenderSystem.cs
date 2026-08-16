using TheGame;
using UnityEngine;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public struct ShieldRendererComp : IComponent
    {
        public Entity targetEntity;
        public Renderer renderer;
        public MaterialPropertyBlock materialPropertyBlock;
    }

    public struct ShieldRenderAnimationComp : IComponent
    {
        public float maxShieldStart;
        public float maxShieldTarget;
        public float currentShieldStart;
        public float currentShieldTarget;
        public float radiusStart;
        public float radiusTarget;
        public float thicknessStart;
        public float thicknessTarget;
        public float gapDegStart;
        public float gapDegTarget;
        public float edgeSoftnessStart;
        public float edgeSoftnessTarget;
        public float glowIntensityStart;
        public float glowIntensityTarget;
        public float pulseAmpStart;
        public float pulseAmpTarget;
        public float pulseFreqStart;
        public float pulseFreqTarget;
        public float damageProgressStart;
        public float damageProgressTarget;
        public float damageFlashStart;
        public float damageFlashTarget;
        public Color activeColorStart;
        public Color activeColorTarget;
        public Color glowColorStart;
        public Color glowColorTarget;

        public Timer durationTimer;
        public float flashTime;

        public void Init(ref ShieldRenderPropertyComp shieldRenderPropertyComp)
        {
            this.maxShieldStart = shieldRenderPropertyComp.maxShield;
            this.maxShieldTarget = shieldRenderPropertyComp.maxShield;
            this.currentShieldStart = shieldRenderPropertyComp.currentShield;
            this.currentShieldTarget = shieldRenderPropertyComp.currentShield;
            this.radiusStart = shieldRenderPropertyComp.radius;
            this.radiusTarget = shieldRenderPropertyComp.radius;
            this.thicknessStart = shieldRenderPropertyComp.thickness;
            this.thicknessTarget = shieldRenderPropertyComp.thickness;
            this.gapDegStart = shieldRenderPropertyComp.gapDeg;
            this.gapDegTarget = shieldRenderPropertyComp.gapDeg;
            this.edgeSoftnessStart = shieldRenderPropertyComp.edgeSoftness;
            this.edgeSoftnessTarget = shieldRenderPropertyComp.edgeSoftness;
            this.glowIntensityStart = shieldRenderPropertyComp.glowIntensity;
            this.glowIntensityTarget = shieldRenderPropertyComp.glowIntensity;
            this.pulseAmpStart = shieldRenderPropertyComp.pulseAmp;
            this.pulseAmpTarget = shieldRenderPropertyComp.pulseAmp;
            this.pulseFreqStart = shieldRenderPropertyComp.pulseFreq;
            this.pulseFreqTarget = shieldRenderPropertyComp.pulseFreq;
            this.damageProgressStart = shieldRenderPropertyComp.damageProgress;
            this.damageProgressTarget = shieldRenderPropertyComp.damageProgress;
            this.damageFlashStart = shieldRenderPropertyComp.damageFlash;
            this.damageFlashTarget = shieldRenderPropertyComp.damageFlash;
            this.activeColorStart = shieldRenderPropertyComp.activeColor;
            this.activeColorTarget = shieldRenderPropertyComp.activeColor;
            this.glowColorStart = shieldRenderPropertyComp.glowColor;
            this.glowColorTarget = shieldRenderPropertyComp.glowColor;

            this.durationTimer = new Timer(1f);
            this.flashTime = XIVMathf.Min(1.12f, durationTimer.Duration * 1.4f);
        }
    }
    
    public struct ShieldRenderPropertyComp : IComponent
    {
        public float maxShield;
        public float currentShield;
        public float radius;
        public float thickness;
        public float gapDeg;
        public float edgeSoftness;
        public float glowIntensity;
        public float pulseAmp;
        public float pulseFreq;
        public float pulseSeed;
        public float damageProgress;
        public float damageFlash;
        public Color activeColor;
        public Color glowColor;

        // [Header("Shield values")]
        // public int totalShield = 7;
        // [Tooltip("Can be fractional (e.g. 3.5) for partial segment fill")]
        // public float currentShield = 7f;
        //
        // [Header("Visuals")]
        // public float radius = 0.65f;
        // public float thickness = 0.08f;
        // public float gapDeg = 6f;
        // public float edgeSoftness = 0.008f;
        // public float glowIntensity = 1.5f;
        // public float pulseAmp = 0.014f;
        // public float pulseFreq = 2f;
        // [Range(0,1)] public float damageFlash = 0.8f;
    }

    public class ShieldRenderSystem : XIV.Ecs.System
    {
        readonly Filter<PositionComp, ShieldComp> shieldFilter = null;

        readonly Filter<ShieldRendererComp, ShieldRenderPropertyComp> shieldRendererPropertyFilter = null; // Update only if has UpdateShieldRendererTag
        readonly Filter<ShieldRendererComp, ShieldRenderPropertyComp, ShieldRenderAnimationComp> shieldRenderAnimationFilter = null;
        
        readonly AssetReferences assetReferences = null;

        public override void Update()
        {
            shieldFilter.ForEach(AddShieldRenderer);
            shieldRendererPropertyFilter.ForEach(DestroyRenderersWithNoShield);
            
            shieldRendererPropertyFilter.ForEach(DetectChanges);
            shieldRenderAnimationFilter.ForEach(AnimateShieldRenderer);
            shieldRendererPropertyFilter.ForEach(UpdateShaderProperties);
        }

        void AddShieldRenderer(Entity entity, ref PositionComp positionComp, ref ShieldComp shieldComp)
        {
            if (shieldComp.shieldRendererEntity.IsAlive()) return;

            var shieldRendererEntity = GameObjectEntity.CreateEntity(world, assetReferences.nodeShieldPrefab, positionComp.position.ToVector3(), Quaternion.identity);
            var renderer = shieldRendererEntity.GetUnityComponent<Renderer>();
            var materialPropertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(materialPropertyBlock);
            var rendererComp = new ShieldRendererComp
            {
                targetEntity = entity,
                renderer = renderer,
                materialPropertyBlock = materialPropertyBlock,
            };
            var propComp = new ShieldRenderPropertyComp
            {
                maxShield = shieldComp.max,
                currentShield = shieldComp.current,
                radius = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.Radius_FloatID),
                thickness = shieldComp.max / 87.5f, //max = 7, 0.08f,
                gapDeg = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.GapDeg_FloatID),
                edgeSoftness = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.EdgeSoftness_FloatID),
                glowIntensity = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.GlowIntensity_FloatID),
                pulseAmp = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseAmp_FloatID),
                pulseFreq = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseFreq_FloatID),
                pulseSeed = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseSeed_FloatID),
                damageProgress = 0f,
                damageFlash = renderer.sharedMaterial.GetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.DamageFlash_FloatID),
                activeColor = renderer.sharedMaterial.GetColor(ShaderConstants.Custom_ShieldCircleAdvanced.ColorActive_ColorID),
                glowColor = renderer.sharedMaterial.GetColor(ShaderConstants.Custom_ShieldCircleAdvanced.GlowColor_ColorID),
            };

            shieldRendererEntity.AddComponent(rendererComp);
            shieldRendererEntity.AddComponent(propComp);
            shieldComp.shieldRendererEntity = shieldRendererEntity;
        }

        void DestroyRenderersWithNoShield(Entity e, ref ShieldRendererComp shieldRendererComp, ref ShieldRenderPropertyComp shieldRenderPropertyComp)
        {
            if (shieldRendererComp.targetEntity.HasComponent<ShieldComp>()) return;
            e.Destroy();
        }

        void DetectChanges(Entity rendererEntity, ref ShieldRendererComp shieldRendererComp, ref ShieldRenderPropertyComp shieldRenderPropertyComp)
        {
            if (shieldRendererComp.targetEntity.HasComponent<ShieldComp>() == false) return;
            ref var shieldComp = ref shieldRendererComp.targetEntity.GetComponent<ShieldComp>();
                
            if (rendererEntity.HasComponent<ShieldRenderAnimationComp>() == false)
            {
                var propertyChangeComp = new ShieldRenderAnimationComp();
                propertyChangeComp.Init(ref shieldRenderPropertyComp);
                if (WriteChanges(ref shieldRendererComp, ref shieldRenderPropertyComp, ref propertyChangeComp, ref shieldComp))
                {
                    rendererEntity.AddComponent(propertyChangeComp);
                }
            }
            else
            {
                ref var shieldRendererAnimationComp = ref rendererEntity.GetComponent<ShieldRenderAnimationComp>();
                WriteChanges(ref shieldRendererComp, ref shieldRenderPropertyComp, ref shieldRendererAnimationComp, ref shieldComp);
            }
        }

        static bool WriteChanges(ref ShieldRendererComp shieldRendererComp, ref ShieldRenderPropertyComp shieldRenderPropertyComp, ref ShieldRenderAnimationComp propertyChangeComp, ref ShieldComp shieldComp)
        {
            bool changed = false;
            if (XIVMathf.Abs(shieldRenderPropertyComp.maxShield - shieldComp.max) > XIVMathf.Epsilon)
            {
                changed = true;
                propertyChangeComp.maxShieldTarget = shieldComp.max;
            }

            if (XIVMathf.Abs(shieldRenderPropertyComp.currentShield - shieldComp.current) > XIVMathf.Epsilon)
            {
                changed = true;
                propertyChangeComp.currentShieldTarget = shieldComp.current;
            }

            var targetThickness = shieldComp.max / 75f;
            if (XIVMathf.Abs(shieldRenderPropertyComp.thickness - targetThickness) > XIVMathf.Epsilon)
            {
                changed = true;
                propertyChangeComp.thicknessTarget = targetThickness;
            }

            if (shieldRendererComp.targetEntity.HasComponent<OccupiedNodeComp>())
            {
                ref var occupiedNodeComp = ref shieldRendererComp.targetEntity.GetComponent<OccupiedNodeComp>();
                ref var unitComp = ref occupiedNodeComp.unitEntity.GetComponent<UnitComp>();
                var unitColor = UnitIdLookup.GetColor(unitComp.unitType).ToUnityColor();
                if (shieldRenderPropertyComp.activeColor != unitColor)
                {
                    changed = true;
                    propertyChangeComp.activeColorTarget = unitColor;
                }

                if (shieldRenderPropertyComp.glowColor != unitColor)
                {
                    propertyChangeComp.glowColorTarget = unitColor;
                }
            }

            return changed;
        }

        void AnimateShieldRenderer(Entity shieldEntity, ref ShieldRendererComp shieldRendererComp, ref ShieldRenderPropertyComp shieldRenderPropertyComp, ref ShieldRenderAnimationComp shieldRenderAnimationComp)
        {
            shieldRenderAnimationComp.durationTimer.Update(XTime.deltaTime);
            float normalizedTime = shieldRenderAnimationComp.durationTimer.NormalizedTime;

            var easedTime = EasingFunction.EaseOutCubic(normalizedTime);
            shieldRenderPropertyComp.currentShield = XIVMathf.Lerp(shieldRenderAnimationComp.currentShieldStart, shieldRenderAnimationComp.currentShieldTarget, easedTime);
            shieldRenderPropertyComp.maxShield = XIVMathf.Lerp(shieldRenderAnimationComp.maxShieldStart, shieldRenderAnimationComp.maxShieldTarget, easedTime);
            shieldRenderPropertyComp.radius = XIVMathf.Lerp(shieldRenderAnimationComp.radiusStart, shieldRenderAnimationComp.radiusTarget, easedTime);
            shieldRenderPropertyComp.thickness = XIVMathf.Lerp(shieldRenderAnimationComp.thicknessStart, shieldRenderAnimationComp.thicknessTarget, easedTime);
            shieldRenderPropertyComp.gapDeg = XIVMathf.Lerp(shieldRenderAnimationComp.gapDegStart, shieldRenderAnimationComp.gapDegTarget, easedTime);
            shieldRenderPropertyComp.edgeSoftness = XIVMathf.Lerp(shieldRenderAnimationComp.edgeSoftnessStart, shieldRenderAnimationComp.edgeSoftnessTarget, easedTime);
            shieldRenderPropertyComp.glowIntensity = XIVMathf.Lerp(shieldRenderAnimationComp.glowIntensityStart, shieldRenderAnimationComp.glowIntensityTarget, easedTime);
            shieldRenderPropertyComp.pulseAmp = XIVMathf.Lerp(shieldRenderAnimationComp.pulseAmpStart, shieldRenderAnimationComp.pulseAmpTarget, easedTime);
            shieldRenderPropertyComp.pulseFreq = XIVMathf.Lerp(shieldRenderAnimationComp.pulseFreqStart, shieldRenderAnimationComp.pulseFreqTarget, easedTime);
            // damage flash progress (0->1->0)
            shieldRenderPropertyComp.damageProgress = XIVMathf.Clamp01(XIVMathf.Sin(XIVMathf.PI * XIVMathf.Clamp01(normalizedTime / shieldRenderAnimationComp.flashTime)));
            shieldRenderPropertyComp.damageFlash  = XIVMathf.Lerp(shieldRenderAnimationComp.damageFlashStart, shieldRenderAnimationComp.damageFlashTarget, easedTime);
            shieldRenderPropertyComp.activeColor = Color.Lerp(shieldRenderAnimationComp.activeColorStart, shieldRenderAnimationComp.activeColorTarget, easedTime);
            shieldRenderPropertyComp.glowColor = Color.Lerp(shieldRenderAnimationComp.glowColorStart, shieldRenderAnimationComp.glowColorTarget, easedTime);

            if (shieldRenderAnimationComp.durationTimer.IsDone)
            {
                // finalize
                shieldRenderPropertyComp.maxShield = shieldRenderAnimationComp.maxShieldTarget;
                shieldRenderPropertyComp.currentShield = shieldRenderAnimationComp.currentShieldTarget;
                shieldRenderPropertyComp.damageProgress = 0f;
                shieldEntity.RemoveComponent<ShieldRenderAnimationComp>();
            }
        }
        
        static void UpdateShaderProperties(ref ShieldRendererComp shieldRendererComp, ref ShieldRenderPropertyComp shieldRenderPropertyComp)
        {
            shieldRendererComp.renderer.GetPropertyBlock(shieldRendererComp.materialPropertyBlock);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.TotalShield_FloatID, shieldRenderPropertyComp.maxShield);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.CurrentShield_FloatID, shieldRenderPropertyComp.currentShield);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.Radius_FloatID, shieldRenderPropertyComp.radius);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.Thickness_FloatID, shieldRenderPropertyComp.thickness);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.GapDeg_FloatID, shieldRenderPropertyComp.gapDeg);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.EdgeSoftness_FloatID, shieldRenderPropertyComp.edgeSoftness);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.GlowIntensity_FloatID, shieldRenderPropertyComp.glowIntensity);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseAmp_FloatID, shieldRenderPropertyComp.pulseAmp);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseFreq_FloatID, shieldRenderPropertyComp.pulseFreq);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.PulseSeed_FloatID, shieldRenderPropertyComp.pulseSeed);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.DamageProgress_FloatID, shieldRenderPropertyComp.damageProgress);
            shieldRendererComp.materialPropertyBlock.SetFloat(ShaderConstants.Custom_ShieldCircleAdvanced.DamageFlash_FloatID, shieldRenderPropertyComp.damageFlash);
            shieldRendererComp.materialPropertyBlock.SetColor(ShaderConstants.Custom_ShieldCircleAdvanced.ColorActive_ColorID, shieldRenderPropertyComp.activeColor);
            shieldRendererComp.materialPropertyBlock.SetColor(ShaderConstants.Custom_ShieldCircleAdvanced.GlowColor_ColorID, shieldRenderPropertyComp.glowColor);
            shieldRendererComp.renderer.SetPropertyBlock(shieldRendererComp.materialPropertyBlock);
        }
    }
}