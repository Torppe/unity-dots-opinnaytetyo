using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class UpdateTargetSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate() {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        var componentData = GetComponentDataFromEntity<LocalToWorld>(false);

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref HasTarget hasTarget) => {
                if(!componentData.Exists(hasTarget.target)) {
                    ecb.RemoveComponent<HasTarget>(entityInQueryIndex, entity);
                } else {
                    LocalToWorld ltw = componentData[hasTarget.target];
                    hasTarget.position = ltw.Position;
                }
            }).Schedule();
    }
}
