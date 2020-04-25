using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FindTarget))]
[RequireComponent(typeof(ShootTarget))]
public class Player : LivingEntity {
    private FindTarget findTarget;
    private ShootTarget shootTarget;

    void Start() {
        findTarget = GetComponent<FindTarget>();
        shootTarget = GetComponent<ShootTarget>();

        List<GameObject> nearbyEnemies = EnemySpawner.nearbyEnemies;
        findTarget.NearbyEnemies = nearbyEnemies;
        shootTarget.NearbyEnemies = nearbyEnemies;
    }

    void Update() {
        if(findTarget.Target == null) {
            findTarget.Find();
        } else {
            shootTarget.Shoot(findTarget.Target);
        }
    }
}
