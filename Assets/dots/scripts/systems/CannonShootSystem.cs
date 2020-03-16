using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class CannonShootSystem : ComponentSystem {
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<Cannon>().WithAll<HasTarget>().ForEach((Entity entity, ref Cannon cannon, ref HasTarget hasTarget) => {
            if(!EntityManager.Exists(hasTarget.target)) {
                PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));
                return;
            }

            cannon.timer -= deltaTime;

            if (cannon.timer < 0) {
                float3 targetPosition = hasTarget.position;

                Entity explosion = EntityManager.CreateEntity();
                EntityManager.AddComponentData(explosion, new AreaDamage { position = targetPosition, range = 2f });

                cannon.timer = cannon.cooldown;
            }
        });
    }
}
