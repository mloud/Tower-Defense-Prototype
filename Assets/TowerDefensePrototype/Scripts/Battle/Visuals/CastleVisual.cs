using UnityEngine;

namespace CastlePrototype.Battle.Visuals
{
    public class CastleVisual : VisualObject
    {
        [SerializeField] private Transform weaponSlot;
        public void AddWeaponVisual(WeaponVisual weaponVisual)
        {
            Debug.Assert(weaponSlot.childCount == 0, "Weapon slot is already occupied");
            AddToSlot(weaponSlot, weaponVisual);
        }

        private void AddToSlot(Transform slot, VisualObject visualObject)
        {
            visualObject.transform.SetParent(slot);
            visualObject.transform.localPosition = Vector3.zero;
        }
    }
}