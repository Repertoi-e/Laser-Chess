using UnityEngine;

public class MenuState : State {
    public MenuState() {
        GameState.Constants.menuUI.SetActive(true);
    }

    public override void End() {
        GameState.Constants.menuUI.SetActive(false);
    }
}
