using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Tile : MonoBehaviour {

    private float elapsedTime = 0;

    private Material material;
    private Color targetColor;

    private GameObject enemyBorder;

    void Start() {
        material = GetComponent<Renderer>().material;

        enemyBorder = gameObject.transform.Find("EnemyBorder").gameObject;
        enemyBorder.SetActive(false);
    }

    void Update() {
        if (elapsedTime < GameState.constants.kGlowAnimationDuration) {
            float t = elapsedTime / GameState.constants.kGlowAnimationDuration;
            t = t * t * (3f - 2f * t);

            material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            material.SetColor("_EmissionColor", targetColor);
        }

        bool shouldEnemyBorderBeActive = false;

        Ray ray = new Ray(transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2)) {
            if (hit.collider != null) {
                var piece = hit.collider.gameObject.GetComponent<Piece>();
                if (piece != null && piece.IsEnemy) {
                    shouldEnemyBorderBeActive = true;
                }
            }
        }

        if (enemyBorder.activeSelf != shouldEnemyBorderBeActive)
            enemyBorder.SetActive(shouldEnemyBorderBeActive);
    }

    void OnMouseEnter() {
        if (!GameState.Instance.IsPlayerOnTurn) return;

        elapsedTime = 0;
        targetColor = GameState.constants.kGlowHoverColor;
    }

    void OnMouseExit() {
        elapsedTime = 0;
        targetColor = Color.black;
    }
}
