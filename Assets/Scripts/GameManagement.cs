using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour{
    public static GameManagement instance;
    private Camera mainCamera;
    public CardSelector _cardSelector;
    public BattleState _battleState;
	
    private Player red;
    private Player black;
    [SerializeField]
    private PlayMat _playMat;

    public bool isBuryingCard;

    void Awake() {
        instance = this;
        mainCamera = Camera.main;
    }

    void Start () {
        red = new Player("red", true);
        black = new Player("black", false);

        _playMat.Initialize();

        _cardSelector = GetComponent<CardSelector>();
        _battleState = GetComponent<BattleState>();

        _battleState.enabled = false;
        _cardSelector.enabled = true;
        isBuryingCard = true;
    }

    public bool DoesCardBelongToPlayerHand(GameObject cardClicked) {
        // TODO get hand of current player
        return _playMat.blackHand.hand.Contains(cardClicked.GetComponent<Card>());
    }

    public void BuryCard(GameObject movingCard, bool isRed) {
        _playMat.BuryCard(movingCard, isRed);
    }

    public void PlaceCard(GameObject movingCard, bool isRed) {
        Debug.Log("Place card");
        _playMat.PlaceCard(movingCard, isRed);
    }

    public void DiscardCard(GameObject blackCard) {
        _playMat.DiscardCard(blackCard, false);
    }
}