using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour{
    public static GameManagement Instance;
    private Camera _mainCamera;
    [NonSerialized] public CardSelector CardSelector;
    [NonSerialized] public BattleState BattleState;
    [SerializeField] private PlayMat _playMat;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text opponentSetsText;
    [SerializeField] private Text playerSetsText;
    
    private Player opponent;
    private Player player;
    private string _scoreText = "     0 : 0";
    
    public bool isBuryingCard;
    
    private int _playerRoundCount;
    private int _aiRoundCount;
    private int _playerSetCount;
    private int _aiSetCount;
    
    
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

    void Update() {
        // TODO Show accurate scores for current round and sets.
        scoreText.text = _scoreText;
        opponentSetsText.text = _aiSetCount.ToString();
        playerSetsText.text = _playerSetCount.ToString();
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

    public void IncrementRoundScore(bool winnerIsPlayer, bool hasWarHappened) {
        Debug.Log(winnerIsPlayer ? "Player won the bout" : "Opponent won the bout");
        
        // TODO check if war happened and make that player the winner of the whole set!
        if (hasWarHappened) {
            Debug.Log("War has happened " + winnerIsPlayer + " wins the whole set");
            if (winnerIsPlayer) {
                _playerSetCount++;
            }
            else {
                _aiSetCount++;  
            }
            
            // TODO Check that the Game isn't over now. Perhaps put this in a method for when a set has been won.
            // TODO Make a reset method to reset everything for the next set
        }
        // Check if 3/5 has been won for the set yet!
        var gameOver = winnerIsPlayer ? _playerRoundCount++ : _aiRoundCount++;
        
        // TODO Update the text string with actual round values.
        _scoreText = string.Format("     {0} : {1}", _aiRoundCount, _playerRoundCount);
    }

    public GameObject PickAiCard(bool isWarCard) {
        return _playMat.PickAiCard(isWarCard);
    }
}