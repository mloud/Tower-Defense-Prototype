using System;
using System.Collections.Generic;
using OneDay.Core.Modules.Data;
using TowerDefense.Battle.Logic.Managers.Skills;
using UnityEngine;

namespace TowerDefense.Data.Definitions
{
    [Serializable]
    public partial class HeroDefinition : BaseDataObject
    {
        // ID
        [Header("Ids")]
        public string UnitId;
        public string VisualId;
        public string ProjectileVisualId;
        public float AttackDelay;

        [Header("Unit type stats")] 
        public AttackType AttackType;
        public bool CreatedBySkill;
      
        // BASIC STATS
        [Header("Basic stats")]
        public float ProjectileSpeed;
        public float MoveSpeed;

        [SerializeField] float Cooldown;
        [SerializeField] float Damage;
        [SerializeField] float AttackDistance;
        [SerializeField] float TargetRange;
        [SerializeField] float Hp;
     
        // SPECIFIC
        [Header("Specific stats")]
        public bool Knockback;
        public bool KnockbackResistent;
        public float FireAgainInterval = 0.5f;

        [SerializeField] int Bounce;
        [SerializeField] int Penetration;
        [SerializeField] int Fireagain;
        [SerializeField] int FireAgainSpread;

       
        // AOE - either projectile or direct AOE      
        [Header("AOE stats")]
        public bool AoeOnly;
        [SerializeField] float AoeRadius;
        [SerializeField] float AoeDamage;
      
        [Header("Level upgrades")] 
        public HeroLevelUpgradePath UpgradePath;

 //       [Header("Skills")] public List<SkillType> SupportedSkills;
    }
}