using System.Collections;
using UnityEngine;
using XIV.Core.XIVMath;
using XIV.UnityEngineIntegration;

namespace TheGame
{
    [RequireComponent(typeof(MeshRenderer))]
    public class TempShield : MonoBehaviour
    {
        public MeshRenderer rend;
        MaterialPropertyBlock mpb;

        [Header("Shield values")]
        public int totalShield = 12;
        [Tooltip("Can be fractional (e.g. 3.5) for partial segment fill")]
        public float currentShield = 12f;

        [Header("Visuals")]
        public float radius = 0.45f;
        public float thickness = 0.08f;
        public float gapDeg = 6f;
        public float edgeSoftness = 0.008f;
        public float glowIntensity = 1.5f;
        public float pulseAmp = 0.06f;
        public float pulseFreq = 1.6f;
        [Range(0,1)] public float damageFlash = 0.8f;

        // internal
        float pulseSeed;
        Coroutine damageCoroutine;

        void Awake()
        {
            rend = GetComponent<MeshRenderer>();
            mpb = new MaterialPropertyBlock();
            pulseSeed = Random.Range(0f, 6.28f); // randomize phase
            WriteAllToMPB();
        }

        void OnEnable()
        {
            // ensure initial values
            WriteAllToMPB();
        }

        void WriteAllToMPB()
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_TotalShield", (float)totalShield);
            mpb.SetFloat("_CurrentShield", currentShield);
            mpb.SetFloat("_Radius", radius);
            mpb.SetFloat("_Thickness", thickness);
            mpb.SetFloat("_GapDeg", gapDeg);
            mpb.SetFloat("_EdgeSoftness", edgeSoftness);
            mpb.SetFloat("_GlowIntensity", glowIntensity);
            mpb.SetFloat("_PulseAmp", pulseAmp);
            mpb.SetFloat("_PulseFreq", pulseFreq);
            mpb.SetFloat("_PulseSeed", pulseSeed);
            mpb.SetFloat("_DamageProgress", 0f);
            mpb.SetFloat("_DamageFlash", damageFlash);
            rend.SetPropertyBlock(mpb);
        }

        // Call this every frame if currentShield or totalShield may change frequently
        public void UpdateShieldValues(int total, float current)
        {
            totalShield = total;
            currentShield = current;
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_TotalShield", (float)totalShield);
            mpb.SetFloat("_CurrentShield", currentShield);
            rend.SetPropertyBlock(mpb);
        }

        public bool useNewShieldVal;
        public float newShieldValue;
        public float speed = 3f;

        [Button(true)]
        public void PlayDamage()
        {
            var newVal = useNewShieldVal ? newShieldValue : XIVRandom.value * totalShield;
            var diff = XIVMathf.Abs(currentShield - newVal);
            var time = diff / speed;
            var duration = useNewShieldVal ? time : 0.35f;
            PlayDamage(newVal, duration);
        }

        // Damage animation: animate currentShield down to newValue over time and flash
        public void PlayDamage(float newCurrent, float duration = 0.35f)
        {
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamageRoutine(newCurrent, duration));
        }

        IEnumerator DamageRoutine(float newCurrent, float duration)
        {
            float start = currentShield;
            float target = newCurrent;
            float t = 0f;

            // small flash in shader via _DamageProgress (0->1->0)
            float flashTime = Mathf.Min(0.12f, duration * 0.4f);
            // animate _DamageProgress separately for sharper flash
            float dmgT = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float s = Mathf.Clamp01(t / duration);
                currentShield = Mathf.Lerp(start, target, EasingOutCubic(s)); // ease out for nicer feel

                // damage flash progress (0->1->0)
                dmgT += Time.deltaTime;
                float dmgProgress = Mathf.Clamp01(Mathf.Sin(Mathf.PI * Mathf.Clamp01(dmgT / flashTime)));

                rend.GetPropertyBlock(mpb);
                mpb.SetFloat("_CurrentShield", currentShield);
                mpb.SetFloat("_DamageProgress", dmgProgress);
                rend.SetPropertyBlock(mpb);

                yield return null;
            }

            // finalize
            currentShield = target;
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat("_CurrentShield", currentShield);
            mpb.SetFloat("_DamageProgress", 0f);
            rend.SetPropertyBlock(mpb);

            damageCoroutine = null;
        }

        static float EasingOutCubic(float x)
        {
            x = Mathf.Clamp01(x);
            return 1f - Mathf.Pow(1f - x, 3f);
        }
    }
}
