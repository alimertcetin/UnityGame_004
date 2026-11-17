using System;
using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public readonly struct Int2
    {
        public readonly int x;
        public readonly int y;

        public Int2(int x, int y) => (this.x, this.y) = (x, y);

        public static Int2 zero => new Int2(0, 0);
        public static Int2 left => new Int2(-1, 0);
        public static Int2 right => new Int2(1, 0);
        public static Int2 up => new Int2(0, 1);
        public static Int2 down => new Int2(0, -1);
        public static Int2 upLeft => new Int2(-1, 1);
        public static Int2 upRight => new Int2(1, 1);
        public static Int2 downLeft => new Int2(-1, -1);
        public static Int2 downRight => new Int2(1, -1);
    }

    public readonly struct SwipeResult
    {
        public readonly Direction direction;
        public readonly Vec3 directionVector;

        public SwipeResult(Direction direction, Vec3 directionVector)
        {
            this.direction = direction;
            this.directionVector = directionVector;
        }

        public static SwipeResult None => new SwipeResult(Direction.None, Vec3.zero);

        public Int2 DirInt2 => direction switch
        {
            Direction.Left => Int2.left,
            Direction.Right => Int2.right,
            Direction.Up => Int2.up,
            Direction.Down => Int2.down,
            Direction.UpLeft => Int2.upLeft,
            Direction.UpRight => Int2.upRight,
            Direction.DownLeft => Int2.downLeft,
            Direction.DownRight => Int2.downRight,
            _ => Int2.zero
        };

        public Vec2 DirVec2 => new Vec2(DirInt2.x, DirInt2.y);
        public Vec3 DirXZ => new Vec3(DirInt2.x, 0, DirInt2.y);
        public Vec3 DirXY => new Vec3(DirInt2.x, DirInt2.y, 0);

        public enum Direction
        {
            None = 0,
            Left,
            Right,
            Up,
            Down,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }
    }

    public enum SwipeMode
    {
        FourWay,
        EightWay
    }

    public struct SwipeDetector
    {
        float thresholdInInch;
        bool onlyWhenFingerUp;
        float timeThreshold;
        SwipeMode swipeMode;

        bool inputStarted;
        Vec3 inputStartScreenPos;
        float timer;

        public static SwipeDetector New(
            bool onlyWhenFingerUp = true, 
            float thresholdInInch = 0.16f, 
            float timeThreshold = -1f,
            SwipeMode swipeMode = SwipeMode.EightWay)
        {
            return new SwipeDetector
            {
                thresholdInInch = thresholdInInch,
                onlyWhenFingerUp = onlyWhenFingerUp,
                timeThreshold = timeThreshold,
                swipeMode = swipeMode,
                inputStarted = false,
                inputStartScreenPos = Vec3.zero,
                timer = 0f,
            };
        }

        public SwipeResult DetectSwipe(ref SingleInputData singleInputData, float deltaTime)
        {
            if (HandleTimeOut(deltaTime))
            {
                return SwipeResult.None;
            }

            if (singleInputData.isFingerDownThisFrameNoUI)
            {
                StartInput(singleInputData.inputScreenPos);
            }

            if (!inputStarted)
            {
                return SwipeResult.None;
            }

            float safeDpi = singleInputData.dpi <= 0 ? 96f : singleInputData.dpi;

            if (onlyWhenFingerUp)
            {
                // Only evaluate when the finger is lifted
                if (singleInputData.isFingerUpThisFrame)
                {
                    inputStarted = false;
                    var delta = (singleInputData.inputScreenPos - inputStartScreenPos) / safeDpi;

                    if (EvaluateThreshold(delta, out var direction))
                    {
                        var screenPosDelta = singleInputData.inputScreenPos - inputStartScreenPos;
                        return new SwipeResult(direction, screenPosDelta.normalized);
                    }
                }
            }
            else
            {
                // Continuous check every frame while dragging
                var delta = (singleInputData.inputScreenPos - inputStartScreenPos) / safeDpi;

                if (EvaluateThreshold(delta, out var direction))
                {
                    // Reset start position for continuous chain-swiping
                    var screenPosDelta = singleInputData.inputScreenPos - inputStartScreenPos;
                    inputStartScreenPos = singleInputData.inputScreenPos;
                    timer = 0f;
                    return new SwipeResult(direction, screenPosDelta.normalized);
                }
                
                if (singleInputData.isFingerUpThisFrame)
                {
                    inputStarted = false;
                }
            }

            return SwipeResult.None;
        }

        void StartInput(Vec3 screenPos)
        {
            inputStarted = true;
            inputStartScreenPos = screenPos;
            timer = 0f;
        }

        bool HandleTimeOut(float deltaTime)
        {
            if (!inputStarted) return false;

            timer += deltaTime;
            if (timeThreshold > 0 && timer > timeThreshold)
            {
                inputStarted = false;
                return true;
            }
            return false;
        }

        bool EvaluateThreshold(Vec3 delta, out SwipeResult.Direction direction)
        {
            direction = SwipeResult.Direction.None;
            float horizontalDelta = Math.Abs(delta.x);
            float verticalDelta = Math.Abs(delta.y);

            if (horizontalDelta < thresholdInInch && verticalDelta < thresholdInInch)
            {
                return false;
            }

            if (swipeMode == SwipeMode.FourWay)
            {
                if (horizontalDelta > verticalDelta)
                {
                    direction = delta.x < 0 ? SwipeResult.Direction.Left : SwipeResult.Direction.Right;
                }
                else
                {
                    direction = delta.y < 0 ? SwipeResult.Direction.Down : SwipeResult.Direction.Up;
                }
            }
            else // EightWay optimization using ratios/signs instead of Atan2
            {
                bool isRight = delta.x > 0;
                bool isUp = delta.y > 0;
                float ratio = horizontalDelta > 0 ? verticalDelta / horizontalDelta : float.MaxValue;

                const float lowThreshold = 0.414f;   // ~22.5 degrees
                const float highThreshold = 2.414f;  // ~67.5 degrees

                if (ratio < lowThreshold)
                {
                    direction = isRight ? SwipeResult.Direction.Right : SwipeResult.Direction.Left;
                }
                else if (ratio > highThreshold)
                {
                    direction = isUp ? SwipeResult.Direction.Up : SwipeResult.Direction.Down;
                }
                else
                {
                    if (isRight && isUp) direction = SwipeResult.Direction.UpRight;
                    else if (!isRight && isUp) direction = SwipeResult.Direction.UpLeft;
                    else if (isRight && !isUp) direction = SwipeResult.Direction.DownRight;
                    else direction = SwipeResult.Direction.DownLeft;
                }
            }

            return true;
        }
    }
}