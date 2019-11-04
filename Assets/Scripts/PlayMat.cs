using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayMat : MonoBehaviour {
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private GameObject playMat;

    [SerializeField] private GameObject aiGraveyard;
    [SerializeField] private GameObject playerGraveyard;
    [SerializeField] private GameObject aiWarCard;
    [SerializeField] private GameObject playerWarCard;
    [SerializeField] private GameObject aiBattlePosition;
    [SerializeField] private GameObject playerBattlePosition;
    [SerializeField] private GameObject aiDeckPosition;
    [SerializeField] private GameObject playerDeckPosition;
    
    private Deck aiDeck { get; set; }
    private Deck playerDeck { get; set; }

    private Hand AiHand;
    [NonSerialized]
    public Hand PlayerHand;

    private Card aiBuriedCard;
    private Card playerBuriedCard;

    public void Initialize() {
        aiDeck = SpawnDeck(false);
        playerDeck = SpawnDeck(true);

        AiHand = SpawnHand(false);
        PlayerHand = SpawnHand(true);

        DrawPlayerHand(true, true);
        DrawPlayerHand(false, true);
    }

    private void DrawPlayerHand(bool isPlayer, bool is6CardHand) {
        var handToUse = isPlayer ? PlayerHand : AiHand;
        var deckToUse = isPlayer ? playerDeck : aiDeck;
        for (var i = handToUse.hand.Count; i < (is6CardHand ? 6 : 5); i++) {
            Card card = deckToUse.Draw();
            handToUse.hand.Add(card);

            // Reparent the card under the hand object.
            GameObject go = card.GetGo();
            go.transform.parent = handToUse.go.transform;
            go.transform.rotation = isPlayer ? 
                Quaternion.Euler(-133f, 0f, 0f) : 
                Quaternion.Euler(133f, 0f, 0f);

            // TODO Taylor fix height problem
            go.transform.position = handToUse.go.transform.position +
                                    (isPlayer
                                        ? new Vector3(-1.9f + (i * .75f), 0, 0)
                                        : new Vector3(-1.9f + (i * .75f), 0, 0));
        }
    }

    private Hand SpawnHand(bool isPlayer) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab,
            // TODO Should this have a spawn point instead of hard coded values? @Taylor
            (isPlayer ? new Vector3(1.85f, 3.6f, 7.35f) : new Vector3(1.8f, 3.5f, .17f)),
            Quaternion.identity, playMat.transform);
        handGo.name = isPlayer ? "PlayerHand" : "OpponentHand";
        var hand = handGo.GetComponent<Hand>();
        hand.go = handGo;

        return hand;
    }

    private Deck SpawnDeck(bool isPlayer) {
        var deckGo = Instantiate(deckPrefab,
            (isPlayer ? playerDeckPosition.transform.position : aiDeckPosition.transform.position),
            playerDeckPosition.transform.rotation,
            (isPlayer ? playerDeckPosition.transform : aiDeckPosition.transform));

        deckGo.name = isPlayer ? "PlayerDeck" : "OpponentDeck";
        Deck deck = deckGo.GetComponent<Deck>();
        deck.IsPlayer = isPlayer;
        deck.go = deckGo;
        
        if (isPlayer) {
            CreateCards(Suit.Spades, true, deckGo, deck);
            CreateCards(Suit.Clubs, true, deckGo, deck); 
        }
        else {
            CreateCards(Suit.Hearts, false, deckGo, deck);
            CreateCards(Suit.Diamonds, false, deckGo, deck);  
        }
        
        deck.Shuffle();
        
        // TODO Move the cards to an iterative height
        for (var i = 0; i<deck.cards.Count; i++) {
            deck.cards[i].GetGo().transform.position = new Vector3(0, i * .017f, 0);
        }

        return deck;
    }

    private void CreateCards(Suit suit, bool isPlayer, GameObject deckGo, Deck deck) {
        Debug.Log("Create cards for suit" + suit);
        for (var i = 2; i <= 14; i++) {
            string stringValue = "Prefabs/"+ suit +"_"+i;
            Debug.Log("Suit as a string: " + stringValue);
            
            
            var prefabToUse = Resources.Load<GameObject>(stringValue);
            var cardGo = GameObject.Instantiate(cardPrefab,
                (isPlayer ? playerDeckPosition.transform.position : aiDeckPosition.transform.position),
                cardPrefab.transform.rotation, deckGo.transform);
            
            // Instantiate Model for Card
            var model = Instantiate(prefabToUse, cardGo.transform.position, cardPrefab.transform.rotation * Quaternion.Euler(180,0,0), cardGo.transform);

            Card card = cardGo.GetComponent<Card>();
            card.Value = i;
            card.name = i + " of " + card.Suit;
            card.Suit = suit;
            card.SetGo(cardGo);
            cardGo.name = i + " of " + card.Suit;

            deck.AddCard(card);
        }
    }


    public void BuryCard(GameObject movingCard, bool isPlayer) {
        var handToRemoveFrom = isPlayer ? PlayerHand : AiHand;

        movingCard.transform.position = isPlayer ? playerWarCard.transform.position : aiWarCard.transform.position;
        movingCard.transform.parent = (isPlayer ? playerWarCard.transform : aiWarCard.transform);

        // TODO @Taylor rotate back to flat

        var card = movingCard.GetComponent<Card>();
        if (isPlayer) {
            playerBuriedCard = card;
        }
        else {
            aiBuriedCard = card;
        }

        handToRemoveFrom.hand.Remove(card);
    }

    public void PlaceCard(GameObject movingCard, bool isPlayer) {
        var handToRemoveFrom = isPlayer ? PlayerHand: AiHand;

        var card = movingCard.GetComponent<Card>();

        // TODO @Taylor rotate back to flat

        movingCard.transform.position =
            (isPlayer ? playerBattlePosition.transform.position : aiBattlePosition.transform.position);
        movingCard.transform.parent = (isPlayer ? playerBattlePosition.transform : aiBattlePosition.transform);

        handToRemoveFrom.hand.Remove(card);
    }

    public void DiscardCard(GameObject blackCard, bool isPlayer) {
        var graveToUse = isPlayer ?playerGraveyard : aiGraveyard  ;
        blackCard.transform.position = graveToUse.transform.position;
        blackCard.transform.parent = graveToUse.transform;
    }

    public GameObject PickAiCard(bool isWarCard) {
        Debug.Log("Ai hand count: " + AiHand.hand.Count);
        Card card = AiHand.hand[Random.Range(0, AiHand.hand.Count)];
        if (isWarCard) {
            BuryCard(card.GetGo(), false);
        }
        else {
            PlaceCard(card.GetGo(), false);
        }

        return card.GetGo();
    }
}