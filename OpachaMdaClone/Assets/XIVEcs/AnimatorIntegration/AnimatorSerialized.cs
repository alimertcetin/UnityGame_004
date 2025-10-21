using System.Collections.Generic;
using UnityEngine;

namespace XIV.Ecs
{
    public struct AnimatorComp : IComponent
    {
        public Animator animator;
        public AnimatorListenerMono animatorListener;
        public HashSet<string> animationEvents;

        public void Play(int stateName)
        {
            animator.Play(stateName,0);
        }

        public void Play(int stateName, float nTime, int layer = 0)
        {
            animator.Play(stateName,layer,nTime);
        }

        public void CrossFade(int stateName,float normalizedTransitionDuration, int layer = 0)
        {
            animator.CrossFade(stateName,normalizedTransitionDuration,layer);
        }

        public float NormalizedTime(int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        public void SetTrigger(int triggerId)
        {
            animator.SetTrigger(triggerId);
        }

        public void ListenEvent(string eventName, AnimatorListenerMono.EventListener eventListener)
        {
            animatorListener.eventListeners.Add(eventName,eventListener);
        }

        public void OnInspectorGUI()
        {
#if UNITY_EDITOR

            var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            // TODO I guess inherited controller returns null 
            if (ac == null) return;

            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField("Test animations in play mode");
            foreach (var layer in ac.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    string stateName = state.state.name;
                    if (GUILayout.Button("Play " + stateName) && Application.isPlaying)
                    {
                        animator.Play(stateName,0,0);
                    }
                }
            }
#endif
        }
    }
    
    public class AnimatorSerialized : SerializedComponent<AnimatorComp>
    {
        public Animator animator;
        
        public override object GetComponentData(World world)
        {
            if (animator == null)
            {
                Debug.LogError("Please attach an animator to , " + gameObject.name + "'s view");
                return component;
            }

            component.animator = animator;
            component.animatorListener = animator.gameObject.AddComponent<AnimatorListenerMono>();
            component.animationEvents = new HashSet<string>();
            return component;
        }

        void OnValidate()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
    }
}