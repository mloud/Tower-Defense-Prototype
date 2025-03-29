using System.Collections.Generic;
using System.Linq;
using OneDay.Core.Extensions;
using Unity.Entities;
using UnityEngine;

namespace CastlePrototype.Battle.Logic.Managers.Slots
{
    public class SlotManager: WorldManager
    {
        private List<Slot> Slots { get; }
      
        public SlotManager(World world) : base(world)
        {
            Slots = new List<Slot>
            {
                new(new Vector3(-3.0f, 0, -7)),
                new(new Vector3(-1.5f, 0, -7)),
                new(new Vector3( 0f, 0, -7)),
                new(new Vector3(1.5f, 0, -7)),
                new(new Vector3(3.0f, 0, -7)),
            };
        }

        public Slot GetInitialSlot() => Slots[2];
       
        public Slot GetFirstAvailableSlot()
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                if (!Slots[i].IsOccupied)
                    return Slots[i];
            }

            return null;
        }

        public Slot GetRandomAvailableSlot()
        {
            var free = Slots.Where(x => !x.IsOccupied).ToArray();
            return free.Length > 0 ? free.GetRandom() : null;
        }


        protected override void OnRelease() => Slots.Clear();
    }
}