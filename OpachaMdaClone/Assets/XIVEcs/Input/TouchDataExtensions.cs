using UnityEngine;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public static class TouchDataExtensions
    {
        public static Ray GetRay(this TouchData touchData, Camera cam) => cam.ScreenPointToRay(touchData.position.ToVector2());
        public static Ray GetStartRay(this TouchData touchData, Camera cam) => cam.ScreenPointToRay(touchData.startPosition.ToVector2());
    }
}