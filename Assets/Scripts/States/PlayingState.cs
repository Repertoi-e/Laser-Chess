using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : State {
    private Turn turn;

    public Turn Turn {
        get => turn;
        set {
            turn?.End();
            turn = value;
        }
    }

    // The bool says whether the object is clicked or not.
    // This book keeping is necessary to dispatch the events below.
    Dictionary<GameObject, bool> previousFrameHovers = new();
    Dictionary<GameObject, bool> frameHovers = new();

    public void OnPieceMouseEnter(Piece piece) {
        piece.SetTargetEmissionColor(piece.IsEnemy ? GameState.Constants.kGlowEnemy : GameState.Constants.kGlowHuman);
        Turn?.OnPieceMouseEnter(piece);
    }

    public void OnPieceMouseExit(Piece piece) {
        piece.SetTargetEmissionColor(Color.black);
        Turn?.OnPieceMouseExit(piece);
    }

    void OnGameObjectMouseEnter(GameObject obj) {
        if (obj.CompareTag("Board")) {
            Turn?.OnBoardMouseEnter();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseEnter(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            Turn?.OnTileMouseEnter(obj.GetComponent<Tile>());
        }
    }

    void OnGameObjectMouseExit(GameObject obj) {
        if (obj.CompareTag("Board")) {
            Turn?.OnBoardMouseExit();
        } else if (obj.CompareTag("Piece")) {
            OnPieceMouseExit(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            Turn?.OnTileMouseExit(obj.GetComponent<Tile>());
        }
    }

    void OnGameObjectMouseDown(GameObject obj) {
        if (obj.CompareTag("Piece")) {
            Turn?.OnPieceClicked(obj.GetComponent<Piece>());
        } else if (obj.GetComponent<Tile>()) {
            Turn?.OnTileClicked(obj.GetComponent<Tile>());
        }
    }

    Queue<IEnumerator> transactionsForThisFrame = new();

    public void QueueUpValidTransaction(Transaction t) {
        transactionsForThisFrame.Enqueue(t.Execute());
    }

    public override void Update() {
        if (transactionsForThisFrame.Count > 0) {
            if (!transactionsForThisFrame.Peek().MoveNext()) {
                transactionsForThisFrame.Dequeue();
            }
            return; // don't do any more logic while transactions are occuring
        }

        bool mousePressed = Input.GetMouseButton(0);

        bool rayHitPieceThisFrame = false;
        bool rayHitTileThisFrame = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, GameState.Constants.kRaycastLayerMask, QueryTriggerInteraction.Collide);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            var obj = hit.collider.gameObject;
            if (obj == null)
                continue;

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
            if (entry.Key == null)
                continue;

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
            if (entry.Key == null)
                continue;

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
}
