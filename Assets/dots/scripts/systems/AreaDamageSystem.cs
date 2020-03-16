using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class AreaDamageSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.WithAll<AreaDamage>().ForEach((Entity explosionEntity, ref AreaDamage areaDamage) => {
            float3 position = areaDamage.position;
            float range = areaDamage.range;

            Entities.WithAll<EnemyTag>().ForEach((Entity enemyEntity, ref Translation translation) => {
                if(math.distance(position, translation.Value) <= range) {
                    PostUpdateCommands.DestroyEntity(enemyEntity);
                }
            });

            PostUpdateCommands.DestroyEntity(explosionEntity);
        });
    }
}
