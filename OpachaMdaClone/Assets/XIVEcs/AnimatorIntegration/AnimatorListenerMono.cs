using UnityEngine;
using System.Collections.Generic;

namespace XIV.Ecs
{
    public class AnimatorListenerMono : MonoBehaviour
    {
        public delegate void IKListener(int layerIdx);
        public delegate void EventListener();

        public readonly List<IKListener> ikListeners = new List<IKListener>(2);
        public readonly Dictionary<string, EventListener> eventListeners = new Dictionary<string, EventListener>();
        public readonly HashSet<string> animationEvents = new HashSet<string>();

        void OnAnimatorIK(int layerIndex)
        {
            foreach (var listener in ikListeners)
            {
                listener?.Invoke(layerIndex);
            }
        }

        // Call this function with animation event
        void HandleEvent(string eventName)
        {
            animationEvents.Add(eventName);
            if (eventListeners.TryGetValue(eventName,out var listener))
            {
                listener.Invoke();
            }
        }
    }
}