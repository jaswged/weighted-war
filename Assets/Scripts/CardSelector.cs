using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour {
    public GameObject cardHighlightPrefab;
    public Material selectedMaterial;
    private bool _cardPickedForBattle = false;
    
    public void EnterState() {
        _cardPickedForBattle = false;
        enabled = true;
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            // Did the user click
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("Mouse clicked");
                GameObject hitGo = hit.transform.gameObject;
                var cardComponent = hitGo.GetComponentInParent<Card>();
                Debug.Log("Card comp: " + cardComponent);
                Debug.Log(cardComponent);
                if(cardComponent != null){
                    Debug.Log("Clicked on a card");
                    if (!_cardPickedForBattle && GameManagement.Instance.DoesCardBelongToPlayerHand(cardComponent)) {
                        _cardPickedForBattle = true;
                        ExitState(hitGo);
                    }
                }
            }
        }
    }
    
    private void ExitState(GameObject movingCard) {
        if (GameManagement.Instance.isBuryingCard) {
            GameManagement.Instance.isBuryingCard = false;
            // TODO figure out how to pass which player.
            GameManagement.Instance.BuryCard(movingCard, true);

            GameManagement.Instance.PickAiCard(true);
        }
        else {
            this.enabled = false;
            //TODO move the card to battle for the correct side.
            GameManagement.Instance.PlaceCard(movingCard, true);
            GameObject opponentCard = GameManagement.Instance.PickAiCard(false);
            GameManagement.Instance.BattleState.EnterState(movingCard, opponentCard);
            
        }
    }
}
