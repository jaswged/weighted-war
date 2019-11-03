using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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


    public Hand aiHand;
    public Hand playerHand;

    private Card aiBuriedCard;
    private Card playerBuriedCard;

    public void Initialize() {
        aiDeck = SpawnDeck(false);
        playerDeck = SpawnDeck(true);

        aiHand = SpawnHand(false);
        playerHand = SpawnHand(true);

        DrawPlayerHand(true, true);
        DrawPlayerHand(false, true);
    }

    void Start() {
    }

    void Update() {
    }

    private void DrawPlayerHand(bool isPlayer, bool is6CardHand) {
        var handToUse = isPlayer ? playerHand : aiHand;
        var deckToUse = isPlayer ? playerDeck : aiDeck;
        for (var i = handToUse.hand.Count; i < (is6CardHand ? 6 : 5); i++) {
            Card card = deckToUse.Draw();
            handToUse.hand.Add(card);

            // Reparent the card under the hand object.
            GameObject go = card.GetGo();
            go.transform.parent = handToUse.go.transform;
            go.transform.rotation = isPlayer ? Quaternion.Euler(-33.44f, 0f, 0f) : Quaternion.Euler(33.44f, 0f, 0f);

            // TODO Taylor fix height problem
            go.transform.position = handToUse.go.transform.position +
                                    (isPlayer
                                        ? new Vector3(-1.9f + (i * .75f), 0, 0)
                                        : new Vector3(-1.9f + (i * .75f), 0, 0));
//            go.transform.localScale -= new Vector3(.3f, .3f, 0);
        }
    }

    private Hand SpawnHand(bool isPlayer) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab,
            (isPlayer ? new Vector3(1.85f, 3.6f, 7.35f) : new Vector3(1.8f, 3.5f, .17f)),
            Quaternion.identity, playMat.transform);
        handGo.name = isPlayer ? "PlayerHand" : "OpponentHand";
        Hand hand = handGo.GetComponent<Hand>();
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
            CreateCards(Suit.Spades, isPlayer, deckGo, deck);
            CreateCards(Suit.Clubs, isPlayer, deckGo, deck); 
        }
        else {
            CreateCards(Suit.Hearts, isPlayer, deckGo, deck);
            CreateCards(Suit.Diamonds, isPlayer, deckGo, deck);  
        }
        
        deck.Shuffle();
        
        // TODO Move the cards to an iterative height
        for (var i = 0; i<deck.cards.Count; i++) {
            deck.cards[i].GetGo().transform.position = new Vector3(0, i * .017f, 0);
        }

        return deck;
    }

    private void CreateCards(Suit suit, bool isPlayer, GameObject deckGo, Deck deck) {
        for (var i = 2; i <= 14; i++) {
            var cardGo = GameObject.Instantiate(cardPrefab,
//                (isPlayer ? new Vector3(-1.891f, 2.904f - .3f, 7.281f) : new Vector3(3.87f, 0.1f - .3f, -3.8f)) +
//                new Vector3(0, i * .05f, 0),
                (isPlayer ? playerDeckPosition.transform.position : aiDeckPosition.transform.position),
                cardPrefab.transform.rotation, deckGo.transform);
            Card card = cardGo.GetComponent<Card>();
            card.Value = i;
            card.name = i + " of " + card.Suit;
            card.Suit = isPlayer ? Suit.Diamonds : Suit.Clubs;
            card.SetGo(cardGo);
            cardGo.name = i + " of " + card.Suit;

            deck.AddCard(card);
        }
    }


    public void BuryCard(GameObject movingCard, bool isPlayer) {
        var handToRemoveFrom = isPlayer ? playerHand : aiHand;

        movingCard.transform.position = isPlayer ? aiWarCard.transform.position : playerWarCard.transform.position;
        movingCard.transform.parent = (isPlayer ? aiWarCard.transform : playerWarCard.transform);

        // TODO @Taylor rotate back to flat

        var card = movingCard.GetComponent<Card>();
        if (isPlayer) {
            aiBuriedCard = card;
        }
        else {
            playerBuriedCard = card;
        }

        handToRemoveFrom.hand.Remove(card);
    }

    public void PlaceCard(GameObject movingCard, bool isPlayer) {
        var handToRemoveFrom = isPlayer ? playerHand: aiHand;

        var card = movingCard.GetComponent<Card>();

        // TODO @Taylor rotate back to flat

        movingCard.transform.position =
            (isPlayer ? aiBattlePosition.transform.position : playerBattlePosition.transform.position);
        movingCard.transform.parent = (isPlayer ? aiBattlePosition.transform : playerBattlePosition.transform);

        handToRemoveFrom.hand.Remove(card);
    }

    public void DiscardCard(GameObject blackCard, bool isPlayer) {
        var graveToUse = isPlayer ? aiGraveyard : playerGraveyard;
        blackCard.transform.position = graveToUse.transform.position;
        blackCard.transform.parent = graveToUse.transform;
    }

    public GameObject PickAiCard(bool isWarCard) {
        Debug.Log("Ai hand count: " + aiHand.hand.Count);
        Card card = aiHand.hand[Random.Range(0, aiHand.hand.Count)];
        if (isWarCard) {
            BuryCard(card.GetGo(), false);
        }
        else {
            PlaceCard(card.GetGo(), false);
        }

        return card.GetGo();
    }
}