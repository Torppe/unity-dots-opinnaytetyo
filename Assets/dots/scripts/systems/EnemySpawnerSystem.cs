using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

//public class EnemySpawnerSystem : ComponentSystem {
//    private int range = 100;
//    private Random random;
//    public int spawnAmount = 10000;

//    protected override void OnCreate() {
//        random = new Random(56);
//    }

//    protected override void OnStartRunning() {
//        Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) => {
//            for (int i = 0; i < spawnAmount; i++) {
//                Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);

//                EntityManager.SetComponentData(
//                    spawnedEntity,
//                    new Translation { Value = new float3(random.NextFloat(-range, range), random.NextFloat(-range, range), 0) }
//                );
//            }
//        });
//    }

//    protected override void OnUpdate() {
    
//    }
//}

public class EnemySpawnerSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate() {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning() {
        base.OnStartRunning();

        int range = 100;
        int spawnAmount = 10000;

        Random random = new Random(56);

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref PrefabEntityComponent prefabEntityComponent) => {
            for (int i = 0; i < spawnAmount; i++) {
                Entity spawnedEntity = ecb.Instantiate(entityInQueryIndex, prefabEntityComponent.prefabEntity);
                ecb.SetComponent<Translation>(
                        entityInQueryIndex, 
                        spawnedEntity,
                        new Translation { Value = new float3(random.NextFloat(-range, range), random.NextFloat(-range, range), 0) }
                    );
            }
            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();
    }

    protected override void OnUpdate() {
    }
}
