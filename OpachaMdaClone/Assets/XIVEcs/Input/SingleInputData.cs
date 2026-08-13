using UnityEngine;
using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public struct SingleInputData
    {
        public bool isFingerDown;
        /// starts with a non ui finger down ends with finger up
        public bool isFingerDownNoUI;
        public bool isFingerDownThisFrame;
        public bool isFingerUpThisFrame;
        public bool isOnUI;

        public Vec3 inputScreenPos;
        public Vec3 inputScreenPosStart;
        public Vec2 deltaMovementInch;

        public float deltaMovementInchX => deltaMovementInch.x;
        public float deltaMovementInchY => deltaMovementInch.y;
        public Vec3 deltaMovementInchXZ => new Vec3(deltaMovementInch.x, 0, deltaMovementInch.y);
        public Vec3 deltaMovementInchXY => deltaMovementInch;
        public Vec2 totalDeltaMovementInInc;
        public float inputDuration;

        public bool isFingerDownThisFrameNoUI => isFingerDownThisFrame && !isOnUI;

        public float dpi;
    }

}