using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovePieceInteraction : PieceInteraction {
    public Vector3[] AllowedMovePositions = null;

    private Piece selectedPiece;

    // This object is a copy of the ActivePiece's model.
    // It's slightly transparent and it shows above the tile
    // the player is hovering, that is if it's valid for move.
    // This gives the player visual feedback
    // that the action on click will be a move action.
    private GameObject selectedPieceGhostModel = null;

    public MovePieceInteraction(Piece piece) {
        selectedPiece = piece;
        AllowedMovePositions = GetAllowedMovePositionsForPiece(piece).ToArray();
        ShowAttackButton(piece.gameObject.transform.position);
    }

    public override void End() {
        OnGhostLostHover();
        if (selectedPieceGhostModel)
            GameObject.Destroy(selectedPieceGhostModel);
        HideAttackButton();
    }

    bool IsMoveBlocked(Vector3 start, Vector3 end) {
        int xIncrement = (start.x == end.x) ? 0 : (start.x < end.x) ? 1 : -1;
        int zIncrement = (start.z == end.z) ? 0 : (start.z < end.z) ? 1 : -1;

        for (int i = 1; i < Math.Max(Math.Abs(start.x - end.x), Math.Abs(start.z - end.z)); i++) {
            var dest = new Vector3(start.x + i * xIncrement, 0, start.z + i * zIncrement);
            Tile tile = GameState.Board.GetTileAt(dest);
            if (tile == null || tile.GetPieceAbove() != null) {
                return true;
            }
        }
        return false;
    }

    IEnumerable<Vector3> GetAllowedMovePositionsForPiece(Piece piece) {
        foreach (var rel in piece.MoveDirectionsByRule) {
            if (rel.x == 0 && rel.z == 0)
                continue;

            var dest = rel + piece.transform.position;

            Tile tile = GameState.Board.GetTileAt(dest);
            if (tile != null && tile.GetPieceAbove() == null) {
                if (piece.CanIgnorePieceBlock || !IsMoveBlocked(piece.gameObject.transform.position, dest)) {
                    yield return dest;
                }
            }
        }
    }

    public override void OnTileMouseEnter(Tile tile) {
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

    public override void OnBoardMouseExit() {
        OnGhostLostHover();
    }

    void OnGhostHover(Tile tile) {
        Debug.Assert(selectedPiece);
        if (selectedPiece) {
            EnsureGhostForPiece(selectedPiece);
            selectedPieceGhostModel.transform.position = tile.gameObject.transform.position;

            var dir = tile.gameObject.transform.position - selectedPiece.gameObject.transform.position;
            dir.Normalize();
            selectedPieceGhostModel.transform.rotation = Quaternion.LookRotation(MathUtils.SnapVectorToCardinal(dir));
        }
    }

    void OnGhostLostHover() {
        if (selectedPieceGhostModel) {
            selectedPieceGhostModel.transform.position = new Vector3(0, 0, -1000);
        }
    }

    void EnsureGhostForPiece(Piece piece) {
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

    public override void OnPieceClicked(Piece piece) {
        // Double clicking on a piece just deselects it
        humanTurn.CurrentPieceInteraction = null;
    }

    public override void OnTileClicked(Tile tile) {
        if (!Array.Exists(AllowedMovePositions, x => x == tile.gameObject.transform.position))
            return;

        var transaction = new MoveTransaction() { piece = selectedPiece, target = tile.transform.position };
        if (transaction.IsValid()) {
            humanTurn.playingState.QueueUpValidTransaction(transaction);
            humanTurn.MovedThisTurn.Add(selectedPiece);
        }
        humanTurn.CurrentPieceInteraction = null;
    }

    void ShowAttackButton(Vector3 pos) {
        var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
        if (attackButton) {
            attackButton.transform.position = pos;
        }
    }

    void HideAttackButton() {
        var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
        if (attackButton) {
            attackButton.transform.position = new Vector3(0, 0, -1000);
        }
    }
}
