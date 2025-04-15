using Unity.Collections;
using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Components
{
    public struct VisualComponent : IComponentData
    { 
        // id for visual
        public FixedString64Bytes VisualId;
        // assigned VisualManager after creation
        public int VisualIndex;

        public int Level;
        public bool HasVisual;
    }
}