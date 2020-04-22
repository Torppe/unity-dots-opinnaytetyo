using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTarget : MonoBehaviour {
    [Range(0, 5)]
    [SerializeField]
    private float attackCooldown = 0.5f;
    private float timer = 0;

    [SerializeField]
    private GameObject explosionPrefab;
    public List<GameObject> NearbyEnemies { private get; set; } = null;

    public void Shoot(Transform target) {
        float currentTime = Time.time;
        if (currentTime > timer) {
            SpawnExplosionEffect(target.position);

            foreach (GameObject enemy in NearbyEnemies) {
                if (enemy == null)
                    continue;

                if ((enemy.transform.position - target.position).magnitude <= 3f) {
                    Destroy(enemy);
                }
            }

            timer = currentTime + attackCooldown;
            target = null;
        }
    }

    private void SpawnExplosionEffect(Vector3 position) {
        GameObject effect = GameObject.Instantiate(explosionPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        Destroy(effect, 1f);
    }
}
