using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : MonoBehaviour {
    private GameObject playerCard;
    private GameObject opponentCard;
    [SerializeField] private GameObject playerFieldCard;
    [SerializeField] private GameObject aiFieldCard;
    [SerializeField] private GameObject opponentWarCard;
    [SerializeField] private GameObject playerWarCard;

    private bool _hasWarBegun;
    
    void Update() {
        // Battle Happens Here
        // Animations and results.

        if (Input.GetMouseButtonDown(1)) {
            // TODO Check if set win condition
            Fight();
            
            // TODO If war just happened pick new bury card and start set over.
            GameManagement.Instance.DiscardCard(opponentCard, false);
            GameManagement.Instance.DiscardCard(playerCard, true);
            ExitState();
        }
    }

    private void Fight() {
        Debug.Log("Battle happens now!");
        
        // TODO Find the winner. 
        Card aiCard = aiFieldCard.GetComponentInChildren<Card>();
        Card playerCard = playerFieldCard.GetComponentInChildren<Card>();
        Debug.LogFormat("Player card: {0}, AI Card {1}", playerCard, aiCard);

        if (aiCard.Value == playerCard.Value) {
            Debug.LogError("WAR HAS BROKE OUT!!!!");
            _hasWarBegun = true;
            
            // Use the war card instead of normal card
            aiCard = opponentWarCard.GetComponentInChildren<Card>();
            playerCard = playerWarCard.GetComponentInChildren<Card>();
        }
        
        #warning fix battle logic here!
        int diff = Mathf.Abs(aiCard.Value - playerCard.Value);
        Debug.Log("Difference between the cards value is: " + diff);

        var range = Random.Range(1, 101);
        Debug.Log("Random Roll is: " + range);
        
        var playerHasAdvantage = playerCard.Value > aiCard.Value;
        Debug.Log("playerHasAdvantage: " + playerHasAdvantage);

        // TODO check the rules for winner here. Inclusive and such @Taylor
        var rangeToWinWith = 50 - diff * 4;
        Debug.Log("Range to win with: " + rangeToWinWith);
        var highCardWon = range > rangeToWinWith;
        Debug.Log("High Card won: " + highCardWon);
        
        // Player won the bout
        // TODO the xor is backwards. Find a way to fix this?
        var opponentWon = playerHasAdvantage ^ highCardWon;
        
        GameManagement.Instance.IncrementRoundScore(!opponentWon, _hasWarBegun);
    }

    public void EnterState(GameObject playCard, GameObject aiCard) {
        this.enabled = true;
        opponentCard = aiCard;
        this.playerCard = playCard;
    }

    public void ExitState() {
        this.enabled = false;
        GameManagement.Instance.CardSelector.EnterState();
    }
}
