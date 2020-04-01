using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(UpdateTargetSystem))]
public class DamageTargetSystem : SystemBase {
    EndSimulationEntityCommandBufferSystem commandBufferSystem;
    protected override void OnCreate() {
        base.OnCreate();

        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
        float delta = Time.DeltaTime;
        var ecb = commandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .ForEach((ref AttackTargetOverTime attack) => {
                if(attack.timer > 0) {
                    attack.timer -= delta;
                }
            }).ScheduleParallel();

        var buffer = GetBufferFromEntity<Damage>();

        Entities
            .WithAll<EnemyTag>()
            .ForEach((int entityInQueryIndex, ref AttackTargetOverTime attack, in HasTarget hasTarget, in LocalToWorld ltw) => {
                float range = 2;
                if (attack.timer > 0 || math.distance(ltw.Position, hasTarget.position) > range)
                    return;

                buffer[hasTarget.target].Add(new Damage { Value = attack.damage });

                attack.timer = attack.cooldownAmount;
            }).Schedule();
    }
}
