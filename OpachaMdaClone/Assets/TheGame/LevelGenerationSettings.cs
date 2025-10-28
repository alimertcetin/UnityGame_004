using System;
using XIV.Core.DataStructures;
using XIV.Core.XIVMath;
using XIV.Ecs;

namespace TheGame
{
    public struct LevelGenerationSettings
    {
        public const float TIGHTNESS = 0f;
        public const float NODE_RADIUS = 0.5f;
        public const float DISTANCE_MULTIPLIER = 4;
        public const float TARGET_DISTANCE_BETWEEN_NODES = DISTANCE_MULTIPLIER * NODE_RADIUS;
        public const float LINK_DISTANCE = (DISTANCE_MULTIPLIER * 2) * NODE_RADIUS;
        public const float SAME_DIRECTION_CUT_THRESHOLD = 0.8f;
        
        public static readonly Vec2 RegionSize = new Vec2(30, 30);
        
        public int seed;
        public Vec2 regionSize;
        public MapSize mapSize;
        public float tightness;
        public float nodeRadius;
        public float distanceMultiplier;
        public float targetDistanceBetweenNodes;
        public float linkDistance;
        public float sameDirectionCutThreshold;

        public LevelGenerationSettings(Vec2 regionSize, MapSize mapSize, int seed, float nodeRadius = NODE_RADIUS)
        {
            this.seed = seed;
            this.regionSize = regionSize;
            this.tightness = TIGHTNESS;
            this.mapSize = mapSize;
            this.nodeRadius = nodeRadius;
            this.distanceMultiplier = DISTANCE_MULTIPLIER;
            this.targetDistanceBetweenNodes = distanceMultiplier * nodeRadius;
            this.linkDistance = (distanceMultiplier * 2) * nodeRadius;
            this.sameDirectionCutThreshold = SAME_DIRECTION_CUT_THRESHOLD;
        }

        public LevelGenerationSettings(Vec2 regionSize, MapSize mapSize, float nodeRadius) : this(regionSize, mapSize, Environment.TickCount, nodeRadius)
        {
        }

        public LevelGenerationSettings(Vec2 regionSize, MapSize mapSize, int seed, float tightness, float nodeRadius) : this(regionSize, mapSize, seed, nodeRadius)
        {
            this.tightness = tightness;
            this.regionSize *= tightness;
        }

        public LevelGenerationSettings(MapSize mapSize, int seed, float tightness, float nodeRadius) : this(RegionSize, mapSize, seed, tightness, nodeRadius)
        {
        }

        public LevelGenerationSettings(MapSize mapSize, int seed, float tightness, float sameDirectionCutThreshold, float nodeRadius) : this(RegionSize, mapSize, seed, tightness, nodeRadius)
        {
            this.sameDirectionCutThreshold = sameDirectionCutThreshold;
        }

        public LevelGenerationSettings(MapSize mapSize, float tightness, float nodeRadius) : this(RegionSize, mapSize, Environment.TickCount, tightness, nodeRadius)
        {
        }

        public LevelGenerationSettings(MapSize mapSize, float tightness) : this(RegionSize, mapSize, Environment.TickCount, tightness, NODE_RADIUS)
        {
        }

        public LevelGenerationSettings(MapSize mapSize) : this(RegionSize, mapSize, Environment.TickCount, NODE_RADIUS)
        {
        }

        public static LevelGenerationSettings CreateRandom()
        {
            var mapSize = (MapSize)XIVRandom.Range(0, (int)MapSize.NumberOfItems);
            var tightness = XIVRandom.value;
            return new LevelGenerationSettings(mapSize, tightness);
        }
        
        public int GetNodeQuantity()
        {
            switch (mapSize)
            {
                case MapSize.Small: return 7;
                case MapSize.Medium: return 12;
                case MapSize.Big: return 17;
                case MapSize.Large: return 25;
                case MapSize.Giant: return 61;
                default: return 7;
            }
        }
    }
}