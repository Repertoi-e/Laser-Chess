using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTurn : Turn {
    private PieceInteraction currentPieceInteraction;

    public PieceInteraction CurrentPieceInteraction {
        get => currentPieceInteraction;
        set {
            currentPieceInteraction?.Cleanup();
            currentPieceInteraction = value;
        }
    }

    public HashSet<Piece> AttackedThisTurn = new();
    public HashSet<Piece> MovedThisTurn = new();

    public override void OnPieceClicked(Piece piece) {
        if (CurrentPieceInteraction != null) {
            CurrentPieceInteraction.OnPieceClicked(piece);
            return;
        }

        // If piece is clicked, check if moved
        if (!piece.IsEnemy) {
            if (MovedThisTurn.Contains(piece)) {
                if (AttackedThisTurn.Contains(piece)) {
                    // TODO: give feedback to the player
                    return;
                }
                CurrentPieceInteraction = new AttackPieceInteraction(piece) { humanTurn = this };
            } else {
                if (AttackedThisTurn.Contains(piece)) {
                    // TODO: give feedback to the player
                    return;
                }
                CurrentPieceInteraction = new MovePieceInteraction(piece) { humanTurn = this };
                if (!CurrentPieceInteraction.IsAvailable())
                    CurrentPieceInteraction = new AttackPieceInteraction(piece) { humanTurn = this };
            }
        }
    }

    public void AttackButtonPressed() {
        (CurrentPieceInteraction as AttackPieceInteraction)?.AttackButtonPressed();
    }

    public override void OnBoardMouseEnter() {
        CurrentPieceInteraction?.OnBoardMouseEnter();
    }

    public override void OnBoardMouseExit() {
        CurrentPieceInteraction?.OnBoardMouseExit();
    }

    public override void OnTileMouseEnter(Tile tile) {
        CurrentPieceInteraction?.OnTileMouseEnter(tile);
    }

    public override void OnTileMouseExit(Tile tile) {
        CurrentPieceInteraction?.OnTileMouseEnter(tile);
    }

    public override void OnTileClicked(Tile tile) {
        CurrentPieceInteraction?.OnTileClicked(tile);
    }

    public override void OnPieceMouseEnter(Piece piece) {
        CurrentPieceInteraction?.OnPieceMouseEnter(piece);
    }

    public override void OnPieceMouseExit(Piece piece) {
        CurrentPieceInteraction?.OnPieceMouseExit(piece);
    }
}
