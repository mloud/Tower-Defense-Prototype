using System;
using TowerDefense.Battle.Logic.Components;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.Battle.Logic
{
    public static class Utils
    {
        public static float3 GetCenter(float3 min, float3 max) => (min + max) * 0.5f;
        public static float2 GetCenter(float2 min, float2 max) => (min + max) * 0.5f;
        
        public static float3 GetRandomPosition(float3 center, float3 size) =>
            new(
                center + new float3(
                    Random.Range(-size.x / 2.0f, size.x / 2.0f),
                    Random.Range(-size.y / 2.0f, size.y / 2.0f),
                    Random.Range(-size.z / 2.0f, size.z / 2.0f)));

        public static float VolumeDistanceSqr(
            float3 entity1Position, 
            in SettingComponent entity1Settings, 
            float3 entity2Position,
            in SettingComponent entity2Settings)
        {

            Debug.Assert(entity1Settings.Width == 0 || entity2Settings.Width == 0, "Two segments are not supported now");
            
            if (entity1Settings.Width > 0 || entity2Settings.Width > 0)
            {
                var centerOfWidth = entity1Settings.Width > 0 ? entity1Position : entity2Position;
                var width = entity1Settings.Width > 0 ? entity1Settings.Width : entity2Settings.Width;

                var leftPoint = centerOfWidth - width / 2.0f;
                var rightPoint = centerOfWidth + width / 2.0f;
                var otherPoint = entity1Settings.Width > 0 ? entity2Position : entity1Position;
                var combinedRadius = entity1Settings.Radius + entity2Settings.Radius;
                if (otherPoint.x > rightPoint.x)
                {
                    var delta = new float3(rightPoint.x - otherPoint.x, 0, rightPoint.y - otherPoint.y);
                    var distanceSqr = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z -
                                      combinedRadius * combinedRadius;
                    return math.max(0, distanceSqr);
                }

                if (otherPoint.x < leftPoint.x)
                {
                    var delta = new float3(leftPoint.x - otherPoint.x, 0, leftPoint.y - otherPoint.y);
                    var distanceSqr = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z -
                                      combinedRadius * combinedRadius;
                    return math.max(0, distanceSqr);
                }

                var zDistanceSqr = (otherPoint.z - centerOfWidth.z) * (otherPoint.z - centerOfWidth.z) -
                                  combinedRadius * combinedRadius;
                return math.max(0, zDistanceSqr);
            
            }
            else
            {
                var delta = entity1Position - entity2Position;
                var weights = entity1Settings.DistanceAxes * entity2Settings.DistanceAxes;

                var distanceSqr =
                    delta.x * delta.x * weights.x +
                    delta.y * delta.y * weights.y +
                    delta.z * delta.z * weights.z;
                var combinedRadius = entity1Settings.Radius + entity2Settings.Radius;

                return math.max(0, distanceSqr - combinedRadius * combinedRadius);
            }
        }
        

        public static float Distance2DSqr(float3 entity1Position, float3 entity2Position)
        {
            float3 combinedDistanceAxes = new float3(1, 0, 1) * new float3(1, 0, 1);
            var x = entity1Position.x - entity2Position.x;
            var y = entity1Position.y - entity2Position.y;
            var z = entity1Position.z - entity2Position.z;
            return x * x * combinedDistanceAxes.x + y * y * combinedDistanceAxes.y + z * z * combinedDistanceAxes.z;
        }

        // public static float DistanceSqr(float3 entity1Position, float3 entity1DistanceAxes, float3 entity2Position,
        //     float3 entity2DistanceAxes)
        // {
        //     float3 combinedDistanceAxes = entity1DistanceAxes * entity2DistanceAxes;
        //     var x = entity1Position.x - entity2Position.x;
        //     var y = entity1Position.y - entity2Position.y;
        //     var z = entity1Position.z - entity2Position.z;
        //     return x * x * combinedDistanceAxes.x + y * y * combinedDistanceAxes.y + z * z * combinedDistanceAxes.z;
        // }

        public static float2 To2D(float3 position)
        {
           // Debug.Assert(position.y == 0);
           position.y = 0;
            return new float2(position.x, position.z);
        }
        
        public static float3 To3D(float2 position) => new(position.x, 0, position.y);

        public static float3 ToDirection(float3 start, float3 end) => new(end.x - start.x, end.y - start.y, end.z - start.z);

        public static float3 Direction2D(float3 from, float3 to)
        {
            float3 dir = to - from;
            dir.y = 0;
            return dir;
        }


        public struct IntersectionResult
        {
            public float Distance;
            public float2 ExitPoint;
            public float2 Normal;
            public bool HasIntersection;
        }

        public static float3 ComputeBounce(float3 direction, float3 normal)
        {
            direction = math.normalize(direction);
            normal = math.normalize(normal);

            return direction - 2 * math.dot(direction, normal) * normal;
        }
        
        /// <summary>
        /// Calculate intersection between a ray originating inside a rectangle and the rectangle boundaries
        /// </summary>
        /// <param name="rectMin">Bottom-left corner of the rectangle</param>
        /// <param name="rectMax">Top-right corner of the rectangle</param>
        /// <param name="rayOrigin">Origin point of the ray (must be inside the rectangle)</param>
        /// <param name="rayDirection">Direction vector of the ray</param>
        /// <returns>Intersection result containing exit point, distance, and surface normal</returns>
        public static IntersectionResult CalculateIntersectionFromRectangleInside(
            float2 rectMin,
            float2 rectMax,
            float2 rayOrigin,
            float2 rayDirection)
        {
            var result = new IntersectionResult
            {
                Distance = float.PositiveInfinity,
                ExitPoint = new float2(),
                Normal = new float2(),
                HasIntersection = false
            };

            // Check if ray direction is zero
            if (rayDirection.x == 0 && rayDirection.y == 0)
            {
                return result;
            }

            // Verify ray origin is inside rectangle
            // if (rayOrigin.x < rectMin.x || rayOrigin.x > rectMax.x ||
            //     rayOrigin.y < rectMin.y || rayOrigin.y > rectMax.y)
            // {
            //     throw new ArgumentException($"Ray origin must be inside the rectangle but its {rayOrigin}");
            // }
            
            rayOrigin.x = math.clamp(rayOrigin.x, rectMin.x, rectMax.x);
            rayOrigin.y = math.clamp(rayOrigin.y, rectMin.y, rectMax.y);


            // Test intersection with each boundary
            // Left boundary (x = rectMin.X)
            if (rayDirection.x < 0) // Ray pointing left
            {
                float t = (rectMin.x - rayOrigin.x) / rayDirection.x;
                float y = rayOrigin.y + t * rayDirection.y;
                if (rectMin.y <= y && y <= rectMax.y && t < result.Distance)
                {
                    result.Distance = t;
                    result.Normal = new float2(-1, 0);
                    result.ExitPoint = new float2(rectMin.x, y);
                    result.HasIntersection = true;
                }
            }

            // Right boundary (x = rectMax.X)
            if (rayDirection.x > 0) // Ray pointing right
            {
                float t = (rectMax.x - rayOrigin.x) / rayDirection.x;
                float y = rayOrigin.y + t * rayDirection.y;
                if (rectMin.y <= y && y <= rectMax.y && t < result.Distance)
                {
                    result.Distance = t;
                    result.Normal = new float2(1, 0);
                    result.ExitPoint = new float2(rectMax.x, y);
                    result.HasIntersection = true;
                }
            }

            // Bottom boundary (y = rectMin.Y)
            if (rayDirection.y < 0) // Ray pointing down
            {
                float t = (rectMin.y - rayOrigin.y) / rayDirection.y;
                float x = rayOrigin.x + t * rayDirection.x;
                if (rectMin.x <= x && x <= rectMax.x && t < result.Distance)
                {
                    result.Distance = t;
                    result.Normal = new float2(0, -1);
                    result.ExitPoint = new float2(x, rectMin.y);
                    result.HasIntersection = true;
                }
            }

            // Top boundary (y = rectMax.Y)
            if (rayDirection.y > 0) // Ray pointing up
            {
                float t = (rectMax.y - rayOrigin.y) / rayDirection.y;
                float x = rayOrigin.x + t * rayDirection.x;
                if (rectMin.x <= x && x <= rectMax.x && t < result.Distance)
                {
                    result.Distance = t;
                    result.Normal = new float2(0, 1);
                    result.ExitPoint = new float2(x, rectMax.y);
                    result.HasIntersection = true;
                }
            }

            return result;
        }
    }
}
