using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour {
    public GameObject cardHighlightPrefab;
    private GameObject cardHighlight;
    public Material selectedMaterial;

    public void EnterState() {
        Debug.Log(("Enter select state"));
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
                    Debug.Log("Clicked a card");
                    // todo check if is your own card
                    if (GameManagement.instance.DoesCardBelongToPlayerHand(hitGo)) {
                        Debug.Log("Clicked own card");
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
            GameManagement.instance.BuryCard(movingCard, false);
        }
        else {
            this.enabled = false;
            //TODO move the card to battle for the correct side.
            GameManagement.instance.PlaceCard(movingCard, false);
            
            GameManagement.instance._battleState.EnterState(movingCard);
        }
    }
}
