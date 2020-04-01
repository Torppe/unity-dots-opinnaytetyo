using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

//public class FindTargetSystem : ComponentSystem {
//    protected override void OnUpdate() {
//        HandleTurret();
//    }

//    private void HandlePlayer() {
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
//                    PostUpdateCommands.AddComponent(entity, new HasTarget { target = closestTarget, position = closestPosition });
//                }
//            });
//    }

//    private void HandleEnemy2() {
//        Entities
//            .WithNone<HasTarget>()
//            .WithAll<EnemyTag>()
//            .ForEach((Entity entity, ref Translation translation) => {
//                float3 unitPosition = translation.Value;
//                Entity closestTarget = Entity.Null;
//                float3 closestPosition = float3.zero;

//                Entities
//                    .WithAll<PlayerTag>()
//                    .ForEach((Entity target, ref Translation targetTranslation) => {
//                        if (closestTarget == Entity.Null || math.distance(unitPosition, targetTranslation.Value) < math.distance(unitPosition, closestPosition)) {
//                            closestTarget = target;
//                            closestPosition = targetTranslation.Value;
//                        }
//                    });

//                if (closestTarget != Entity.Null) {
//                    PostUpdateCommands.AddComponent(entity, new HasTarget { target = closestTarget, position = closestPosition });
//                }
//            });
//    }

//    private void HandleEnemy() {
//        Entities
//            .WithNone<HasTarget>()
//            .WithAll<EnemyTag>()
//            .ForEach((Entity entity, ref Translation translation) => {
//                float3 unitPosition = translation.Value;
//                Entity closestTarget = Entity.Null;
//                float3 closestPosition = float3.zero;

//                Entities
//                    .WithAll<PlayerTag>()
//                    .ForEach((Entity target, ref Translation targetTranslation) => {
//                        if (closestTarget == Entity.Null || math.distance(unitPosition, targetTranslation.Value) < math.distance(unitPosition, closestPosition)) {
//                            closestTarget = target;
//                            closestPosition = targetTranslation.Value;
//                        }
//                    });

//                if (closestTarget != Entity.Null) {
//                    PostUpdateCommands.AddComponent(entity, new HasTarget { target = closestTarget, position = closestPosition });
//                }
//            });
//    }

//    private void HandleTurret() {
//        Entities
//            .WithNone<HasTarget>()
//            .ForEach((Entity entity, ref LocalToWorld translation, ref FindingTarget findTarget) => {
//                float range = findTarget.FindRange;

//                float3 unitPosition = translation.Position;
//                Entity closestTarget = Entity.Null;
//                float3 closestPosition = float3.zero;

//                if (findTarget.targetType == TargetType.Enemy) {
//                    Entities
//                        .WithAll<EnemyTag>()
//                        .ForEach((Entity target, ref LocalToWorld targetTranslation) => {
//                            float distance = math.distance(unitPosition, targetTranslation.Position);
//                            if (distance > range)
//                                return;

//                            if (closestTarget == Entity.Null || distance < math.distance(unitPosition, closestPosition)) {
//                                closestTarget = target;
//                                closestPosition = targetTranslation.Position;
//                            }
//                        });
//                } else {
//                    Entities
//                        .WithAll<PlayerTag>()
//                        .ForEach((Entity target, ref LocalToWorld targetTranslation) => {
//                            float distance = math.distance(unitPosition, targetTranslation.Position);
//                            if (distance > range)
//                                return;

//                            if (closestTarget == Entity.Null || distance < math.distance(unitPosition, closestPosition)) {
//                                closestTarget = target;
//                                closestPosition = targetTranslation.Position;
//                            }
//                        });
//                }

//                if (closestTarget != Entity.Null) {
//                    PostUpdateCommands.AddComponent(entity, new HasTarget { target = closestTarget, position = closestPosition });
//                }
//            });
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
        query2 = GetEntityQuery(typeof(PlayerTag), typeof(LocalToWorld));
    }

    protected override void OnUpdate() {
        FindTargets();
    }

    private void FindTargets() {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        NativeArray<LocalToWorld> enemyPositionArray = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        NativeArray<Entity> enemyEntityArray = query.ToEntityArray(Allocator.TempJob);

        NativeArray<LocalToWorld> playerPositionArray = query2.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        NativeArray<Entity> playerEntityArray = query2.ToEntityArray(Allocator.TempJob);

        Entities
            .WithNone<HasTarget>()
            .ForEach((Entity entity, int entityInQueryIndex, in LocalToWorld translation, in FindingTarget findTarget) => {
                float range = findTarget.FindRange;

                float3 unitPosition = translation.Position;
                Entity closestTarget = Entity.Null;
                float3 closestPosition = float3.zero;

                NativeArray<LocalToWorld> tempPosArray = findTarget.targetType == TargetType.Enemy ? enemyPositionArray : playerPositionArray;
                NativeArray<Entity> tempEntityArray = findTarget.targetType == TargetType.Enemy ? enemyEntityArray : playerEntityArray;

                for (int i = 0; i < tempPosArray.Length; i++) {
                    float3 targetPosition = tempPosArray[i].Position;
                    float distance = math.distance(unitPosition, targetPosition);

                    if (distance > range)
                        continue;

                    if (closestTarget == Entity.Null || distance < math.distance(unitPosition, closestPosition)) {
                        closestTarget = tempEntityArray[i];
                        closestPosition = targetPosition;
                    }
                }

                if (closestTarget != Entity.Null) {
                    ecb.AddComponent(entityInQueryIndex, entity, new HasTarget { target = closestTarget, position = closestPosition });
                }
            }).ScheduleParallel();

        this.CompleteDependency();

        enemyPositionArray.Dispose();
        enemyEntityArray.Dispose();
        playerPositionArray.Dispose();
        playerEntityArray.Dispose();
    }

}
