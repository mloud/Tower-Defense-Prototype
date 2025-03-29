using System.Collections.Generic;
using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data
{
    public class WeaponDeck : BaseDataObject
    {
        public Dictionary<string, WeaponProgress> Weapons;
    }
}