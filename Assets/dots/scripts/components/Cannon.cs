using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Cannon : IComponentData {
    public float cooldown;
    public float timer;
    public float3 direction;
}
