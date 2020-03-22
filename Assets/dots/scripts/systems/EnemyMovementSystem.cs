using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class EnemyMovementSystem : ComponentSystem {
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithAll<HasTarget>()
            .WithAll<EnemyTag>()
            .ForEach((ref Translation translation, ref HasTarget hasTarget) => {
                float3 direction = hasTarget.position - translation.Value;
                float distance = math.length(direction);
                if (distance > 2) {
                    translation.Value += (direction / distance) * deltaTime;
                }
            });
    }
}

//public class EnemyMovementJobSystem : JobComponentSystem {
//    protected override JobHandle OnUpdate(JobHandle inputDeps) {
//        float deltaTime = Time.DeltaTime;

//        JobHandle jobHandle = Entities
//            .WithAll<EnemyTag>()
//            .ForEach((ref Translation translation, in HasTarget hasTarget) => {
//                float3 direction = hasTarget.position - translation.Value;
//                float distance = math.length(direction);
//                if (distance > 2) {
//                    translation.Value += (direction / distance) * deltaTime;
//                }
//            }).Schedule(inputDeps);
//        return jobHandle;
//    }
//}