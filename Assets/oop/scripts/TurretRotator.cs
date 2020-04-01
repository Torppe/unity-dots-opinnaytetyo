using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotator : MonoBehaviour {
    [SerializeField]
    private Transform turretParent = null;

    [Range(0,100)]
    [SerializeField]
    private float rotationSpeed = 40f;
    private Player player; 

    private Quaternion targetRotation = Quaternion.identity;

    void Start() {
        player = GetComponent<Player>();
    }

    void Update() {
        RotateTurret();
    }

    private void RotateTurret() {
        if (player.Target != null && Quaternion.Angle(turretParent.rotation, targetRotation) < 1) {
            Vector3 heading = (player.Target.position - turretParent.position).normalized;
            targetRotation = Quaternion.LookRotation(Vector3.forward, heading);
        }
        turretParent.rotation = Quaternion.Lerp(turretParent.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
