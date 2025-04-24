using UnityEngine;

namespace TowerDefense.Battle.Logic.Managers.Slots
{
    public class Slot
    {
        public bool IsOccupied { get; set; }
        public Vector3 Position { get; }

        public Slot(Vector3 position) => Position = position;
    }
}