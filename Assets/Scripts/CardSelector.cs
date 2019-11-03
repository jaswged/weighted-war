using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour {
    public GameObject cardHighlightPrefab;
    public Material selectedMaterial;
    private bool cardPickedForBattle = false;
    public void EnterState() {
        cardPickedForBattle = false;
        enabled = true;
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            // Did the user click
            if (Input.GetMouseButtonDown(0)) {
                GameObject hitGo = hit.transform.gameObject;
                if(hitGo.GetComponent<Card>() != null){
                    // todo check if is your own card
                    if (!cardPickedForBattle && GameManagement.instance.DoesCardBelongToPlayerHand(hitGo)) {
                        cardPickedForBattle = true;
                        ExitState(hitGo);
                    }
                }
            }
        }
    }
    
    private void ExitState(GameObject movingCard) {
        if (GameManagement.instance.isBuryingCard) {
            GameManagement.instance.isBuryingCard = false;
            // TODO figure out how to pass which player.
            GameManagement.instance.BuryCard(movingCard, true);

            GameManagement.instance.PickAiCard(true);
        }
        else {
            this.enabled = false;
            //TODO move the card to battle for the correct side.
            GameManagement.instance.PlaceCard(movingCard, true);
            GameObject opponentCard = GameManagement.instance.PickAiCard(false);
            GameManagement.instance._battleState.EnterState(movingCard, opponentCard);
            
        }
    }
}
