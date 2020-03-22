using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {
    [SerializeField]
    private float health;

    [Range(0, 5)]
    [SerializeField]
    private float attackCooldown = 0.5f;
    [SerializeField]
    private float attackRange = 10f;
    public Transform Target { get; private set; } = null;

    public LayerMask layerMask;
    public GameObject explosionPrefab;

    public List<GameObject> nearbyEnemies;
    private float timer = 0;

    void Start() {
        nearbyEnemies = EnemySpawner.nearbyEnemies;
    }

    void Update() {
        if(Target == null) {
            FindTarget();
        } else {
            ShootTarget();
        }
    }

    void FindTarget() {
        if (nearbyEnemies.Count < 1)
            return;

        GetClosestTarget();
    }

    void GetClosestTarget() {
        Transform closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in nearbyEnemies) {
            if (enemy == null)
                continue;

            float distanceSqrToTarget = (enemy.transform.position - currentPosition).sqrMagnitude;
            float attackRangeSqr = attackRange * attackRange;

            if(distanceSqrToTarget <= attackRangeSqr && distanceSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = distanceSqrToTarget;
                closestTarget = enemy.transform;
            }
        }

        Target = closestTarget;
    }

    void ShootTarget() {
        float currentTime = Time.time;
        if(currentTime > timer) {
            float range = 3f;

            GameObject effect = GameObject.Instantiate(explosionPrefab, Target.position, Quaternion.Euler(0,0, Random.Range(0,360)));

            foreach(GameObject killedTarget in nearbyEnemies) {
                if (killedTarget == null)
                    continue;

                if((killedTarget.transform.position - effect.transform.position).magnitude <= range) {
                    Destroy(killedTarget);
                }
            }

            Destroy(effect, 1);

            timer = currentTime + attackCooldown;
            Target = null;
        }
    }

    public void TakeDamage(float amount) {
        if(health <= 0) {
            Destroy(gameObject);
        } else {
            health -= amount;
        }
    }
}
