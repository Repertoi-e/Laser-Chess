using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {

    // This here is a dump for stuff that can be tweaked in the editor

    public float kGlowAnimationDuration = 0.1f;

    public Color kGlowEnemy;
    public Color kGlowHuman;

    public Color kEnemyColor;
    public Color kAllyColor;

    public Color kGlowHoverAttackColor;
    public Color kGlowAvailableAction;

    public Color kTileGlowCanMove = new Color(0.35f, 0.35f, 0.35f, 0.35f);
    public Color kTileGlowCanAttack = new Color(0.35f, 0.35f, 0.35f, 0.35f);
    public Color kTileGlowCanAttackPiece = new Color(0.35f, 0.35f, 0.35f, 0.35f);

    public float kUnitSpeed = 2;
    public Material kUnitTransparentMaterial;

    public LayerMask kRaycastLayerMask;

    public GameObject kHealthBarTemplate;

    public float kFeedbackDuration = 3;
    public float kFeedbackFadeOutTime = 3;

    public GameObject kEnemyExplosion;
    public GameObject kAllyExplosion;

    public GameObject kShootLaser;

    public GameObject yourTurnUI;
    public GameObject aiTurnUI;


    void Start() {
        GameState.Constants = this;    
    }
}
