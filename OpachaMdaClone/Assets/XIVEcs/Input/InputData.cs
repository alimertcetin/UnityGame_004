using UnityEngine;

namespace XIV.Ecs
{
    public struct InputData
    {
        public bool isFingerDown;
        /// starts with a non ui fingerdown ends with finger up
        public bool isFingerDownNoUI;
        public bool isFingerDownThisFrame;
        public bool isFingerUpThisFrame;
        public bool isOnUI;

        public Vector3 inputScreenPos;
        public Vector3 inputScreenPosStart;
        public Vector2 deltaMovementInch;

        public float deltaMovementInchX => deltaMovementInch.x;
        public float deltaMovementInchY => deltaMovementInch.y;
        public Vector3 deltaMovementInchXZ => new Vector3(deltaMovementInch.x, 0, deltaMovementInch.y);
        public Vector3 deltaMovementInchXY => deltaMovementInch;
        public Vector2 totalDeltaMovementInInc;
        public float inputDuration;

        public bool isFingerDownThisFrameNoUI => isFingerDownThisFrame && !isOnUI;

        public float dpi;

        public Ray InputRay => Camera.main.ScreenPointToRay(inputScreenPos);
        public Ray InputStartRay => Camera.main.ScreenPointToRay(inputScreenPosStart);

        // public Vector3 InputRayAtY(float y)
        // {
        //     return VectorMath.RayToPointAtY(InputRay,y);
        // }
        //
        // public Vector3 InputRayAtZ(float z)
        // {
        //     return VectorMath.RayToPointAtZ(InputRay, z);
        // }
        //
        // public Vector3 InputRayAtX(float x)
        // {
        //     return VectorMath.RayToPointAtX(InputRay,x);
        // }
        //
        // public Vector3 InputRayAtPlane(Plane plane)
        // {
        //     var ray = InputRay;
        //     plane.Raycast(ray, out var enter);
        //     return ray.GetPoint(enter);
        // }
        //
        // public Vector3 InputStartRayAtY(float y)
        // {
        //     return VectorMath.RayToPointAtY(InputStartRay,y);
        // }
        //
        // public Vector3 InputStartRayAtZ(float z)
        // {
        //     return VectorMath.RayToPointAtZ(InputStartRay, z);
        // }
        //
        // public Vector3 InputStartRayAtX(float x)
        // {
        //     return VectorMath.RayToPointAtX(InputStartRay,x);
        // }
        //
        // public Vector3 InputStartRayAtPlane(Plane plane)
        // {
        //     var ray = InputStartRay;
        //     plane.Raycast(ray, out var enter);
        //     return ray.GetPoint(enter);
        // }

    }
}