using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public abstract class Piece : MonoBehaviour {
    abstract public int HitPoints {
        get;
    }

    abstract public int AttackPower {
        get;
    }

    abstract public bool IsEnemy {
        get;
    }

    abstract public IEnumerable<Vector3> MoveDirectionsByRule {
        get;
    }

    public IEnumerable<Vector3> GetAllowedMovePositions() {
        foreach (var rel in MoveDirectionsByRule) {
            if (rel.x == 0 && rel.z == 0)
                continue;

            var dest = rel + gameObject.transform.position;
            GameObject tile;
            GameState.Board.PositionToTile.TryGetValue(dest, out tile);
            if (tile != null) {
                var tileComp = tile.GetComponent<Tile>();
                if (tileComp && tileComp.GetPieceAbove() == null) {
                    yield return dest;
                }
            } 
        }
    }

    private float elapsedTime = 0;

    private List<Material> materials;
    private Color targetColor;

    void Start() {
        materials = (from c in GetComponentsInChildren<Renderer>() select c.material).ToList();
    }

    void Update() {
        if (elapsedTime < GameState.Constants.kGlowAnimationDuration) {
            float t = elapsedTime / GameState.Constants.kGlowAnimationDuration;
            t = t * t * (3f - 2f * t);

            foreach (var material in materials)
                material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            foreach (var material in materials)
                material.SetColor("_EmissionColor", targetColor);
        }
    }

    void OnMouseDown() {
        if (!GameState.Instance.IsPlayerOnTurn)
            return;

        GameState.Board.ClearBoardHovers();

        // Toggle selection when clicking multiple times
        if (GameState.Instance.ActivePiece == this.gameObject) {
            GameState.Instance.ActivePiece = null;
        } else {
            GameState.Instance.ActivePiece = this.gameObject;
            foreach (var tile in GetAllowedMovePositions()) {
                GameState.Board.PositionToTile[tile].GetComponent<Tile>().GlowForAllowedMove = true;
            }
        }
    }

    void OnMouseEnter() {
        if (GameState.Instance.CurrentState != GameState.State.Playing)
            return;

        elapsedTime = 0;
        targetColor = IsEnemy ? GameState.Constants.kGlowEnemy : GameState.Constants.kGlowHuman;
    }

    void OnMouseExit() {
        elapsedTime = 0;
        targetColor = Color.black;
    }
}
