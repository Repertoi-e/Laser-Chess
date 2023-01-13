using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class GameState {
    private Piece selectedPiece = null;

    IEnumerable<Vector3> GetAllowedMovePositionsForPiece(Piece piece) {
        foreach (var rel in piece.MoveDirectionsByRule) {
            if (rel.x == 0 && rel.z == 0)
                continue;

            var dest = rel + piece.transform.position;

            GameObject tile;
            Board.PositionToTile.TryGetValue(dest, out tile);
            if (tile != null) {
                var tileComp = tile.GetComponent<Tile>();
                if (tileComp && tileComp.GetPieceAbove() == null) {
                    yield return dest;
                }
            }
        }
    }

    public void OnTileMouseEnter(Tile tile) {
        if (tile.IsGlowingToSignifyAvailableMovePosition) {
            // move ghost to available move tile
            Debug.Assert(selectedPiece);
            if (selectedPiece) {
                EnsureGhostForPiece(selectedPiece);
                selectedPieceGhostModel.transform.position = tile.gameObject.transform.position;
            }
            return;
        } else {
            if (selectedPieceGhostModel) {
                // if hovering non-allowed move tile,
                // move the ghost out the way 
                selectedPieceGhostModel.transform.position = new Vector3(0, 0, -1000);
            }
        }

        bool isAttacking = false; // @TODO XXX
        if (!IsPlayerOnTurn || !isAttacking)
            return;

        tile.SetTargetEmissionColor(Constants.kGlowHoverAttackColor);
    }

    public void OnTileMouseExit(Tile tile) {
        if (tile.IsGlowingToSignifyAvailableMovePosition)
            return;

        tile.SetTargetEmissionColor(Color.black);
    }

    public void OnPieceClicked(Piece piece) {
        if (!IsPlayerOnTurn || piece.IsEnemy)
            return;

        // Clear all tiles glowing
        foreach (var item in Board.PositionToTile) {
            var tile = item.Value.GetComponent<Tile>();
            if (tile.IsGlowingToSignifyAvailableMovePosition) {
                tile.IsGlowingToSignifyAvailableMovePosition = false;
                tile.SetTargetEmissionColor(Color.black);
            }
        }
        if (selectedPieceGhostModel) {
            Destroy(selectedPieceGhostModel);
        }

        // Toggle selection when clicking multiple times
        if (selectedPiece == piece) {
            selectedPiece = null;
        } else {
            selectedPiece = piece;
            foreach (var tileDest in GetAllowedMovePositionsForPiece(piece)) {
                var tile = Board.PositionToTile[tileDest].GetComponent<Tile>();
                // washed-out white
                tile.SetTargetEmissionColor(new Color(0.35f, 0.35f, 0.35f, 0.35f));
                tile.IsGlowingToSignifyAvailableMovePosition = true;
            }
        }
    }

    // This object is a copy of the ActivePiece's model.
    // It's slightly transparent and it shows above the tile
    // the player is hovering, that is if it's valid for move.
    // This gives the player visual feedback
    // that the action on click will be a move action.
    private GameObject selectedPieceGhostModel = null;

    void EnsureGhostForPiece(Piece piece) {
        if (selectedPieceGhostModel != null)
            return; // we assume it's for the same piece @TODO @Robustness Perhaps add checks to see if it really is the same model

        var model = piece.gameObject.transform.Find("Model");
        if (model) {
            selectedPieceGhostModel = new GameObject("GhostHover");

            var modelCopy = Instantiate(model.gameObject);
            modelCopy.transform.parent = selectedPieceGhostModel.transform;
        }
    }
}
