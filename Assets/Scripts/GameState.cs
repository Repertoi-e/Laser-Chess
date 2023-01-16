using System.Collections.Generic;
using UnityEngine;

public sealed partial class GameState : MonoBehaviour {
    public LayerMask raycastLayerMask;

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

    public Queue<Transaction> transactionsForThisFrame = new();

    // The bool says whether it's clicked or not
    Dictionary<GameObject, bool> previousFrameHovers = new();
    Dictionary<GameObject, bool> frameHovers = new();

    void OnGameObjectMouseEnter(GameObject obj) {
        if (obj.CompareTag("Board")) {
            OnBoardMouseEnter();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseEnter(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            OnTileMouseEnter(obj.GetComponent<Tile>());
        }
    }

    void OnGameObjectMouseExit(GameObject obj) {
        if (obj.CompareTag("Board")) {
            OnBoardMouseExit();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseExit(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            OnTileMouseExit(obj.GetComponent<Tile>());
        }
    }

    void OnGameObjectMouseDown(GameObject obj) {
        if (obj.CompareTag("Piece")) {
            OnPieceClicked(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            OnTileClicked(obj.GetComponent<Tile>());
        }
    }

    void Update() {
        bool mousePressed = Input.GetMouseButton(0);

        bool rayHitPieceThisFrame = false;
        bool rayHitTileThisFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, raycastLayerMask, QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits) {
            var obj = hit.collider.gameObject;

            if (obj.layer == LayerMask.NameToLayer("UI")) {
                frameHovers.Clear();
                break; // Bail. Act as if nothing is pressed/hovered this frame, i.e. everything is released and unhovered.
            } else if (obj.CompareTag("Piece")) {
                if (rayHitPieceThisFrame) // here we make sure we stop at the first piece hit so no multiple pieces are ever registered
                    continue;
                rayHitPieceThisFrame = true;
            } else if (obj.GetComponent<Tile>()) {
                if (rayHitPieceThisFrame || rayHitTileThisFrame) // .. also for tiles + avoid hitting a tile if a piece is hit
                    continue;
                rayHitTileThisFrame = true;
            }

            frameHovers.Add(obj, mousePressed);
        }

        foreach (var entry in previousFrameHovers) {
            bool objClicked;
            if (!frameHovers.TryGetValue(entry.Key, out objClicked)) {
                // Not hovered this frame, also means it's not clicked anymore
                OnGameObjectMouseExit(entry.Key);
            } else {
                // Still hovered
                if (mousePressed != entry.Value) {
                    if (mousePressed) {
                        // Clicked
                        OnGameObjectMouseDown(entry.Key);
                    } else {
                        // Released
                    }
                }
            }
        }

        foreach (var entry in frameHovers) {
            bool objClicked;
            if (!previousFrameHovers.TryGetValue(entry.Key, out objClicked)) {
                // Hovered this frame
                OnGameObjectMouseEnter(entry.Key);
                if (entry.Value) {
                    // Also clicked this frame, somehow, maybe user is very fast
                    OnGameObjectMouseDown(entry.Key);
                }
            }
        }

        previousFrameHovers = new(frameHovers);
        frameHovers.Clear();

        foreach (var transaction in transactionsForThisFrame) {
            if (transaction.IsValid()) {
                transaction.Execute();
            }
        }
        transactionsForThisFrame.Clear();
    }
}