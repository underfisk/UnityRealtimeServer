using System;
using System.Collections.Generic;
using System.Text;

namespace UServer.GameEntity
{
    [Serializable]
    public class DeckCard
    {
        public uint Id;
        public string Name;
        public string Description;
        public int ManaCost;
        public int Faction;
        public uint Attack;
        public uint Defense;
    }
}
