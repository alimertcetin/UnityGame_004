using TheGame;
using TheGame.Extensions;
using UnityEngine;
using XIV.Core.DataStructures;
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    public struct NodeShieldRenderComp : IComponent
    {
        public LineRenderer lineRenderer;
    }
    
    public class ShieldRenderSystem : XIV.Ecs.System
    {
        readonly Filter<TransformComp, NodeComp> nodeFilter = null;
        readonly Filter<TransformComp, NodeComp, NodeShieldRenderComp> shieldFilter = null;
        readonly PrefabReferences prefabReferences = null;

        public override void Start()
        {
            nodeFilter.ForEach((Entity nodeEntity, ref TransformComp transformComp, ref NodeComp nodeComp) =>
            {
                var lineRenderer = new GameObject().AddComponent<LineRenderer>();
                lineRenderer.material = prefabReferences.shieldLineRendererMaterial;
                lineRenderer.XIVSetWidth(0.1f);
                nodeEntity.AddComponent(new NodeShieldRenderComp
                {
                    lineRenderer = lineRenderer
                });
            });
        }

        public override void Update()
        {
            shieldFilter.ForEach((ref TransformComp transformComp, ref NodeComp nodeComp, ref NodeShieldRenderComp nodeShieldRenderComp) =>
            {
                var detail = nodeComp.shieldPoints / nodeComp.totalShieldPoints;
                nodeShieldRenderComp.lineRenderer.positionCount = (int)(detail * 10f) + 3; // min 3 points
                DrawCircle(nodeShieldRenderComp.lineRenderer, transformComp.transform.position);
            });
        }

        static void DrawCircle(LineRenderer lineRenderer, Vec3 entityPosition)
        {
            var radius = 0.375f;
            int detail = lineRenderer.positionCount - 1;
            var rotation = Quaternion.FromToRotation(Vec3.forward, Vec3.forward);
            // var startPoint = (Vector3)entityPosition + rotation * Vec3.right * radius;
            // var p1 = startPoint;
            for (int i = 0; i <= detail; i++)
            {
                float angle = (i + 1) * (360f / detail);
                var p2 = (Vector3)entityPosition + rotation * Quaternion.AngleAxis(angle, Vec3.forward) * Vec3.right * radius;
                lineRenderer.SetPosition(i, p2);
            }
            lineRenderer.loop = detail > 2;
        }
    }
}