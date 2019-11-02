using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayMat : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private GameObject deckPrefab;
    [SerializeField]
    private GameObject handPrefab;
    [SerializeField]
    private GameObject playMat;

    private Deck redDeck;
    private Deck blackDeck;

    private Hand redHand;
    private Hand blackHand;
    
    public void Initialize() {
        redDeck = SpawnDeck(true);
        blackDeck = SpawnDeck(false);
        
        redHand = SpawnHand(true);
        blackHand = SpawnHand(false);
        
        Debug.Log(("Draw the hand!"));
        DrawPlayerHand(true, true);
        DrawPlayerHand(false,true);
    }
    
    void Start() {
        
    }
    void Update() {
        
    } 

    private void DrawPlayerHand(bool isRed, bool is6CardHand) {
        var handToUse = isRed ? redHand : blackHand;
        var deckToUse = isRed ? redDeck : blackDeck;
        for (var i = handToUse.hand.Count; i < (is6CardHand ? 6 : 5); i++) {
            Card card = deckToUse.Draw();
            handToUse.hand.Add(card);   
            
            // Reparent the card under the hand object.
            card.go.transform.parent = handToUse.go.transform;
            // TODO Taylor fix height problem
            card.go.transform.position = handToUse.go.transform.position;
        }
    }

    private Hand SpawnHand(bool isRed) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab, (isRed ? new Vector3(-2, 0, -2) : new Vector3(5, 0, 5)),
                                Quaternion.identity, playMat.transform);
        handGo.name = isRed ? "RedHand" : "BlackHand";
        Hand hand = handGo.GetComponent<Hand>();
        hand.go = handGo;

        return hand;
    }
    private Deck SpawnDeck(bool isRed) {
        // TODO for reuse, pass in deck object?
        var deckGo = Instantiate(deckPrefab, playMat.transform);
        
        deckGo.name = isRed ? "RedDeck" : "BlackDeck";
        Deck deck = deckGo.GetComponent<Deck>();
        deck.isRed = isRed;
        deck.go = deckGo;

        // TODO spawn and shuffle the deck object first with its cards.
        // TODO then spawn the cards from a foreach off the deck, so they spawn in the shuffled order.
        
        for (int i = 2; i <= 14; i++) {
            var cardGo = GameObject.Instantiate(cardPrefab,
                (isRed ? Vector3.zero : new Vector3(2, 0, 2)) + new Vector3(0, 0, i * .01f),
                cardPrefab.transform.rotation, deckGo.transform);
            Card card = cardGo.GetComponent<Card>();
            card.value = i;
            card.suit = isRed ? Suit.Diamonds : Suit.Clubs;
            card.go = cardGo;
            cardGo.name = i + " of " + card.suit;
            
            deck.AddCard(card);
            //            cardGo.GetComponent<Material>() = redMaterial;

            // Create second card for same value
            var cardGo2 = GameObject.Instantiate(cardPrefab,
                (isRed ? Vector3.zero : new Vector3(2, 0, 2)) + new Vector3(0, 0, i * .01f),
                cardPrefab.transform.rotation, deckGo.transform);
            Card card2 = cardGo2.GetComponent<Card>();
            card2.value = i;
            card2.go = cardGo2;
            card2.suit = isRed ? Suit.Hearts : Suit.Spades;
            cardGo2.name = i + " of " + card2.suit;
//            cardGo2.GetComponent<Material>() = redMaterial;
            deck.AddCard(card2);
        }

        return deck;
    }
}
