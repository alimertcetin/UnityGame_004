using UnityEngine;
using XIV.Core.Collections;

namespace TheGame
{
    public class LineRendererPositionData
    {
        public readonly DynamicArray<int> connectionIndices = new DynamicArray<int>(); // index in ConnectionDB
        public readonly DynamicArray<Vector3> movementDirections = new DynamicArray<Vector3>();
        public readonly DynamicArray<Vector3> movementPositions = new DynamicArray<Vector3>();
    }
}