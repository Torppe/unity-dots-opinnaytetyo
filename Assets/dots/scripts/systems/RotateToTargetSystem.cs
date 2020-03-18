using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class RotateToTargetSystem : ComponentSystem {
    float3 target = float3.zero;
    quaternion targetRotation = quaternion.identity;

    protected override void OnUpdate() {
        GetTarget();
        Rotate();
    }
    private void GetTarget() {
        Entities.WithAll<PlayerTag>().WithAll<HasTarget>().ForEach((ref HasTarget hasTarget) => {
             target = hasTarget.position;
        });
    }
    private void Rotate() {
        float delta = Time.DeltaTime;

        Entities.WithAll<TurretTag>().ForEach((ref Rotation rotation, ref Translation translation) => {

            if (Quaternion.Angle(rotation.Value, targetRotation) < 1) {
                float3 position = new float3(translation.Value.x, translation.Value.y, 0);
                float3 heading = math.normalize(target - position);
                float targetAngle = math.atan2(target.y, target.x);
                targetRotation = Quaternion.LookRotation(Vector3.forward, heading);
            }

            rotation.Value = Quaternion.Lerp(rotation.Value, targetRotation, delta * 20f);
        });
    }
}
