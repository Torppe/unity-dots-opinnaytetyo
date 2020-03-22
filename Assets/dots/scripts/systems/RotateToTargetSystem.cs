using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class RotateToTargetSystem : ComponentSystem {
    protected override void OnUpdate() {
        Rotate();
        Move();
    }

    private void Move() {
        float delta = Time.DeltaTime;

        Entities
            .WithAll<PlayerTag>()
            .WithAll<MoveDirection>()
            .ForEach((ref Translation translation) => {
                translation.Value.x += 1 * delta;
            });
    }

    private void Rotate() {
        float delta = Time.DeltaTime;

        Entities
            .WithAll<RotateToTarget>()
            .WithAll<HasTarget>()
            .ForEach((Entity entity, ref Rotation rotation, ref LocalToWorld translation, ref HasTarget hasTarget) => {
                float3 position = new float3(translation.Position.x, translation.Position.y, 0);
                float3 heading = math.normalize(hasTarget.position - position);
                quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, heading);

                if(!EntityManager.Exists(hasTarget.target)) {
                    EntityManager.RemoveComponent<HasTarget>(entity);
                } else {
                    rotation.Value = Quaternion.Lerp(rotation.Value, targetRotation, delta * 20f);
                }
            });
    }
}
