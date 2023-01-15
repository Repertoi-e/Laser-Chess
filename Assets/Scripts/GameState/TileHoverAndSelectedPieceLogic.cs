using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class GameState {
    public Material pieceTransparentMaterial;
    
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

    void OnTileClicked(Tile tile) {
        if (selectedPiece) {
            selectedPiece.transform.position = tile.transform.position;
            selectedPiece.HasMovedThisTurn = true;
            selectedPiece = null;
            OnGhostLostHover();
            ClearTilesGlowingToSignifyAvailableMovePosition();
        }
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

    void ClearTilesGlowingToSignifyAvailableMovePosition() {
        foreach (var item in Board.PositionToTile) {
            var tile = item.Value.GetComponent<Tile>();
            if (tile.IsGlowingToSignifyAvailableMovePosition) {
                tile.IsGlowingToSignifyAvailableMovePosition = false;
                tile.SetTargetEmissionColor(Color.black);
            }
        }
    }

    void OnPieceClicked(Piece piece) {
        if (!IsPlayerOnTurn || piece.IsEnemy || piece.HasMovedThisTurn)
            return;

        ClearTilesGlowingToSignifyAvailableMovePosition();
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

            var renderers = modelCopy.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers) {
                renderer.material = pieceTransparentMaterial;
            }
        }
    }

    void OnGhostHover(Tile tile) {
        Debug.Assert(selectedPiece);
        if (selectedPiece) {
            EnsureGhostForPiece(selectedPiece);
            selectedPieceGhostModel.transform.position = tile.gameObject.transform.position;
        }
    }


    // Get out of the way
    void OnGhostLostHover() {
        if (selectedPieceGhostModel) {
            selectedPieceGhostModel.transform.position = new Vector3(0, 0, -1000);
        }
    }
}
