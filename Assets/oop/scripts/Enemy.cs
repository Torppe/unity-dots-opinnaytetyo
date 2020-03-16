using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float movementSpeed = 2f;

    private float nextAttackTime = 0f;
    private Transform target;
    private Rigidbody2D rb;
    
    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget() {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > 2) {
            rb.MovePosition(transform.position + (direction.normalized * movementSpeed * Time.deltaTime));
        } else {
            Attack();
        }
    }

    void Attack() {
        float currentTime = Time.time;
        if(currentTime > nextAttackTime) {
            nextAttackTime = currentTime + 4f;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
