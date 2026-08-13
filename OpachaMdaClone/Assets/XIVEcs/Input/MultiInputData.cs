using XIV.Core.Collections;
using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public struct TouchData
    {
        public int fingerId;
        public Vec2 position;
        public Vec2 startPosition;
        public Vec2 deltaPosition;
        public Vec2 deltaMovementInch;
        public InputTouchPhase phase;
        public bool isOnUI;
    }

    public struct MultiInputData
    {
        public const int MAX_TOUCHES = 10;

        public DynamicArray<TouchData> activeTouches;
        public DynamicArray<TouchData> prevActiveTouches;
        public int touchCount;

        // Global State Flags
        public bool hasActiveTouch;
        public bool isAnyTouchDownThisFrame;
        public bool isAnyTouchUpThisFrame;
        public bool isAllTouchNoUI;

        // --- Persistent Finger-Tracked Aggregates ---
        public Vec2 centroidPosition;        // Current center of active touches
        public Vec2 prevCentroidPosition;    // Center of matched touches in previous frame
        public Vec2 centroidDeltaPixels;     // Jump-free centroid delta
        public Vec2 centroidDeltaInch;

        public float averageSpreadPixels;    // Current average spread
        public float prevAverageSpreadPixels;// Prev average spread of matched touches
        public float spreadDeltaPixels;      // Jump-free spread delta (+ = zoom in, - = zoom out)
        public float spreadDeltaInch;

        public float dpi;

        public readonly bool TryGetTouch(int index, out TouchData touchData)
        {
            if (index >= 0 && index < touchCount)
            {
                touchData = activeTouches[index];
                return true;
            }

            touchData = default;
            return false;
        }

        public readonly bool TryGetPrevTouch(int fingerId, out TouchData prevTouch)
        {
            int count = prevActiveTouches.Count;
            for (int i = 0; i < count; i++)
            {
                if (prevActiveTouches[i].fingerId == fingerId)
                {
                    prevTouch = prevActiveTouches[i];
                    return true;
                }
            }
            prevTouch = default;
            return false;
        }
    }
}