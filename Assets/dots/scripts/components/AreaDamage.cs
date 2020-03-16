using Unity.Entities;
using Unity.Mathematics;

public struct AreaDamage : IComponentData {
    public float3 position;
    public float range;
}

