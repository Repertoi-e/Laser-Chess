using System;
using UnityEngine;

public class Tile : MonoBehaviour {
    private Material material;

    private float elapsedTime = 0;
    private Color targetColor;

    private GameObject enemyBorder;

    void Start() {
        material = GetComponent<Renderer>().material;

        enemyBorder = gameObject.transform.Find("Border").gameObject;
        enemyBorder.SetActive(false);
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

        bool shouldEnemyBorderBeActive = false;
        Color newTargetColor = Color.black;

        bool isPlayingState = GameState.CurrentState?.GetType() == typeof(PlayingState);
        var isHumanTurn = isPlayingState ? ((PlayingState) GameState.CurrentState).Turn?.GetType() == typeof(HumanTurn) : false;

        var pieceAbove = GetPieceAbove();
        if (pieceAbove) {
            if (pieceAbove.IsEnemy) {
                shouldEnemyBorderBeActive = true;
            } else {
                if (isHumanTurn) {
                    var humanTurn = (HumanTurn) ((PlayingState)GameState.CurrentState).Turn;
                    if (!humanTurn.AttackedThisTurn.Contains(pieceAbove) || !humanTurn.MovedThisTurn.Contains(pieceAbove)) {
                        newTargetColor = GameState.Constants.kGlowAvailableAction;
                    }
                }
            }
        } else {
            if (isHumanTurn) {
                var humanTurn = (HumanTurn) ((PlayingState)GameState.CurrentState).Turn;
                var interaction = humanTurn.CurrentPieceInteraction;
                if (interaction?.GetType() == typeof(MovePieceInteraction)) {
                    var allowedMovePositions = ((MovePieceInteraction)interaction).AllowedMovePositions;
                    if (allowedMovePositions != null) {
                        if (Array.Exists(allowedMovePositions, x => x == transform.position)) {
                            newTargetColor = GameState.Constants.kTileGlowCanMove;
                        }
                    }
                }
            }
        }

        if (enemyBorder.activeSelf != shouldEnemyBorderBeActive)
            enemyBorder.SetActive(shouldEnemyBorderBeActive);
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
