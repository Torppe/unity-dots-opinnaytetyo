using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public int amount = 1000;

    public GameObject objectToSpawn;
    void Start() {
        SpawnEnemies();
    }

    void SpawnEnemies() {
        if (objectToSpawn == null)
            return;

        float range = 100;

        for(int i=0; i < amount; i++) {
            Instantiate(objectToSpawn, new Vector3(Random.Range(-range, range), Random.Range(-range, range)), Quaternion.identity);
        }
    }
}
