using UnityEngine;

public sealed partial class GameState : MonoBehaviour {
    // This finds the global object we use to store the game state.
    // Instead of making this a singleton or a static class,
    // this has the benefit of allowing us to destroy/recreate
    // the state at will, and while switching levels, etc.
    public static GameState It {
        get {
            var objs = GameObject.FindGameObjectsWithTag("GameState");
            Debug.Assert(objs.Length == 1);
            return objs.Length == 1 ? objs[0].GetComponent<GameState>() : null;
        }
    }

    public Board Board; // set in editor
    public Constants Constants; // set in editor

    public enum State {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    private State currentState;

    public State CurrentState {
        get => currentState;
        set => currentState = value;
    }

    public enum OnTurn {
        Human,
        AI
    }

    private OnTurn onTurn;

    public OnTurn WhoIsOnTurn {
        get => onTurn;
        set => onTurn = value;
    }

    public bool IsPlayerOnTurn {
        get => currentState == State.Playing && onTurn == OnTurn.Human;
    }

    void Start() {
        CurrentState = State.Playing;
        WhoIsOnTurn = OnTurn.Human;
    }
}