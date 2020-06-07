using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class RotateToTargetSystem : SystemBase {
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
            })
            .ScheduleParallel();
    }

    private void Rotate() {
        float delta = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in LocalToWorld translation, in HasTarget hasTarget, in RotateToTarget rotateToTarget) => {
                float3 position = new float3(translation.Position.x, translation.Position.y, 0);
                float3 heading = math.normalize(hasTarget.position - position);
                quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, heading);

                rotation.Value = Quaternion.Lerp(rotation.Value, targetRotation, delta * rotateToTarget.rotationSpeed);
            })
            .ScheduleParallel();
    }
}
