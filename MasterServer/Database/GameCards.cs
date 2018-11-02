using System;
using SqlKata;
using SqlKata.Execution;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MasterServer.Entity;

namespace MasterServer.Database
{   
    public static class GameCards
    {
        /// <summary>
        /// Instance of the database
        /// </summary>
        private static QueryFactory db = new QueryFactory(Adapter.Connection, Adapter.Compiler);
        
        /// <summary>
        /// Retrieves all the game cars from the database
        /// This is mostly used as memory cache at client for
        /// card preview, store etc
        /// </summary>
        /// <returns></returns>
        public static List<DeckCard> GetAll()
        {
            List<DeckCard> deckCards = new List<DeckCard>();
            var query = db.Query("card").Get();

            if (query.ToList().Count > 0)
            {
                foreach(var card in query.ToList())
                {
                    deckCards.Add(new DeckCard
                    {
                        Id = card.id,
                        Name = card.name,
                        Description = card.description,
                        Faction = card.faction,
                        ManaCost = card.mana_cost,
                        Attack = card.attack,
                        Defense = card.defense
                    });
                }
            }
            return deckCards;
        }
    }
}