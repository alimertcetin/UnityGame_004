using System.Collections.Generic;
using System.Text;

namespace XIV.Ecs
{
    public class Query
    {
        public ComponentMask componentMask = new ComponentMask();
        public List<Archetype> archetypes = new List<Archetype>();

        public int NumberOfEntities()
        {
            int nEntities = 0;
            foreach (var archetype in archetypes)
            {
                nEntities += archetype.entities.Count;
            }

            return nEntities;
        }

        public bool IsEmpty()
        {
            foreach (var archetype in archetypes)
            {
                if (archetype.entities.Count != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void IncludeComp<T>() where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            componentMask.IncludeComponent(componentId);
        }

        public void ExcludeComp<T>() where T : struct, IComponent
        {
            int componentId = ComponentIdManager.GetComponentId<T>();
            componentMask.ExcludeComponent(componentId);
        }

        public void IncludeTag<T>() where T : struct, ITag
        {
            int tagId = TagIdManager.GetTagId<T>();
            componentMask.IncludeTag(tagId);
        }

        public void ExcludeTag<T>() where T : struct, ITag
        {
            int tagId = TagIdManager.GetTagId<T>();
            componentMask.ExcludeTag(tagId);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var includedComps = ComponentIdManager.GetComponentNames(componentMask.includeComponentSet, sb);
            var excludedComps = ComponentIdManager.GetComponentNames(componentMask.excludeComponentSet, sb);

            var includedTags = TagIdManager.GetTagNames(componentMask.includeTagSet, sb);
            var excludedTags = TagIdManager.GetTagNames(componentMask.excludeTagSet, sb);

            return $"Comp({includedComps}) - Exclude({excludedComps}) - Tag({includedTags}) - Exclude({excludedTags})";
        }
    }
}