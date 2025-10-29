using UnityEngine;

namespace XIV.Ecs
{
    public static class InputDataExtensions
    {
        public static Ray GetInputStartRay(this in InputData inputData, Camera cam)
        {
            return cam.ScreenPointToRay(inputData.inputScreenPosStart);
        }
        
        public static Ray GetInputRay(this in InputData inputData, Camera cam)
        {
            return cam.ScreenPointToRay(inputData.inputScreenPos);
        }
        
        public static Vector3 InputStartRayAtPlane(this in InputData inputData, Plane plane)
        {
            var ray = GetInputStartRay(in inputData, Camera.main);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
        
        public static Vector3 InputRayAtPlane(this in InputData inputData, Plane plane)
        {
            var ray = GetInputRay(inputData, Camera.main);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
    }
}