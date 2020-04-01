using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

//public class AreaDamageSystem : ComponentSystem {
//    protected override void OnUpdate() {
//        Entities.WithAll<AreaDamage>().ForEach((Entity explosionEntity, ref AreaDamage areaDamage) => {
//            float3 position = areaDamage.position;
//            float range = areaDamage.range;

//            Entities.WithAll<EnemyTag>().ForEach((Entity enemyEntity, ref Translation translation) => {
//                if (math.distance(position, translation.Value) <= range) {
//                    PostUpdateCommands.DestroyEntity(enemyEntity);
//                }
//            });

//            PostUpdateCommands.DestroyEntity(explosionEntity);
//        });
//    }
//}

public class AreaDamageSystem : SystemBase {
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private EntityQuery query;

    protected override void OnCreate() {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        query = GetEntityQuery(typeof(AreaDamage));
    }

    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        NativeArray<AreaDamage> asd = query.ToComponentDataArray<AreaDamage>(Allocator.TempJob);

        Entities
            .WithAll<EnemyTag>()
            .WithAll<Health>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Damage> buffer, in Translation translation) => {
                for (int i = 0; i < asd.Length; i++) {
                    if (math.distance(asd[i].position, translation.Value) <= asd[i].range) {
                        buffer.Add(new Damage { Value = asd[i].damage });
                    }
                }
            })
            .WithDeallocateOnJobCompletion(asd)
            .WithStoreEntityQueryInField(ref query)
            .ScheduleParallel();

        Entities
            .WithAll<AreaDamage>()
            .ForEach((Entity entity, int entityInQueryIndex) => {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            })
            .Schedule();
    }
}