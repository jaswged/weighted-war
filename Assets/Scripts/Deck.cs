using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {

    private List<Card> deck;
    private static Random random = new Random();
    
    public void Shuffle() {
        var count = deck.Count;
        while (count > 1) {
            count--;
            var randomNext = Random.Range(0, count);
            var card = deck[randomNext];
            deck[randomNext] = deck[count];
            deck[count] = card;
        }
    }

    public Card Draw() {
        Card drawnCard = deck[0];
        deck.Remove(drawnCard);
        return drawnCard;
    }
    
    
}
