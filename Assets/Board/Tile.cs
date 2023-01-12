using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
    private float elapsedTime = 0;

    private Material material;
    private Color targetColor;

    private GameObject border;

    void Start() {
        material = GetComponent<Renderer>().material;

        border = gameObject.transform.Find("Border").gameObject;
        border.SetActive(false);
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

        bool shouldEnemyBorderBeActive = CheckForEnemyAbove();
        if (border.activeSelf != shouldEnemyBorderBeActive)
            border.SetActive(shouldEnemyBorderBeActive);
    }

    public bool GlowForAllowedMove {
        set {
            if (value && !GameState.Instance.IsPlayerOnTurn)
                return;

            elapsedTime = 0;
            targetColor = value ? Color.white : Color.black;
        }
    }

    public GameObject GetPieceAbove() {
        Ray ray = new Ray(transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2)) {
            if (hit.collider != null) {
                var piece = hit.collider.gameObject.GetComponent<Piece>();
                if (piece != null)
                    return hit.collider.gameObject;
            }
        }
        return null;
    }

    public bool CheckForEnemyAbove() {
        var piece = GetPieceAbove();
        return piece ? piece.GetComponent<Piece>().IsEnemy : false;
    }

    void OnMouseEnter() {
        bool isAttacking = false; // XXXX TODO:
        if (!GameState.Instance.IsPlayerOnTurn || GameState.Instance.ActivePiece == null || !isAttacking)
            return;

        elapsedTime = 0;
        targetColor = GameState.Constants.kGlowHoverAttackColor;
    }

    void OnMouseExit() {
        if (GameState.Instance.ActivePiece != null)
            return;

        elapsedTime = 0;
        targetColor = Color.black;
    }
}
