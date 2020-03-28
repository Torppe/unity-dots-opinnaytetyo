using Unity.Entities;
using Unity.Mathematics;
using System;
public enum TargetType {
    Player,
    Enemy
}

[GenerateAuthoringComponent]
public struct FindingTarget : IComponentData {
    public TargetType targetType;
    public float FindRange;
}
