using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public LayerMask layerMask;

    private Transform currentTarget = null;
    private readonly Collider2D[] nearbyEnemies = new Collider2D[100];
    private float timer = 0;

    void Update() {
        if(currentTarget == null) {
            FindTarget();
        } else {
            ShootTarget();
        }
    }

    void FindTarget() {
        print("finding target!");
        Physics2D.OverlapCircleNonAlloc(transform.position, 10, nearbyEnemies, layerMask);
        if (nearbyEnemies.Length < 1)
            return;

        GetClosestTarget();
    }

    void GetClosestTarget() {
        Transform closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D enemy in nearbyEnemies) {
            if (enemy == null)
                continue;

            float distanceSqrToTarget = (enemy.transform.position - currentPosition).sqrMagnitude;
            if(distanceSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = distanceSqrToTarget;
                closestTarget = enemy.transform;
            }
        }

        currentTarget = closestTarget;
    }

    void ShootTarget() {
        float currentTime = Time.time;
        if(currentTime > timer) {
            print("boom!");

            Collider2D[] killedTargets = Physics2D.OverlapCircleAll(currentTarget.position, 2, layerMask);
            foreach(Collider2D killedTarget in killedTargets) {
                Destroy(killedTarget.gameObject);
            }

            timer = currentTime + 1;
            currentTarget = null;
        }
    }
}
