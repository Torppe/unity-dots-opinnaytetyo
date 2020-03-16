using Unity.Entities;
using Unity.Mathematics;
public struct HasTarget : IComponentData {
    public Entity target;
    public float3 position;
}
