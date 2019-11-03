using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayMat : MonoBehaviour {
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private GameObject deckPrefab;
    [SerializeField]
    private GameObject handPrefab;
    [SerializeField]
    private GameObject playMat;

    [SerializeField]
    private GameObject aiGraveyard;
    [SerializeField]
    private GameObject playerGraveyard;
    [SerializeField]
    private GameObject aiWarCard;
    [SerializeField]
    private GameObject playerWarCard;
    [SerializeField]
    private GameObject aiBattlePosition;
    [SerializeField]
    private GameObject playerBattlePosition;
    
    public Deck aiDeck;
    public Deck playerDeck;

    public Hand aiHand;
    public Hand playerHand;

    private Card aiBuriedCard;
    private Card playerBuriedCard;
    
    public void Initialize() {
        aiDeck = SpawnDeck(true);
        playerDeck = SpawnDeck(false);
        
        aiHand = SpawnHand(true);
        playerHand = SpawnHand(false);
        
        DrawPlayerHand(true, true);
        DrawPlayerHand(false,true);
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
            go.transform.rotation = isPlayer ? Quaternion.Euler(-33.44f, 0f,0f) : Quaternion.Euler(33.44f, 0f,0f) ; 
            // TODO Taylor fix height problem
            go.transform.position = handToUse.go.transform.position + 
                                         (isPlayer ? new Vector3(-1.9f+(i*.75f), 0,0) : 
                                             new Vector3(-1.9f+(i*.75f), 0,0));
            go.transform.localScale -= new Vector3(.3f, .3f, 0);
        }
    }

    private Hand SpawnHand(bool isPlayer) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab, (isPlayer ? new Vector3(0, .61f, -4.5f) : new Vector3(0, .91f, 3f)),  
            Quaternion.identity, playMat.transform);
        handGo.name = isPlayer ? "PlayerHand" : "OpponentHand";
        Hand hand = handGo.GetComponent<Hand>();
        hand.go = handGo;

        return hand;
    }

    private Deck SpawnDeck(bool isPlayer) {
        // TODO for reuse, pass in deck object?
        var deckGo = Instantiate(deckPrefab, playMat.transform);
        
        deckGo.name = isPlayer ? "PlayerDeck" : "OpponentDeck";
        Deck deck = deckGo.GetComponent<Deck>();
        deck.isPlayer = isPlayer;
        deck.go = deckGo;

        // TODO spawn and shuffle the deck object first with its cards.
        // TODO then spawn the cards from a foreach off the deck, so they spawn in the shuffled order.
        
        for (int i = 2; i <= 14; i++) {
            var cardGo = GameObject.Instantiate(cardPrefab,
                    (isPlayer ? new Vector3(-4.01f, 0.1f-.3f, 3.5f) : new Vector3(3.87f, 0.1f-.3f, -3.8f)) + new Vector3(0, i * .05f, 0),
                    cardPrefab.transform.rotation, deckGo.transform);
            Card card = cardGo.GetComponent<Card>();
            card.value = i;
            card.suit = isPlayer ? Suit.Diamonds : Suit.Clubs;
            card.SetGo(cardGo);
            cardGo.name = i + " of " + card.suit;
            
            deck.AddCard(card);
            //            cardGo.GetComponent<Material>() = redMaterial;

            // Create second card for same value
            var cardGo2 = GameObject.Instantiate(cardPrefab,
                (isPlayer ? new Vector3(3.87f, 0.1f, -3.8f) : new Vector3(-4.01f, 0.1f, 3.5f)) + new Vector3(0, cardGo.transform.position.y + .025f-0.09f, 0),
                cardPrefab.transform.rotation, deckGo.transform);
            Card card2 = cardGo2.GetComponent<Card>();
            card2.value = i;
            card2.SetGo(cardGo2);
            card2.suit = isPlayer ? Suit.Hearts : Suit.Spades;
            cardGo2.name = i + " of " + card2.suit;
//            cardGo2.GetComponent<Material>() = redMaterial;
            deck.AddCard(card2);
        }

        return deck;
    }

    public void BuryCard(GameObject movingCard, bool isPlayer) {
        var handToRemoveFrom = isPlayer ? aiHand : playerHand;
        
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
        var handToRemoveFrom = isPlayer ? aiHand : playerHand;
        
        var card = movingCard.GetComponent<Card>();
        // TODO @Taylor rotate back to flat

            movingCard.transform.position = (isPlayer ? aiBattlePosition.transform.position: playerBattlePosition.transform.position);
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
