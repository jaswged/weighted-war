using UnityEngine;

public class CardSelector : MonoBehaviour {
    public GameObject cardHighlightPrefab;
    public bool _cardPickedForBattle = false;
    
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
                GameObject hitGo = hit.transform.parent.gameObject;
                var cardComponent = hitGo.GetComponentInParent<Card>();
                if(cardComponent != null){
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
            GameManagement.Instance.BuryCard(movingCard, true);

            GameManagement.Instance.PickAiCard(true);
            _cardPickedForBattle = false;
            //GameManagement.Instance.RearrangeHands();
        }
        else {
            enabled = false;
            _cardPickedForBattle = true;
            GameManagement.Instance.PlaceCard(movingCard, true);
            GameObject opponentCard = GameManagement.Instance.PickAiCard(false);
            //GameManagement.Instance.RearrangeHands();
            GameManagement.Instance.BattleState.EnterState(movingCard, opponentCard);
        }
    }

    public void ExitState() {
        enabled = false;
    }
}
