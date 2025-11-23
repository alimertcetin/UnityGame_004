using UnityEngine;

namespace XIV.Ecs
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component) return component;
            return gameObject.AddComponent<T>();
        }
    }
}