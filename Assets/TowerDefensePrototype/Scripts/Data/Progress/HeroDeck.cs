using System.Collections.Generic;
using OneDay.Core.Modules.Data;

namespace CastlePrototype.Data.Progress
{
    public class HeroDeck : BaseDataObject
    {
        public Dictionary<string, HeroProgress> Heroes;
    }
}