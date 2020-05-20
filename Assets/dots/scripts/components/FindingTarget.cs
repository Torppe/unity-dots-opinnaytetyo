using Unity.Entities;

[GenerateAuthoringComponent]
public struct FindingTarget : IComponentData {
    public float FindRange;
}
