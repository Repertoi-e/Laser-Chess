using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Tile : MonoBehaviour {

    public float glowDuration = 0.1f;

    private float elapsedTime = 0;

    private Material material;
    private Color targetColor;

    void Start() {
        material = GetComponent<Renderer>().material;
    }

    void Update() {
        if (elapsedTime < glowDuration) {
            float t = elapsedTime / glowDuration;
            t = t * t * (3f - 2f * t);

            material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            material.SetColor("_EmissionColor", targetColor);
        }
    }

    void OnMouseEnter() {
        elapsedTime = 0;
        targetColor = new Color(0, 1, 0, 1);
    }

    void OnMouseExit() {
        elapsedTime = 0;
        targetColor = Color.black;
    }
}
