#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace XIV.Ecs
{
    [CustomEditor(typeof(EasyLevelController),editorForChildClasses:true)]
    public class EasyLevelControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
           
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                return;
            }
            var easyLevelController = target as EasyLevelController;
            if (easyLevelController == null)
            {
                return;
            }

            if (easyLevelController.manager == null)
            {
                return;
            }
            
            var redText = new GUIStyle
            {
                normal =
                {
                    textColor = Color.red
                }
            };

            var greenText = new GUIStyle
            {
                normal =
                {
                    textColor = Color.green
                }
            };

            foreach (var system in easyLevelController.manager.systems)
            {
                var systemName = system.GetType().Name;

                var durations = easyLevelController.manager.executionTimer.systemExeTimeDic[system];
                EditorGUILayout.LabelField(systemName,system.active ? greenText : redText);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.TextArea($"Awake \n {durations[(int)SystemExecutionTimer.MethodType.Awake]} ms");
                    GUILayout.TextArea($"Start \n {durations[(int)SystemExecutionTimer.MethodType.Start]} ms");
                    GUILayout.TextArea($"FixedUpdate \n {durations[(int)SystemExecutionTimer.MethodType.FixedUpdate]} ms");
                    GUILayout.TextArea($"PreUpdate \n {durations[(int)SystemExecutionTimer.MethodType.PreUpdate]} ms");
                    GUILayout.TextArea($"Update \n {durations[(int)SystemExecutionTimer.MethodType.Update]} ms");
                    GUILayout.TextArea($"LateUpdate \n {durations[(int)SystemExecutionTimer.MethodType.LateUpdate]} ms");
                }
                GUILayout.EndHorizontal();

                if (GUILayout.Button(system.active ? "Disable" : "Enable" ))
                {
                    system.active = !system.active;
                }
            }
            Repaint();
        }
    }
}

#endif
