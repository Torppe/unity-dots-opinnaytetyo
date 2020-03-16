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
            .WithAll<EnemyTag>()
            .ForEach((ref Translation translation, ref MoveDirection moveDirection, ref HasTarget hasTarget) => {
                if (math.distance(hasTarget.position, translation.Value) > 2) {
                    moveDirection.Value = math.normalize(hasTarget.position - translation.Value);
                    translation.Value += moveDirection.Value * deltaTime;
                }
            });
    }
}

//public class EnemyMovementJobSystem : JobComponentSystem {
//    protected override JobHandle OnUpdate(JobHandle inputDeps) {
//        float deltaTime = Time.DeltaTime;

//        JobHandle jobHandle = Entities
//            .WithAll<EnemyTag>()
//            .ForEach((ref Translation translation, ref MoveDirection moveDirection, ref HasTarget hasTarget) => {
//                if (math.distance(hasTarget.position, translation.Value) > 2) {
//                    moveDirection.Value = math.normalize(hasTarget.position - translation.Value);
//                    translation.Value += moveDirection.Value * deltaTime;
//                }
//            }).Schedule(inputDeps);
//        return jobHandle;
//    }
//}