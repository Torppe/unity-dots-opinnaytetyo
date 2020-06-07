using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class EnemySpawnerSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate() {
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning() {
        base.OnStartRunning();
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        int range = 100;
        Random random = new Random(56);

        Entities.ForEach((Entity entity, int entityInQueryIndex, in PrefabEntityComponent prefabEntityComponent) => {
            for (int i = 0; i < prefabEntityComponent.spawnAmount; i++) {
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