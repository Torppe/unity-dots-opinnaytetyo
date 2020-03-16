using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class EnemySpawnerSystem : ComponentSystem {
    private int range = 100;
    private Random random;
    public int spawnAmount = 10000;

    protected override void OnCreate() {
        random = new Random(56);
    }

    protected override void OnStartRunning() {
        Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) => {
            for (int i = 0; i < spawnAmount; i++) {
                Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);

                EntityManager.SetComponentData(
                    spawnedEntity,
                    new Translation { Value = new float3(random.NextFloat(-range, range), random.NextFloat(-range, range), 0) }
                );
            }
        });
    }

    protected override void OnUpdate() {
    
    }
}
