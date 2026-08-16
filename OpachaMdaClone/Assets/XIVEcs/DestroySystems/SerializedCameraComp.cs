using System;
using XIV.UnityEngineIntegration;
using UnityEngine;

namespace XIV.Ecs
{
    [Serializable]
    public struct CameraComp : IComponent
    {
        public Camera camera;
    }
    
    public class SerializedCameraComp : SerializedComponent<CameraComp>
    {
        void OnValidate()
        {
            component.camera ??= GetComponentInChildren<Camera>();
        }
    }
}