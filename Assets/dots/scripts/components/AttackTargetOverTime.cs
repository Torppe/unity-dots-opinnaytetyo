using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AttackTargetOverTime : IComponentData {
    public float damage;
    public float timer;
    public float cooldownAmount;
}
