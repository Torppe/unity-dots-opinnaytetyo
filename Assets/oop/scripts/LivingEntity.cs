using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable{
    [SerializeField]
    protected float health;
    public bool IsDead { get; protected set; } = false;

    public void TakeDamage(float amount) {
        if (IsDead)
            return;

        health -= amount;

        if (health <= 0) {
            IsDead = true;
            Destroy(gameObject);
        }
    }
}
