using UnityEngine;

public static class GameState {
    // These fields can be accessed globally from
    // anywhere and it's very convenient.
    // We can also do a game state object and tag it,
    // but that would require searching for it every time,
    // which might not be the best for performance.
    // And doing a singleton pattern would basically accomplish
    // what we have now as a static class, but with more bureaucracy.
    // That's why we roll with this.
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