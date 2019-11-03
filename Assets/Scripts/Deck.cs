using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour {
    public bool IsPlayer { get; set; }
    [NonSerialized]public List<Card> cards = new List<Card>();
    public GameObject go;

    public void Shuffle() {
        var count = cards.Count;
        while (count > 1) {
            count--;
            var randomNext = Random.Range(0, count);
            var card = cards[randomNext];
            cards[randomNext] = cards[count];
            cards[count] = card;
        }
    }

    public Card Draw() {
        Card drawnCard = cards[0];
        cards.Remove(drawnCard);
        return drawnCard;
    }

    public void AddCard(Card card) {
        cards.Add(card);
    }

    public void CreateDeck() {
        if (IsPlayer) {
           CreateCards(Suit.Spades);
           CreateCards(Suit.Clubs); 
        }
        else {
            CreateCards(Suit.Hearts);
            CreateCards(Suit.Diamonds);  
        }
    }

    private void CreateCards(Suit suit) {
        for (int i = 2; i <= 14; i++) {
            var card = new Card();
            card.Value = i;
            card.Suit = suit;
            card.Name = i + " of " + card.Suit;
            AddCard(card);
        }
    }

    public void RemoveNonGoCards() {
        foreach (var card in cards) {
            if(card.GetGo() == null) cards.Remove(card);
        }
    }
}