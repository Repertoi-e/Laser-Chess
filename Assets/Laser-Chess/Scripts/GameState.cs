using UnityEngine;

public static class GameState {
    public static Board Board;
    public static Constants Constants;
    public static FeedbackText FeedbackText;

    private static State currentState = null;

    public static State CurrentState {
        get => currentState;
        set {
            currentState?.End();
            currentState = value;
        }
    }

    public static void Update() {
        CurrentState?.Update();
    }
}