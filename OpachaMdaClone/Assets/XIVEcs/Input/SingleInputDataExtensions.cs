using UnityEngine;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public static class SingleInputDataExtensions
    {
        public static Ray GetInputStartRay(this in SingleInputData singleInputData, Camera cam)
        {
            return cam.ScreenPointToRay(singleInputData.inputScreenPosStart.ToVector3());
        }
        
        public static Ray GetInputRay(this in SingleInputData singleInputData, Camera cam)
        {
            return cam.ScreenPointToRay(singleInputData.inputScreenPos.ToVector3());
        }
        
        public static Vector3 InputStartRayAtPlane(this in SingleInputData singleInputData, Plane plane)
        {
            var ray = GetInputStartRay(in singleInputData, Camera.main);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
        
        public static Vector3 InputRayAtPlane(this in SingleInputData singleInputData, Plane plane)
        {
            var ray = GetInputRay(singleInputData, Camera.main);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
    }

}