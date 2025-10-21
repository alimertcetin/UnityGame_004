using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace XIV.Ecs
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameObjectEntity))]
    public class GameObjectEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ShowComponents();
        }

        void ShowComponents()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (targets.Length > 1)
            {
                if (GUILayout.Button("Destroy All"))
                {
                    foreach (var t in targets)
                    {
                        var goe = (t as GameObjectEntity);
                        if (goe == null)
                        {
                            continue;
                        }
                        goe.entity.Destroy();
                    }
                    return;
                }
            }
            
            foreach (var t in targets)
            {
                var goe = (t as GameObjectEntity);
                if (goe == null)
                {
                    continue;
                }
                EcsDebugUtils.DrawEntityInspector(goe.entity);
            }
            
            Repaint();
        }
    }
}

#endif
