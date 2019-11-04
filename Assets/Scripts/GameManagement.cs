using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour{
    public static GameManagement instance;
    private Camera mainCamera;
    public CardSelector _cardSelector;
    public BattleState _battleState;
	
    private Player opponent;
    private Player player;
    [SerializeField]
    private PlayMat _playMat;

    public bool isBuryingCard;
    
    int playerRoundCount;
    int aiRoundCount;
    
    AudioSource sound;

    void Awake() {
        instance = this;
        mainCamera = Camera.main;
        sound = gameObject.GetComponent<AudioSource>();
    }

    void Start () {
        opponent = new Player("AI", true);
        player = new Player("Player", false);

        _playMat.Initialize();

        _cardSelector = GetComponent<CardSelector>();
        _battleState = GetComponent<BattleState>();

        _battleState.enabled = false;
        _cardSelector.enabled = true;
        isBuryingCard = true;
    }

    public bool HasAPlayerWonASet() {
        return false;
    }
    
    public bool DoesCardBelongToPlayerHand(Card card) {
        // TODO get hand of current player
        Debug.Log("Does card belong to player: " );
        return _playMat.playerHand.hand.Contains(card);
    }

    public void BuryCard(GameObject movingCard, bool isPlayer) {
        PlayCardSound();
        _playMat.BuryCard(movingCard, isPlayer);
    }

    public void PlaceCard(GameObject movingCard, bool isPlayer) {
        PlayCardSound();
        _playMat.PlaceCard(movingCard, isPlayer);
    }

    private void PlayCardSound() {
        sound.Play(0);
    }

    public void DiscardCard(GameObject card, bool isPlayer) {
        _playMat.DiscardCard(card, isPlayer);
    }

    public void IncrementRoundScore(bool winner) {
        var asdf = winner ? playerRoundCount : aiRoundCount;
    }

    public GameObject PickAiCard(bool isWarCard) {
        return _playMat.PickAiCard(isWarCard);
    }
}