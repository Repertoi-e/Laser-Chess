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

    public virtual bool CanIgnorePieceBlock {
        get => false;
    } // only true for knight-like Jumpship for now

    public bool IsMoving = false; // only used to disable tile glow, sigh

    private float elapsedTime = 0;

    private List<Material> materials;
    private Color targetColor;

    public Tile GetTileBelow() {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2)) {
            if (hit.collider != null) {
                var tile = hit.collider.gameObject.GetComponent<Tile>();
                return tile;
            }
        }
        return null;
    }

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

    public void SetTargetEmissionColor(Color c) {
        elapsedTime = 0;
        targetColor = c;
    }
}
