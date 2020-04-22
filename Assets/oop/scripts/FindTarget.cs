using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour {
    [SerializeField]
    private float findRange = 10f;
    public Transform Target { get; private set; } = null;
    public List<GameObject> NearbyEnemies { private get; set; } = null;
    
    public void Find() {
        if (NearbyEnemies.Count < 1)
            return;

        Transform closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject enemy in NearbyEnemies) {
            if (enemy == null)
                continue;

            float distanceSqrToTarget = (enemy.transform.position - transform.position).sqrMagnitude;
            float attackRangeSqr = findRange * findRange;

            if (distanceSqrToTarget <= attackRangeSqr && distanceSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = distanceSqrToTarget;
                closestTarget = enemy.transform;
            }
        }

        Target = closestTarget;
    }
}
