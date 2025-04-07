using System.Collections.Generic;
using System.Linq;
using CastlePrototype.Battle.Visuals;
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
                new(VisualManager.Default.GetObjectPosition("slot_0")),
                new(VisualManager.Default.GetObjectPosition("slot_1")),
                new(VisualManager.Default.GetObjectPosition("weapon_slot")),
                new(VisualManager.Default.GetObjectPosition("slot_2")),
                new(VisualManager.Default.GetObjectPosition("slot_3")),
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