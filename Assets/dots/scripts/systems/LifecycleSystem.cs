using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class LifecycleSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate() {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning() {
        base.OnStartRunning();

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Health health) => {
            health.Value = health.max;
            ecb.AddBuffer<Damage>(entityInQueryIndex, entity);
        }).ScheduleParallel();
    }
    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithAll<DeadTag>()
            .ForEach((Entity entity, int entityInQueryIndex) => {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            })
            .ScheduleParallel();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
