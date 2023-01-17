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
        DoPieceInteraction(piece);
    }

    public void DoPieceInteraction(Piece target) {
        CurrentPieceInteraction = null;

        // If piece is clicked, check if moved
        if (!target.IsEnemy) {
            if (MovedThisTurn.Contains(target)) {
                if (AttackedThisTurn.Contains(target)) {
                    GameState.FeedbackText.DoPieceOutOfActions();
                    return;
                }
                CurrentPieceInteraction = new AttackPieceInteraction(target) { humanTurn = this };
            } else {
                if (AttackedThisTurn.Contains(target)) {
                    GameState.FeedbackText.DoPieceOutOfActions();
                    return;
                }
                CurrentPieceInteraction = new MovePieceInteraction(target) { humanTurn = this };
                if (!CurrentPieceInteraction.IsAvailable())
                    CurrentPieceInteraction = new AttackPieceInteraction(target) { humanTurn = this };
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
