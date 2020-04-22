using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable{
    [SerializeField]
    protected float health;
    public bool IsDead { get; protected set; }

    public void TakeDamage(float amount) {
        if (health <= 0 && !IsDead) {
            IsDead = true;
            Destroy(gameObject);
        } else {
            health -= amount;
        }
    }
}
