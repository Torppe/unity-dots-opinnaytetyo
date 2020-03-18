using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class FindTargetSystem : ComponentSystem {
    protected override void OnUpdate() {
        HandleEnemy();
        HandlePlayer();
    }

    private void HandlePlayer() {
        Entities
            .WithNone<HasTarget>()
            .WithAll<PlayerTag>()
            .ForEach((Entity entity, ref Translation translation) => {
                float3 unitPosition = translation.Value;
                Entity closestTarget = Entity.Null;
                float3 closestPosition = float3.zero;

                Entities
                    .WithAll<EnemyTag>()
                    .ForEach((Entity target, ref Translation targetTranslation) => {
                        if (closestTarget == Entity.Null) {
                            closestTarget = target;
                            closestPosition = targetTranslation.Value;
                        } else if (math.distance(unitPosition, targetTranslation.Value) < math.distance(unitPosition, closestPosition)) {
                            closestTarget = target;
                            closestPosition = targetTranslation.Value;
                        }
                    });

                if (closestTarget != Entity.Null) {
                    PostUpdateCommands.AddComponent(entity, new HasTarget { target = closestTarget, position = closestPosition });
                }
            });
    }

    private void HandleEnemy() {
        Entities
            .WithNone<HasTarget>()
            .WithAll<EnemyTag>()
            .ForEach((Entity entity, ref Translation translation) => {
                Entities
                    .WithAll<PlayerTag>()
                    .ForEach((Entity foundTarget, ref Translation targetTranslation) => {
                        PostUpdateCommands.AddComponent(entity, new HasTarget { target = foundTarget, position = targetTranslation.Value });
                    });
            });
    }
}
