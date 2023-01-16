using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
    private Material material;
    
    private float elapsedTime = 0;
    private Color targetColor;

    private GameObject enemyBorder;

    public bool IsGlowingToSignifyAvailableMovePosition = false; // this property controlled by the GameState

    void Start() {
        material = GetComponent<Renderer>().material;

        enemyBorder = gameObject.transform.Find("Border").gameObject;
        enemyBorder.SetActive(false);
    }

    void Update() {
        if (elapsedTime < GameState.It.Constants.kGlowAnimationDuration) {
            float t = elapsedTime / GameState.It.Constants.kGlowAnimationDuration;
            t = t * t * (3f - 2f * t);

            material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            material.SetColor("_EmissionColor", targetColor);
        }

        // @Speed: Perhaphs we can call this less frequently?
        bool shouldEnemyBorderBeActive = CheckForEnemyAbove();
        if (enemyBorder.activeSelf != shouldEnemyBorderBeActive)
            enemyBorder.SetActive(shouldEnemyBorderBeActive);
    }

    // Changes the emission color of the tile,
    // pass c as Color.black to disable the effect.
    public void SetTargetEmissionColor(Color c) {
        elapsedTime = 0;
        targetColor = c;
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
