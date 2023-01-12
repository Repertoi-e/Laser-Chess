using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour {

    public float kGlowAnimationDuration = 0.1f;

    public Color kGlowEnemy;
    public Color kGlowHuman;

    public Color kGlowHoverAttackColor;

    void Start() {
        GameState.Constants = this;
    }

    void Update() {

    }
}
