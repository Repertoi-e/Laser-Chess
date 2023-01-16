using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial class GameState {
    public Material pieceTransparentMaterial;
    
    void OnTileMouseEnter(Tile tile) {
        if (tile.IsGlowingToSignifyAvailableMovePosition) {
            // move ghost to available move tile
            OnGhostHover(tile);
            return;
        } else {
            // if hovering non-allowed move tile,
            // move the ghost out the way 
            OnGhostLostHover();
        }

        bool isAttacking = false; // @TODO XXX
        if (!IsPlayerOnTurn || !isAttacking)
            return;

        tile.SetTargetEmissionColor(Constants.kGlowHoverAttackColor);
    }

    void OnTileLostHover(Tile tile) {
        if (tile.IsGlowingToSignifyAvailableMovePosition)
            return;

        tile.SetTargetEmissionColor(Color.black);
    }

    void OnTileMouseExit(Tile tile) {
        OnTileLostHover(tile);
    }

    bool mouseOnBoard = false;

    void OnBoardMouseEnter() {
        if (mouseOnBoard)
            return;
        mouseOnBoard = true;
    }

    void OnBoardMouseExit() {
        if (!mouseOnBoard)
            return;

        foreach (var item in Board.PositionToTile) {
            var tile = item.Value.GetComponent<Tile>();
            OnTileLostHover(tile);
        }

        // if no longer hovering board,
        // move the ghost out the way 
        OnGhostLostHover();

        mouseOnBoard = false;
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

            var renderers = modelCopy.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers) {
                renderer.material = pieceTransparentMaterial;
            }
        }
    }

    void OnGhostHover(Tile tile) {
        // @Coupling Relying on code in PieceLogic
        Debug.Assert(selectedPiece);
        if (selectedPiece) {
            EnsureGhostForPiece(selectedPiece);
            selectedPieceGhostModel.transform.position = tile.gameObject.transform.position;

            var dir = tile.gameObject.transform.position - selectedPiece.gameObject.transform.position;
            dir.Normalize();
            selectedPieceGhostModel.transform.rotation = Quaternion.LookRotation(MathUtils.SnapVectorToCardinal(dir));
        }
    }

    // Get out of the way
    void OnGhostLostHover() {
        if (selectedPieceGhostModel) {
            selectedPieceGhostModel.transform.position = new Vector3(0, 0, -1000);
        }
    }
}
