using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
public class Enemy : LivingEntity {
    public Transform Target { get; private set; }

    private EnemyMovement movement;
    private EnemyAttack attack;

    private void Start() {
        Target = GameObject.FindGameObjectWithTag("Player").transform;

        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();

        attack.target = Target;
        movement.target = Target;
        movement.transformToMove = transform;
    }

    private void Update() {
        if(movement.DistanceToTarget <= attack.attackRange) {
            attack.Attack();
        } else {
            movement.Move();
        }
    }
}
