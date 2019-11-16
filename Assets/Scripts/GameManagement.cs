using System;
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
    [SerializeField] private Text gameOverText;
    
    private Player opponent;
    private Player player;
    private string _scoreText = "     0 : 0";
    
    public bool isBuryingCard;
    
    private int _playerRoundCount;
    private int _aiRoundCount;
    private int _playerSetCount;
    private int _aiSetCount;
    private bool isGameOver;
    
    
    AudioSource _sound;

    private void Awake() {
        Instance = this;
        _mainCamera = Camera.main;
        _sound = gameObject.GetComponent<AudioSource>();
        
        gameOverText.enabled = false;
    }

    private void Start () {
        opponent = new Player("AI", true);
        player = new Player("Player", false);

        _playMat.Initialize();

        CardSelector = GetComponent<CardSelector>();
        BattleState = GetComponent<BattleState>();

        BattleState.enabled = false;
        CardSelector.enabled = true;
        isBuryingCard = true;
    }

    private void Update() {
        scoreText.text = _scoreText;
        opponentSetsText.text = _aiSetCount.ToString();
        playerSetsText.text = _playerSetCount.ToString();

        if (!isGameOver) return;
        
        gameOverText.enabled = true;
        CardSelector.enabled = false;
        BattleState.enabled = false;
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
        _playMat.PlaceCard(movingCard, isPlayer, false);
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

            if (!GameOver()) {
                ResetMatch(true);
            }
        }
        
        // move these 3 lines down below set over check
        var temp = winnerIsPlayer ? _playerRoundCount++ : _aiRoundCount++;
        UpdateScoreText();
        Debug.Log($"Print out round counts     {_aiRoundCount} : {_playerRoundCount}");  
        
        // TODO Check that the set hasn't been won
        // Check if 3/5 has been won for the set yet!
        if (_playerRoundCount != 3 && _aiRoundCount != 3) return;
        
        Debug.Log($"The set is over! {winnerIsPlayer} has won the SET!");
        if (winnerIsPlayer) {
            _playerSetCount++;
        }
        else {
            _aiSetCount++;
        }

        if (!GameOver()) {
            //TODO Call the set Reset Method if game isn't over. Just like above
            ResetMatch(false);
        }
    }

    private void UpdateScoreText() {
        _scoreText = $"     {_aiRoundCount} : {_playerRoundCount}";
    }

    private bool GameOver() {
        isGameOver = _aiSetCount == 3 || _playerSetCount == 3;
        return isGameOver;
    }

    private void ResetMatch(bool hasWarHappened) {
        _playerRoundCount = 0;
        _aiRoundCount = 0;

        UpdateScoreText();
        isBuryingCard = hasWarHappened;

        // Reset the hands.
        _playMat.DrawPlayerHand(true, isBuryingCard);
        _playMat.DrawPlayerHand(false, isBuryingCard);

        _playMat.RearrangeHands();
    }

    public GameObject PickAiCard(bool isWarCard) {
        return _playMat.PickAiCard(isWarCard);
    }

    public void MoveWarCardsIntoBattle(Card playerCard, Card aiCard) {
        _playMat.MoveWarCardsIntoBattle(playerCard, aiCard);
    }

    public void RearrangeHands() {
        _playMat.RearrangeHands();
    }
}