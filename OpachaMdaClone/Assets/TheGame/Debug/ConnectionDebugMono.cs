using System;
using UnityEditor;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.XIVMath;
using XIV.UnityEngineIntegration;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public class ConnectionDebugMono : MonoBehaviour
    {
        public GameObject selected1;
        public GameObject selected2;
        public int mode;
        public Vector3 dragStartPos;

        const int DEFAULT = 0;
        const int ENABLED = 1;
        
        static readonly int[] modes = new int[]
        {
            DEFAULT, ENABLED
        };

        void Update()
        {
            var cam = Camera.main;
            if (Input.GetMouseButtonDown(1))
            {
                // Right click
                dragStartPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var targetPos = Input.mousePosition;
                var diff = targetPos - dragStartPos;
                
                var pos = cam.transform.position;
                pos -= (diff * Time.deltaTime);
                cam.transform.position = pos;
                dragStartPos = targetPos;
            }

            var size = cam.orthographicSize;
            size -= Input.mouseScrollDelta.y;
            cam.orthographicSize = XIVMathf.Max(size, 1f);
            
            if (GetKeyDown(KeyCode.Alpha1)) mode = (mode + 1) % modes.Length;
            if (mode != ENABLED) return;
            if (GetKeyDown(KeyCode.C)) ClearSelection();
            if (GetKeyDown(KeyCode.Mouse0)) HandleSelection();
        }

        void OnDrawGizmos()
        {
            if (this.enabled == false || this.gameObject.activeSelf == false) return;
            if (mode != ENABLED) return;
            
            if (selected1 && (selected2 == false))
            {
                var pos = GetCamera().ScreenToWorldPoint(Input.mousePosition);
                XIVDebug.DrawLine(selected1.transform.position, pos, XIVColor.cyan);
            }

            if (selected1 && selected2)
            {
                var p0 = selected1.transform.position;
                var p1 = selected2.transform.position;
                XIVDebug.DrawTextOnLine(p0.ToVec3(), p1.ToVec3(), Vector3.Distance(p0, p1).ToString(), 20);
                XIVDebug.DrawLine(p0.ToVec3(), p1.ToVec3(), XIVColor.cyan);
            }
            
        }

        void HandleSelection()
        {
            if (Physics.Raycast(GetCamera().ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (selected1 == false) selected1 = hit.transform.gameObject;
                else if (selected2 == false) selected2 = hit.transform.gameObject;
            }
        }

        void ClearSelection()
        {
            selected1 = default;
            selected2 = default;
        }

        bool GetKeyDown(KeyCode code)
        {
            return Input.GetKeyDown(code);
            // var e = Event.current;
            // var b = e.keyCode == code && (e.type == EventType.KeyDown || e.type == EventType.MouseDown);
            // if (b)
            // {
            //     e.Use();
            //     Repaint();
            // }
            //
            // return b;
        }

        static T DrawObjectField<T>(T obj, string label) where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), true);
        }

        static Camera GetCamera()
        {
            return Camera.main;
        }
    }
}