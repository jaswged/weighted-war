using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : MonoBehaviour {
    private GameObject playerCard;
    private GameObject opponentCard;
    [SerializeField]
    private GameObject playerFieldCard;
    [SerializeField]
    private GameObject aiFieldCard;
    
    void Update(){
        // Battle Happens Here
        // Animations and results.

        if (Input.GetMouseButtonDown(1)) {
            // TODO Check if set win condition
            //Fight();
            
            // TODO If war just happened pick new bury card and start set over.
            GameManagement.Instance.DiscardCard(opponentCard, false);
            GameManagement.Instance.DiscardCard(playerCard, true);
            ExitState();
        }
    }

    private void Fight() {
        // TODO Find the winner. 
        Card aiCard = aiFieldCard.GetComponentInChildren<Card>();
        Card playerCard = playerFieldCard.GetComponentInChildren<Card>();
        Debug.LogFormat("Player card: {0}, AI Card {1}", playerCard.Value, aiCard.Value);

        if (aiCard.Value == playerCard.Value) {
            Debug.LogError("WAR HAS BROKE OUT!!!!");
        }
        else {
            var winner = aiCard.Value > playerCard.Value;
            GameManagement.Instance.IncrementRoundScore(winner);
        }
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
