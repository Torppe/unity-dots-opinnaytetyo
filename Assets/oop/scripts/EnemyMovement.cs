using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public Transform transformToMove;
    
    [Range(0, 100)]
    [SerializeField]
    private float rotationSpeed = 40f;
    [SerializeField]
    private float movementSpeed = 2f;

    public float DistanceToTarget { get; private set; } = Mathf.Infinity;
    private Vector3 directionToTarget;

    public void Move() {
        if (target == null || transformToMove == null)
            return;

        CalculatePositions();
        RotateTowardsTarget();
        MoveTowardsTarget();
    }

    private void CalculatePositions() {
        directionToTarget = target.position - transformToMove.position;
        DistanceToTarget = directionToTarget.magnitude;
    }

    private void MoveTowardsTarget() {
        transformToMove.Translate(directionToTarget.normalized * movementSpeed * Time.deltaTime, Space.World);
    }

    private void RotateTowardsTarget() {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToTarget.normalized);
        transformToMove.rotation = Quaternion.Lerp(transformToMove.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
