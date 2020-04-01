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
        var children = GetBufferFromEntity<Child>(false);

        Entities
            .WithAll<DeadTag>()
            .ForEach((Entity entity, int entityInQueryIndex) => {
                if (children.Exists(entity)) {
                    var child = children[entity];

                    for (int i = 0; i < child.Length; i++) {
                        ecb.DestroyEntity(entityInQueryIndex, child[i].Value);
                    }
                }

                ecb.DestroyEntity(entityInQueryIndex, entity);
            })
            .Schedule();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
