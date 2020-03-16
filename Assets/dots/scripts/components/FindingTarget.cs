using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FindingTarget : IComponentData {
    public float FindRange;
}
