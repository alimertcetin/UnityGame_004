using XIV.Core.DataStructures;

namespace XIV.Ecs
{
    public struct RotationComp : IComponent
    {
        public float rotX;
        public float rotY;
        public float rotZ;

        public Vec3 eulerAngles => new Vec3(rotX, rotY, rotZ);

        public void Set(Vec3 eulerAngles)
        {
            rotX = eulerAngles.x;
            rotY = eulerAngles.y;
            rotZ = eulerAngles.z;
        }
    }
}