using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour{
    public static GameManagement Instance;
    private Camera _mainCamera;
    [NonSerialized]
    public CardSelector CardSelector;
    [NonSerialized]
    public BattleState BattleState;
	
    private Player opponent;
    private Player player;
    [SerializeField]
    private PlayMat _playMat;

    public bool isBuryingCard;
    
    int playerRoundCount;
    int aiRoundCount;
    
    AudioSource _sound;

    void Awake() {
        Instance = this;
        _mainCamera = Camera.main;
        _sound = gameObject.GetComponent<AudioSource>();
    }

    void Start () {
        opponent = new Player("AI", true);
        player = new Player("Player", false);

        _playMat.Initialize();

        CardSelector = GetComponent<CardSelector>();
        BattleState = GetComponent<BattleState>();

        BattleState.enabled = false;
        CardSelector.enabled = true;
        isBuryingCard = true;
    }

    public bool HasAPlayerWonASet() {
        return false;
    }
    
    public bool DoesCardBelongToPlayerHand(Card card) {
        return _playMat.PlayerHand.hand.Contains(card);
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
        _sound.Play(0);
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