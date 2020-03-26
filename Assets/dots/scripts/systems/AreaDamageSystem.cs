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
        query = GetEntityQuery(typeof(EnemyTag), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        NativeArray<Translation> translationArray = query.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.TempJob);

        Entities.ForEach((Entity entity, int entityInQueryIndex, in AreaDamage areaDamage) => {
            for(int i=0; i < translationArray.Length; i++) {
                if (math.distance(translationArray[i].Value, areaDamage.position) <= areaDamage.range) {
                    ecb.DestroyEntity(entityInQueryIndex, entityArray[i]);
                }
            }
            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();

        translationArray.Dispose(this.Dependency);
        entityArray.Dispose(this.Dependency);
    }
}