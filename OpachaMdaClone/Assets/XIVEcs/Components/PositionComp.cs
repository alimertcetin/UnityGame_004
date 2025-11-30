using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public struct PositionComp : IComponent
    {
        public float posX;
        public float posY;
        public float posZ;
        public Vec3 position => new Vec3(posX, posY, posZ);

        public void Set(Vec3 pos)
        {
            posX = pos.x;
            posY = pos.y;
            posZ = pos.z;
        }
    }
}