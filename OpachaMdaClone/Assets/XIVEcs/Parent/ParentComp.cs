using UnityEngine;

namespace XIV.Ecs
{
    public struct ParentComp : IComponent
    {
        public Entity parentEntity;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }

    public struct ParentTransformComp : IComponent
    {
        public Transform parentTransform;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }
    
    public struct ParentNoRotationSyncComp : ITag
    {
        
    }

}