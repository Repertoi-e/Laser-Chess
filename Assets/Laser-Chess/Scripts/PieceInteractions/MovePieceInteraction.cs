using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovePieceInteraction : PieceInteraction {
    public Vector3[] AllowedMovePositions = null;

    // This object is a copy of the ActivePiece's model.
    // It's slightly transparent and it shows above the tile
    // the player is hovering, that is if it's valid for move.
    // This gives the player visual feedback
    // that the action on click will be a move action.
    private GameObject selectedPieceGhostModel = null;

    public MovePieceInteraction(Piece piece) : base(piece) {
        AllowedMovePositions = GetAllowedMovePositions().ToArray();
    }

    public override void Cleanup() {
        OnGhostLostHover();
        if (selectedPieceGhostModel)
            GameObject.Destroy(selectedPieceGhostModel);
    }

    public override bool IsAvailable() {
        return AllowedMovePositions?.Length != 0;
    }

    IEnumerable<Vector3> GetAllowedMovePositions() {
        foreach (var rel in piece.MoveTilesByRule) {
            if (rel.x == 0 && rel.z == 0)
                continue;

            var dest = rel + piece.transform.position;

            Tile tile = GameState.Board.GetTileAt(dest);
            if (tile != null && tile.GetPieceAbove() == null) {
                if (piece.CanIgnorePieceBlock || !MathUtils.IsInteractionBlocked(GameState.Board, piece.gameObject.transform.position, dest)) {
                    yield return dest;
                }
            }
        }
    }

    public override void OnBoardMouseExit() {
        OnGhostLostHover();
    }

    void OnGhostHover(Tile tile) {
        if (piece) {
            EnsureGhostForPiece();
            selectedPieceGhostModel.transform.position = tile.gameObject.transform.position;

            var dir = tile.gameObject.transform.position - piece.gameObject.transform.position;
            dir.Normalize();
            selectedPieceGhostModel.transform.rotation = Quaternion.LookRotation(MathUtils.SnapVectorToCardinal(dir));
        }
    }

    void OnGhostLostHover() {
        if (selectedPieceGhostModel) {
            selectedPieceGhostModel.transform.position = new Vector3(0, 0, -1000);
        }
    }

    void EnsureGhostForPiece() {
        if (selectedPieceGhostModel != null)
            return; // we assume it's for the same piece

        var model = piece.gameObject.transform.Find("Model");
        if (model) {
            selectedPieceGhostModel = new GameObject("GhostHover");

            var modelCopy = GameObject.Instantiate(model.gameObject);
            modelCopy.transform.parent = selectedPieceGhostModel.transform;

            var renderers = modelCopy.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers) {
                renderer.material = GameState.Constants.kUnitTransparentMaterial;
            }
        }
    }

    public override void OnTileMouseEnter(Tile tile) {
        if (humanTurn == null)
            return;

        if (Array.Exists(AllowedMovePositions, x => x == tile.gameObject.transform.position)) {
            // move ghost to available move tile
            OnGhostHover(tile);
            return;
        } else {
            // if hovering non-allowed move tile,
            // move the ghost out the way 
            OnGhostLostHover();
        }
    }

    public override void OnPieceClicked(Piece target) {
        if (humanTurn == null)
            return;

        // Double clicking on a piece goes into attack mode (stays in place)
        if (target == piece) {
            humanTurn.CurrentPieceInteraction = new AttackPieceInteraction(piece);
        } else {
            humanTurn.DoPieceInteraction(target);
        }
    }

    public override void OnTileClicked(Tile tile) {
        if (humanTurn == null)
            return;

        if (!Array.Exists(AllowedMovePositions, x => x == tile.gameObject.transform.position)) {
            GameState.FeedbackText.DoPieceOutOfRangeMove();
            return;
        }

        var transaction = new MoveTransaction() { piece = piece, target = tile.transform.position };
        if (transaction.IsValid()) {
            playingState.QueueUpValidTransaction(transaction);
            humanTurn.MovedThisTurn.Add(piece);
        }
        humanTurn.CurrentPieceInteraction = null;
    }
}
