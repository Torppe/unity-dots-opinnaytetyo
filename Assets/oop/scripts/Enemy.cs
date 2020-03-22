using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float movementSpeed = 2f;
    public float attackRange = 2f;

    private float nextAttackTime = 0f;
    private float distanceToTarget;
    private Vector3 directionToTarget;
    private Transform target;
    private Rigidbody2D rb;
    
    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        CalculatePositions();
        if(distanceToTarget <= attackRange) {
            Attack();
        } else {
            MoveTowardsTarget();
        }
    }

    //void FixedUpdate() {
    //    if (distanceToTarget > attackRange) {
    //        MoveTowardsTarget();
    //    }
    //}

    void CalculatePositions() {
        if (target == null)
            return;

        directionToTarget = target.position - transform.position;
        distanceToTarget = directionToTarget.magnitude;
    }

    void Attack() {
        if (target == null)
            return;

        float currentTime = Time.time;
        if(currentTime >= nextAttackTime && target.TryGetComponent<IDamageable>(out var damageable)) {
            damageable.TakeDamage(1);
            nextAttackTime = currentTime + 2f;
        }
    }

    void MoveTowardsTarget() {
        //rb.MovePosition(transform.position + (directionToTarget.normalized * movementSpeed * Time.deltaTime));
        transform.Translate(directionToTarget.normalized * movementSpeed * Time.deltaTime);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
