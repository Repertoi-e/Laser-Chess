using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameState {
    private Piece selectedPiece = null;

    bool IsMoveBlocked(Vector3 start, Vector3 end) {
        int xIncrement = (start.x == end.x) ? 0 : (start.x < end.x) ? 1 : -1;
        int zIncrement = (start.z == end.z) ? 0 : (start.z < end.z) ? 1 : -1;

        for (int i = 1; i < Math.Max(Math.Abs(start.x - end.x), Math.Abs(start.z - end.z)); i++) {
            var dest = new Vector3(start.x + i * xIncrement, 0, start.z + i * zIncrement);
            Tile tile;
            Board.PositionToTile.TryGetValue(dest, out tile);
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

            Tile tile;
            Board.PositionToTile.TryGetValue(dest, out tile);
            if (tile != null && tile.GetPieceAbove() == null) {
                if (piece.CanIgnorePieceBlock || !IsMoveBlocked(piece.gameObject.transform.position, dest)) {
                    yield return dest;
                }
            }
        }
    }

    public void OnPieceMouseEnter(Piece piece) {
        if (CurrentState != State.Playing)
            return;

        piece.SetTargetEmissionColor(piece.IsEnemy ? Constants.kGlowEnemy : Constants.kGlowHuman);
    }

    public void OnPieceMouseExit(Piece piece) {
        piece.SetTargetEmissionColor(Color.black);
    }

    void OnPieceClicked(Piece piece) {
        if (!IsPlayerOnTurn || piece.IsEnemy)
            return;

        if (piece.HasMovedThisTurn && piece.HasAttackedThisTurn) {
            // TODO: give feedback to the player
            return;
        }

        // @Coupling Relying on code in TileLogic
        if (selectedPieceGhostModel) {
            Destroy(selectedPieceGhostModel); // we need to spawn new model next time
        }
        Board.ClearTilesGlowingToSignifyAvailableMovePosition();

        var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
        if (attackButton) {
            attackButton.transform.position = new Vector3(0, 0, -1000);
        }

        // Toggle selection when clicking multiple times
        if (selectedPiece == piece) {
            selectedPiece = null;
        } else {
            selectedPiece = piece;
            if (attackButton) {
                attackButton.transform.position = selectedPiece.gameObject.transform.position;
            }
            foreach (var tileDest in GetAllowedMovePositionsForPiece(piece)) {
                Tile tile;
                Board.PositionToTile.TryGetValue(tileDest, out tile);
                if (tile) {
                    // washed-out white
                    tile.SetTargetEmissionColor(new Color(0.35f, 0.35f, 0.35f, 0.35f));
                    tile.IsGlowingToSignifyAvailableMovePosition = true;
                }
            }
        }
    }

    void OnTileClicked(Tile tile) {
        // @Robustness This is not ideal
        if (!tile.IsGlowingToSignifyAvailableMovePosition)
            return;

        if (selectedPiece) {
            var attackButton = GameObject.FindGameObjectWithTag("AttackButtonUIWorld");
            if (attackButton) {
                attackButton.transform.position = new Vector3(0, 0, -1000);
            }

            QueueUpTransaction(new MoveTransaction() { piece = selectedPiece, target = tile.transform.position });
            selectedPiece = null;

            // @Coupling Relying on code in TileLogic
            OnGhostLostHover();
            Board.ClearTilesGlowingToSignifyAvailableMovePosition();
        }
    }
}
