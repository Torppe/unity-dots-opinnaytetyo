using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField]
    public float movementSpeed = 2f;
    [SerializeField]
    public float attackRange = 2f;
    
    private float nextAttackTime = 0f;
    
    private float distanceToTarget;
    private Vector3 directionToTarget;
    private Transform target;

    [Range(0, 100)]
    [SerializeField]
    private float rotationSpeed = 40f;
    //private Quaternion targetRotation;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //targetRotation = Quaternion.identity;
    }

    private void Update() {
        CalculatePositions();
        RotateTowardsTarget();

        if(distanceToTarget <= attackRange) {
            Attack();
        } else {
            MoveTowardsTarget();
        }
    }

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
        transform.Translate(directionToTarget.normalized * movementSpeed * Time.deltaTime, Space.World);
    }

    void RotateTowardsTarget() {
        //float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToTarget.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
