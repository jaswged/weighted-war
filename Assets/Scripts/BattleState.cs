using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : MonoBehaviour {
    private GameObject redCard;
    private GameObject blackCard;
    [SerializeField]
    private Material battleMaterial;
    
    void Update(){
        // Battle Happens Here
        
        // Animations and results.

        if (Input.GetMouseButtonDown(1)) {
            // TODO Check if set win condition
            
            // TODO If war just happened pick new bury card and start set over.
            GameManagement.instance.DiscardCard(blackCard);
            ExitState();
        }
    }

    public void EnterState(GameObject movingCard) {
        this.enabled = true;
        blackCard = movingCard;
        
        MeshRenderer renderers = movingCard.GetComponentInChildren<MeshRenderer>();
        renderers.material = battleMaterial;
        
        Debug.Log("Time to do some combat yo!");
    }

    public void ExitState() {
        Debug.Log("Exit battle state");
        this.enabled = false;
        GameManagement.instance._cardSelector.enabled = true;
    }
}
