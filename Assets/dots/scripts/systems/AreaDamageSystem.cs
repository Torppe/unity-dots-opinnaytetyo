using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;

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
        NativeArray<AreaDamage> areaDamages = query.ToComponentDataArray<AreaDamage>(Allocator.TempJob);

        Entities
            .WithAll<EnemyTag>()
            .WithAll<Health>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<Damage> buffer, in Translation translation) => {
                for (int i = 0; i < areaDamages.Length; i++) {
                    if (math.distance(areaDamages[i].position, translation.Value) <= areaDamages[i].range) {
                        buffer.Add(new Damage { Value = areaDamages[i].damage });
                    }
                }
            })
            .WithDeallocateOnJobCompletion(areaDamages)
            .ScheduleParallel();

        Entities
            .WithAll<AreaDamage>()
            .ForEach((Entity entity, int entityInQueryIndex) => {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            })
            .Schedule();
    }
}
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