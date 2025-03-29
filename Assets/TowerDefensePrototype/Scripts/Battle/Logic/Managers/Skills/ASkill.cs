using Unity.Entities;

namespace CastlePrototype.Battle.Logic.Managers.Skills
{
    public abstract class ASkill
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DefinitionId { get; set; }
        public SkillType SkillType { get; protected set; }
        public bool NeedsUnit { get; protected set; }
        public float Value { get; protected set; }
        
        // runtime
        public Entity RelatedEntity { get; set; }
        protected ASkill(string name, string description, float value)
        {
            Name = name;
            Description = description;
            Value = value;
        }
        public abstract void Apply(EntityManager entityManager);
    }
}