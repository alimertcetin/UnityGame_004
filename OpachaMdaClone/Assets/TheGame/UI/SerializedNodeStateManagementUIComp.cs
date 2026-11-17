using UnityEngine;
using XIV.Core.DataStructures;
using XIV.Core.Utils;
using XIV.Core.XIVMath;
using XIV.Ecs;
using XIV.UnityEngineIntegration;
using XIVUnityEngineIntegration.Extensions;

namespace TheGame
{
    public struct NodeStateManagementUIComp : IComponent
    {
        public RectTransform container;
        public uint selectionVersion;
        public Vector2 visiblePosition;
        public Vector2 hiddenPosition;
    }

    public class SerializedNodeStateManagementUIComp : SerializedComponent<NodeStateManagementUIComp>
    {
        public RectTransform uiContainer;
        [DisplayWithoutEdit]
        public Canvas canvas;

        public override void AddComponentForEntity(Entity entity)
        {
            component.container = uiContainer;
            var parent = uiContainer.parent;
            while (canvas == false && parent)
            {
                if (parent.TryGetComponent(out canvas)) break;
                parent = parent.parent;
            }
            var canvasRect = canvas.transform as RectTransform;
            
            using var corners = ArrayUtils.GetBuffer<Vector3>(4);
            // corners[0] = sol alt, corners[1] = sol üst, corners[2] = sağ üst, corners[3] = sağ alt
            // En üst kenar corners[1].y ya da corners[2].y.
            uiContainer.GetWorldCorners(corners);
            float topY = XIVMathf.Max(corners[1].y, corners[2].y); 
            
            canvasRect.GetWorldCorners(corners);
            float screenBottomY = XIVMathf.Min(corners[0].y, corners[3].y);
            float yOffset = topY - screenBottomY;
            
            var visibleLocalPos = uiContainer.anchoredPosition;
            var hiddenLocalPos = new Vector2(visibleLocalPos.x, visibleLocalPos.y - yOffset);
            component.visiblePosition = visibleLocalPos;
            component.hiddenPosition = hiddenLocalPos;
            
            base.AddComponentForEntity(entity);
        }
    }
}