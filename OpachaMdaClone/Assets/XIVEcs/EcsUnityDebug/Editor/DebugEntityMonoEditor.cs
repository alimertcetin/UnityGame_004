
#if UNITY_EDITOR

using UnityEditor;

namespace XIV.Ecs
{
    [CustomEditor(typeof(DebugEntityMono))]
    public class DebugEntityMonoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DebugEntityMono entityMono = target as DebugEntityMono;
            var entity = entityMono.entity;
            EcsDebugUtils.DrawEntityInspector(entity);
            Repaint();
        }
    }
}

#endif
