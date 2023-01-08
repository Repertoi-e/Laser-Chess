using UnityEngine;

public class GameState : Singleton<GameState> {
    public Board board;

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

    }

    void Update() {

    }
}