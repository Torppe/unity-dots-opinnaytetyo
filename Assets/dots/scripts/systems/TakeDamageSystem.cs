using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(LifecycleSystem))]
public class TakeDamageSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate() {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<DeadTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Damage> damageBuffer, ref Health health) => {
                if (damageBuffer.Length == 0)
                    return;

                for(int i = 0; i < damageBuffer.Length; i++) {
                    health.Value -= damageBuffer[i].Value;
                    if(health.Value <= 0) {
                        ecb.AddComponent<DeadTag>(entityInQueryIndex, entity);
                        break;
                    }
                }

                damageBuffer.Clear();
            }).Schedule();

        Entities
            .WithAll<DeadTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Child> children) => {
                for(int i = 0; i < children.Length; i++) {
                    ecb.AddComponent<DeadTag>(entityInQueryIndex, children[i].Value);
                }
            }).Schedule();
    }
}

