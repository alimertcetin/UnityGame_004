using XIV.Core.DataStructures;
using NotImplementedException = System.NotImplementedException;

namespace XIV.Ecs
{
    public struct ScaleComp : IComponent
    {
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public Vec3 scale => new Vec3(scaleX, scaleY, scaleZ);

        public void Set(Vec3 scale)
        {
            scaleX = scale.x;
            scaleY = scale.y;
            scaleZ = scale.z;
        }
    }
}