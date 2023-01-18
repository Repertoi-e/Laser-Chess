using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackPieceInteraction : PieceInteraction {
    public Vector3[] AllowedAttackPositions = null; // this is used for attack range visualization, doesn't contain valid attacks
    public Vector3[] AllowedAttackPieces = null;

    public AttackPieceInteraction(Piece piece) : base(piece) {
        if (piece.AttackType == Piece.EAttackType.None)
            return;

        AllowedAttackPositions = GetAllowedAttackPositions().ToArray();
        AllowedAttackPieces = (from p in
                                   from c in AllowedAttackPositions
                                   select GameState.Board.GetTileAt(c)?.GetPieceAbove()
                               where p != null && p.IsEnemy != piece.IsEnemy
                               select p.gameObject.transform.position).ToArray();

        // Region attacks don't require input for specific pieces,
        // so we provide an attack button to stomp.
        if (piece.AttackType == Piece.EAttackType.Region) {
            if (humanTurn != null)
                ShowAttackButton(piece.transform.position);
        }
    }

    public override void Cleanup() {
        HideAttackButton();
    }

    public override bool IsAvailable() {
        return AllowedAttackPieces?.Length != 0;
    }

    public void AttackButtonPressed() {
        var targets = (from p in AllowedAttackPieces select GameState.Board.GetTileAt(p)?.GetPieceAbove()).ToArray();

        var transaction = new AttackTransaction(targets) { piece = piece };
        if (transaction.IsValid()) {
            playingState.QueueUpValidTransaction(transaction);
            humanTurn.AttackedThisTurn.Add(piece);
        }

        humanTurn.CurrentPieceInteraction = null;
    }

    IEnumerable<Vector3> GetAllowedAttackPositions() {
        foreach (var rel in piece.AttackTilesByRule) {
            if (rel.x == 0 && rel.z == 0)
                continue;

            var dest = rel + piece.transform.position;
            Tile tile = GameState.Board.GetTileAt(dest);
            Piece pieceAbove = tile != null ? tile.GetPieceAbove() : null;
            if (pieceAbove != null && piece.IsEnemy == pieceAbove.IsEnemy) {
                continue; // Friendly piece
            }

            if (!MathUtils.IsInteractionBlocked(GameState.Board, piece.gameObject.transform.position, dest)) {
                yield return dest;
            }
        }
    }

    void ShowAttackButton(Vector3 pos) {
        if (humanTurn == null)
            return;

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

    public override void OnTileClicked(Tile tile) {
        if (humanTurn == null)
            return;

        humanTurn.CurrentPieceInteraction = null;
    }

    public override void OnPieceClicked(Piece target) {
        if (humanTurn == null)
            return;

        if (!target.IsEnemy) {
            humanTurn.DoPieceInteraction(target);
            return;
        }

        // This is only run for shoot attacks, which require input
        if (piece.AttackType != Piece.EAttackType.Shoot) {
            GameState.FeedbackText.DoPieceDoesntShoot();
            return;
        }

        if (!Array.Exists(AllowedAttackPieces, x => x == target.gameObject.transform.position)) {
            GameState.FeedbackText.DoPieceOutOfRangeAttack();
            return;
        }

        var transaction = new AttackTransaction(target) { piece = piece };
        if (transaction.IsValid()) {
            playingState.QueueUpValidTransaction(transaction);
            humanTurn.AttackedThisTurn.Add(piece);
        }

        humanTurn.CurrentPieceInteraction = null;
    }

}
