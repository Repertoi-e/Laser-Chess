using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackText : MonoBehaviour {
    private TMPro.TextMeshProUGUI text;

    float elapsedTime = 1000;

    void Start() {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        GameState.FeedbackText = this;
     }

    void Update() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > GameState.Constants.kFeedbackDuration) {
            float t = (elapsedTime - GameState.Constants.kFeedbackDuration) / GameState.Constants.kFeedbackFadeOutTime;
            t *= t; // quad ease out
            text.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
        }
    }

    void SwitchText(string newText) {
        elapsedTime = 0;
        text.SetText(newText);
        text.color = Color.white;
    }

    public void DoTryingToMoveToAnOccupiedTile() {
        SwitchText("Can't move to an occupied tile.");
    }

    public void DoPieceOutOfRangeAttack() {
        SwitchText("Ops. That piece is out of attack range.");
    }
    public void DoPieceOutOfRangeMove() {
        SwitchText("Ops. That tile is out of move range.");
    }

    public void DoPieceDoesntShoot() {
        SwitchText("This piece doesn't shoot. Click the attack button to stomp all enemies around it.");
    }

    public void DoPieceOutOfActions() {
        SwitchText("This piece is out of actions.");
    }
}
