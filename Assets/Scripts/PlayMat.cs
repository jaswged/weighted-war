using System;
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
    [SerializeField] private GameObject aiHandPosition;
    [SerializeField] private GameObject playerHandPosition;
    
    private Deck aiDeck { get; set; }
    private Deck playerDeck { get; set; }

    private Hand AiHand;
    [NonSerialized] public Hand PlayerHand;

    private Card aiBuriedCard;
    private Card playerBuriedCard;

    public void Initialize() {
        aiDeck = SpawnDeck(false);
        playerDeck = SpawnDeck(true);

        AiHand = SpawnHand(false);
        PlayerHand = SpawnHand(true);

        DrawPlayerHand(true, true);
        DrawPlayerHand(false, true);
        
        RearrangeHands();
    }
    
    public void DrawPlayerHand(bool isPlayer, bool is6CardHand) {
        //TODO Draw cards not on top of each other
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
        }
    }

    private Hand SpawnHand(bool isPlayer) {
        // Spawn Hand
        var handGo = Instantiate(handPrefab,

            // TODO Should this have a spawn point instead of hard coded values? @Taylor
            (isPlayer ? new Vector3(1.85f, 3.6f, 7.35f) : new Vector3(1.8f, 3.5f, .17f)),

            //(isPlayer ? playerHandPosition.transform.position: aiHandPosition.transform.position),
            Quaternion.identity, playMat.transform);
            //(isPlayer ? playerHandPosition.transform: aiHandPosition.transform));
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
        var deck = deckGo.GetComponent<Deck>();
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
        var deckPos = deckGo.transform.position;
        for (var i = 0; i<deck.cards.Count; i++) {
            deck.cards[i].GetGo().transform.position = new Vector3(deckPos.x, deckPos.y + (i*.005f), deckPos.z);
        }

        return deck;
    }

    private void CreateCards(Suit suit, bool isPlayer, GameObject deckGo, Deck deck) {
        var prefabRot = cardPrefab.transform.rotation;
        for (var i = 2; i <= 14; i++) {
            var cardGo = GameObject.Instantiate(cardPrefab,
                (isPlayer ? playerDeckPosition.transform.position : aiDeckPosition.transform.position),
                prefabRot, deckGo.transform);
            
            // Instantiate Model for Card
            var stringValue = "Prefabs/"+ suit + "_" + i;
            var prefabToUse = Resources.Load<GameObject>(stringValue);
            var model = Instantiate(prefabToUse, cardGo.transform.position, prefabRot * Quaternion.Euler(180,0,0), cardGo.transform);

            var card = cardGo.GetComponent<Card>();
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
        movingCard.transform.rotation = isPlayer ? Quaternion.Euler(180,0,180) : Quaternion.identity;
        movingCard.transform.parent = (isPlayer ? playerWarCard.transform : aiWarCard.transform);

        var card = movingCard.GetComponent<Card>();
        if (isPlayer) {
            playerBuriedCard = card;
        }
        else {
            aiBuriedCard = card;
        }

        handToRemoveFrom.hand.Remove(card);
    }

    public void PlaceCard(GameObject movingCard, bool isPlayer, bool isWarCard) {
        if (isWarCard) {
            Debug.Log("Is place war car");
            //#warning do i need to explicitly remove the war card?
            DiscardCard(isPlayer ? playerBattlePosition.transform.GetChild(0).gameObject 
                                 : aiBattlePosition.transform.GetChild(0).gameObject, isPlayer);
        }
        else {
            Debug.Log("Remove card from hand.");
            var card = movingCard.GetComponent<Card>();
            var handToRemoveFrom = isPlayer ? PlayerHand: AiHand;
            handToRemoveFrom.hand.Remove(card);
        }
        
        movingCard.transform.position =
            (isPlayer ? playerBattlePosition.transform.position : aiBattlePosition.transform.position);
        movingCard.transform.rotation = Quaternion.Euler(180, 0, 0);
        movingCard.transform.parent = (isPlayer ? playerBattlePosition.transform : aiBattlePosition.transform);
    }

    public void DiscardCard(GameObject discardCard, bool isPlayer) {
        var graveToUse = isPlayer ? playerGraveyard : aiGraveyard;
        var discardCardTransform = discardCard.transform;
        discardCardTransform.position = graveToUse.transform.position;
        discardCardTransform.parent = graveToUse.transform;
    }

    public GameObject PickAiCard(bool isWarCard) {
        var card = AiHand.hand[Random.Range(0, AiHand.hand.Count)];
        if (isWarCard) {
            BuryCard(card.GetGo(), false);
        }
        else {
            PlaceCard(card.GetGo(), false, false);
        }

        return card.GetGo();
    }

    public void MoveWarCardsIntoBattle(Card playerCard, Card aiCard) {
        #warning Move the war cards
        PlaceCard(playerCard.GetGo(), true, true);
        PlaceCard(aiCard.GetGo(), false, true);
    }
    
    public void RearrangeHands() {
        for (var i = 0; i < PlayerHand.hand.Count; i++) {
            PlayerHand.hand[i].gameObject.transform.position = playerHandPosition.transform.position + new Vector3(-1.9f + (i * .96f), 0, 0);
            AiHand.hand[i].gameObject.transform.position = aiHandPosition.transform.position + new Vector3(-1.9f + (i * .96f), 0, 0);
        }
    }
}