using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class AreaDamageSystem : ComponentSystem {
    //float timer = 0f;
    protected override void OnUpdate() {
        //timer -= Time.DeltaTime;

        //if(timer < 0) {
        //    Entity explosion = EntityManager.CreateEntity();
        //    EntityManager.AddComponentData(explosion, new AreaDamage { position = new float3(2, 2, 0), range = 2f });
        //    Debug.Log("Explosion created");
        //    timer = 2f;
        //}

        Entities.WithAll<AreaDamage>().ForEach((Entity explosionEntity, ref AreaDamage areaDamage) => {
            float3 position = areaDamage.position;
            float range = areaDamage.range;

            Entities.WithAll<EnemyTag>().ForEach((Entity enemyEntity, ref Translation translation) => {
                if(math.distance(position, translation.Value) <= range) {
                    PostUpdateCommands.DestroyEntity(enemyEntity);
                }
            });

            PostUpdateCommands.DestroyEntity(explosionEntity);
            Debug.Log("Explosion called");
        });
    }
}
