namespace XIV.Ecs
{
    public class ParentSystem : XIV.Ecs.System
    {
        readonly Filter<ParentComp, TransformComp> parentFilter = new Filter<ParentComp, TransformComp>()
            .ExcludeTag<ParentNoRotationSyncComp>();
        
        readonly Filter<ParentTransformComp,TransformComp> parentTransformFilter = new Filter<ParentTransformComp, TransformComp>()
            .ExcludeTag<ParentNoRotationSyncComp>();
        
        readonly Filter<ParentComp,TransformComp> parentNoRotFilter = new Filter<ParentComp, TransformComp>()
            .Tag<ParentNoRotationSyncComp>();
        
        readonly Filter<ParentTransformComp,TransformComp> parentTransformNoRotFilter = new Filter<ParentTransformComp, TransformComp>()
            .Tag<ParentNoRotationSyncComp>();


        public override void Update()
        {
            parentFilter.ForEach((Entity entity, ref ParentComp parentComp, ref TransformComp transformComp) =>
            {
                if (!parentComp.parentEntity.IsAlive())
                {
                    entity.Destroy();
                    return;
                }
                
                var transform = transformComp.transform;
                var parentTransform = parentComp.parentEntity.GetComponent<TransformComp>().transform;

                transform.SetPositionAndRotation(
                    parentTransform.TransformPoint(parentComp.localPosition)
                    ,parentTransform.rotation * parentComp.localRotation);
            });
            
           
            parentTransformFilter.ForEach(
                (Entity entity, ref ParentTransformComp parentComp, ref TransformComp transformComp) =>
                {
                    if (parentComp.parentTransform == null)
                    {
                        entity.Destroy();
                        return;
                    }

                    var transform = transformComp.transform;
                    var parentTransform = parentComp.parentTransform;
                
                    transform.SetPositionAndRotation(parentTransform.TransformPoint(parentComp.localPosition),
                        parentTransform.rotation * parentComp.localRotation);
                });


            parentNoRotFilter.ForEach((
                Entity entity, ref ParentComp parentComp, ref TransformComp transformComp) =>
            {
                if (!parentComp.parentEntity.IsAlive())
                {
                    entity.Destroy();
                    return;
                }
                
                var transform = transformComp.transform;
                var parentTransform = parentComp.parentEntity.GetComponent<TransformComp>().transform;

                transform.position = parentTransform.TransformPoint(parentComp.localPosition);
            });
            
            
            parentTransformNoRotFilter.ForEach(
                (Entity entity, ref ParentTransformComp parentComp, ref TransformComp transformComp) =>
            {
                if (parentComp.parentTransform == null)
                {
                    entity.Destroy();
                    return;
                }

                var transform = transformComp.transform;
                var parentTransform = parentComp.parentTransform;
            
                transform.position = parentTransform.TransformPoint(parentComp.localPosition);
                
            });

            
            
        }
    }
}