using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    
public abstract class Piece : MonoBehaviour {
    abstract public int MaxHitPoints {
        get;
    }

    abstract public int AttackPower {
        get;
    }

    abstract public bool IsEnemy {
        get;
    }

    abstract public IEnumerable<Vector3> MoveTilesByRule {
        get;
    }

    public enum EAttackType {
        None,
        Shoot,
        Region
    }

    abstract public EAttackType AttackType {
        get;
    }

    abstract public IEnumerable<Vector3> AttackTilesByRule {
        get;
    }


    public virtual bool CanIgnorePieceBlock {
        get => false;
    } // only true for knight-like Jumpship for now

    private HealthBar healthBar = null;
    private int hitPoints;

    public int HitPoints {
        get => hitPoints;
        set {
            hitPoints = value;
            if (hitPoints <= 0) {
                hitPoints = 0;
                
                var playingState = GameState.CurrentState as PlayingState;
                if (playingState == null) {
                    Destroy(gameObject);
                } else {
                    var transaction = new PieceDeathTransaction() { piece = this };
                    if (transaction.IsValid())
                        playingState.QueueUpValidTransaction(transaction);
                }
            }
            healthBar?.SetHitPoints(hitPoints);
        }
    }

    private float elapsedTime = 0;

    private Material[] materials;
    private Color targetColor;

    void Start() {
        materials = (from c in GetComponentsInChildren<Renderer>() select c.material).ToArray();
    }

    ~Piece() {
        // God knows when the garbage collected calls this,
        // but when the unit dies the health bar becomes 0 anyway.
        // So it's invisible to the player.
        // Eventually it will go away from the scene.
        Destroy(healthBar?.gameObject);
    }

    void Update() {
        // We need to wait for Constants to get initted,
        // that's why this is not in Start()... Sigh...
        if (!healthBar) {
            var hp = Instantiate(GameState.Constants.kHealthBarTemplate);
            healthBar = hp.GetComponent<HealthBar>();
            healthBar.SetMaxHitPoints(MaxHitPoints);

            HitPoints = MaxHitPoints;
            if (MaxHitPoints == 0)
                Destroy(gameObject);
        }

        // Move along the health bar since it needs to point to the camera
        // and parenting it with this piece messes it up on rotations.
        healthBar.gameObject.transform.position = transform.position;

        // Pieces glow like tiles on hover.
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

    public void SetTargetEmissionColor(Color c) {
        elapsedTime = 0;
        targetColor = c;
    }
}
