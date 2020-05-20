using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

//public class FindTargetSystem : SystemBase {
//    protected override void OnUpdate() {
//        Entities
//            .WithNone<HasTarget>()
//            .WithAll<PlayerTag>()
//            .ForEach((Entity entity, ref Translation translation) => {
//                float3 unitPosition = translation.Value;
//                Entity closestTarget = Entity.Null;
//                float3 closestPosition = float3.zero;
//                float range = 8f;

//                Entities
//                    .WithAll<EnemyTag>()
//                    .ForEach((Entity target, ref Translation targetTranslation) => {
//                        if (closestTarget == Entity.Null || math.distance(unitPosition, targetTranslation.Value) < math.distance(unitPosition, closestPosition)) {
//                            closestTarget = target;
//                            closestPosition = targetTranslation.Value;
//                        }
//                    });

//                if (closestTarget != Entity.Null && math.distance(unitPosition, closestPosition) < range) {
//                    EntityManager.AddComponentData(entity, new HasTarget { target = closestTarget, position = closestPosition });
//                }
//            }).Run();
//    }
//}

public class FindTargetSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    EntityQuery query;
    EntityQuery query2;

    protected override void OnCreate() {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        query = GetEntityQuery(typeof(EnemyTag), typeof(LocalToWorld));
        query2 = GetEntityQuery(typeof(PlayerTag), typeof(LocalToWorld), typeof(Health));
    }

    protected override void OnUpdate() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        FindEnemyTarget(ecb);
        FindPlayerTarget(ecb);
    }

    private void FindEnemyTarget(EntityCommandBuffer.Concurrent ecb) {

        NativeArray<LocalToWorld> playerPositionArray = query2.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        NativeArray<Entity> playerEntityArray = query2.ToEntityArray(Allocator.TempJob);

        Entities
            .WithNone<HasTarget>()
            .WithAll<EnemyTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in LocalToWorld translation, in FindingTarget findTarget) => {
                float range = findTarget.FindRange;

                float3 unitPosition = translation.Position;
                Entity closestTarget = Entity.Null;
                float3 closestPosition = float3.zero;

                for (int i = 0; i < playerPositionArray.Length; i++) {
                    float3 targetPosition = playerPositionArray[i].Position;
                    float distance = math.distance(unitPosition, targetPosition);

                    if (distance > range)
                        continue;

                    if (closestTarget == Entity.Null || distance < math.distance(unitPosition, closestPosition)) {
                        closestTarget = playerEntityArray[i];
                        closestPosition = targetPosition;
                    }
                }

                if (closestTarget != Entity.Null) {
                    ecb.AddComponent(entityInQueryIndex, entity, new HasTarget { target = closestTarget, position = closestPosition });
                }
            })
            .WithDeallocateOnJobCompletion(playerPositionArray)
            .WithDeallocateOnJobCompletion(playerEntityArray)
            .ScheduleParallel();
    }

    private void FindPlayerTarget(EntityCommandBuffer.Concurrent ecb) {
        NativeArray<LocalToWorld> enemyPositionArray = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        NativeArray<Entity> enemyEntityArray = query.ToEntityArray(Allocator.TempJob);

        Entities
            .WithNone<HasTarget>()
            .WithAll<PlayerTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in LocalToWorld translation, in FindingTarget findTarget) => {
                float range = findTarget.FindRange;

                float3 unitPosition = translation.Position;
                Entity closestTarget = Entity.Null;
                float3 closestPosition = float3.zero;

                for (int i = 0; i < enemyPositionArray.Length; i++) {
                    float3 targetPosition = enemyPositionArray[i].Position;
                    float distance = math.distance(unitPosition, targetPosition);

                    if (distance > range)
                        continue;

                    if (closestTarget == Entity.Null || distance < math.distance(unitPosition, closestPosition)) {
                        closestTarget = enemyEntityArray[i];
                        closestPosition = targetPosition;
                    }
                }

                if (closestTarget != Entity.Null) {
                    ecb.AddComponent(entityInQueryIndex, entity, new HasTarget { target = closestTarget, position = closestPosition });
                }
            })
            .WithDeallocateOnJobCompletion(enemyPositionArray)
            .WithDeallocateOnJobCompletion(enemyEntityArray)
            .ScheduleParallel();
    }
}
