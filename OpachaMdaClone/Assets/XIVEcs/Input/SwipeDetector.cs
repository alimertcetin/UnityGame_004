using UnityEngine;
using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public struct SwipeResult
    {
        public Direction direction;
        public Vec3 directionVector;
        
        public Vector2Int DirVec2i
        {
            get
            {
                return direction switch
                {
                    Direction.None => Vector2Int.zero,
                    Direction.Left => Vector2Int.left,
                    Direction.Right => Vector2Int.right,
                    Direction.Up => Vector2Int.up,
                    Direction.Down => Vector2Int.down,
                    _ => Vector2Int.zero
                };
            }
        }

        public Vec2 DirVec2 => new Vec2(DirVec2i.x, DirVec2i.y);
        public Vec3 DirXZ => new Vec3(DirVec2i.x, 0, DirVec2i.y);
        public Vec3 DirXY => new Vec3(DirVec2i.x,  DirVec2i.y,0);

        public enum Direction
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
        }

    }

    public struct SwipeDetector
    {
        float thresholdInInch;
        bool onlyWhenFingerUp;
        float timeThreshold;

        bool inputStarted;
        Vec3 inputStartScreenPos;
        Vec3 swipeDirection;
        float timer;

        public static SwipeDetector New(bool onlyWhenFingerUp = true, float thresholdInInch = 0.16f, float timeThreshold = -1)
        {
            return new SwipeDetector
            {
                thresholdInInch = thresholdInInch,
                onlyWhenFingerUp = onlyWhenFingerUp,
                timeThreshold = timeThreshold,

                inputStarted = false,
                inputStartScreenPos = Vec3.zero,
                swipeDirection = Vec3.zero,
                timer = 0,
            };
        }

        // Call every frame, returns swipe direction when swipe occurs
        public SwipeResult DetectSwipe(ref SingleInputData singleInputData, float deltaTime)
        {
            var direction = onlyWhenFingerUp ? DetectSwipeOnlyWhenFingerUp(ref singleInputData, deltaTime) : DetectSwipeContinues(ref singleInputData, deltaTime);
            var startScreenPos = singleInputData.inputScreenPosStart;
            var endScreenPos = singleInputData.inputScreenPos;
            var screenPosDelta = endScreenPos - startScreenPos;
            var result = new SwipeResult
            {
                direction = direction,
                directionVector = screenPosDelta.normalized,
            };
            return result;
        }

        SwipeResult.Direction DetectSwipeContinues(ref SingleInputData singleInputData, float deltaTime)
        {
            if (singleInputData.isFingerDownThisFrameNoUI)
            {
                inputStarted = true;
                inputStartScreenPos = singleInputData.inputScreenPos;
            }

            if (!inputStarted)
            {
                timer = 0;
                return SwipeResult.Direction.None;
            }

            timer += deltaTime;

            if (timeThreshold > 0 && timer > timeThreshold)
            {
                return SwipeResult.Direction.None;
            }

            var delta = (singleInputData.inputScreenPos - inputStartScreenPos) / singleInputData.dpi;
            var horizontalDelta = Mathf.Abs(delta.x);
            var verticalDelta = Mathf.Abs(delta.y);

            if (horizontalDelta > thresholdInInch || verticalDelta > thresholdInInch)
            {
                inputStarted = false;
                if (horizontalDelta > verticalDelta)
                {
                    return delta.x < 0 ? SwipeResult.Direction.Left : SwipeResult.Direction.Right;
                }

                return delta.y < 0 ? SwipeResult.Direction.Down : SwipeResult.Direction.Up;
            }

            return SwipeResult.Direction.None;
        }

        SwipeResult.Direction DetectSwipeOnlyWhenFingerUp(ref SingleInputData singleInputData, float deltaTime)
        {
            if (singleInputData.isFingerDownThisFrameNoUI)
            {
                inputStarted = true;
                inputStartScreenPos = singleInputData.inputScreenPos;
                timer = 0;
            }

            if (!inputStarted)
            {
                return SwipeResult.Direction.None;
            }

            timer += deltaTime;

            if (timeThreshold > 0 && timer > timeThreshold)
            {
                return SwipeResult.Direction.None;
            }

            if (singleInputData.isFingerUpThisFrame)
            {
                inputStarted = false;
                timer = 0;
            }

            var delta = (singleInputData.inputScreenPos - inputStartScreenPos) / singleInputData.dpi;
            var horizontalDelta = Mathf.Abs(delta.x);
            var verticalDelta = Mathf.Abs(delta.y);

            if (horizontalDelta > thresholdInInch || verticalDelta > thresholdInInch)
            {
                inputStartScreenPos = singleInputData.inputScreenPos;

                if (horizontalDelta > verticalDelta)
                {
                    return delta.x < 0 ? SwipeResult.Direction.Left : SwipeResult.Direction.Right;
                }

                return delta.y < 0 ? SwipeResult.Direction.Down : SwipeResult.Direction.Up;
            }

            return SwipeResult.Direction.None;
        }
    }
}