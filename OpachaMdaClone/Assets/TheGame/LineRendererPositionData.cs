using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.DataStructures;

namespace TheGame
{
    public class LineRendererPositionData
    {
        public readonly DynamicArray<int> connectionIndices = new DynamicArray<int>(); // index in ConnectionDB
        public readonly DynamicArray<Vec3> movementDirections = new DynamicArray<Vec3>();
        public readonly DynamicArray<Vec3> movementPositions = new DynamicArray<Vec3>();
    }
}