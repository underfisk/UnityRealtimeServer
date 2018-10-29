using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UServer.GameEntity;

namespace UServer.Database
{
    public static class PlayerDeckCards
    {
        private static QueryFactory db = new QueryFactory(Adapter.Connection, Adapter.Compiler);
        private static readonly string card_table = "card",
            deck_cards_table = "deck_cards", 
            user_decks_table = "user_decks";

        /// <summary>
        /// Retrieves all the player decks and by default with all the cards included
        /// </summary>
        /// <param name="cardsIncluded"></param>
        /// <returns></returns>
        public static List<PlayerDeck> GetPlayerDecks(uint playerId)
        {
            List<PlayerDeck> playerDecks = new List<PlayerDeck>();
            var decks = db.Query(user_decks_table)
                .Where("user_id", playerId)
                .Get();

            if (decks.ToList().Count > 0)
            {
                
                foreach (var deck in decks.ToList())
                {
                    Console.WriteLine("DECK = " + deck);
                    playerDecks.Add(new PlayerDeck
                    {
                        Id = deck.id,
                        Name = deck.name,
                        Faction = deck.faction,
                        PlayerId = deck.user_id
                    });
                }

                if (playerDecks.Count > 0 )
                {
                    Console.WriteLine("Deck size = " + playerDecks.Count);
                    foreach(var deck in playerDecks)
                    {
                        Console.WriteLine("Getting deck id cards " + deck.Id);
                        var cards = GetDeckCards(deck.Id);
                        Console.WriteLine(cards.ToString());
                        deck.deckCards = cards;
                    }
                }
            }

            
            return playerDecks;
        }

        public static List<DeckCard> GetDeckCards(uint deckId)
        {
            List<DeckCard> deckCards = new List<DeckCard>();
            var query = db.Query("deck_cards")
                .Select("card.*")
                .Join("card", "deck_cards.card_id", "card.id")
                .Where("card_id", deckId)
                .Get();

            if (query.ToList().Count > 0)
            {
                Console.WriteLine("Cards size = " + query.ToList().Count);
                foreach(var card in query.ToList())
                {
                    Console.WriteLine(card);

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
