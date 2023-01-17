using System;
using UnityEngine;

public class Tile : MonoBehaviour {
    private Material material;

    private float elapsedTime = 0;
    private Color targetColor;

    void Start() {
        material = GetComponent<Renderer>().material;
    }

    void Update() {
        if (elapsedTime < GameState.Constants.kGlowAnimationDuration) {
            float t = elapsedTime / GameState.Constants.kGlowAnimationDuration;
            t = t * t * (3f - 2f * t);

            material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            material.SetColor("_EmissionColor", targetColor);
        }

        Color newTargetColor = Color.black;

        PlayingState playingState = GameState.CurrentState as PlayingState;
        HumanTurn humanTurn = playingState?.Turn as HumanTurn;

        var pieceAbove = GetPieceAbove();
        if (pieceAbove) {
            if (!pieceAbove.IsEnemy) {
                if (humanTurn != null) {
                    if (!humanTurn.AttackedThisTurn.Contains(pieceAbove) || (!humanTurn.MovedThisTurn.Contains(pieceAbove))) {
                        newTargetColor = GameState.Constants.kGlowAvailableAction;
                    }
                }
            }
        } else {
            var moveInteraction = humanTurn?.CurrentPieceInteraction as MovePieceInteraction;
            if (moveInteraction != null) {
                var allowedMovePositions = moveInteraction.AllowedMovePositions;
                if (allowedMovePositions != null) {
                    if (Array.Exists(allowedMovePositions, x => x == transform.position)) {
                        newTargetColor = GameState.Constants.kTileGlowCanMove;
                    }
                }
            }
        }

        var attackInteraction = humanTurn?.CurrentPieceInteraction as AttackPieceInteraction;
        if (attackInteraction != null) {
            var allowedAttackPositions = attackInteraction.AllowedAttackPositions;
            var allowedAttackPieces = attackInteraction.AllowedAttackPieces;
            if (allowedAttackPieces != null && Array.Exists(allowedAttackPieces, x => x == transform.position)) {
                newTargetColor = GameState.Constants.kTileGlowCanAttackPiece;
            } else if (allowedAttackPositions != null && Array.Exists(allowedAttackPositions, x => x == transform.position)) {
                newTargetColor = GameState.Constants.kTileGlowCanAttack;
            }
        }

        if (newTargetColor != targetColor) {
            elapsedTime = 0;
            targetColor = newTargetColor;
        }
    }

    public Piece GetPieceAbove() {
        Ray ray = new Ray(transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3)) {
            if (hit.collider != null) {
                var piece = hit.collider.gameObject.GetComponent<Piece>();
                return piece;
            }
        }
        return null;
    }

    public bool CheckForEnemyAbove() {
        var piece = GetPieceAbove();
        return piece ? piece.IsEnemy : false;
    }
}
