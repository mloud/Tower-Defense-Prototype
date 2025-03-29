using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace CastlePrototype.Battle.Logic
{
    public static class Utils
    {
        public static float3 GetRandomPosition(float3 center, float3 size) =>
            new(
                center + new float3(
                    Random.Range(-size.x / 2.0f, size.x / 2.0f),
                    Random.Range(-size.y / 2.0f, size.y / 2.0f),
                    Random.Range(-size.z / 2.0f, size.z / 2.0f)));

        
        public static float Distance2DSqr(float3 entity1Position, float3 entity2Position)
        {
            float3 combinedDistanceAxes = new float3(1,0,1) * new float3(1,0,1);
            var x = entity1Position.x - entity2Position.x;
            var y = entity1Position.y - entity2Position.y;
            var z = entity1Position.z - entity2Position.z;
            return x * x * combinedDistanceAxes.x + y * y * combinedDistanceAxes.y + z * z * combinedDistanceAxes.z;
        }

        public static float DistanceSqr(float3 entity1Position, float3 entity1DistanceAxes, float3 entity2Position,
            float3 entity2DistanceAxes)
        {
            float3 combinedDistanceAxes = entity1DistanceAxes * entity2DistanceAxes;
            var x = entity1Position.x - entity2Position.x;
            var y = entity1Position.y - entity2Position.y;
            var z = entity1Position.z - entity2Position.z;
            return x * x * combinedDistanceAxes.x + y * y * combinedDistanceAxes.y + z * z * combinedDistanceAxes.z;
        }

        public static float3 Direction2D(float3 from, float3 to)
        {
            float3 dir = to - from;
            dir.y = 0;
            return dir;
        }

    }
}