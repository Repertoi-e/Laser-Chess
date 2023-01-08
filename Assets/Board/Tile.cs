using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Tile : MonoBehaviour {

    private float elapsedTime = 0;

    private Material material;
    private Color targetColor;

    void Start() {
        material = GetComponent<Renderer>().material;
    }

    void Update() {
        if (elapsedTime < GameState.Instance.board.kGlowDuration) {
            float t = elapsedTime / GameState.Instance.board.kGlowDuration;
            t = t * t * (3f - 2f * t);

            material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            material.SetColor("_EmissionColor", targetColor);
        }
    }

    void OnMouseEnter() {
        if (!GameState.Instance.IsPlayerOnTurn) return;

        elapsedTime = 0;
        targetColor = GameState.Instance.board.kGlowColor;
    }

    void OnMouseExit() {
        elapsedTime = 0;
        targetColor = Color.black;
    }
}
