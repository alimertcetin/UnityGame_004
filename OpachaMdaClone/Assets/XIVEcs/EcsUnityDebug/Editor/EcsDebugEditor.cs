#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XIV.Ecs;

[CustomEditor(typeof(EcsDebug))]
public class EcsDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
        {
            var debug = target as EcsDebug;
            debug.RefreshWorld();
        }
    }
}
#endif
