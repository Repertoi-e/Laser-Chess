using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {

    public float kGlowAnimationDuration = 0.1f;

    public Color kGlowEnemy;
    public Color kGlowHuman;

    public Color kGlowHoverAttackColor;

    public Color kGlowAvailableAction;

    public Color kTileGlowCanMove = new Color(0.35f, 0.35f, 0.35f, 0.35f);
    public Color kTileGlowCanAttack = new Color(0.35f, 0.35f, 0.35f, 0.35f);

    public float kUnitSpeed = 2;
    public Material kUnitTransparentMaterial;

    public LayerMask kRaycastLayerMask;

    void Start() {
        GameState.Constants = this;    
    }
}
