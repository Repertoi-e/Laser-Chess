using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    abstract public void Move();

    private float elapsedTime = 0;

    private List<Material> materials;
    private Color targetColor;

    void Start() {
        materials = (from c in GetComponentsInChildren<Renderer>() select c.material).ToList();
    }

    void Update() {
        if (elapsedTime < GameState.constants.kGlowAnimationDuration) {
            float t = elapsedTime / GameState.constants.kGlowAnimationDuration;
            t = t * t * (3f - 2f * t);

            foreach (var material in materials)
                material.SetColor("_EmissionColor", Color.Lerp(material.GetColor("_EmissionColor"), targetColor, t));
            elapsedTime += Time.deltaTime;
        } else {
            foreach (var material in materials)
                material.SetColor("_EmissionColor", targetColor);
        }
    }

    void OnMouseEnter() {
        if (GameState.Instance.CurrentState != GameState.State.Playing)
            return;

        elapsedTime = 0;
        targetColor = IsEnemy ? GameState.constants.kGlowEnemy : GameState.constants.kGlowHuman;
    }

    void OnMouseExit() {
        elapsedTime = 0;
        targetColor = Color.black;
    }
}
