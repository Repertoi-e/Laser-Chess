using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
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
        set {
            if (value != State.Playing)
                CurrentPieceInteraction = null;
            currentState = value;
        }
    }

    public enum OnTurn {
        Human,
        AI
    }

    private OnTurn onTurn;

    public OnTurn WhoIsOnTurn {
        get => onTurn;
        set {
            if (value != OnTurn.Human)
                CurrentPieceInteraction = null;
            onTurn = value;
        }
    }

    public bool IsPlayerOnTurn {
        get => currentState == State.Playing && onTurn == OnTurn.Human;
    }

    void Start() {
        CurrentState = State.Playing;
        WhoIsOnTurn = OnTurn.Human;
    }

    Queue<IEnumerator> transactionsForThisFrame = new();

    public void QueueUpTransaction(Transaction t) {
        if (!t.IsValid())
            return;
        transactionsForThisFrame.Enqueue(t.Execute());
    }

    private PieceInteraction currentPieceInteraction;

    // Right now anyone anywhere can modify this but is this a good idea?
    // This should only be potentially non-null if it's the player's turn
    // and the game state is "Playing".
    public PieceInteraction CurrentPieceInteraction {
        get => currentPieceInteraction;
        set {
            currentPieceInteraction?.End();
            currentPieceInteraction = value;
        }
    }

    // The bool says whether it's clicked or not
    Dictionary<GameObject, bool> previousFrameHovers = new();
    Dictionary<GameObject, bool> frameHovers = new();

    public void OnPieceMouseEnter(Piece piece) {
        if (It.CurrentState != State.Playing)
            return;

        piece.SetTargetEmissionColor(piece.IsEnemy ? Constants.kGlowEnemy : Constants.kGlowHuman);

        CurrentPieceInteraction?.OnPieceMouseEnter(piece);
    }

    public void OnPieceMouseExit(Piece piece) {
        piece.SetTargetEmissionColor(Color.black);
        CurrentPieceInteraction?.OnPieceMouseExit(piece);
    }

    void OnGameObjectMouseEnter(GameObject obj) {
        if (obj.CompareTag("Board")) {
            CurrentPieceInteraction?.OnBoardMouseEnter();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseEnter(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            CurrentPieceInteraction?.OnTileMouseEnter(obj.GetComponent<Tile>());
        }
    }

    void OnGameObjectMouseExit(GameObject obj) {
        if (obj.CompareTag("Board")) {
            CurrentPieceInteraction?.OnBoardMouseExit();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseExit(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            CurrentPieceInteraction?.OnTileMouseExit(obj.GetComponent<Tile>());
        }
    }

    void OnPieceClicked(Piece piece) {
        if (CurrentPieceInteraction != null) {
            CurrentPieceInteraction.OnPieceClicked(piece);
            return;
        }
        
        if (IsPlayerOnTurn && !piece.IsEnemy) {
            if (piece.HasMovedThisTurn && piece.HasAttackedThisTurn) {
                // TODO: give feedback to the player
                return;
            }
            CurrentPieceInteraction = new MovePieceInteraction(piece);
        }
    }

    void OnGameObjectMouseDown(GameObject obj) {
        if (obj.CompareTag("Piece")) {
            OnPieceClicked(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            CurrentPieceInteraction?.OnTileClicked(obj.GetComponent<Tile>());
        }
    }

    void Update() {
        if (transactionsForThisFrame.Count > 0) {
            if (!transactionsForThisFrame.Peek().MoveNext()) {
                transactionsForThisFrame.Dequeue();
            }
            return; // don't do any logic while transactions are occuring
        }

        bool mousePressed = Input.GetMouseButton(0);

        bool rayHitPieceThisFrame = false;
        bool rayHitTileThisFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, raycastLayerMask, QueryTriggerInteraction.Collide);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
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
    }

    public void ShowAttackButton(Vector3 pos) {
        var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
        if (attackButton) {
            attackButton.transform.position = pos;
        }
    }

    public void HideAttackButton() {
        var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
        if (attackButton) {
            attackButton.transform.position = new Vector3(0, 0, -1000);
        }
    }
}