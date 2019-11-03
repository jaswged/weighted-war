using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayMat : MonoBehaviour {
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private GameObject playMat;

    [SerializeField] private GameObject redGraveyard;
    [SerializeField] private GameObject blackGraveyard;
    [SerializeField] private GameObject redBuryPit;
    [SerializeField] private GameObject blackberryPit;
    [SerializeField] private GameObject redBattlePosition;
    [SerializeField] private GameObject blackBattlePosition;

    public Deck redDeck;
    public Deck blackDeck;

    public Hand redHand;
    public Hand blackHand;

    private Card redBuriedCard;
    private Card blackberryCard;

    public void Initialize() {
        redDeck = SpawnDeck(true);
        blackDeck = SpawnDeck(false);

        Debug.Log(("Draw the hand!"));
        redHand = SpawnHand(true);
        blackHand = SpawnHand(false);

        DrawPlayerHand(true, true);
        DrawPlayerHand(false, true);
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
            card.go.transform.rotation = isRed ? Quaternion.Euler(-33.44f, 0f, 0f) : Quaternion.Euler(33.44f, 0f, 0f);

            // TODO Taylor fix height problem
            card.go.transform.position = handToUse.go.transform.position +
                                         (isRed
                                             ? new Vector3(-1.9f + (i * .75f), 0, 0)
                                             : new Vector3(-1.9f + (i * .75f), 0, 0));
            card.go.transform.localScale -= new Vector3(.3f, .3f, 0);
        }
    }

    private Hand SpawnHand(bool isRed) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab, (isRed ? new Vector3(0, .91f, 3f) : new Vector3(0, .61f, -4.5f)),
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
                (isRed ? new Vector3(-4.01f, 0.1f - .3f, 3.5f) : new Vector3(3.87f, 0.1f - .3f, -3.8f)) +
                new Vector3(0, i * .05f, 0),
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
                (isRed ? new Vector3(-4.01f, 0.1f, 3.5f) : new Vector3(3.87f, 0.1f, -3.8f)) +
                new Vector3(0, cardGo.transform.position.y + .025f - 0.09f, 0),
                cardPrefab.transform.rotation, deckGo.transform);
            Card card2 = cardGo2.GetComponent<Card>();
            card2.value = i;
            card2.go = cardGo2;
            card2.go = cardGo2;
            card2.suit = isRed ? Suit.Hearts : Suit.Spades;
            cardGo2.name = i + " of " + card2.suit;

//            cardGo2.GetComponent<Material>() = redMaterial;
            deck.AddCard(card2);
        }

        return deck;
    }

    public void BuryCard(GameObject movingCard, bool isRed) {
        var handToRemoveFrom = isRed ? redHand : blackHand;

        movingCard.transform.position = isRed ? redBuryPit.transform.position : blackberryPit.transform.position;
        movingCard.transform.parent = (isRed ? redBuryPit.transform : blackberryPit.transform);

        // TODO @Taylor rotate back to flat

        var card = movingCard.GetComponent<Card>();
        if (isRed) {
            redBuriedCard = card;
        }
        else {
            blackberryCard = card;
        }

        handToRemoveFrom.hand.Remove(card);
    }

    public void PlaceCard(GameObject movingCard, bool isRed) {
        var handToRemoveFrom = isRed ? redHand : blackHand;

        var card = movingCard.GetComponent<Card>();

        // TODO @Taylor rotate back to flat

        movingCard.transform.position =
            (isRed ? redBattlePosition.transform.position : blackBattlePosition.transform.position);
        movingCard.transform.parent = (isRed ? redBattlePosition.transform : blackBattlePosition.transform);

        handToRemoveFrom.hand.Remove(card);
    }

    public void DiscardCard(GameObject blackCard, bool isRed) {
        var graveToUse = isRed ? redGraveyard : blackGraveyard;
        blackCard.transform.position = graveToUse.transform.position;
        blackCard.transform.parent = graveToUse.transform;
    }
}