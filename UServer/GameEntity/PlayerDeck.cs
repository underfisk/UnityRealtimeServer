using System;
using System.Collections.Generic;
using System.Text;

namespace UServer.GameEntity
{
    [Serializable]
    public class PlayerDeck
    {
        public uint Id;
        public uint PlayerId;
        public string Name;
        public int Faction;

        public List<DeckCard> deckCards;
        
    }
}
