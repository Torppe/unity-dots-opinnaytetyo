using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {
    [HideInInspector]
    public Transform target;

    [SerializeField]
    public float attackRange = 2f;
    private float nextAttackTime = 0f;

    public void Attack() {
        if (target == null)
            return;

        float currentTime = Time.time;
        if (currentTime >= nextAttackTime && target.TryGetComponent<IDamageable>(out var damageable)) {
            damageable.TakeDamage(1);
            nextAttackTime = currentTime + 2f;
        }
    }
}
