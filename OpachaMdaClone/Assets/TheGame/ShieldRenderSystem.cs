using TheGame;
using TheGame.Extensions;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.UnityEngineIntegration;
using XIVUnityEngineIntegration.Extensions;

namespace XIV.Ecs
{
    public struct NodeShieldRenderComp : IComponent
    {
        public LineRenderer lineRenderer;
    }
    
    public class ShieldRenderSystem : XIV.Ecs.System
    {
        readonly Filter<NodeComp> nodesWithoutShieldFilter = new Filter<NodeComp>().Exclude<NodeShieldRenderComp>();
        readonly Filter<PositionComp, NodeComp, NodeShieldRenderComp> shieldFilter = null;
        readonly AssetReferences assetReferences = null;

        public override void Start()
        {
            nodesWithoutShieldFilter.ForEach((Entity nodeEntity, ref NodeComp nodeComp) =>
            {
                var lineRenderer = GameObjectEntity.CreateEntity(world, assetReferences.shieldLineRendererPrefab).GetTransform().GetComponent<LineRenderer>();
                lineRenderer.XIVSetWidth(0.1f);
                nodeEntity.AddComponent(new NodeShieldRenderComp
                {
                    lineRenderer = lineRenderer
                });
            });
        }

        public override void Update()
        {
            shieldFilter.ForEach((Entity e, ref PositionComp positionComp, ref NodeComp nodeComp, ref NodeShieldRenderComp nodeShieldRenderComp) =>
            {
                if (nodeComp.shieldPoints < 2) return;
                var detail = nodeComp.shieldPoints / assetReferences.generationConfigs[nodeComp.configIdx].shieldPoints;
                nodeShieldRenderComp.lineRenderer.positionCount = (int)(detail * 10f);
                DrawCircle(nodeShieldRenderComp.lineRenderer, positionComp.position);
            });
        }

        static void DrawCircle(LineRenderer lineRenderer, Vec3 entityPosition)
        {
            var radius = lineRenderer.transform.localScale.y * 0.5f + lineRenderer.startWidth;
            int detail = lineRenderer.positionCount - 1;
            var rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.forward);
            // var startPoint = (Vector3)entityPosition + rotation * Vec3.right * radius;
            // var p1 = startPoint;
            var entityPosVector3 = entityPosition.ToVector3();
            for (int i = 0; i <= detail; i++)
            {
                float angle = (i + 1) * (360f / detail);
                var p2 = entityPosVector3 + rotation * Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right * radius;
                lineRenderer.SetPosition(i, p2);
            }
            lineRenderer.loop = detail > 2;
        }
    }
}